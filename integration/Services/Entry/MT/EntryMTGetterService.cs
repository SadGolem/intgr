using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Entry.Storage;
using integration.Services.Interfaces;
using integration.Services.Location;
using Microsoft.Extensions.Options;

public class EntryMTGetterService : ServiceGetterBase<EntryMTDataResponse>, IGetterService<EntryMTDataResponse>
{
    private readonly string _connectionString;
    private readonly IOptions<AuthSettings> _apiSettings;
    private readonly ILogger<EntryMTGetterService> _logger;
    private IHttpClientFactory _httpClientFactory;
    private IEntryStorageService<EntryMTDataResponse> _storageService;
    private const int CHUNK_SIZE_MINUTES = 30;

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
        _connectionString = _apiSettings.Value.MTconnect.BaseUrl + 
                           _apiSettings.Value.MTconnect.ApiClientSettings.EntryEndpointGetFromMT;
    }
    
    public async Task Get()
    {
        var lastUpdate = TimeManager.GetLastUpdateTime("entryMT");
        var now = DateTime.UtcNow;
        
        if (lastUpdate > now)
        {
            _logger.LogWarning($"Last update time is in future: {lastUpdate}. Resetting to 30 minutes ago.");
            lastUpdate = now.AddMinutes(60);
            TimeManager.SetLastUpdateTime("entryMT", lastUpdate);
        }
        
        while (lastUpdate < now)
        {
            var chunkEnd = lastUpdate.AddMinutes(CHUNK_SIZE_MINUTES);
            if (chunkEnd > now) chunkEnd = now;

            try
            {
                _logger.LogInformation($"Processing time range: {lastUpdate:o} - {chunkEnd:o}");
                await ProcessTimeChunk(lastUpdate, chunkEnd);
                
                TimeManager.SetLastUpdateTime("entryMT", chunkEnd);
                lastUpdate = chunkEnd;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to process time chunk: {lastUpdate:o} - {chunkEnd:o}");
                break; 
            }
            
            await Task.Delay(500);
        }
    }

    private async Task ProcessTimeChunk(DateTime startTime, DateTime endTime)
    {
        var endpoint = BuildEmitterEndpoint(startTime);
        var response = await GetFullResponse<EntryMTDataResponse>(_httpClientFactory, endpoint, false);
        
        if (response?.Data != null)
        {
            // Add timestamp to each entry if not already present
            foreach (var entry in response.Data)
            {
                if (entry.Timestamp == default)
                {
                    entry.Timestamp = response.Timestamp;
                }
            }
            
            // Filter entries that fall within the current time chunk
            var filteredEntries = response.Data
                .Where(e => e.Timestamp >= startTime && e.Timestamp < endTime)
                .ToList();
            
            if (filteredEntries.Any())
            {
                var filteredResponse = new EntryMTDataResponse
                {
                    Message = response.Message,
                    Timestamp = response.Timestamp,
                    Count = filteredEntries.Count,
                    Data = filteredEntries
                };
                
                await GetNewEntry(filteredResponse);
            }
        }
    }

    private string BuildEmitterEndpoint(DateTime startTime)
    {
        DateTimeOffset startDto = new DateTimeOffset(startTime, TimeSpan.Zero);
        DateTimeOffset inPlus7 = startDto.ToOffset(TimeSpan.FromHours(7));
        
        return _connectionString + inPlus7.ToString("yyyy-MM-ddTHH:mm:ss");
    }

    private async Task GetNewEntry(EntryMTDataResponse entry)
    {
        try
        {
            _storageService.Set(entry);
            _logger.LogInformation($"Saved {entry.Count} entries");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving entries to storage");
        }
    }
}