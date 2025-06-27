using integration.Context.MT;
using integration.Helpers;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Entry.Storage;
using integration.Services.Interfaces;
using integration.Services.Location;
using Microsoft.Extensions.Options;

namespace integration.Services.Entry.MT;

public class EntryMTGetterService : ServiceGetterBase<EntryMTDataResponse>, IGetterService<EntryMTDataResponse>
{
    private readonly string _connectionString;
    private readonly IOptions<AuthSettings> _apiSettings;
    private readonly ILogger<EntryMTGetterService> _logger;
    private IHttpClientFactory _httpClientFactory;
    private IEntryStorageService<EntryMTDataResponse> _storageService;

    public EntryMTGetterService(
        IHttpClientFactory httpClientFactory,
        ILogger<EntryMTGetterService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> apiSettings,
        IEntryStorageService<EntryMTDataResponse> storageService)
        : base(httpClientFactory, logger, authorizer, apiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _apiSettings = apiSettings;
        _storageService = storageService;
    }
    
    public async Task Get()
    {
        var endpoint = await BuildEmitterEndpoint();
        try
        {
            var response = await GetFullResponse<EntryMTDataResponse>(_httpClientFactory, endpoint, false);
            await GetNewEntry(response);
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            throw;
        }
    }

    private async Task GetNewEntry(EntryMTDataResponse entry)
    {
        try
        {
            _storageService.Set(entry);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing entry {LocationId}", entry.Data.Count);
        }

        TimeManager.SetLastUpdateTime("entryMT");
    }

    private async Task<string> BuildEmitterEndpoint()
    {
        string basePath;
             basePath = _apiSettings.Value.MTconnect.BaseUrl +_apiSettings.Value.MTconnect.ApiClientSettings.EntryEndpointGetFromMT + await GetDateTimeHalfHourAgo();

        return $"{basePath}";
    }
    
    public async Task<string> GetDateTimeHalfHourAgo()
    {
        var lastUpdate = TimeManager.GetLastUpdateTime("entryMT");
        DateTime halfHourAgo = lastUpdate.AddMinutes(0);
        
        DateTimeOffset dto = new DateTimeOffset(halfHourAgo, TimeSpan.Zero);
        
        DateTimeOffset inPlus7 = dto.ToOffset(TimeSpan.FromHours(7));
        
        return inPlus7.ToString("yyyy-MM-ddTHH:mm:ss");
    }
}