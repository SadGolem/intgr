using integration.Context.MT;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Client.Storage;
using integration.Services.Interfaces;
using integration.Services.Location.fromMT.Storage;
using Microsoft.Extensions.Options;

namespace integration.Services.Location.fromMT;

public class LocationMTGetterService: ServiceGetterBase<LocationMTDataResponse>, IGetterService<LocationMTDataResponse>
{
    private readonly IClientStorageService _clientStorage;
    private readonly MTconnectSettings _apiSettings;
    private string  _getEndpoint;
    private readonly string _photoEndpointTemplate;
    private readonly ILogger<LocationMTGetterService> _logger;
    private IHttpClientFactory _httpClientFactory;
    private ILocationMTStorageService _storageService;
    
    public LocationMTGetterService(IHttpClientFactory httpClientFactory,
        ILogger<LocationMTGetterService> logger,
        IAuthorizer authorizer, IOptions<AuthSettings> apiSettings,
        ILocationMTStorageService storageService) : base(httpClientFactory, logger, authorizer, apiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _apiSettings = apiSettings.Value.MTconnect;
        _getEndpoint = _apiSettings.BaseUrl +
                       _apiSettings.ApiClientSettings.LocationGetStatusEndpoint;
        _photoEndpointTemplate = _apiSettings.BaseUrl +
                                 _apiSettings.ApiClientSettings.LocationGetPhotoEndpoint;
        _storageService = storageService;
    }
   
    public async Task Get()
    {
        // Получаем статусы локаций
        var locationsStatus = await base.Get(_httpClientFactory, _getEndpoint, false);
        
        if (locationsStatus != null)
        {
            await ProcessLocationsWithPhotos(locationsStatus);
            _storageService.Set(locationsStatus);
        }
    }
private async Task ProcessLocationsWithPhotos(List<LocationMTDataResponse> locations)
    {
        foreach (var location in locations)
        {
            try
            {
                location.images = await DownloadLocationPhotos(location.idMT, _photoEndpointTemplate, false);
                
                // Помечаем как обработанное
              //  location.PhotoLastUpdated = DateTime.UtcNow;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading photos for location {location.idMT}");
            }
        }
    }

    public Task<List<(LocationMTDataResponse, bool IsNew)>> GetSync()
    {
        throw new NotImplementedException();
    }
    
    
    
}