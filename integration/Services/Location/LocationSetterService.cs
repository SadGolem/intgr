using System.Text;
using System.Text.Json;
using integration.Context;
using integration.HelpClasses;
using integration.Services.Interfaces;


namespace integration.Services.Location
{
    public class LocationSetterService : ServiceBase, ISetterService<LocationData>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<LocationSetterService> _logger; 
        private readonly IConfiguration _configuration;
        private readonly string _mtConnectCreate;
        private readonly string _mtConnectUpdate;
        
        public LocationSetterService(IHttpClientFactory httpClientFactory, HttpClient httpClient, ILogger<LocationSetterService> logger, IConfiguration configuration) : base(httpClientFactory, httpClient, logger, configuration)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _configuration = configuration;

            _mtConnectCreate = _configuration.GetSection("MTconnect").Get<AuthSettings>().CallbackUrl.Replace("auth", "api/v2/location/create_from_asupro");
            _mtConnectUpdate = _configuration.GetSection("MTconnect").Get<AuthSettings>().CallbackUrl.Replace("auth", "api/v2/location/update_from_asupro");
        }
        public bool Check(LocationData data)
        {
                if (data.address == null || data.address == "")
                {
                    Message("Address is empty");
                    return false;
                }
                return true;
        }
        
        public async Task PostAndPatch(LocationData location, bool isNew)
        {
            var client = _httpClientFactory.CreateClient();
            await Authorize(client, false);
            try
            {
                if (!Check(location)) return;
                var mappedLocation = MappingData(location);
                var jsonBody = JsonSerializer.Serialize(mappedLocation);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                HttpResponseMessage response;
                if (isNew)
                    response = await client.PostAsync(_mtConnectCreate, content);
                else
                {
                    response = await client.PatchAsync(_mtConnectUpdate, content);
                }

                response.EnsureSuccessStatusCode();  
                var responseContent = await response.Content.ReadAsStringAsync();
                Message($"Successfully posted location with id: {location.id}. Response: {responseContent}");
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

        public async Task PostAndPatch(List<(LocationData,bool)> data)
        {
            foreach (var location in data)
            {
                if (location.Item2)
                    await Post(location.Item1);
                else
                {
                    await Patch(location.Item1);
                }
            }
        }
        public async Task Post(LocationData location)
        {
            var client = _httpClientFactory.CreateClient();
            await Authorize(client, false);
            try
            {
                if (!Check(location)) return;
                var mappedLocation = MappingData(location);
                var jsonBody = JsonSerializer.Serialize(mappedLocation);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                HttpResponseMessage response;
                response = await client.PostAsync(_mtConnectCreate, content);

                response.EnsureSuccessStatusCode();  
                var responseContent = await response.Content.ReadAsStringAsync();
                Message($"Successfully posted location with id: {location.id}. Response: {responseContent}");
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
        public async Task Patch(LocationData location)
        {
            var client = _httpClientFactory.CreateClient();
            await Authorize(client, false);
            try
            {
                if (!Check(location)) return;
                var mappedLocation = MappingData(location);
                var jsonBody = JsonSerializer.Serialize(mappedLocation);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                HttpResponseMessage response;
                response = await client.PatchAsync(_mtConnectUpdate, content);

                response.EnsureSuccessStatusCode();  
                var responseContent = await response.Content.ReadAsStringAsync();
                Message($"Successfully patched location with id: {location.id}. Response: {responseContent}");
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
        public object MappingData(LocationData location)
        {
            return new
            {
                idBT = location.id,
                longitude = (decimal)location.lon,
                latitude = (decimal)location.lat,
                status = StatusCoder.ToCorrectLocationStatus(location.status), //необходимо принимать статусы по кодам
                address = location.address
            };
        }
        public override void Message(string ex)
        {
           
        }
    }
}
