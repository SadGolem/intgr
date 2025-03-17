using System.Text.Json;
using integration.Context;
using integration.HelpClasses;
using integration.Services.ContractPosition.Storage;
using integration.Services.Interfaces;
using integration.Services.Location;

namespace integration.Services.Schedule;

public class ScheduleGetterService : ServiceBase, IGetterService<ScheduleData>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ScheduleGetterService> _logger; // Correct logger type
    private readonly IScheduleStorageService _scheduleStorageService;
    private readonly IContractPositionStorage _contractPositionStorage;
    private ConnectngStringApro _aproConnect;
    private string asuPro = "https://test.asu2.big3.ru/api/wf__wastesitescheduleset__waste_site_schedule_set/?position=3215337&query={id,waste_site{id},containers{id},schedule, dates}";
    
    public ScheduleGetterService(IHttpClientFactory httpClientFactory, HttpClient httpClient, ILogger<ScheduleGetterService> logger, IConfiguration configuration, IScheduleStorageService scheduleStorageService, IContractPositionStorage contractPositionStorage)
        : base(httpClientFactory, httpClient, logger, configuration)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _aproConnect = new ConnectngStringApro(configuration, asuPro);
        _scheduleStorageService = scheduleStorageService;
        _contractPositionStorage = contractPositionStorage;
    }

    public async Task Get()
    {
        _logger.LogInformation($"Try getting scheduls from {_aproConnect}...");
        var schedules = new List<ScheduleData>();
        var positions = _contractPositionStorage.GetPosition();
        foreach (var pos in positions)
        {
           // var url = _aproConnect.ReplaceStringUtlWithoutDate("3215337", pos.id.ToString());
           var url = asuPro.Replace("3215337", pos.id.ToString());
            try
            {
                using var httpClient = _httpClientFactory.CreateClient();
                await Authorize(httpClient, true);
                var response = await httpClient.GetAsync(url);

                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                schedules = await JsonSerializer.DeserializeAsync<List<ScheduleData>>(
                    await response.Content.ReadAsStreamAsync(), options);
                _scheduleStorageService.SetSchedules(schedules);
                Message("Got schedules: " + content);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Error during GET request to {_aproConnect}");
                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, $"Error during JSON deserialization of response from {_aproConnect}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while fetching data from {_aproConnect}");
                throw;
            }
        }
    }

    public override void Message(string ex)
    {
        EmailMessageBuilder.PutInformation(EmailMessageBuilder.ListType.getschedule, ex);
    }
}