using System.Text.Json;
using integration.Context;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services;
using integration.Services.ContractPosition.Storage;
using integration.Services.Interfaces;
using integration.Services.Schedule;
using Microsoft.Extensions.Options;

public class ScheduleGetterService : ServiceBase, IGetterService<ScheduleDataResponse>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ScheduleGetterService> _logger;
    private readonly IScheduleStorageService _scheduleStorage;
    private readonly IContractPositionStorageService _positionStorage;
    private readonly APROconnectSettings _apiSettings;
    private readonly JsonSerializerOptions _jsonOptions;

    public ScheduleGetterService(
        IHttpClientFactory httpClientFactory,
        ILogger<ScheduleGetterService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> apiSettings,
        IScheduleStorageService scheduleStorage,
        IContractPositionStorageService positionStorage
    )
        : base(httpClientFactory, logger, authorizer, apiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _scheduleStorage = scheduleStorage;
        _positionStorage = positionStorage;
        _apiSettings = apiSettings.Value.APROconnect;

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };
    }

    public async Task Get()
    {
        _logger.LogInformation("Starting schedules synchronization");

        var positions = _positionStorage.GetPosition();
        if (!positions.Any())
        {
            _logger.LogWarning("No contract positions found for schedules sync");
            return;
        }

        var tasks = positions.Select(ProcessPositionAsync);
        await Task.WhenAll(tasks);
    }

    private async Task ProcessPositionAsync(ContractPositionDataResponse position)
    {
        try
        {
            var schedules = await FetchSchedulesAsync(position.id);
            _scheduleStorage.SetSchedules(schedules);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing position {PositionId}", position.id);
        }
    }

    private async Task<List<ScheduleDataResponse>> FetchSchedulesAsync(int positionId)
    {
        using var client  = await Authorize(true);

        var endpoint = BuildEndpointUrl(positionId);
        var response = await client.GetAsync(endpoint);

        response.EnsureSuccessStatusCode();

        return await DeserializeResponseAsync(response);
    }

    private string BuildEndpointUrl(int positionId)
    {
        return $"{_apiSettings.BaseUrl}wf__wastesitescheduleset__waste_site_schedule_set/" +
               $"?position={positionId}" +
               "&query={id,waste_site{id},containers{id},schedule,dates}";
    }

    private async Task<List<ScheduleDataResponse>> DeserializeResponseAsync(HttpResponseMessage response)
    {
        try
        {
            await using var stream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<List<ScheduleDataResponse>>(stream, _jsonOptions);
            return result ?? new List<ScheduleDataResponse>();
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize schedules response");
            throw;
        }
    }
    
    public void Message(string message)
    {
        EmailMessageBuilder.PutInformation(
            EmailMessageBuilder.ListType.getschedule,
            $"Schedule sync message: {message[..50]}..." // Обрезаем длинные сообщения
        );
    }
}