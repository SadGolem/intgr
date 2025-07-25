using integration.Context;
using integration.Factory.GET.Interfaces;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.ContractPosition.Storage;
using integration.Services.Emitter;
using integration.Services.Emitter.Storage;
using integration.Services.Interfaces;
using integration.Services.Storage;
using Microsoft.Extensions.Options;

namespace integration.Factory.GET;

public class EmitterGetterServiceFactory : IGetterServiceFactory<EmitterDataResponse>
{
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<EmitterGetterService> _logger;
        private readonly IAuthorizer _authorizer;
        private readonly IOptions<AuthSettings> _configuration;
        private readonly IConverterToStorageService _converterToStorageService;
        private readonly IContractPositionStorageService _contractPositionStorageService;
        private readonly IEmitterStorageService _emitterStorageService;

        public EmitterGetterServiceFactory(
            IHttpClientFactory httpClientFactory,
            ILogger<EmitterGetterService> logger,
            IAuthorizer authorizer,
            IOptions<AuthSettings> configuration,
            IContractPositionStorageService contractPositionStorageService,
            IEmitterStorageService emitterStorageService)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _authorizer = authorizer;
            _configuration = configuration;
            _contractPositionStorageService = contractPositionStorageService;
            _emitterStorageService = emitterStorageService;
        }

        public IGetterService<EmitterDataResponse> Create()
        {
            return new EmitterGetterService(_httpClientFactory, _logger, _authorizer, _configuration , _contractPositionStorageService, _emitterStorageService);
        }
    
}