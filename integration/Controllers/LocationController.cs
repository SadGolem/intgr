using Microsoft.AspNetCore.Mvc;
using integration.Context;
using integration.Context.MT;
using integration.Services.Interfaces;
using integration.Factory.GET.Interfaces;
using integration.Factory.SET.Interfaces;
using integration.Services.Location;

namespace integration.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase, IController
    {
        private readonly ILogger<LocationController> _logger;
        private readonly IGetterLocationServiceFactory<LocationDataResponse> _serviceGetter;
        private readonly ISetterServiceFactory<LocationDataResponse> _serviceSetter;
        private IGetterLocationService<LocationDataResponse> _locationServiceGetter;
        private IGetterServiceFactory<LocationMTDataResponse> _locationMTServiceGetter;
        private ISetterService<LocationDataResponse> _locationServiceSetter;
        private ILocationIdService _locationIdService;
        
        public LocationController(ILogger<LocationController> logger, 
            IGetterLocationServiceFactory<LocationDataResponse> serviceGetter, IGetterServiceFactory<LocationMTDataResponse> locationMTServiceGetter,
            ISetterServiceFactory<LocationDataResponse> serviceSetter,
            ILocationIdService locationIdService
        )
        {
            _logger = logger;
            _serviceGetter = serviceGetter;
            _serviceSetter = serviceSetter;
            _locationMTServiceGetter = locationMTServiceGetter;
            _locationIdService = locationIdService;
        }
        //[HttpGet("syncLocations")] // This endpoint can be used for manual triggers
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
            List<(LocationDataResponse,bool)> locations = new List<(LocationDataResponse,bool)>();
            _locationServiceGetter = _serviceGetter.Create();
            locations = await _locationServiceGetter.GetSync();
            
            _locationIdService.SetLocation(locations);
            _logger.LogInformation($"Received {locations.Count} locations");
        }

        public async Task<IActionResult> Get()
        {
            try
            {
                var getterMT = _locationMTServiceGetter.Create();
                await getterMT.Get();
                
                return Ok("Locations get from MT successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during location get from mt.");
                return StatusCode(500, "Error during location get from mt.");
            }
        }
    }
}

