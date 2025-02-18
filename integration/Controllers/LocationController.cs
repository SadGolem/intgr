using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using integration.Context;
using System.Text;
using integration.HelpClasses;
using integration.Services.Interfaces;
using integration.Factory.GET.Interfaces;

namespace integration.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase, IController
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<LocationController> _logger;
        private readonly AuthSettings _sourceApiUrl;
        private readonly AuthSettings _destinationApiUrl;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IGetterServiceFactory<LocationData> _service;
        private readonly string _mtConnect;
        private readonly string url = "wf__waste_site__waste_site/?query={id,datetime_create, datetime_update,lon,  lat, address, status_id}";
        private readonly string _aproConnect = "wf__waste_site__waste_site/?query={id,datetime_create, datetime_update,lon,  lat, address, status_id}";
        private IGetterService<LocationData> locationServiceGetter;
        private ISetterService<LocationData> locationServiceSetter;
        public LocationController(HttpClient httpClient, ILogger<LocationController> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory, IGetterServiceFactory<LocationData> service)
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
            _service = service;
        }

        [HttpGet("syncLocations")] // This endpoint can be used for manual triggers
        public async Task<IActionResult> Sync()
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
            List<LocationData> locations = new List<LocationData>();
            locationServiceGetter = _service.Create();
            locations = await locationServiceGetter.GetSync();

            await PostOrPatch(locations);
            _logger.LogInformation($"Received {locations.Count} locations");
        }

        public async Task PostOrPatch(List<LocationData> locations)
        {
            locationServiceSetter.PostOrPatch(locations);
        }
        

        
        
    }
}

