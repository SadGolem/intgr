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
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        public static List<EntryData> newEntry = new List<EntryData>();
        public static List<EntryData> updateEntry = new List<EntryData>();

        public WasteSiteEntryController(HttpClient httpClient, ILogger<WasteSiteEntryController> logger, IMemoryCache memoryCache, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _memoryCache = memoryCache;
            _configuration = configuration;
            _aproConnectSettings = _configuration.GetSection("APROconnect").Get<AuthSettings>().CallbackUrl.ToString()
                .Replace("token-auth/", "wf__wastetakeoutrequest__garbage_collection_request/?query={id, datetime_create, datetime_update,client_contact, author{name},status,volume,date, capacity{capacity},type{id,name},ext_id, comment}");
        }

        [HttpGet]
        public async Task<IActionResult> GetEntriesData()
        {
            _logger.LogInformation("Starting manual entry sync...");
            newEntry.Clear();
            try
            {
                await FetchAndPostEntry();
                return Ok("Locations synced successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during location sync.");
                return StatusCode(500, "Error during location sync.");
            }
        }

        private async Task FetchAndPostEntry()
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
                    if (entry.DateTimeUpdate > entry.DateTimeCreate)
                    {
                        updateEntry.Add(entry);
                    }
                    else
                    {
                        newEntry.Add(entry);
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

                entries = JsonSerializer.Deserialize<List<EntryData>>(content);

                return entries;
            }
            catch (HttpRequestException ex)
            {
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
    }
}