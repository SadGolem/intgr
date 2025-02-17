using integration.Context;
using integration.HelpClasses;
using integration.Services.Factory.Interfaces;
using integration.Services.Interfaces;

namespace integration.Services.Factory
{
    public class LocationGetterServiceFactory : IGetterServiceFactory<LocationData>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<LocationGetterService> _logger;
        private readonly IConfiguration _configuration;

        public LocationGetterServiceFactory(
            IHttpClientFactory httpClientFactory,
            ILogger<LocationGetterService> logger,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _configuration = configuration;
        }

        public IGetterService<LocationData> Create()
        {
            return new LocationGetterService(_httpClientFactory, _logger, _configuration);
        }
    }
}
