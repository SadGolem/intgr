using integration.Context.MT;
using integration.Factory.GET.Interfaces;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Entry.MT;
using integration.Services.Entry.Storage;
using integration.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace integration.Factory.GET.MT;

public class EntryMTGetterServiceFactory : IGetterServiceFactory<EntryMTDataResponse>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<EntryMTGetterService> _logger;
    private readonly IAuthorizer _authorizer;
    private readonly IOptions<AuthSettings> _configuration;
    private IEntryStorageService<EntryMTDataResponse> _storageService;

    public EntryMTGetterServiceFactory(
        IHttpClientFactory httpClientFactory,
        ILogger<EntryMTGetterService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> configuration,
        IEntryStorageService<EntryMTDataResponse> storageService)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _authorizer = authorizer;
        _configuration = configuration;
        _storageService = storageService;
    }

    public IGetterService<EntryMTDataResponse> Create()
    {
        return new EntryMTGetterService(_httpClientFactory, _logger, _authorizer, _configuration, _storageService);
    }

}