using integration.Context;
using integration.Factory.SET.Interfaces;
using integration.HelpClasses;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Interfaces;
using integration.Services.Location;
using Microsoft.Extensions.Options;

namespace integration.Factory.SET;

public class LocationSetterServiceFactory :  ISetterServiceFactory<LocationData>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<LocationSetterService> _logger;
    private readonly IAuthorizer _authorizer;
    private readonly IOptions<AuthSettings> _apiSettings;
    private readonly ILocationMapper _mapper;
    private readonly ILocationValidator _validator;

    public LocationSetterServiceFactory(
        IHttpClientFactory httpClientFactory,
        ILogger<LocationSetterService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> apiSettings,
        ILocationMapper mapper,
        ILocationValidator validator)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _authorizer = authorizer;
        _apiSettings = apiSettings;
        _mapper = mapper;
        _validator = validator;
    }

    public ISetterService<LocationData> Create()
    {
        return new LocationSetterService(_httpClientFactory, _logger, _authorizer, _apiSettings, _mapper, _validator );
    }
}