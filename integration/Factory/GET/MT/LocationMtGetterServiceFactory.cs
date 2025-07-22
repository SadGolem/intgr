using integration.Context.MT;
using integration.Factory.GET.Interfaces;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Interfaces;
using integration.Services.Location.fromMT;
using integration.Services.Location.fromMT.Storage;
using Microsoft.Extensions.Options;

namespace integration.Factory.GET.MT;

public class LocationMtGetterServiceFactory: IGetterServiceFactory<LocationMTDataResponse>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<LocationMTPhotoGetterService> _logger;
    private readonly IAuthorizer _authorizer;
    private readonly IOptions<AuthSettings> _configuration;
    private ILocationMTStorageService _storageService;

    public LocationMtGetterServiceFactory(
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

    public IGetterService<LocationMTDataResponse> Create()
    {
        return new LocationMTGetterService(_httpClientFactory, _logger, _authorizer, _configuration, _storageService);
    }
}