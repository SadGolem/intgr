using Microsoft.Extensions.Caching.Memory;
using System.Text.Json.Serialization;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

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
        private TokenController _tokenController;
        private class AuthSettings
        {
            public string Login { get; set; }
            public string Password { get; set; }
            public string CallbackUrl { get; set; }
        }

        public WasteSiteEntryController(HttpClient httpClient, ILogger<WasteSiteEntryController> logger, IMemoryCache memoryCache, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _logger = logger;
            _memoryCache = memoryCache;
            _configuration = configuration;
            _aproConnectSettings = _configuration.GetSection("APROconnect").Get<AuthSettings>();
            _tokenController = TokenController.tokenController;
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
            var apiUrl = _aproConnectSettings.CallbackUrl.Replace("token-auth", "wf__wastetakeoutrequest__garbage_collection_request"); ;
            var token = GetCachedToken();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
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
                        if (data.datetime_create > lastUpdate)
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
                return newWasteData;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, $"Error while deserializing the response from: {apiUrl}");
                return newWasteData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected error while getting new data from : {apiUrl}");
                return newWasteData;
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

        public class WasteDataResponse
        {
            [JsonPropertyName("count")]
            public int Count { get; set; }
            [JsonPropertyName("next")]
            public string Next { get; set; }
            [JsonPropertyName("previous")]
            public string Previous { get; set; }
            [JsonPropertyName("results")]
            public List<WasteData> Results { get; set; }
        }

        public class WasteData
        {
            [JsonPropertyName("id")]
            public int idBT { get; set; }
            [JsonPropertyName("datetime_create")]
            public DateTime datetime_create { get; set; }
            [JsonPropertyName("datetime_update")]
            public DateTime datetime_update { get; set; }
            [JsonPropertyName("date")]
            public DateTime date { get; set; }
            [JsonPropertyName("volume")]
            public float volume { get; set; }
            [JsonPropertyName("assignee")]
            public string creator { get; set; }
            [JsonPropertyName("Status")]//такой же айди?
            public string statusID { get; set; }
            [JsonPropertyName("waste_site")]
            public float idLocation { get; set; }
            [JsonPropertyName("type")] //такой же айди?
            public int idContainerType { get; set; }
            [JsonPropertyName("number")] //такой же айди?
            public int amount { get; set; }
            [JsonPropertyName("comment")] //такой же айди?
            public int commentByRO { get; set; }
        }

        private async Task<string> GetCachedToken()
        {
            var cacheKey = $"Token_{new Uri(_aproConnectSettings.CallbackUrl).Host}";
            if (_memoryCache.TryGetValue(cacheKey, out string cachedToken))
            {
                _logger.LogInformation($"Returning cached token: {cachedToken}");
                return cachedToken;
            }

            _logger.LogInformation("Getting new token.");
            await _tokenController.GetTokens();
            var token = TokenController.tokens.First().Value;
            _logger.LogInformation($"Got new token: {token}");
            return token;
        }
    }
}