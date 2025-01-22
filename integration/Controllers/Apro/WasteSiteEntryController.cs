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
        private const string LastUpdateKey = "LastWasteDataUpdate";
        private readonly IConfiguration _configuration;
        public static List<EntryData> newEntry = new List<EntryData>();

        public WasteSiteEntryController(HttpClient httpClient, ILogger<WasteSiteEntryController> logger, IMemoryCache memoryCache, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _memoryCache = memoryCache;
            _configuration = configuration;
            _aproConnectSettings = _configuration.GetSection("APROconnect").Get<AuthSettings>().CallbackUrl.ToString()
                .Replace("token-auth/", "wf__wastetakeoutrequest__garbage_collection_request/?query={id, datetime_create, datetime_update,client_contact, author{name},status,volume,date, capacity{capacity},type{id,name}}");
        }

        [HttpGet]
        public async Task<IActionResult> GetEntriesData()
        {
            _logger.LogInformation("Starting manual entry sync...");
            newEntry.Clear();
            try
            {
                await FetchAndPostLocations();
                return Ok("Locations synced successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during location sync.");
                return StatusCode(500, "Error during location sync.");
            }
        }

        private async Task FetchAndPostLocations()
        {
            _logger.LogInformation($"Fetching locations from {_aproConnectSettings}...");
            List<EntryData> entries = new List<EntryData>();
            try
            {
                entries = await FetchLocationData();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during locations fetch");
                return;
            }

            _logger.LogInformation($"Received {entries.Count} locations");
            var lastUpdate = GetLastUpdateTime();

            foreach (var entry in entries)
            {
                if (entry.DateTimeCreate > lastUpdate || entry.DateTimeUpdate > lastUpdate)
                {
                    newEntry.Add(entry);
                }
            }
        }

        private async Task<List<EntryData>> FetchLocationData()
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

       /* private async Task<List<EntryData>> FetchNewData(DateTime lastUpdate)
        {
            var apiUrl = _aproConnectSettings.CallbackUrl.Replace("token-auth", "wf__wastetakeoutrequest__garbage_collection_request");
            string token = "";
            try
            {
                token = await TokenController._authorizer.GetCachedTokenAPRO();
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Can not fetch token");
                return new List<EntryData>();
            }

            var newWasteData = new List<EntryData>();
            try
            {
                var response = await _httpClient.GetAsync(apiUrl);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                
                var result = JsonSerializer.Deserialize<WasteDataResponse>(content, options);
                if (result != null && result.Results != null)
                {
                    foreach (var data in result.Results)
                    {
                        if (data.datetime_create > lastUpdate || data.datetime_update > lastUpdate)
                        {
                            newWasteData.Add(data);
                        }
                    }
                }
                return newWasteData;
            }*/
 /*           catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Error getting data from API: {apiUrl}");
                return new List<EntryData>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, $"Error while deserializing the response from: {apiUrl}");
                return new List<EntryData>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while getting new data from : {apiUrl}");
                return new List<EntryData>();
            }
        }*/

        private DateTime GetLastUpdateTime()
        {
            if (_memoryCache.TryGetValue(LastUpdateKey, out DateTime lastUpdate))
            {
                return lastUpdate;
            }
            return DateTime.MinValue;
        }

        private void SetLastUpdateTime(DateTime lastUpdate)
        {
            _memoryCache.Set(LastUpdateKey, lastUpdate, TimeSpan.FromHours(1));
        }
    }
}