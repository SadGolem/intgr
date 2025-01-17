using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Xml.Linq;
namespace integration.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataController : ControllerBase
    {
        private readonly ILogger<DataController> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private TokenController _tokenController;

        private class AuthSettings
        {
            public string Token { get; set; }
            public string Login { get; set; }
            public string Password { get; set; }
            public string CallbackUrl { get; set; }
        }

        private AuthSettings _mtConnectSettings;


        public DataController(ILogger<DataController> logger, IMemoryCache memoryCache, IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            try
            {
                _logger = logger;
                _memoryCache = memoryCache;
                _httpClientFactory = httpClientFactory;
                _configuration = configuration;
                _mtConnectSettings = _configuration.GetSection("MTconnect").Get<AuthSettings>();
                _logger.LogInformation("DataController initialized.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error in DataController constructor");
            }

        }

        [HttpPost("create_entry")]
        public async Task<IActionResult> CreateEntry(
            /* [FromQuery] string consumerName,
             [FromQuery] int btNumber,
             [FromQuery] string creator,
             [FromQuery] string status,
             [FromQuery] int idLocation,
             [FromQuery] int amount,
             [FromQuery] float volume,
             [FromQuery] DateTime creationDate,
             [FromQuery] DateTime planDateRO,
             [FromQuery] string commentByRO,
             [FromQuery] string type,
             [FromQuery] int idContainerType*/)
        {
            try
            {
                var token = await GetCachedToken();
                var client = _httpClientFactory.CreateClient();
                _logger.LogInformation($"Using token for request: {token}");
                //client.DefaultRequestHeaders.Add("Authorization: ", $"Bearer {token}");
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");

                var apiUrl = _mtConnectSettings.CallbackUrl.Replace("auth", "api/v2/entry/create_from_bt");

                var requestBody = new
                {
                    /*consumerName = consumerName,
                    btNumber = btNumber,
                    creator = creator,
                    status = status,
                    idLocation = idLocation,
                    amount = amount,
                    volume = volume,
                    creationDate = creationDate.ToString("yyyy-MM-dd"),
                    planDateRO = planDateRO.ToString("yyyy-MM-dd"),
                    commentByRO = commentByRO,
                    type = type,
                    idContainerType = idContainerType*/

                    consumerName = "name",
                    btNumber = 125,
                    creator = "creator",
                    status = "Новая",
                    idLocation = 45678,
                    amount = 1,
                    volume = 0.77,
                    creationDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                    planDateRO = DateTime.UtcNow.AddDays(4).ToString("yyyy-MM-dd"),
                    commentByRO = "comment",
                    type = "Заявка",
                    idContainerType = 5
                };

                var jsonBody = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                using var response = await client.PostAsync(apiUrl, content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Response>(responseContent);

                return Ok(result);
            }
            catch (TokenRequestException ex)
            {
                _logger.LogError(ex, "Ошибка получения токена");
                return StatusCode(500, new
                {
                    Error = "Ошибка получения токена",
                    Details = ex.Message,
                    StackTrace = ex.StackTrace,
                    InnerException = ex.InnerException?.Message
                });

            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Ошибка HTTP при отправке данных");
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

        private async Task<string> GetCachedToken()
        {
            var cacheKey = $"Token_{new Uri(_mtConnectSettings.CallbackUrl).Host}";
            if (_memoryCache.TryGetValue(cacheKey, out string cachedToken))
            {
                _logger.LogInformation($"Returning cached token: {cachedToken}");
                return cachedToken;
            }

            _logger.LogInformation("Getting new token.");
            await TokenController.tokenController.GetTokens();
            var token = TokenController.tokens.First().Key;
            _logger.LogInformation($"Got new token: {token}");
            _memoryCache.Set(cacheKey, token, TimeSpan.FromMinutes(60));
            return token;
        }


/*        private async Task<string> GetTokenFromSecondSystem()
        {
            var apiUrl = _mtConnectSettings.CallbackUrl;

            var requestBody = new
            {
                username = _mtConnectSettings.Login,
                password = _mtConnectSettings.Password
            };

            var jsonBody = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            try
            {
                using var client = _httpClientFactory.CreateClient();
                _logger.LogInformation($"Getting token from {apiUrl}");
                using var response = await client.PostAsync(apiUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Error getting token from host {new Uri(apiUrl).Host}: Status Code: {response.StatusCode}, {errorContent}");
                    throw new TokenRequestException($"Error getting token from host {new Uri(apiUrl).Host}: {response.StatusCode}, {errorContent}");
                }
                _logger.LogInformation($"Token request successful. Status code: {response.StatusCode}");
                if (response.Headers.TryGetValues("Authorization", out var values))
                {
                    _logger.LogInformation("Authorization header found");
                    var token = values.FirstOrDefault();
                    if (!string.IsNullOrEmpty(token))
                    {
                        _logger.LogInformation($"Received token for host {new Uri(apiUrl).Host}: {token}");
                        return token;
                    }
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogWarning($"No Authorization header found in response from host {new Uri(apiUrl).Host}. Returning full response.");
                return responseContent;
            }
            catch (TokenRequestException)
            {
                _logger.LogError("TokenRequestException caught in GetTokenFromSecondSystem.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting token from host {new Uri(apiUrl).Host}");
                throw new TokenRequestException($"Error getting token from host {new Uri(apiUrl).Host}", ex);
            }
        }*/


        public class TokenRequestException : Exception
        {
            public TokenRequestException(string message) : base(message) { }
            public TokenRequestException(string message, Exception innerException) : base(message, innerException) { }
        }

        public class Response
        {
            public string result { get; set; }
        }
    }
}

