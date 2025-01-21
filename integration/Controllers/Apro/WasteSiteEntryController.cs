using Microsoft.Extensions.Caching.Memory;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using integration.Context;

namespace integration.Controllers.Apro
{
    [ApiController]
    [Route("api/[controller]")]
    public class WasteSiteEntryController : ControllerBase
    {
        private AuthSettings _aproConnectSettings;
        private readonly HttpClient _httpClient;
        private readonly ILogger<WasteSiteEntryController> _logger;
        private readonly IMemoryCache _memoryCache;
        private const string LastUpdateKey = "LastWasteDataUpdate";
        private readonly IConfiguration _configuration;

        public WasteSiteEntryController(HttpClient httpClient, ILogger<WasteSiteEntryController> logger, IMemoryCache memoryCache, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _memoryCache = memoryCache;
            _configuration = configuration;
            _aproConnectSettings = _configuration.GetSection("APROconnect").Get<AuthSettings>();
        }

        [HttpGet]
        public async Task<List<WasteData>> GetNewWasteData()
        {
            _logger.LogInformation("Start getting new waste data...");
            var lastUpdate = GetLastUpdateTime();

            var newWasteData = await FetchNewData(lastUpdate);
            if (newWasteData.Count > 0)
            {
                SetLastUpdateTime(DateTime.Now);
            }
            return newWasteData;
        }

        private async Task<List<WasteData>> FetchNewData(DateTime lastUpdate)
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
                return new List<WasteData>();
            }


            var newWasteData = new List<WasteData>();
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
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Error getting data from API: {apiUrl}");
                return new List<WasteData>();
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, $"Error while deserializing the response from: {apiUrl}");
                return new List<WasteData>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while getting new data from : {apiUrl}");
                return new List<WasteData>();
            }
        }

        private DateTime GetLastUpdateTime()
        {
            if (_memoryCache.TryGetValue(LastUpdateKey, out DateTime lastUpdate))
            {
                return lastUpdate;
            }
            return DateTime.MinValue; // Default value
        }

        private void SetLastUpdateTime(DateTime lastUpdate)
        {
            _memoryCache.Set(LastUpdateKey, lastUpdate, TimeSpan.FromHours(1));
        }
    }
}