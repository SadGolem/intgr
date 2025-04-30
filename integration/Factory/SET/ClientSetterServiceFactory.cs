using integration.Context;
using integration.Factory.SET.Interfaces;
using integration.Services.Client;
using integration.Services.Interfaces;
using integration.Services.Storage;

namespace integration.Factory.SET;

public class ClientSetterServiceFactory : ISetterServiceFactory<ClientData>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ClientSetterService> _logger;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;
    private IStorageService _storageService;

    public ClientSetterServiceFactory(
        IHttpClientFactory httpClientFactory,
        ILogger<ClientSetterService> logger,
        IConfiguration configuration, IStorageService storageService)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _configuration = configuration;
        _httpClient = new HttpClient();
        _storageService = storageService;
    }
    public ISetterService<ClientData> Create()
    {
        return new ClientSetterService(_httpClientFactory, _httpClient, _logger, _configuration, _storageService);
    }
}