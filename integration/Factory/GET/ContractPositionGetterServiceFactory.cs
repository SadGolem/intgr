using integration.Context;
using integration.Factory.GET.Interfaces;
using integration.Services.ContractPosition;
using integration.Services.ContractPosition.Storage;
using integration.Services.Interfaces;
using integration.Services.Location;
using integration.Services.Storage;

namespace integration.Factory.GET
{
    public class ContractPositionGetterServiceFactory : IGetterServiceFactory<ContractPositionData>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ContractPositionGetterService> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly IConverterToStorageService _converterToStorageService;

        public ContractPositionGetterServiceFactory(
            IHttpClientFactory httpClientFactory,
            ILogger<ContractPositionGetterService> logger,
            IConfiguration configuration, IConverterToStorageService converterToStorageService)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _configuration = configuration;
            _httpClient = new HttpClient();
            _converterToStorageService = converterToStorageService;
        }

        public IGetterService<ContractPositionData> Create()
        {
            return new ContractPositionGetterService(_httpClientFactory,_httpClient,  _logger, _configuration,
                new LocationIdService(), new ContractPositionStorageService() , new StorageService(_converterToStorageService));
        }
    }
}
