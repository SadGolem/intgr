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
        _storageService = storageService;
    }
   
    public async Task Get()
    {
        var locationsStatus = await base.Get(_httpClientFactory, _getEndpoint, false);
    }

    public Task<List<(LocationMTDataResponse, bool IsNew)>> GetSync()
    {
        throw new NotImplementedException();
    }
    
    
    
}