using integration.Context.MT;
using integration.Factory.GET.Interfaces;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Agre;
using integration.Services.Agre.Storage;
using integration.Services.Entry.Storage;
using integration.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace integration.Factory.GET.MT;

public class AgreMTGetterServiceFactory: IGetterServiceFactory<AgreMTDataResponse>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AgreMTGetterService> _logger;
    private readonly IAuthorizer _authorizer;
    private readonly IOptions<AuthSettings> _configuration;
    private readonly IAgreStorageService _storage;

    public AgreMTGetterServiceFactory(
        IHttpClientFactory httpClientFactory,
        ILogger<AgreMTGetterService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> configuration,
        IAgreStorageService storage)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _authorizer = authorizer;
        _configuration = configuration;
        _storage = storage;
    }

    public IGetterService<AgreMTDataResponse> Create()
    {
        return new AgreMTGetterService(_httpClientFactory, _logger, _authorizer, _configuration, _storage);
    } 
}