using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using integration.Context;
using integration.Context.MT;
using integration.Domain.Entities;
using integration.Services.Interfaces;
using integration.Factory.GET.Interfaces;
using integration.Factory.SET.Interfaces;
using integration.Infrastructure;
using integration.Services.Location;
using Newtonsoft.Json;

namespace integration.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LocationController : ControllerBase, IController
    {
        private readonly ILogger<LocationController> _logger;
        private readonly IGetterLocationServiceFactory<LocationDataResponse> _serviceGetter;
        private readonly ISetterServiceFactory<LocationDataResponse> _serviceSetter;
        private readonly ISetterServiceFactory<LocationMTPhotoDataResponse> _serviceSetterFromMTPhototoAPRO;
        private readonly ISetterServiceFactory<LocationMTDataResponse> _serviceSetterFromMTtoAPRO;
        private IGetterLocationService<LocationDataResponse> _locationServiceGetter;
        private IGetterServiceFactory<LocationMTPhotoDataResponse> _locationMTServiceGetter;
        private IGetterServiceFactory<LocationMTDataResponse> _locationMTStatusServiceGetter;
        private ISetterService<LocationDataResponse> _locationServiceSetter;
        private ILocationIdService _locationIdService;
        private AppDbContext _context;
        private IMapper _mapper;

        public LocationController(ILogger<LocationController> logger,
            IGetterLocationServiceFactory<LocationDataResponse> serviceGetter,
            IGetterServiceFactory<LocationMTPhotoDataResponse> locationMTServiceGetter,
            IGetterServiceFactory<LocationMTDataResponse> locationMTStatusServiceGetter,
            ISetterServiceFactory<LocationDataResponse> serviceSetter,
            ISetterServiceFactory<LocationMTPhotoDataResponse> serviceSetterFromMTPhototoApro,
            ISetterServiceFactory<LocationMTDataResponse> serviceSetterFromMTtoApro,
            ILocationIdService locationIdService,
            AppDbContext context,
            IMapper mapper
        )
        {
            _logger = logger;
            _serviceGetter = serviceGetter;
            _serviceSetter = serviceSetter;
            _locationMTServiceGetter = locationMTServiceGetter;
            _locationMTStatusServiceGetter = locationMTStatusServiceGetter;
            _locationIdService = locationIdService;
            _serviceSetterFromMTPhototoAPRO = serviceSetterFromMTPhototoApro;
            _serviceSetterFromMTtoAPRO = serviceSetterFromMTtoApro;
            _context = context;
            _mapper = mapper;
        }

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
        
        public async Task<IActionResult> AddLocation(LocationDataResponse location)
        {
            var testEntity = new LocationEntity
            {
                IdAsuPro = location.id,
                Address = location.address?.Length > 500 ? location.address.Substring(0, 500) : location.address,
                Status = StatusCoder.ToCorrectLocationStatus(location.status.id, location.id),
                Latitude = Math.Round(location.lat, 6),
                Longitude = Math.Round(location.lon, 6),
                Comment = location.comment?.Length > 1000 ? location.comment.Substring(0, 1000) : location.comment,
                IdParticipant = location.participant?.id,
                IdClient = location.client?.id,
                AuthorUpdate = location.author_update?.Length > 100 ? location.author_update.Substring(0, 100) : location.author_update,
                ExtId = location.ext_id?.Length > 100 ? location.ext_id.Substring(0, 100) : location.ext_id
            };

// Попробуйте сохранить этот объект вручную
            _context.LocationRecords.Add(testEntity);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private async Task FetchtLocations()
        {
            List<(LocationDataResponse, bool)> locations = new List<(LocationDataResponse, bool)>();
            _locationServiceGetter = _serviceGetter.Create();
            locations = await _locationServiceGetter.GetSync();

            _locationIdService.SetLocation(locations);

            foreach (var loc in locations)
            {
                await AddLocation(loc.Item1);
            }
            
            _logger.LogInformation($"Received {locations.Count} locations");
        }

        public async Task<IActionResult> Get()
        {
            try
            {
                var getterMT = _locationMTServiceGetter.Create();
                await getterMT.Get();
                
                var getterMTStatus = _locationMTStatusServiceGetter.Create();
                await getterMTStatus.Get();

                return Ok("Locations get from MT successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during location get from mt.");
                return StatusCode(500, "Error during location get from mt.");
            }
        }

        public async Task<IActionResult> Set()
        {
            try
            {
                var service = _serviceSetterFromMTtoAPRO.Create();
                await service.Set();
                
                var servicePhoto = _serviceSetterFromMTPhototoAPRO.Create();
                await servicePhoto.Set();
                
                return Ok("Locations set from MT successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during location set from mt.");
                return StatusCode(500, "Error during location get from mt.");
            }
        }
    }
}

