using AutoMapper;
using integration.Context.MT;
using integration.Factory.SET.Interfaces;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Interfaces;
using integration.Services.Location;
using integration.Services.Location.fromMT;
using integration.Services.Location.fromMT.Storage;
using Microsoft.Extensions.Options;

namespace integration.Factory.SET;

public class LocationFromMTSetterServiceFactory:  ISetterServiceFactory<LocationMTDataResponse>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<LocationSetterService> _logger;
    private readonly IAuthorizer _authorizer;
    private readonly IOptions<AuthSettings> _apiSettings;
    private readonly IMapper _mapper;
    private ILocationMTStorageService _storageService;
    
    public LocationFromMTSetterServiceFactory(
        IHttpClientFactory httpClientFactory,
        ILogger<LocationSetterService> logger,
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
    public ISetterService<LocationMTDataResponse> Create()
    {
        return new LocationFromMTSetterService(_httpClientFactory, _logger, _authorizer, _apiSettings, _mapper, _storageService);
    }
}