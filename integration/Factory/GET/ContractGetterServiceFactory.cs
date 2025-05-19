using integration.Context;
using integration.Factory.GET.Interfaces;
using integration.HelpClasses;
using integration.Services.Client;
using integration.Services.Client.Storage;
using integration.Services.ContractPosition.Storage;
using integration.Services.Interfaces;
using integration.Services.Location;
using integration.Services.Storage;
using Microsoft.Extensions.Options;

namespace integration.Factory.GET;

public class ContractGetterServiceFactory : IGetterServiceFactory<ContractData>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ContractGetterService> _logger;
    private readonly IOptions<AuthSettings> _configuration;
    private readonly HttpClient _httpClient;
    private readonly IConverterToStorageService _converterToStorageService;
    private readonly IContractPositionStorageService _contractPositionStorageService;
    private readonly IStorageService _storageService;
    private readonly IContractStorageService _contractStorageService;

    public ContractGetterServiceFactory(
        IHttpClientFactory httpClientFactory,
        ILogger<ContractGetterService> logger,
        IOptions<AuthSettings> configuration, IContractPositionStorageService contractPositionStorageService, IContractStorageService contractStorageService)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _configuration = configuration;
        _contractPositionStorageService = contractPositionStorageService;
        _contractStorageService = contractStorageService;
    }

    public IGetterService<ContractData> Create()
    {
        return new ContractGetterService(_httpClientFactory, _logger, _configuration, _contractPositionStorageService, _contractStorageService);
    }
}