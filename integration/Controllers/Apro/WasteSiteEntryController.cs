using Microsoft.Extensions.Caching.Memory;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using integration.Context;
using Microsoft.Extensions.Options;

namespace integration.Controllers.Apro
{
    [ApiController]
    [Route("api/[controller]")]
    public class WasteSiteEntryController : ControllerBase
    {
        private string _aproConnectSettings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<WasteSiteEntryController> _logger;
        private readonly IConfiguration _configuration;
        public static List<EntryData> newEntry = new List<EntryData>();
        public static List<EntryData> updateEntry = new List<EntryData>();
        private string url = "wf__wastetakeoutrequest__garbage_collection_request/?query={id, datetime_create, datetime_update,waste_site{id},client_contact{id,name}, author{name},status,volume,date, capacity{id},type{id,name},ext_id, comment, containers{id}}";

        public WasteSiteEntryController(HttpClient httpClient, ILogger<WasteSiteEntryController> logger, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
            ConnectngStringApro _connectngStringApro = new ConnectngStringApro(_configuration, url);
            _aproConnectSettings = _connectngStringApro.GetAproConnectSettings();
        }

        [HttpGet]
        public async Task<IActionResult> GetEntriesData()
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
        }

        private async Task FetchEntry()
        {
            _logger.LogInformation($"Fetching locations from {_aproConnectSettings}...");
            List<EntryData> entries = new List<EntryData>();
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
            var lastUpdate = LastUpdateTextFileManager.GetLastUpdateTime("entry");

            foreach (var entry in entries)
            {
                if (entry.DateTimeCreate > lastUpdate || entry.DateTimeUpdate > lastUpdate)
                {
                    //надо найти объем
                    if (entry.DateTimeCreate > lastUpdate) //здесь менять логику незлья, так как у них  апдейт чуть позже криеэйт
                    {
                        newEntry.Add(entry);
                    }
                    else if (entry.DateTimeUpdate > lastUpdate)
                    {
                        updateEntry.Add(entry);
                    }
                }
            }
        }

        private async Task<List<EntryData>> FetchEntryData()
        {
            var entries = new List<EntryData>();
            var token = await TokenController._authorizer.GetCachedTokenAPRO();
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            try
            {
                var response = await _httpClient.GetAsync(_aproConnectSettings);

                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                ToMessage(content);
                entries = JsonSerializer.Deserialize<List<EntryData>>(content);

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

        void ToMessage(string ex)
        {
            EmailMessageBuilder.PutInformation(EmailMessageBuilder.ListType.GetEntryInfo, ex);
        }
    }
}