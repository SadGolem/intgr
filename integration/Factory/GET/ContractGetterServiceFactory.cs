using integration.Context;
using integration.Factory.GET.Interfaces;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Client;
using integration.Services.ContractPosition.Storage;
using integration.Services.Interfaces;
using integration.Services.Storage;
using Microsoft.Extensions.Options;

namespace integration.Factory.GET;

public class ContractGetterServiceFactory : IGetterServiceFactory<ContractDataResponseResponse>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ContractGetterService> _logger;
    private readonly IAuthorizer _authorizer;
    private readonly IOptions<AuthSettings> _configuration;
    private readonly IConverterToStorageService _converterToStorageService;
    private readonly IContractPositionStorageService _contractPositionStorageService;
    private readonly IStorageService _storageService;
    private readonly IContractStorageService _contractStorageService;

    public ContractGetterServiceFactory(
        IHttpClientFactory httpClientFactory,
        ILogger<ContractGetterService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> configuration,
        IContractPositionStorageService contractPositionStorageService,
        IContractStorageService contractStorageService)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _authorizer = authorizer;
        _configuration = configuration;
        _contractPositionStorageService = contractPositionStorageService;
        _contractStorageService = contractStorageService;
    }

    public IGetterService<ContractDataResponseResponse> Create()
    {
        return new ContractGetterService(_httpClientFactory, _logger, _authorizer, _configuration , _contractPositionStorageService, _contractStorageService);
    }
}