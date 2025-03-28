using integration.Context;
using integration.Factory.GET.Interfaces;
using integration.Services.Client;
using integration.Services.ContractPosition.Storage;
using integration.Services.Interfaces;
using integration.Services.Storage;

namespace integration.Factory;

public class ClientGetterServiceFactory : IGetterServiceFactory<ClientData>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ClientGetterService> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private readonly IConverterToStorageService _converterToStorageService;
    private readonly IContractPositionStorageService _contractPositionStorageService;
    private readonly IStorageService _storageService;
    private readonly IContractStorageService _contractStorageService;

    public ClientGetterServiceFactory(
        IHttpClientFactory httpClientFactory,
        ILogger<ClientGetterService> logger,
        IConfiguration configuration, IConverterToStorageService converterToStorageService, IContractPositionStorageService contractPositionStorageService, IContractStorageService contractStorageService)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _configuration = configuration;
        _httpClient = new HttpClient();
        _converterToStorageService = converterToStorageService;
        _contractPositionStorageService = contractPositionStorageService;
        _contractStorageService = contractStorageService;
    }

    public IGetterService<ClientData> Create()
    {
        return new ClientGetterService(_httpClientFactory, _httpClient, _logger, _configuration, _contractPositionStorageService);
    }
}