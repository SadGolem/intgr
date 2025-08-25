using System.Globalization;
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
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IEntryStorageService<EntryMTDataResponse> _storageService;

    private const int CHUNK_SIZE_MINUTES = 30;
    
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
        if (!_initialRequestDone)
        {
            await InitialFetchFromZero();
            _initialRequestDone = true;
        }

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

            if (lastUpdate >= now.AddMinutes(-CHUNK_SIZE_MINUTES))
                break;

            var chunkEnd = lastUpdate.AddMinutes(CHUNK_SIZE_MINUTES);
            if (chunkEnd > now) chunkEnd = now;

            try
            {
                _logger.LogInformation($"Processing time range: {lastUpdate:o} - {chunkEnd:o}");
                var maxProcessed = await ProcessTimeChunk(lastUpdate, chunkEnd);

                if (maxProcessed.HasValue)
                {
                    lastUpdate = EnsureUtc(maxProcessed.Value);
                    TimeManager.SetLastUpdateTime("entryMT", lastUpdate);
                }
                else
                {
                    // ничего нового — НЕ двигаем lastUpdate, чтобы не пропустить поздние записи с этим же окном
                    // на следующем запуске окно повторим (это безопасно из‑за per‑ID дедупликации)
                    break;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to process time chunk: {lastUpdate:o} - {chunkEnd:o}");
                await Task.Delay(5000);
            }
        }

    }

    private async Task InitialFetchFromZero()
    {
        var endpoint = _connectionString + "0";
        _logger.LogInformation("Initial fetch on process start: {endpoint}", endpoint);

        var response = await GetFullResponse<EntryMTDataResponse>(endpoint, false);

        DateTime? maxTs = null;

        if (response?.Data != null && response.Data.Count > 0)
        {
            // нормализуем
            foreach (var e in response.Data)
            {
                if (e.Timestamp == default) continue;
                e.Timestamp = EnsureUtc(e.Timestamp);
            }

            // фильтрация НА ПЕРВЫЙ ЗАПУСК по per-id (на случай повторного старта процесса)
            var fresh = response.Data
                .Where(e => e.Timestamp != default)
                .Where(e => e.Timestamp > GetPerIdOffset(e.id))
                .ToList();

            if (fresh.Count > 0)
            {
                // сохраняем только новые/обновлённые
                var filteredResponse = new EntryMTDataResponse
                {
                    Message = response.Message,
                    Timestamp = DateTime.UtcNow,
                    Count = fresh.Count,
                    Data = fresh
                };

                _storageService.Set(filteredResponse);
                _logger.LogInformation("Initial fetch saved {count} entries.", fresh.Count);

                // пер‑ID фиксация
                foreach (var e in fresh)
                    SetPerIdOffset(e.id, e.Timestamp);

                maxTs = fresh.Max(e => e.Timestamp);
            }
            else
            {
                _logger.LogInformation("Initial fetch has no fresh entries.");
            }
        }
        else
        {
            _logger.LogInformation("Initial fetch returned no data.");
        }

        // глобальный lastUpdate двигаем только если что‑то действительно обработали
        if (maxTs.HasValue)
            TimeManager.SetLastUpdateTime("entryMT", EnsureUtc(maxTs.Value));
        else
            TimeManager.SetLastUpdateTime("entryMT", EnsureUtc(DateTime.UtcNow));
    }


    private async Task<DateTime?> ProcessTimeChunk(DateTime startTime, DateTime endTime)
    {
        var endpoint = BuildEmitterEndpoint(startTime);
        var response = await GetFullResponse<EntryMTDataResponse>(endpoint, false);

        if (response?.Data == null || response.Data.Count == 0)
            return null;

        // нормализуем
        foreach (var e in response.Data)
            if (e.Timestamp != default)
                e.Timestamp = EnsureUtc(e.Timestamp);

        // 1) только новые по времени окна
        // 2) и только те, что ещё НЕ попадали (пер‑ID watermark)
        var fresh = response.Data
            .Where(e => e.Timestamp != default)
            .Where(e => e.Timestamp > startTime && e.Timestamp < endTime)
            .Where(e => e.Timestamp > GetPerIdOffset(e.id))
            .ToList();

        if (fresh.Count == 0)
            return null;

        var filteredResponse = new EntryMTDataResponse
        {
            Message = response.Message,
            Timestamp = DateTime.UtcNow, // для совместимости модели, не участвует в логике
            Count = fresh.Count,
            Data = fresh
        };

        _storageService.Set(filteredResponse);
        _logger.LogInformation("Saved {count} fresh entries", fresh.Count);

        // пер‑ID фиксация
        foreach (var e in fresh)
            SetPerIdOffset(e.id, e.Timestamp);

        // вернём максимум фактически обработанных таймштампов
        return fresh.Max(e => e.Timestamp);
    }

    private static DateTime EnsureUtc(DateTime dt) =>
        dt.Kind == DateTimeKind.Utc ? dt : DateTime.SpecifyKind(dt, DateTimeKind.Utc);

    private static DateTime GetPerIdOffset(int id)
    {
        var t = TimeManager.GetLastUpdateTime($"entryMT:{id}");
        return t == default ? DateTime.MinValue : EnsureUtc(t);
    }

    private static void SetPerIdOffset(int id, DateTime tsUtc)
    {
        TimeManager.SetLastUpdateTime($"entryMT:{id}", EnsureUtc(tsUtc));
    }

    private string BuildEmitterEndpoint(DateTime startTime)
    {
        var startDto = new DateTimeOffset(startTime, TimeSpan.Zero).ToOffset(TimeSpan.FromHours(7));
        return _connectionString + startDto.ToString("yyyy-MM-dd'T'HH:mm:ss", CultureInfo.InvariantCulture);
    }
}
