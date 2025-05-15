using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json.Serialization;
using integration.HelpClasses;

namespace integration
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TokenController> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private AuthSettings _aproConnectSettings;
        private AuthSettings _mtConnectSettings;
        public static TokenController tokenController;
        public static Authorizer _authorizer;
        public static Dictionary<string, string> tokens = new Dictionary<string, string>();

        public TokenController(ILogger<TokenController> logger, IMemoryCache memoryCache, IConfiguration configuration)
        {
            _logger = logger;
            _memoryCache = memoryCache;
            _configuration = configuration;
            _aproConnectSettings = _configuration.GetSection("APROconnect").Get<AuthSettings>();
            _mtConnectSettings = _configuration.GetSection("MTconnect").Get<AuthSettings>();
            tokenController = this;
            _authorizer = new Authorizer(_logger, _memoryCache, _configuration, tokenController);

            var httpClientHandler = new HttpClientHandler
            {
                SslProtocols = SslProtocols.Tls13
            };
            _httpClient = new HttpClient(httpClientHandler);
        }

        [HttpPost("getTokens")]
        public async Task<IActionResult> GetTokens()
        {
            try
            {
                var token1 = await GetTokenAsync(_mtConnectSettings);
                var token2 = await GetTokenAsync(_aproConnectSettings);
                var cacheKey = $"Token_{new Uri(_mtConnectSettings.CallbackUrl).Host}";
                var cacheKey2 = $"Token_{new Uri(_aproConnectSettings.CallbackUrl).Host}";
                _logger.LogInformation($"Got new token: {token1}");
                _memoryCache.Set(cacheKey, token1, TimeSpan.FromHours(24));
                _memoryCache.Set(cacheKey2, token2, TimeSpan.FromHours(24));
                tokens.Clear();
                tokens.Add(token1, token2);
                return Ok(new { Token1 = token1, Token2 = token2 });
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Ошибка HTTP при получении токенов");
                return StatusCode(500, new
                {
                    Error = "Ошибка HTTP",
                    Details = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message
                });
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Ошибка JSON при обработке ответа");
                return StatusCode(500, new
                {
                    Error = "Ошибка JSON",
                    Details = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Непредвиденная ошибка");
                return StatusCode(500, new
                {
                    Error = "Непредвиденная ошибка",
                    Details = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message
                });
            }
        }

        private async Task<string> GetTokenAsync(AuthSettings authSettings)
        {
            var requestBody = new { username = authSettings.Login, password = authSettings.Password };
            var jsonBody = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            using var response = await _httpClient.PostAsync(authSettings.CallbackUrl, content);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TokenResponse>(responseContent)?.Token;
        }
        
        private class TokenResponse
        {
            [JsonPropertyName("token")]
            public string Token { get; set; }
        }
    }
}
