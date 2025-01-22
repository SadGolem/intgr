using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using System.Text.Json;
using Newtonsoft.Json.Linq;
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
        private readonly string _aproConnect;

        public LocationController(HttpClient httpClient, ILogger<LocationController> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _sourceApiUrl = _configuration.GetSection("APROconnect").Get<AuthSettings>();/*"https://test.asu2.big3.ru/api/wf__waste_site__waste_site/?query={id,datetime_create, datetime_update,lon,  lat, address}"; */// можно вынести в конфигурацию
            _destinationApiUrl = _configuration.GetSection("MTconnect").Get<AuthSettings>();
            _mtConnect = _destinationApiUrl.CallbackUrl.Replace("auth", "api/v2/location/create");
            _aproConnect = _sourceApiUrl.CallbackUrl.Replace("token-auth/", "wf__waste_site__waste_site/?query={id,datetime_create, datetime_update,lon,  lat, address}");

        }

        [HttpGet("syncLocations")] // This endpoint can be used for manual triggers
        public async Task<IActionResult> SyncLocations()
        {
            _logger.LogInformation("Starting manual location sync...");
            try
            {
                await FetchAndPostLocations();
                return Ok("Locations synced successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during location sync.");
                return StatusCode(500, "Error during location sync.");
            }
        }

        private async Task FetchAndPostLocations()
        {
            _logger.LogInformation($"Fetching locations from {_sourceApiUrl}...");
            List<LocationData> locations = new List<LocationData>();
            try
            {
                locations = await FetchLocationData();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during locations fetch");
                return;
            }

            _logger.LogInformation($"Received {locations.Count} locations");

            foreach (var location in locations)
            {
                try
                {
                    await PostLocation(location);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error posting location with id: {location.id}");
                }
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

        private async Task PostLocation(LocationData location)
        {
            var client = _httpClientFactory.CreateClient();
            var token = await TokenController._authorizer.GetCachedTokenAPRO();
            //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            try
            {
                var mappedLocation = MapLocationData(location);
                var jsonBody = JsonSerializer.Serialize(mappedLocation);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                using var response = await client.PostAsync(_mtConnect, content);


                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
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
            var datetimeCreate = string.IsNullOrEmpty(location.datetime_create) ? DateTime.UtcNow.ToString("yyyy-MM-dd") : DateTime.Parse(location.datetime_create).ToString("yyyy-MM-dd");
            var datetimeUpdate = string.IsNullOrEmpty(location.datetime_update) ? DateTime.UtcNow.ToString("yyyy-MM-dd") : DateTime.Parse(location.datetime_update).ToString("yyyy-MM-dd");

            return new
            {
                idBT = location.id,
                longitude = location.lon,
                latitude = location.lat,
                status = "Действующая", //необходимо принимать статусы по кодам
                location.address
            };
        }
    }

}

