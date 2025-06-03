using System.Text.Json;
using integration.Factory.GET.Interfaces;
using Microsoft.AspNetCore.Mvc;
using integration.Helpers;
using integration.Helpers.Auth;
using Microsoft.Extensions.Options;

namespace integration.Controllers.Apro
{
    [ApiController]
    [Route("api/[controller]")]
    public class WasteSiteEntryController : BaseSyncController<EntryDataResponse>
    {
        private string _aproConnectSettings;
        private readonly ILogger<WasteSiteEntryController> _logger;
        public static List<EntryDataResponse> newEntry = new List<EntryDataResponse>();
        public static List<EntryDataResponse> updateEntry = new List<EntryDataResponse>();
        
        public WasteSiteEntryController(
            ILogger<WasteSiteEntryController> logger,
            IGetterServiceFactory<EntryDataResponse> serviceGetter)
            : base(logger, serviceGetter) { }
        
        public async Task<IActionResult> Sync()
        {
            return await base.Sync();
        }

        [HttpGet]
        /*public async Task<IActionResult> GetEntriesData()
        {
            _logger.LogInformation("Starting manual entry sync...");
            newEntry.Clear();
            try
            {
                await FetchEntry();
                return Ok("Locations synced successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during location sync.");
                return StatusCode(500, "Error during location sync.");
            }
        }*/

        /*private async Task FetchEntry()
        {
            _logger.LogInformation($"Fetching locations from {_aproConnectSettings}...");
            List<EntryDataResponse> entries = new List<EntryDataResponse>();
            try
            {
                entries = await FetchEntryData();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during locations fetch");
                return;
            }

            _logger.LogInformation($"Received {entries.Count} locations");
            var lastUpdate = TimeManager.GetLastUpdateTime("entry");

            foreach (var entry in entries)
            {
                if (entry.datetime_create > lastUpdate || entry.datetime_update > lastUpdate)
                {
                    //надо найти объем
                    if (entry.datetime_create > lastUpdate) //здесь менять логику незлья, так как у них  апдейт чуть позже криеэйт
                    {
                        newEntry.Add(entry);
                    }
                    else if (entry.datetime_update > lastUpdate)
                    {
                        updateEntry.Add(entry);
                    }
                }
            }
        }
        */

        /*
        private async Task<List<EntryDataResponse>> FetchEntryData()
        {
            var entries = new List<EntryDataResponse>();
            var token = await TokenController._authorizer.GetCachedTokenAPRO();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            try
            {
                var response = await _httpClient.GetAsync(_aproConnectSettings);

                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                ToMessage(content);
                entries = JsonSerializer.Deserialize<List<EntryDataResponse>>(content);

                return entries;
            }
            catch (HttpRequestException ex)
            {
                ToMessage(ex.Message.ToString());
                _logger.LogError(ex, $"Error during GET request to {_aproConnectSettings}");
                throw;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, $"Error during JSON deserialization of response from {_aproConnectSettings}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while fetching data from {_aproConnectSettings}");
                throw;
            }
        }
        */

        void ToMessage(string ex)
        {
            EmailMessageBuilder.PutInformation(EmailMessageBuilder.ListType.getentry, ex);
        }
    }
}