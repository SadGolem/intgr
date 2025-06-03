using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Entry.Storage;
using integration.Services.Interfaces;
using integration.Services.Location;
using Microsoft.Extensions.Options;

namespace integration.Services.Entry;

public class EntryGetterService : ServiceGetterBase<EntryDataResponse>, IGetterService<EntryDataResponse>
{
    private readonly APROconnectSettings _apiSettings;
    private readonly ILogger<EntryGetterService> _logger;
    private IHttpClientFactory _httpClientFactory;
    private IEntryStorageService _storageService;

    public EntryGetterService(
        IHttpClientFactory httpClientFactory,
        ILogger<EntryGetterService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> apiSettings,
        IEntryStorageService storageService)
        : base(httpClientFactory, logger, authorizer, apiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _apiSettings = apiSettings.Value.APROconnect;
        _storageService = storageService;
    }
    
    public async Task Get()
    {
        var endpoint = BuildEmitterEndpoint();
        var response = await Get(_httpClientFactory, endpoint);
        
    }
    
    private string BuildEmitterEndpoint()
    {
        var basePath = _apiSettings.BaseUrl + (_apiSettings.ApiClientSettings.EntryEndopint);

        return $"{basePath}";
    }
}