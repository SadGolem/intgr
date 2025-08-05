using integration.Context;
using integration.Context.MT;
using integration.Domain.Entities;
using integration.Factory.GET.Interfaces;
using integration.Factory.SET.Interfaces;
using integration.Infrastructure;
using integration.Services.Location;

public interface ILocationManagerService
{
    Task SyncLocationsAsync();
    Task AddLocationAsync(LocationDataResponse location);
    Task GetFromMTAsync();
    Task SetFromMTAsync();
}

public class LocationManagerService : ILocationManagerService
{
    private readonly AppDbContext _context;
    private readonly IGetterLocationServiceFactory<LocationDataResponse> _serviceGetter;
    private readonly IGetterServiceFactory<LocationMTPhotoDataResponse> _locationMTServiceGetter;
    private readonly IGetterServiceFactory<LocationMTDataResponse> _locationMTStatusServiceGetter;
    private readonly ISetterServiceFactory<LocationDataResponse> _serviceSetter;
    private readonly ISetterServiceFactory<LocationMTPhotoDataResponse> _serviceSetterFromMTPhototoAPRO;
    private readonly ISetterServiceFactory<LocationMTDataResponse> _serviceSetterFromMTtoAPRO;
    private readonly ILocationIdService _locationIdService;
    private readonly ILogger<LocationManagerService> _logger;

    public LocationManagerService(
        AppDbContext context,
        IGetterLocationServiceFactory<LocationDataResponse> serviceGetter,
        IGetterServiceFactory<LocationMTPhotoDataResponse> locationMTServiceGetter,
        IGetterServiceFactory<LocationMTDataResponse> locationMTStatusServiceGetter,
        ISetterServiceFactory<LocationDataResponse> serviceSetter,
        ISetterServiceFactory<LocationMTPhotoDataResponse> serviceSetterFromMTPhototoAPRO,
        ISetterServiceFactory<LocationMTDataResponse> serviceSetterFromMTtoAPRO,
        ILocationIdService locationIdService,
        ILogger<LocationManagerService> logger)
    {
        _context = context;
        _serviceGetter = serviceGetter;
        _locationMTServiceGetter = locationMTServiceGetter;
        _locationMTStatusServiceGetter = locationMTStatusServiceGetter;
        _serviceSetter = serviceSetter;
        _serviceSetterFromMTPhototoAPRO = serviceSetterFromMTPhototoAPRO;
        _serviceSetterFromMTtoAPRO = serviceSetterFromMTtoAPRO;
        _locationIdService = locationIdService;
        _logger = logger;
    }

    public async Task SyncLocationsAsync()
    {
        try
        {
            var locationServiceGetter = _serviceGetter.Create();
            var locations = await locationServiceGetter.GetSync();

            _locationIdService.SetLocation(locations);

            foreach (var location in locations)
            {
                await AddLocationAsync(location.Item1);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Location sync");
            throw;
        }
    }

    public async Task AddLocationAsync(LocationDataResponse location)
    {
        var entity = new LocationEntity
        {
            IdAsuPro = location.id,
            Address = location.address?.Length > 500 ? location.address[..500] : location.address,
            Status = StatusCoder.ToCorrectLocationStatus(location.status.id, location.id),
            Latitude = Math.Round(location.lat, 6),
            Longitude = Math.Round(location.lon, 6),
            Comment = location.comment?.Length > 1000 ? location.comment[..1000] : location.comment,
            IdParticipant = location.participant?.id,
            IdClient = location.client?.id,
            AuthorUpdate = location.author_update?.Length > 100 ? location.author_update[..100] : location.author_update,
            ExtId = location.ext_id?.Length > 100 ? location.ext_id[..100] : location.ext_id
        };

        _context.LocationRecords.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task GetFromMTAsync()
    {
        try
        {
            var getterMT = _locationMTServiceGetter.Create();
            await getterMT.Get();
            
            var getterMTStatus = _locationMTStatusServiceGetter.Create();
            await getterMTStatus.Get();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Location get from MT");
            throw;
        }
    }

    public async Task SetFromMTAsync()
    {
        try
        {
            var service = _serviceSetterFromMTtoAPRO.Create();
            await service.Set();
            
            var servicePhoto = _serviceSetterFromMTPhototoAPRO.Create();
            await servicePhoto.Set();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Location set from MT");
            throw;
        }
    }
}