using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using System.Text.Json.Serialization;
using integration.Context;

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
                var token1 = await GetTokenFromSecondSystem();
                var token2 = await GetTokenFromFirstSystem();
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

        private async Task<string> GetTokenFromFirstSystem()
        {
            var requestBody = new
            {
                username = _aproConnectSettings.Login,
                password = _aproConnectSettings.Password
            };
            var apiUrl = _aproConnectSettings.CallbackUrl;
            var jsonBody = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            try
            {
                using var response = await _httpClient.PostAsync(apiUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Ошибка при запросе к первой системе: {response.StatusCode}, {errorContent}");
                }

                var responseContent = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseContent))
                {
                    throw new Exception("Не удалось получить токен от первой системы");
                }

                var token = JsonSerializer.Deserialize<TokenResponse>(responseContent)?.Token;
                return token;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Не удалось получить токен от первой системы");
                return null;
            }
        }


        private async Task<string> GetTokenFromSecondSystem()
        {
            var requestBody = new
            {
                username = _mtConnectSettings.Login,
                password = _mtConnectSettings.Password
            };
            var apiUrl = _mtConnectSettings.CallbackUrl;
            var jsonBody = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            try
            {
                using var response = await _httpClient.PostAsync(apiUrl, content);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    throw new HttpRequestException($"Ошибка при запросе ко второй системе: {response.StatusCode}, {errorContent}");
                }
                var responseContent = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(responseContent))
                {
                    throw new Exception("Не удалось получить токен от второй системы");
                }
                var token = JsonSerializer.Deserialize<TokenResponse>(responseContent)?.Token;
                return token;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Не удалось получить токен от второй системы");
                return null;
            }
        }

        private class TokenResponse
        {
            [JsonPropertyName("token")]
            public string Token { get; set; }
        }
    }
}
