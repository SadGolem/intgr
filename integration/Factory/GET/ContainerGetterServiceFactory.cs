using integration.Context;
using integration.Factory.GET.Interfaces;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Client;
using integration.Services.Client.Storage;
using integration.Services.Container;
using integration.Services.ContractPosition.Storage;
using integration.Services.Interfaces;
using integration.Services.Storage;
using Microsoft.Extensions.Options;

namespace integration.Factory.GET;

public class ContainerGetterServiceFactory : IGetterServiceFactory<Container>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ContainerGetterService> _logger;
    private readonly IOptions<AuthSettings> _configuration;
    private readonly IAuthorizer _authorizer;
    private readonly IContractPositionStorageService _contractPositionStorageService;

    public ContainerGetterServiceFactory(
        IHttpClientFactory httpClientFactory,
        ILogger<ContainerGetterService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> configuration,
        IContractPositionStorageService contractPositionStorageService)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _authorizer = authorizer;
        _configuration = configuration;
        _contractPositionStorageService = contractPositionStorageService;
    }

    public IGetterService<Container> Create()
    {
        return new ContainerGetterService(_httpClientFactory, _logger, _authorizer, _configuration,
            _contractPositionStorageService);
    }
}