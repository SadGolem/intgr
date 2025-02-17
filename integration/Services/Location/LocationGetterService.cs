using integration.Context;
using integration.HelpClasses;
using integration.Services.Interfaces;
using System.Net.Http;
using System.Text.Json;

namespace integration.Services.Location
{
    public class LocationGetterService : ServiceBase, IGetterService<LocationData>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<LocationGetterService> _logger; // Correct logger type
        private readonly IConfiguration _configuration;
        private readonly string _aproConnect;

        public LocationGetterService(IHttpClientFactory httpClientFactory, ILogger<LocationGetterService> logger, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _configuration = configuration;
            _aproConnect = _configuration.GetSection("APROconnect").Get<AuthSettings>().CallbackUrl.Replace("token-auth/", "wf__waste_site__waste_site/?query={id,datetime_create, datetime_update,lon,  lat, address, status_id}");

        }

        public async Task<List<LocationData>> FetchData()
        {
            var data = new List<LocationData>();

            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
                await Authorize(httpClient);
                var response = await httpClient.GetAsync(_aproConnect);

                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                data = await JsonSerializer.DeserializeAsync<List<LocationData>>(
                   await response.Content.ReadAsStreamAsync(), options);
                Message("Got: " + content);

                return data;
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

        public async Task<List<LocationData>> GetSync()
        {
            try
            {
                return await FetchData();
            }
            catch (Exception e)
            {
                //_logger.LogError(e, "Error in GetSync");
                throw;
            }
        }

        public void Message(string ex)
        {
            EmailMessageBuilder.PutInformation(EmailMessageBuilder.ListType.getlocation, ex);
        }
    }
}
