using integration.Context.MT;
using integration.Factory.GET.Interfaces;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Interfaces;
using integration.Services.Location.fromMT;
using integration.Services.Location.fromMT.Storage;
using Microsoft.Extensions.Options;

namespace integration.Factory.GET.MT;

public class LocationMTPhotoGetterServiceFactory: IGetterServiceFactory<LocationMTPhotoDataResponse>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<LocationMTPhotoGetterService> _logger;
    private readonly IAuthorizer _authorizer;
    private readonly IOptions<AuthSettings> _configuration;
    private ILocationMTStorageService _storageService;

    public LocationMTPhotoGetterServiceFactory(
        IHttpClientFactory httpClientFactory,
        ILogger<LocationMTPhotoGetterService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> configuration,
        ILocationMTStorageService storageService)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _authorizer = authorizer;
        _configuration = configuration;
        _storageService = storageService;
    }

    public IGetterService<LocationMTPhotoDataResponse> Create()
    {
        return new LocationMTPhotoGetterService(_httpClientFactory, _logger, _authorizer, _configuration, _storageService);
    }
    
}