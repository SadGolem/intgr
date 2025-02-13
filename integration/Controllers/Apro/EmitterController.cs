using integration.Context;
using integration.HelpClasses;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;

namespace integration.Controllers.Apro
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmitterController : ControllerBase
    {
        private readonly string _aproConnectSettings;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<EmitterController> _logger;
        private string url = "wf__wastesource__waste_source/?query={id, datetime_create, datetime_update,participant{id,name}, address, name, normative_unit_value_exist, status{id,name},waste_source_category{id}, author{name}}";

        public EmitterController(
            IHttpClientFactory httpClientFactory,
            ILogger<EmitterController> logger,
            IConfiguration configuration
            )
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            ConnectngStringApro connectngStringApro = new ConnectngStringApro(configuration, url);
            _aproConnectSettings = connectngStringApro.GetAproConnectSettings();
        }

        [HttpGet]
        public async Task<IActionResult> GetData()
        {
            _logger.LogInformation("Starting manual emitters sync...");
            try
            {
                var entries = await FetchAndProcess();
                if (entries == null)
                {
                    return StatusCode(500, "Error during emitters sync.");
                }
                return Ok(entries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during emitters sync.");
                ToGetMessage(ex + "Error during emitters sync.");
                return StatusCode(500, "Error during emitters sync.");
            }
        }

        private async Task<SyncResult> FetchAndProcess()
        {
            _logger.LogInformation($"Fetching emitters from {_aproConnectSettings}...");
            List<EmitterData> emitters;
            try
            {
                emitters = await FetchData();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during emitters fetch");
                ToGetMessage(ex + "Error during emitters fetch");
                return null;
            }

            _logger.LogInformation($"Received {emitters.Count} emitters");
            ToGetMessage($"Received {emitters.Count} emitters");
            var lastUpdate = LastUpdateTextFileManager.GetLastUpdateTime("emitter");
            var newEmitter = new List<EmitterData>();
            var updateEntries = new List<EmitterData>();

            foreach (var emitter in emitters)
            {
                if (emitter.client != null)
                {
                    if (emitter.datetime_update > lastUpdate)
                    {
                        updateEntries.Add(emitter);
                    }
                    else if (emitter.datetime_create > lastUpdate)
                    {
                        newEmitter.Add(emitter);
                    }
                }
            }
            return new SyncResult(newEmitter, updateEntries);
        }

        private async Task<List<EmitterData>> FetchData()
        {
            var emitters = new List<EmitterData>();
            var token = await TokenController._authorizer.GetCachedTokenAPRO();
            using var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                var response = await httpClient.GetAsync(_aproConnectSettings);

                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                emitters = JsonSerializer.Deserialize<List<EmitterData>>(content);
                LastUpdateTextFileManager.SetLastUpdateTime("emitter");
                ToGetMessage(content);
                return emitters;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"HTTP Error fetching emitters. Status code {ex.StatusCode}, URL: {_aproConnectSettings} ");
                ToGetMessage(ex + $"HTTP Error fetching emitters. Status code {ex.StatusCode}, URL: {_aproConnectSettings} ");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching or processing emitters from: {_aproConnectSettings}");
                ToGetMessage(ex + $"Error fetching or processing emitters from: {_aproConnectSettings}");
                throw;
            }
        }

        void ToGetMessage(string ex)
        {
            EmailMessageBuilder.PutInformation(EmailMessageBuilder.ListType.getemitter, ex);
        }
    }
    public record SyncResult(List<EmitterData> NewEntries, List<EmitterData> UpdateEntries);
}

        


