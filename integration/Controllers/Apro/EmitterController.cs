using integration.Context;
using integration.Factory.GET.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace integration.Controllers.Apro
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmitterController : BaseSyncController<EmitterDataResponse>
    {
        public EmitterController(
            ILogger<EmitterController> logger,
            IGetterServiceFactory<EmitterDataResponse> serviceGetter)
            : base(logger, serviceGetter) { }
    
        public async Task<IActionResult> Sync()
        {
            return await base.Sync();
        }
        /*public async Task<IActionResult> GetData()
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
        }*/

        /*private async Task<SyncResult> FetchAndProcess()
        {
            _logger.LogInformation($"Fetching emitters from {_aproConnectSettings}...");
            List<EmitterDataResponse> emitters;
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
            var lastUpdate = TimeManager.GetLastUpdateTime("emitter");
            var newEmitter = new List<EmitterDataResponse>();
            var updateEntries = new List<EmitterDataResponse>();

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
        }*/

        /*private async Task<List<EmitterDataResponse>> FetchData()
        {
            var emitters = new List<EmitterDataResponse>();
            var token = await TokenController._authorizer.GetCachedTokenAPRO();
            using var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            try
            {
                var response = await httpClient.GetAsync(_aproConnectSettings);

                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                emitters = JsonSerializer.Deserialize<List<EmitterDataResponse>>(content);
                TimeManager.SetLastUpdateTime("emitter");
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
        */

        void ToGetMessage(string ex)
        {
            EmailMessageBuilder.PutInformation(EmailMessageBuilder.ListType.getemitter, ex);
        }
    }
}

        


