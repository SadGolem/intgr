using System.Globalization;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Entry.Storage;
using integration.Services.Interfaces;
using integration.Services.Location;
using Microsoft.Extensions.Options;

// ... ваши using'и

public class EntryMTGetterService : ServiceGetterBase<EntryMTDataResponse>, IGetterService<EntryMTDataResponse>
{
    private readonly string _connectionString;
    private readonly IOptions<AuthSettings> _apiSettings;
    private readonly ILogger<EntryMTGetterService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IEntryStorageService<EntryMTDataResponse> _storageService;

    private const int CHUNK_SIZE_MINUTES = 30;

    // ✅ Гарантия одного запроса на старт процесса
    private static bool _initialRequestDone = false;

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
        // ✅ 1) Всегда сделать один запрос при старте процесса
        if (!_initialRequestDone)
        {
            await InitialFetchFromZero();
            _initialRequestDone = true;
        }

        // ✅ 2) Дальше — ваша простая логика с чанками по 30 минут
        var lastUpdate = TimeManager.GetLastUpdateTime("entryMT");
        if (lastUpdate.Kind != DateTimeKind.Utc)
            lastUpdate = DateTime.SpecifyKind(lastUpdate, DateTimeKind.Utc).ToUniversalTime();

        var now = DateTime.UtcNow;

        if (lastUpdate > now)
        {
            _logger.LogWarning($"Last update time is in future: {lastUpdate:o}. Resetting to {CHUNK_SIZE_MINUTES} minutes ago.");
            lastUpdate = now.AddMinutes(-CHUNK_SIZE_MINUTES);
            TimeManager.SetLastUpdateTime("entryMT", lastUpdate);
        }

        while (true)
        {
            now = DateTime.UtcNow;

            // Стоп: если уже догнали последние 30 минут — ждём следующего запуска изнаружи
            if (lastUpdate >= now.AddMinutes(-CHUNK_SIZE_MINUTES))
                break;

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
                await Task.Delay(5000);
            }
        }
    }

    private static DateTime EnsureUtc(DateTime dt)
    {
        // Если сервер отдаёт без Kind (Unspecified), считаем это UTC.
        // Если у вас реально локальная зона сервера — замените на ConvertTimeToUtc(dt, вашаTZ).
        return dt.Kind == DateTimeKind.Utc 
            ? dt 
            : DateTime.SpecifyKind(dt, DateTimeKind.Utc);
    }

    private async Task InitialFetchFromZero()
    {
        var endpoint = _connectionString + "0";
        _logger.LogInformation("Initial fetch on process start: {endpoint}", endpoint);

        var response = await GetFullResponse<EntryMTDataResponse>(endpoint, false);

        if (response?.Data != null && response.Data.Count > 0)
        {
            // Проставляем Timestamp там, где он пуст, и нормализуем Kind=Utc
            foreach (var e in response.Data)
            {
                if (e.Timestamp == default)
                    e.Timestamp = response.Timestamp;

                e.Timestamp = EnsureUtc(e.Timestamp);
            }

            _storageService.Set(response);
            _logger.LogInformation("Initial fetch saved {count} entries.", response.Data.Count);
        }
        else
        {
            _logger.LogInformation("Initial fetch returned no data.");
        }

        // ✅ НОРМАЛИЗАЦИЯ ПЕРЕД Сохранением
        var ts = response?.Timestamp != default ? response.Timestamp : DateTime.UtcNow;
        var newLastUtc = EnsureUtc(ts);
        TimeManager.SetLastUpdateTime("entryMT", newLastUtc); // больше не упадёт
    }
    private async Task ProcessTimeChunk(DateTime startTime, DateTime endTime)
    {
        var endpoint = BuildEmitterEndpoint(startTime);
        var response = await GetFullResponse<EntryMTDataResponse>(endpoint, false);

        if (response?.Data == null) return;

        foreach (var entry in response.Data.Where(e => e.Timestamp == default))
            entry.Timestamp = response.Timestamp;

        // Полузакрытый интервал [start, end) — без дублей на границе
        var filteredEntries = response.Data
            .Where(e => e.Timestamp >= startTime && e.Timestamp < endTime)
            .ToList();

        if (filteredEntries.Count == 0) return;

        var filteredResponse = new EntryMTDataResponse
        {
            Message = response.Message,
            Timestamp = response.Timestamp,
            Count = filteredEntries.Count,
            Data = filteredEntries
        };

        _storageService.Set(filteredResponse);
        _logger.LogInformation($"Saved {filteredEntries.Count} entries");
    }

    private string BuildEmitterEndpoint(DateTime startTime)
    {
        var startDto = new DateTimeOffset(startTime, TimeSpan.Zero).ToOffset(TimeSpan.FromHours(7));
        return _connectionString + startDto.ToString("yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture);
    }
}
