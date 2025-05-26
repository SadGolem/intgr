using integration.Context;
using integration.Factory.SET.Interfaces;
using integration.HelpClasses;
using integration.Helpers;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Client;
using integration.Services.Interfaces;
using integration.Services.Storage;
using Microsoft.Extensions.Options;

namespace integration.Factory.SET;

public class ClientSetterServiceFactory : ISetterServiceFactory<ClientData>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ClientSetterService> _logger;
    private readonly IAuthorizer _authorizer;
    private readonly IOptions<AuthSettings> _configuration;
    
    private IStorageService _storageService;

    public ClientSetterServiceFactory(
        IHttpClientFactory httpClientFactory,
        ILogger<ClientSetterService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> configuration, IStorageService storageService)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _authorizer = authorizer;
        _configuration = configuration;
        _storageService = storageService;
    }
    public ISetterService<ClientData> Create()
    {
        return new ClientSetterService(_httpClientFactory, _logger, _authorizer, _configuration, _storageService);
    }
}