using integration.Context;
using integration.Factory.GET.Interfaces;
using integration.HelpClasses;
using integration.Helpers;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Client;
using integration.Services.Client.Storage;
using integration.Services.ContractPosition.Storage;
using integration.Services.Interfaces;
using integration.Services.Storage;
using Microsoft.Extensions.Options;

namespace integration.Factory;

public class ClientGetterServiceFactory : IGetterServiceFactory<ClientDataResponseResponse>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ClientGetterService> _logger;
    private readonly IOptions<AuthSettings> _configuration;
    private readonly IAuthorizer _authorizer;
    private readonly IConverterToStorageService _converterToStorageService;
    private readonly IContractPositionStorageService _contractPositionStorageService;
    private readonly IStorageService _storageService;
    private readonly IClientStorageService _storageClientService;

    public ClientGetterServiceFactory(
        IHttpClientFactory httpClientFactory,
        ILogger<ClientGetterService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> configuration, 
        IConverterToStorageService converterToStorageService, 
        IContractPositionStorageService contractPositionStorageService, 
        IClientStorageService storageClientService)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _authorizer = authorizer;
        _configuration = configuration;
        _converterToStorageService = converterToStorageService;
        _contractPositionStorageService = contractPositionStorageService;
        _storageClientService = storageClientService;
    }

    public IGetterService<ClientDataResponseResponse> Create()
    {
        return new ClientGetterService(_httpClientFactory, _logger, _authorizer, _configuration, _contractPositionStorageService, _storageClientService);
    }
}