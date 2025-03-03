using integration.Context;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using integration.Factory.GET.Interfaces;
using integration.HelpClasses;
using integration.Services.Interfaces;

namespace integration.Controllers.Apro
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleController :  ControllerBase, IController
    {
        private readonly string _aproConnectSettings;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<ScheduleController> _logger;
        private IGetterService<ScheduleData> _scheduleGetterService;
        private readonly IGetterServiceFactory<ScheduleData> _serviceGetter;
        private string url = "wf__waste_site_schedule_set__waste_site_schedule_set/?query={id,waste_site{id},datetime_create, datetime_update, dates, schedule, containers{id, type {id}}}";

        public ScheduleController(
            IHttpClientFactory httpClientFactory,
            ILogger<ScheduleController> logger,
            IConfiguration configuration, IGetterServiceFactory<ScheduleData> serviceGetter
            )
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            ConnectngStringApro connectngStringApro = new ConnectngStringApro(configuration, url);
            _aproConnectSettings = connectngStringApro.GetAproConnectSettings();
            _serviceGetter = serviceGetter;
        }
        
        public async Task<IActionResult> Sync()
        {
            _logger.LogInformation("Starting manual schedule sync...");
            try
            {
                await Fetcht();
                
                return Ok("Schedules synced successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during schedule sync.");
                return StatusCode(500, "Error during schedule sync.");
            }
        }
        private async Task Fetcht()
        {
            _scheduleGetterService = _serviceGetter.Create();
            await _scheduleGetterService.Get();

            // await PostOrPatch(locations);
            /*_locationIdService.SetLocation(locations);
            _logger.LogInformation($"Received {locations.Count} locations");*/
        }

        void ToGetMessage(string ex)
        {
            EmailMessageBuilder.PutInformation(EmailMessageBuilder.ListType.getschedule, ex);
        }
    }
}


