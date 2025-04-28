using integration.Context;
using integration.Factory.GET.Interfaces;
using integration.Services.Client;
using integration.Services.Client.Storage;
using integration.Services.ContractPosition.Storage;
using integration.Services.Interfaces;
using integration.Services.Location;
using integration.Services.Storage;

namespace integration.Factory.GET;

public class ContractGetterServiceFactory : IGetterServiceFactory<ContractData>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ContractGetterService> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly IConverterToStorageService _converterToStorageService;
    private readonly IContractPositionStorageService _contractPositionStorageService;
    private readonly IStorageService _storageService;
    private readonly IContractStorageService _contractStorageService;
    private readonly ITokenService _tokenService;

    public ContractGetterServiceFactory(
        IHttpClientFactory httpClientFactory,
        ILogger<ContractGetterService> logger,
        IConfiguration configuration, IConverterToStorageService converterToStorageService,
        IContractPositionStorageService contractPositionStorageService, IContractStorageService contractStorageService, ITokenService tokenService)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _configuration = configuration;
        _httpClient = new HttpClient();
        _converterToStorageService = converterToStorageService;
        _contractPositionStorageService = contractPositionStorageService;
        _contractStorageService = contractStorageService;
        _tokenService = tokenService;
    }

    public IGetterService<ContractData> Create()
    {
        return new ContractGetterService(_httpClientFactory, _httpClient, _logger, _configuration, _contractPositionStorageService, _contractStorageService, _tokenService);
    }
}