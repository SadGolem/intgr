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
              [FromQuery] int idBT,
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
                    idBT = btNumber,
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
                    idBT = 125,
                    creator = "creator",
                    status = "Новая",
                    idLocation = 45678,
                    amount = 1,
                    volume = 0.77,
                    creationDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                    planDateRO = DateTime.UtcNow.AddDays(4).ToString("yyyy-MM-dd"),
                    commentByRO = "новая",
                    type = "Заявка",
                    idContainerType = 5
                };

                var jsonBody = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                using var response = await client.PostAsync(apiUrl, content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                //var result = JsonSerializer.Deserialize<Response>(responseContent);

                return Ok(responseContent);
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

        [HttpPatch("edit_entry")]
        public async Task<IActionResult> EditEntry(
              /* [FromQuery] string consumerName,
               [FromQuery] int idBT,
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

                var apiUrl = _mtConnectSettings.CallbackUrl.Replace("auth", "api/v2/entry/update_from_bt");

                var requestBody = new
                {
                    /*consumerName = consumerName,
                    idBT = btNumber,
                    creator = creator,
                    status = status,
                    idLocation = idLocation,
                    commentByRO = commentByRO*/

                    consumerName = "name",
                    idBT = 127,
                    creator = "creator",
                    status = "Выполнена",
                    idLocation = 45678,
                    commentByRO = "изменена"
                };

                var jsonBody = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                using var response = await client.PatchAsync(apiUrl, content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                //var result = JsonSerializer.Deserialize<Response>(responseContent);

                return Ok(responseContent);
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
            return token;
        }

        public class TokenRequestException : Exception
        {
            public TokenRequestException(string message) : base(message) { }
            public TokenRequestException(string message, Exception innerException) : base(message, innerException) { }
        }

    }
}

