using integration.Context.MT;
using integration.Context.Request;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Agre.Storage;
using integration.Services.Interfaces;
using integration.Services.Location;
using Microsoft.Extensions.Options;

namespace integration.Services.Agre;

public class AgreMTGetterService : ServiceGetterBase<AgreMTDataResponse>,
    IGetterService<AgreMTDataResponse>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AgreMTGetterService> _logger;
    private readonly string _apiSettings;
    private readonly string _connectionStringGetLocation;
    private readonly IAgreStorageService _storage;
    private AgreMTDataResponse agre;
    private IMessageBroker _messageBroker;
    private const int CHUNK_SIZE_MINUTES = 30;

    public AgreMTGetterService(IHttpClientFactory httpClientFactory,
        ILogger<AgreMTGetterService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> apiSettings,
        IAgreStorageService storage,
        IMessageBroker messageBroker) : base(httpClientFactory, logger, authorizer, apiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _apiSettings = apiSettings.Value.MTconnect.BaseUrl +
                       apiSettings.Value.MTconnect.ApiClientSettings.AgreEndpointGet;
        _storage = storage;
        _connectionStringGetLocation = apiSettings.Value.APROconnect.BaseUrl
                                       + apiSettings.Value.APROconnect.ApiClientSettings.LocationGetEndpoint;
        _messageBroker = messageBroker;
    }

    public async Task Get()
    {
        await GetAgre();
    }

    public async Task GetAgre()
    {
        var now = DateTime.UtcNow;
        var lastUpdate = TimeManager.GetLastUpdateTime("agreMT");

        if (lastUpdate == DateTime.MinValue)
        {
            _logger.LogInformation("First run detected. Setting last update to 30 minutes ago.");
            lastUpdate = now.AddMinutes(-CHUNK_SIZE_MINUTES);
            TimeManager.SetLastUpdateTime("agreMT", lastUpdate);
        }
        else if (lastUpdate > now)
        {
            _logger.LogWarning($"Last update time is in future: {lastUpdate}. Resetting to 30 minutes ago.");
            lastUpdate = now.AddMinutes(-CHUNK_SIZE_MINUTES);
            TimeManager.SetLastUpdateTime("agreMT", lastUpdate);
        }

        while (lastUpdate <= now)
        {
            var chunkEnd = lastUpdate.AddMinutes(CHUNK_SIZE_MINUTES);

            try
            {
                _logger.LogInformation($"Processing agre time range: {lastUpdate:o} - {chunkEnd:o}");
                await ProcessTimeChunk(lastUpdate, chunkEnd);

                TimeManager.SetLastUpdateTime("agreMT", chunkEnd);
                lastUpdate = chunkEnd;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to process agre time chunk: {lastUpdate:o} - {chunkEnd:o}");
                await Task.Delay(5000);
                continue;
            }

            if (lastUpdate < now)
            {
                await Task.Delay(500);
            }
        }
    }

      private async Task ProcessTimeChunk(DateTime startTime, DateTime endTime)
    {
        var endpoint = BuildEmitterEndpoint(startTime);
        var responses = await GetFullResponse<AgreMTDataResponse>(endpoint, false);
        if (responses?.Data == null || responses.Data.Count == 0) return;

        // Проставим Timestamp там, где пусто
        foreach (var agre in responses.Data.Where(a => a.Timestamp == default))
            agre.Timestamp = responses.Timestamp;

        // Здесь будем копить успешно обработанные sourceKey
        var processedSourceKeys = new List<string>(responses.Data.Count);

        foreach (var agre in responses.Data)
        {
            try
            {
                var locationEndpoint = BuildLocationEndpoint(agre.idLocation);
                var locResponse = await Get<Context.Location>(locationEndpoint, true);

                _storage.Set((agre, Convert.ToInt32(locResponse.FirstOrDefault()?.id)));
                processedSourceKeys.Add(agre.idLocation); // <— собираем sourceKey
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing agre with idLocation={IdLocation}", agre.idLocation);
            }
        }

        _logger.LogInformation("Processed {Count} agre records", responses.Data.Count);

        // Отправляем ACK только если есть, что подтверждать
        if (processedSourceKeys.Count > 0)
        {
            try
            {
                // Отправляем массив вида ["5","6","9"] в МТ
                var ack = await _messageBroker.PublishAckAsync(processedSourceKeys);

                // На стороне сервиса – просто логируем успешный ответ брокера
                if (!string.IsNullOrWhiteSpace(ack.Message))
                {
                    _logger.LogInformation("ACK from MT: {Message}; ts={Ts:o}; data={Data}",
                        ack.Message, ack.Timestamp, ack.Data);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish ACK for {Count} sourceKeys", processedSourceKeys.Count);
            }
        }
    }

    private string BuildEmitterEndpoint(DateTime startTime)
    {
        DateTimeOffset startDto = new DateTimeOffset(startTime, TimeSpan.Zero);
        DateTimeOffset inPlus7 = startDto.ToOffset(TimeSpan.FromHours(7));
        return _apiSettings + inPlus7.ToString("yyyy-MM-ddTHH:mm:ss");
    }

    private string BuildLocationEndpoint(string loc)
    {
        return _connectionStringGetLocation + "?query={id}&ext_id_2=" + loc;
    }
}
