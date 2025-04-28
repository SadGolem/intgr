using integration.Context;
using integration.Factory.SET.Interfaces;
using integration.Services.Interfaces;
using integration.Services.Location;

namespace integration.Factory.SET;

public class LocationSetterServiceFactory :  ISetterServiceFactory<LocationData>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<LocationSetterService> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private ITokenService _tokenService; 

    public LocationSetterServiceFactory(
        IHttpClientFactory httpClientFactory,
        ILogger<LocationSetterService> logger,
        IConfiguration configuration, ITokenService tokenService)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _configuration = configuration;
        _httpClient = new HttpClient();
        _tokenService = tokenService;
    }

    public ISetterService<LocationData> Create()
    {
        return new LocationSetterService(_httpClientFactory, _httpClient, _logger, _configuration, _tokenService);
    }
}