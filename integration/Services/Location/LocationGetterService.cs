using integration.Context;
using integration.HelpClasses;
using integration.Services.Interfaces;
using System.Text.Json;
using integration.Helpers;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using Microsoft.Extensions.Options;

namespace integration.Services.Location
{
    public class LocationGetterService : ServiceGetterBase<LocationData>, IGetterLocationService<LocationData>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<LocationGetterService> _logger;
        private readonly ILocationIdService _locationIdService;
        private readonly ConnectingStringApro _aproConnect;
        private readonly IOptions<AuthSettings> _configuration;
        private readonly JsonSerializerOptions _jsonOptions;

        public LocationGetterService(
            IHttpClientFactory httpClientFactory,
            ILogger<LocationGetterService> logger,
            IAuthorizer authorizer,
            IOptions<AuthSettings> apiSettings,
            ILocationIdService locationIdService)
            : base(httpClientFactory, logger, authorizer, apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _locationIdService = locationIdService;
            _configuration = apiSettings;
            
            _aproConnect = new ConnectingStringApro(
                _configuration,
                "wf__waste_site__waste_site/?query={id,datetime_create,datetime_update,lon,lat,address,status_id,ext_id}"
            );

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<List<(LocationData, bool IsNew)>> GetSync()
        {
            try
            {
                var locations = await FetchData();
                return await ProcessLocationsByDate(locations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting location data");
                throw;
            }
        }

        private async Task<List<LocationData>> FetchData()
        {
            _logger.LogInformation("Fetching locations from {Endpoint}", _aproConnect.GetAproConnectSettings());
            
            using var httpClient = await Authorize(true);

            var response = await httpClient.GetAsync(_aproConnect.GetAproConnectSettings());
            response.EnsureSuccessStatusCode();

            await using var responseStream = await response.Content.ReadAsStreamAsync();
            var data = await JsonSerializer.DeserializeAsync<List<LocationData>>(responseStream, _jsonOptions);

            LogResponseContent(response);
            return data ?? new List<LocationData>();
        }
        
        private void LogResponseContent(HttpResponseMessage response)
        {
            try
            {
                var content = response.Content.ReadAsStringAsync().Result;
                Message($"Received locations data: {content}");
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to read response content");
            }
        }

        private async Task<List<(LocationData Location, bool IsNew)>> ProcessLocationsByDate(List<LocationData> locations)
        {
            if (!locations.Any())
            {
                _logger.LogInformation("No locations found in response");
                TimeManager.SetLastUpdateTime("locations");
                return new List<(LocationData, bool)>();
            }

            var lastUpdate = TimeManager.GetLastUpdateTime("locations");
            var result = new List<(LocationData, bool)>();

            foreach (var location in locations)
            {
                try
                {
                    var isNew = DetermineIfNew(location, lastUpdate);
                    if (isNew.HasValue)
                    {
                        result.Add((location, isNew.Value));
                        _locationIdService.SetLocationIds(location.id);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing location {LocationId}", location.id);
                }
            }

            TimeManager.SetLastUpdateTime("locations");
            return result;
        }

        private bool? DetermineIfNew(LocationData location, DateTime lastUpdate)
        {
            if (location.datetime_create > lastUpdate)
            {
                return true;
            }
            
            if (location.datetime_update > lastUpdate)
            {
                return false;
            }
            
            return null;
        }

        public void Message(string message)
        {
            EmailMessageBuilder.PutInformation(
                EmailMessageBuilder.ListType.getlocation, 
                message
            );
        }

        public override Task HandleErrorAsync(string errorMessage)
        {
            throw new NotImplementedException();
        }

        // Реализация неиспользуемых методов интерфейса
        public Task Get() => Task.CompletedTask;
    }
}