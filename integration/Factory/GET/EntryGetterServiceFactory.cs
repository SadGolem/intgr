using integration.Factory.GET.Interfaces;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Entry;
using integration.Services.Entry.Storage;
using integration.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace integration.Factory.GET;

public class EntryGetterServiceFactory : IGetterServiceFactory<EntryDataResponse>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<EntryGetterService> _logger;
    private readonly IAuthorizer _authorizer;
    private readonly IOptions<AuthSettings> _configuration;
    private IEntryStorageService<EntryDataResponse> _storageService;

    public EntryGetterServiceFactory(
        IHttpClientFactory httpClientFactory,
        ILogger<EntryGetterService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> configuration,
        IEntryStorageService<EntryDataResponse> storageService)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _authorizer = authorizer;
        _configuration = configuration;
        _storageService = storageService;
    }

    public IGetterService<EntryDataResponse> Create()
    {
        return new EntryGetterService(_httpClientFactory, _logger, _authorizer, _configuration, _storageService);
    }

}