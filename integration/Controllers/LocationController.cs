using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using integration.Context;
using System.Text;
using integration.HelpClasses;
using integration.Services.Interfaces;
using integration.Factory.GET.Interfaces;
using integration.Factory.SET.Interfaces;

namespace integration.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase, IController
    {
        private readonly ILogger<LocationController> _logger;
        private readonly AuthSettings _destinationApiUrl;
        private readonly IConfiguration _configuration;
        private readonly IGetterServiceFactory<LocationData> _serviceGetter;
        private readonly ISetterServiceFactory<LocationData> _serviceSetter;
        private IGetterService<LocationData> locationServiceGetter;
        private ISetterService<LocationData> locationServiceSetter;
        public LocationController(HttpClient httpClient, ILogger<LocationController> logger, IConfiguration configuration, 
            IHttpClientFactory httpClientFactory, IGetterServiceFactory<LocationData> serviceGetter,
            ISetterServiceFactory<LocationData> serviceSetter
        )
        {
            _logger = logger;
            _configuration = configuration;
            _destinationApiUrl = _configuration.GetSection("MTconnect").Get<AuthSettings>();
            _serviceGetter = serviceGetter;
            _serviceSetter = serviceSetter;
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
            locationServiceGetter = _serviceGetter.Create();
            locations = await locationServiceGetter.GetSync();

            await PostOrPatch(locations);
            _logger.LogInformation($"Received {locations.Count} locations");
        }
        public async Task PostOrPatch(List<LocationData> locations)
        {
            locationServiceSetter = _serviceSetter.Create();
            await locationServiceSetter.PostOrPatch(locations);
        }
    }
}

