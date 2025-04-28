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
        private readonly AuthSettings _aproConnectSettings;
        private readonly AuthSettings _mtConnectSettings;

        public TokenController(ILogger<TokenController> logger, IMemoryCache memoryCache, IConfiguration configuration)
        {
            _logger = logger;
            _memoryCache = memoryCache;
            _configuration = configuration;
            _aproConnectSettings = _configuration.GetSection("APROconnect").Get<AuthSettings>();
            _mtConnectSettings = _configuration.GetSection("MTconnect").Get<AuthSettings>();

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
                
                var today = DateTime.UtcNow.Date; // Midnight UTC of the current day
                var cacheKey1 = $"Token_{new Uri(_mtConnectSettings.CallbackUrl).Host}_{today:yyyyMMdd}";
                var cacheKey2 = $"Token_{new Uri(_aproConnectSettings.CallbackUrl).Host}_{today:yyyyMMdd}";

                _logger.LogInformation($"Got new token (System 1): {token1}");
                _logger.LogInformation($"Got new token (System 2): {token2}");
                
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetAbsoluteExpiration(today.AddDays(1));

                _memoryCache.Set(cacheKey1, token1, cacheEntryOptions);
                _memoryCache.Set(cacheKey2, token2, cacheEntryOptions);

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
        
        [HttpGet("getToken/{system}")]
        public IActionResult GetToken(string system)
        {
            string cacheKey;
            if (system.ToLower() == "first")
            {
                var today = DateTime.UtcNow.Date;
                cacheKey = $"Token_{new Uri(_aproConnectSettings.CallbackUrl).Host}_{today:yyyyMMdd}";
            }
            else if (system.ToLower() == "second")
            {
                var today = DateTime.UtcNow.Date;
                cacheKey = $"Token_{new Uri(_mtConnectSettings.CallbackUrl).Host}_{today:yyyyMMdd}";
            }
            else
            {
                return BadRequest("Invalid system.  Must be 'first' or 'second'.");
            }

            if (_memoryCache.TryGetValue(cacheKey, out string token))
            {
                _logger.LogInformation($"Retrieved token from cache for system {system}.");
                return Ok(new { Token = token });
            }
            else
            {
                _logger.LogWarning($"Token not found in cache for system {system}.");
                return NotFound(new { Message = "Token not found. Please refresh tokens." }); // Consider refreshing the token here.
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

        public class AuthSettings
        {
            public string Login { get; set; }
            public string Password { get; set; }
            public string CallbackUrl { get; set; }
        }
    }
}
