using integration.Context;
using integration.Factory.GET.Interfaces;
using integration.HelpClasses;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Interfaces;
using integration.Services.Location;
using Microsoft.Extensions.Options;

namespace integration.Factory.GET
{
    public class LocationGetterServiceFactory : IGetterLocationServiceFactory<LocationData>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<LocationGetterService> _logger;
        private readonly IOptions<AuthSettings> _apiSettings;
        private readonly ILocationIdService _locationIdService;
        private readonly IAuthorizer _authorizer;
        public LocationGetterServiceFactory(
            IHttpClientFactory httpClientFactory,
            ILogger<LocationGetterService> logger,
            IAuthorizer authorizer,
            IOptions<AuthSettings> apiSettings,
            ILocationIdService locationIdService)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _apiSettings = apiSettings;
            _authorizer = authorizer;
            _locationIdService = locationIdService;
        }
        public IGetterLocationService<LocationData> Create()
        {
            return new LocationGetterService(_httpClientFactory, _logger, _authorizer, _apiSettings, _locationIdService);
        }
    }
}
