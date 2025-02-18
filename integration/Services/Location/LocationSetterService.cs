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
        private readonly HttpClient _httpClient;
        private readonly ILogger<LocationSetterService> _logger; 
        private readonly IConfiguration _configuration;
        private readonly string _mtConnectCreate;
        private readonly string _mtConnectUpdate;
        
        public LocationSetterService(IHttpClientFactory httpClientFactory, HttpClient httpClient, ILogger<LocationSetterService> logger, IConfiguration configuration) : base(httpClientFactory, httpClient, logger, configuration)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
            _mtConnectCreate = _configuration.GetSection("APROconnect").Get<AuthSettings>().CallbackUrl.Replace("auth", "api/v2/location/create");
            _mtConnectUpdate = _configuration.GetSection("APROconnect").Get<AuthSettings>().CallbackUrl.Replace("auth", "api/v2/location/update");
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
        public async Task PostOrPatch(List<LocationData> locations)
        {
            var lastUpdate = TimeManager.GetLastUpdateTime("locations");

            foreach (var location in locations)
            {
                if (location.datetime_create > lastUpdate || location.datetime_update > lastUpdate)
                {
                    if (location.datetime_create > lastUpdate) //здесь менять логику незлья, так как у них  апдейт чуть позже криеэйт
                    {
                        await PostAndPatch(location, true);
                    }
                    else if (location.datetime_update > lastUpdate)
                    {
                        await PostAndPatch(location, false);
                    }
                }
                TimeManager.SetLastUpdateTime("locations");
            }
        }
        public async Task PostAndPatch(LocationData location, bool isNew)
        {
            var client = _httpClientFactory.CreateClient();
            await Authorize(client);
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
        public object MappingData(LocationData data)
        {
            throw new NotImplementedException();
        }
        public override void Message(string ex)
        {
           EmailMessageBuilder.PutInformation(EmailMessageBuilder.ListType.setlocation, ex);
        }
    }
}
