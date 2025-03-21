using integration.Context;
using integration.HelpClasses;
using integration.Services.Interfaces;
using System.Text.Json;

namespace integration.Services.Location
{
    public class LocationGetterService : ServiceBase, IGetterLocationService<LocationData>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<LocationGetterService> _logger; // Correct logger type
        private readonly ILocationIdService _locationIdService;
        private readonly ConnectingStringApro _aproConnect;

        public LocationGetterService(IHttpClientFactory httpClientFactory, ILogger<LocationGetterService> logger, IConfiguration configuration, HttpClient httpClient,ILocationIdService locationIdService) 
            : base(httpClientFactory, httpClient, logger, configuration)
        { 
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _locationIdService = locationIdService;
            _aproConnect = new ConnectingStringApro(configuration, "wf__waste_site__waste_site/?query={id,datetime_create, datetime_update,lon,  lat, address, status_id}");
        }

        public Task Get()
        {
            throw new NotImplementedException();
        }

        public async Task<List<(LocationData, bool)>> FetchData()
        {
            _logger.LogInformation($"Try getting locations from {_aproConnect}...");
            var data = new List<LocationData>();

            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
                await Authorize(httpClient, true);
                var response = await httpClient.GetAsync(_aproConnect.GetAproConnectSettings());

                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                data = await JsonSerializer.DeserializeAsync<List<LocationData>>(
                   await response.Content.ReadAsStreamAsync(), options);
                Message("Got: " + content);
                
                return await ProcessLocationsByDate(data);
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
        public async Task<List<(LocationData, bool)>> ProcessLocationsByDate(List<LocationData> locations)
        {
            List<(LocationData, bool)> data = new List<(LocationData, bool)>();

            if (locations.Count <= 0)
            {
                TimeManager.SetLastUpdateTime("locations");
                return data;
            }
            var lastUpdate = TimeManager.GetLastUpdateTime("locations");

            foreach (var location in locations)
            {
                if (location.datetime_create > lastUpdate || location.datetime_update > lastUpdate)
                {
                    if (location.datetime_create > lastUpdate) //здесь менять логику незлья, так как у них  апдейт чуть позже криеэйт
                    {
                        try
                        {
                            data.Add((location,true));
                            _locationIdService.SetLocationIds(location.id);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                            throw;
                        }
                    }
                    else if (location.datetime_update > lastUpdate)
                    {
                        data.Add((location,false));
                        _locationIdService.SetLocationIds(location.id);
                    }
                }
            }
            TimeManager.SetLastUpdateTime("locations");
            return data;
        }
        public async Task<List<(LocationData,bool)>> GetSync()
        {
            return await FetchData();
        }
        public bool Check(LocationData locationData)
        {
            throw new NotImplementedException();
        }
        public override void Message(string ex)
        {
            EmailMessageBuilder.PutInformation(EmailMessageBuilder.ListType.getlocation, ex);
        }
    }
}
