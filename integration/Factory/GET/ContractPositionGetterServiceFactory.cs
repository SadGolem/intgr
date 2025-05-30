using integration.Context;
using integration.Factory.GET.Interfaces;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.ContractPosition;
using integration.Services.ContractPosition.Storage;
using integration.Services.Interfaces;
using integration.Services.Location;
using integration.Services.Storage;
using Microsoft.Extensions.Options;

namespace integration.Factory.GET
{
    public class ContractPositionGetterServiceFactory : IGetterServiceFactory<ContractPositionDataResponse>
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ContractPositionGetterService> _logger;
        private readonly IOptions<AuthSettings> _configuration;
        private readonly IAuthorizer _authorizer;
        private readonly IConverterToStorageService _converterToStorageService;

        public ContractPositionGetterServiceFactory(
            IHttpClientFactory httpClientFactory,
            ILogger<ContractPositionGetterService> logger, IAuthorizer authorizer,
            IOptions<AuthSettings> configuration, IConverterToStorageService converterToStorageService)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _authorizer = authorizer;
            _configuration = configuration;
            _converterToStorageService = converterToStorageService;
            
        }

        public IGetterService<ContractPositionDataResponse> Create()
        {
            return new ContractPositionGetterService(_httpClientFactory,  _logger, _authorizer,
                _configuration,
                new LocationIdService(), new ContractPositionStorageService());
        }
    }
}
