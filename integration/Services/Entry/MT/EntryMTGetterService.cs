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
            var response = await Get(_httpClientFactory, endpoint);
            await GetNewEntry(response);
        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
            throw;
        }
    }
    
    private async Task GetNewEntry(List<EntryMTDataResponse> entry)
    {
        foreach (var data in entry)
        {
            try
            {
                _storageService.Set(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing entry {LocationId}", data.id);
            }
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
        DateTime halfHourAgo = lastUpdate.AddMinutes(30);
    
        // Создаем DateTimeOffset с исходным временем (предполагая, что оно в UTC)
        DateTimeOffset dto = new DateTimeOffset(halfHourAgo, TimeSpan.Zero);
    
        // Применяем смещение +7 часов
        DateTimeOffset inPlus7 = dto.ToOffset(TimeSpan.FromHours(7));
    
        // Форматируем результат (без указания смещения)
        return inPlus7.ToString("yyyy-MM-ddTHH:mm:ss");
    }
}