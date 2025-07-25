using AutoMapper;
using integration.Context.MT;
using integration.Factory.SET.Interfaces;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Interfaces;
using integration.Services.Location.fromMT;
using integration.Services.Location.fromMT.Storage;
using Microsoft.Extensions.Options;

namespace integration.Factory.SET;

public class LocationFromMTPhotoSetterServiceFactory : ISetterServiceFactory<LocationMTPhotoDataResponse>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<LocationFromMTPhotoSetterService> _logger;
    private readonly IAuthorizer _authorizer;
    private readonly IOptions<AuthSettings> _apiSettings;
    private readonly IMapper _mapper;
    private ILocationMTStorageService _storageService;
    
    public LocationFromMTPhotoSetterServiceFactory(
        IHttpClientFactory httpClientFactory,
        ILogger<LocationFromMTPhotoSetterService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> apiSettings,
        IMapper mapper,
        ILocationMTStorageService storageService)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _authorizer = authorizer;
        _apiSettings = apiSettings;
        _mapper = mapper;
        _storageService = storageService;
    }
    public ISetterService<LocationMTPhotoDataResponse> Create()
    {
        return new LocationFromMTPhotoSetterService(_httpClientFactory, _logger, _authorizer, _apiSettings, _mapper, _storageService);
    }
}