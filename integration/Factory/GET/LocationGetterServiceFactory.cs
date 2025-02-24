using integration.Context;
using integration.Factory.GET.Interfaces;
using integration.HelpClasses;
using integration.Services.Interfaces;
using integration.Services.Location;

namespace integration.Factory.GET
{
    public class LocationGetterServiceFactory : IGetterLocationServiceFactory<LocationData>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<LocationGetterService> _logger;
        private readonly IConfiguration _configuration;
        private readonly ILocationIdService _locationIdService;
        private readonly HttpClient _httpClient;
        public LocationGetterServiceFactory(
            IHttpClientFactory httpClientFactory,
            ILogger<LocationGetterService> logger,
            IConfiguration configuration, ILocationIdService locationIdService)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _configuration = configuration;
            _httpClient = new HttpClient();
            _locationIdService = locationIdService;
        }
        public IGetterLocationService<LocationData> Create()
        {
            return new LocationGetterService(_httpClientFactory, _logger, _configuration, _httpClient, _locationIdService);
        }
    }
}
