using integration.Context;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Net.Http.Headers;
using System.Text.Json;
using integration.HelpClasses;

namespace integration.Controllers.Apro
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly string _aproConnectSettings;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ClientController> _logger;
        private string url = "wf__waste_site_schedule_set__waste_site_schedule_set/?query={id,waste_site{id},datetime_create, datetime_update, dates, schedule, containers{id, type {id}}}";

        public ScheduleController(
            IHttpClientFactory httpClientFactory,
            ILogger<ClientController> logger,
            IConfiguration configuration
            )
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            ConnectngStringApro connectngStringApro = new ConnectngStringApro(configuration, url);
            _aproConnectSettings = connectngStringApro.GetAproConnectSettings();
        }

        [HttpGet]
        public async Task<IActionResult> GetScheduleData()
        {
            _logger.LogInformation("Starting manual schedule sync...");
            try
            {
                var contragent = await FetchAndProcessSchedule();
                if (contragent == null)
                {
                    return StatusCode(500, "Error during schedule sync.");
                }
                return Ok(contragent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during schedule sync.");
                return StatusCode(500, "Error during schedule sync.");
            }
        }

        private async Task<SyncResult<ScheduleData>> FetchAndProcessSchedule()
        {
            _logger.LogInformation($"Fetching schedule from {_aproConnectSettings}...");
            List<ScheduleData> schedules;
            try
            {
                schedules = await FetchEntryData();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during schedule fetch");
                return null;
            }

            _logger.LogInformation($"Received {schedules.Count} schedules");
            var lastUpdate = LastUpdateTextFileManager.GetLastUpdateTime("schedule");
            var newSchedules = new List<ScheduleData>();
            var updateSchedules = new List<ScheduleData>();

            foreach (var contr in schedules)
            {
                if (contr.datetime_update > lastUpdate)
                {
                    updateSchedules.Add(contr);
                }
                else if (contr.datetime_create > lastUpdate)
                {
                    newSchedules.Add(contr);
                }
            }
            return new SyncResult<ScheduleData>(newSchedules, updateSchedules);
        }

        private async Task<List<ScheduleData>> FetchEntryData()
        {
            var schedules = new List<ScheduleData>();
            var token = await TokenController._authorizer.GetCachedTokenAPRO();
            using var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                var response = await httpClient.GetAsync(_aproConnectSettings);

                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                schedules = JsonSerializer.Deserialize<List<ScheduleData>>(content);
                LastUpdateTextFileManager.SetLastUpdateTime("schedule");
                ToGetMessage(content);
                return schedules;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"HTTP Error fetching schedule. Status code {ex.StatusCode}, URL: {_aproConnectSettings} ");
                ToGetMessage(ex + $"HTTP Error fetching schedule. Status code {ex.StatusCode}, URL: {_aproConnectSettings} ");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching or processing schedule from: {_aproConnectSettings}");
                ToGetMessage(ex + $"Error fetching or processing schedule from: {_aproConnectSettings} ");
                throw;
            }
        }

        void ToGetMessage(string ex)
        {
            EmailMessageBuilder.PutInformation(EmailMessageBuilder.ListType.getschedule, ex);
        }
    }

    public record SyncResult<T>(List<T> NewSchedule, List<T> UpdateScedule) where T : Data;
}


