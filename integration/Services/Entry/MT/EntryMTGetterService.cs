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
        var now = DateTime.UtcNow;
        var lastUpdate = TimeManager.GetLastUpdateTime("entryMT");

        // Корректная обработка будущего времени
        if (lastUpdate > now)
        {
            _logger.LogWarning($"Last update time is in future: {lastUpdate}. Resetting to 30 minutes ago.");
            lastUpdate = now.AddMinutes(-CHUNK_SIZE_MINUTES);
            TimeManager.SetLastUpdateTime("entryMT", lastUpdate);
        }

        // Обработка до текущего момента включительно
        while (lastUpdate <= now)
        {
            var chunkEnd = lastUpdate.AddMinutes(CHUNK_SIZE_MINUTES);

            // Всегда включаем текущий момент в последний чанк
            if (chunkEnd > now) chunkEnd = now;

            try
            {
                _logger.LogInformation($"Processing time range: {lastUpdate:o} - {chunkEnd:o}");
                await ProcessTimeChunk(lastUpdate, chunkEnd);

                // Обновляем только если успешно обработали
                TimeManager.SetLastUpdateTime("entryMT", chunkEnd);
                lastUpdate = chunkEnd;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to process time chunk: {lastUpdate:o} - {chunkEnd:o}");

                // Увеличиваем задержку при ошибках
                await Task.Delay(5000);
                continue; // Повторяем попытку для этого же интервала
            }

            // Короткая пауза между запросами
            if (lastUpdate < now)
            {
                await Task.Delay(500);
            }
        }
    }

    private async Task ProcessTimeChunk(DateTime startTime, DateTime endTime)
    {
        var endpoint = BuildEmitterEndpoint(startTime);
        var response = await GetFullResponse<EntryMTDataResponse>(endpoint, false);

        if (response?.Data == null) return;

        // Устанавливаем timestamp по умолчанию
        foreach (var entry in response.Data.Where(e => e.Timestamp == default))
        {
            entry.Timestamp = response.Timestamp;
        }

        // Фильтруем записи по временному интервалу (включая конечную границу)
        var filteredEntries = response.Data
            .Where(e => e.Timestamp >= startTime && e.Timestamp <= endTime)
            .ToList();

        if (filteredEntries.Count == 0) return;

        await ProcessFilteredEntries(response, filteredEntries);
    }

    private async Task ProcessFilteredEntries(EntryMTDataResponse originalResponse, List<EntryData> filteredEntries)
    {
        var filteredResponse = new EntryMTDataResponse
        {
            Message = originalResponse.Message,
            Timestamp = originalResponse.Timestamp,
            Count = filteredEntries.Count,
            Data = filteredEntries
        };

        try
        {
            _storageService.Set(filteredResponse);
            _logger.LogInformation($"Saved {filteredEntries.Count} entries");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving entries to storage");
            throw; // Пробрасываем исключение для повторной попытки
        }
    }

    private string BuildEmitterEndpoint(DateTime startTime)
    {
        DateTimeOffset startDto = new DateTimeOffset(startTime, TimeSpan.Zero);
        DateTimeOffset inPlus7 = startDto.ToOffset(TimeSpan.FromHours(7));

        return _connectionString + inPlus7.ToString("yyyy-MM-ddTHH:mm:ss");
    }
}