using System.Text.Json;
using integration.Context;
using integration.HelpClasses;
using integration.Services;
using integration.Services.ContractPosition.Storage;
using integration.Services.Interfaces;
using integration.Services.Schedule;
using Microsoft.Extensions.Options;

public class ScheduleGetterService : ServiceBase, IGetterService<ScheduleData>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ScheduleGetterService> _logger;
    private readonly IScheduleStorageService _scheduleStorage;
    private readonly IContractPositionStorageService _positionStorage;
    private readonly AuthSettings _apiSettings;
    private readonly JsonSerializerOptions _jsonOptions;

    public ScheduleGetterService(
        IHttpClientFactory httpClientFactory,
        ILogger<ScheduleGetterService> logger,
        IOptions<AuthSettings> apiSettings,
        IScheduleStorageService scheduleStorage,
        IContractPositionStorageService positionStorage
    )
        : base(httpClientFactory, logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _scheduleStorage = scheduleStorage;
        _positionStorage = positionStorage;
        _apiSettings = apiSettings.Value;

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

    private async Task ProcessPositionAsync(ContractPositionData position)
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

    private async Task<List<ScheduleData>> FetchSchedulesAsync(int positionId)
    {
        using var client = _httpClientFactory.CreateClient();
        await AuthorizeClient(client);

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

    private async Task<List<ScheduleData>> DeserializeResponseAsync(HttpResponseMessage response)
    {
        try
        {
            await using var stream = await response.Content.ReadAsStreamAsync();
            var result = await JsonSerializer.DeserializeAsync<List<ScheduleData>>(stream, _jsonOptions);
            return result ?? new List<ScheduleData>();
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to deserialize schedules response");
            throw;
        }
    }

    private async Task AuthorizeClient(HttpClient client)
    {
        try
        {
            // Реализация авторизации из базового класса
            await base.Authorize(client, useCache: true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Authorization failed");
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

    public override Task HandleErrorAsync(string errorMessage)
    {
        throw new NotImplementedException();
    }
}