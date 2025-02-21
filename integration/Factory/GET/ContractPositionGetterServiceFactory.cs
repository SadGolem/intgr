using integration.Context;
using integration.Factory.GET.Interfaces;
using integration.HelpClasses;
using integration.Services.ContractPosition;
using integration.Services.Interfaces;
using integration.Services.Location;

namespace integration.Factory.GET
{
    public class ContractPositionGetterServiceFactory : IGetterServiceFactory<ContractData>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ContractPositionGetterService> _logger;
        private readonly IConfiguration _configuration;
        private readonly ILocationIdService _locationIdService;
        private readonly HttpClient _httpClient;

        public ContractPositionGetterServiceFactory(
            IHttpClientFactory httpClientFactory,
            ILogger<ContractPositionGetterService> logger,
            IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _configuration = configuration;
            _httpClient = new HttpClient();
            _locationIdService = new LocationIdService();
        }

        public IGetterService<ContractData> Create()
        {
            return new ContractPositionGetterService(_httpClientFactory,_httpClient,  _logger, _configuration, _locationIdService);
        }
    }
}
