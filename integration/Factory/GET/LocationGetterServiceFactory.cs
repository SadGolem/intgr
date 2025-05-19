using integration.Context;
using integration.Factory.GET.Interfaces;
using integration.HelpClasses;
using integration.Helpers.Interfaces;
using integration.Services.Interfaces;
using integration.Services.Location;

namespace integration.Factory.GET
{
    public class LocationGetterServiceFactory : IGetterLocationServiceFactory<LocationData>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<LocationGetterService> _logger;
        private readonly IAuthorizer _authorizer;
        /*private readonly ILocationIdService _locationIdService;*/
        public LocationGetterServiceFactory(
            IHttpClientFactory httpClientFactory,
            ILogger<LocationGetterService> logger,
            IAuthorizer authorizer, ILocationIdService locationIdService)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _authorizer = authorizer;

        }
        public IGetterLocationService<LocationData> Create()
        {
            return new LocationGetterService(_httpClientFactory, _logger, _authorizer, _);
        }
    }
}
