using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using integration.Context;
using System.Text;

namespace integration.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<LocationController> _logger;
        private readonly AuthSettings _sourceApiUrl;
        private readonly AuthSettings _destinationApiUrl;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly string _mtConnect;
        private readonly string url = "wf__waste_site__waste_site/?query={id,datetime_create, datetime_update,lon,  lat, address, status_id}";
        private readonly string _aproConnect = "wf__waste_site__waste_site/?query={id,datetime_create, datetime_update,lon,  lat, address, status_id}";

        public LocationController(HttpClient httpClient, ILogger<LocationController> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _sourceApiUrl = _configuration.GetSection("APROconnect").Get<AuthSettings>();/*"https://test.asu2.big3.ru/api/wf__waste_site__waste_site/?query={id,datetime_create, datetime_update,lon,  lat, address}"; */// можно вынести в конфигурацию
            _destinationApiUrl = _configuration.GetSection("MTconnect").Get<AuthSettings>();
            _mtConnect = _destinationApiUrl.CallbackUrl.Replace("auth", "api/v2/location/create");
            ConnectngStringApro _connectngStringApro = new ConnectngStringApro(_configuration, url);
            _aproConnect = _connectngStringApro.GetAproConnectSettings();
        }

        [HttpGet("syncLocations")] // This endpoint can be used for manual triggers
        public async Task<IActionResult> SyncLocations()
        {
            _logger.LogInformation("Starting manual location sync...");
            try
            {
                await FetchtLocations();
                
                return Ok("Locations synced successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during location sync.");
                return StatusCode(500, "Error during location sync.");
            }
        }

        private async Task FetchtLocations()
        {
            _logger.LogInformation($"Fetching locations from {_sourceApiUrl}...");
            List<LocationData> locations = new List<LocationData>();
            try
            {
                locations = await FetchLocationData();
                
                await PostOrPatch(locations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during locations fetch");
                return;
            }
            _logger.LogInformation($"Received {locations.Count} locations");
        }

        private async Task PostOrPatch(List<LocationData> locations)
        {
            var lastUpdate = LastUpdateTextFileManager.GetLastUpdateTime("locations");

            foreach (var location in locations)
            {
                if (location.datetime_create > lastUpdate || location.datetime_update > lastUpdate)
                {
                    if (location.datetime_create > lastUpdate) //здесь менять логику незлья, так как у них  апдейт чуть позже криеэйт
                    {
                        await PostAndPatchLocation(location, true);
                    }
                    else if (location.datetime_update > lastUpdate)
                    {
                        await PostAndPatchLocation(location, false);
                    }
                }
                LastUpdateTextFileManager.SetLastUpdateTime("locations");
            }
            
        }

        private async Task<List<LocationData>> FetchLocationData()
        {
            var locations = new List<LocationData>();
            var token = await TokenController._authorizer.GetCachedTokenAPRO();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            try
            {
                var response = await _httpClient.GetAsync(_aproConnect);

                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                locations = await JsonSerializer.DeserializeAsync<List<LocationData>>(
                   await response.Content.ReadAsStreamAsync(), options);
                ToGetMessage("Got locations: " + content);
                
                return locations;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Error during GET request to {_aproConnect}");
                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, $"Error during JSON deserialization of response from {_aproConnect}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while fetching data from {_aproConnect}");
                throw;
            }
        }

        private async Task PostAndPatchLocation(LocationData location, bool isNew)
        {
            var client = _httpClientFactory.CreateClient();
            var token = await TokenController._authorizer.GetCachedTokenAPRO();
            //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            try
            {
                if (!CheckLocation(location)) return;
                var mappedLocation = MapLocationData(location);
                var jsonBody = JsonSerializer.Serialize(mappedLocation);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                HttpResponseMessage response;
                if (isNew)
                    response = await client.PostAsync(_mtConnect, content);
                else
                {
                    response = await client.PatchAsync(_mtConnect, content);
                }

                response.EnsureSuccessStatusCode();  
                var responseContent = await response.Content.ReadAsStringAsync();
                ToSetMessage($"Successfully posted location with id: {location.id}. Response: {responseContent}");
                _logger.LogInformation($"Successfully posted location with id: {location.id}. Response: {responseContent}");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"HTTP error while posting location with id: {location.id}");
                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, $"Json Exception while posting location with id: {location.id}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected Exception while posting location with id: {location.id}");
                throw;
            }
        }

        private object MapLocationData(LocationData location)
        {
            return new
            {
                idBT = location.id,
                longitude = location.lon,
                latitude = location.lat,
                status = StatusCoder.ToCorrectLocationStatus(location.status), //необходимо принимать статусы по кодам
                address = location.address,
                idMtUser = _configuration.GetSection("idMtUser")
            };
        }

        private bool CheckLocation(LocationData location)
        {
            if (location.address == null || location.address == "")
            {
                ToSetMessage("Address is empty");
                return false;
            }
            return true;
        }

        void ToSetMessage(string ex)
        {
            EmailMessageBuilder.PutInformation(EmailMessageBuilder.ListType.SetLocationInfo, ex);
        }

        void ToGetMessage(string ex)
        {
            EmailMessageBuilder.PutInformation(EmailMessageBuilder.ListType.GetLocationInfo, ex);
        }
    }
}

