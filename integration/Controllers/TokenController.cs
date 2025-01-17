using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory;

namespace integration
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<TokenController> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpClientFactory _httpClientFactory;

        public TokenController(ILogger<TokenController> logger, IMemoryCache memoryCache, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _memoryCache = memoryCache;

            var httpClientHandler = new HttpClientHandler
            {
                // Временно отключаем проверку сертификата, ТОЛЬКО ДЛЯ ОТЛАДКИ
            //  ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true,

                // Явно указываем поддерживаемые протоколы TLS
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
              //  var token2 = await GetTokenFromFirstSystem();
                

                return Ok(new { Token1 = token1/*,Token2 = token2*/ });
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
            var apiUrl = "https://test.asu2.big3.ru/api";

            var requestBody = new
            {
                username = "kemerovo_test_api",
                password = "D29CuAmR"
            };

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

                return responseContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при запросе к первой системе");
                throw;
            }
        }

        private async Task<string> GetTokenFromSecondSystem()
        {
            var apiUrl = "http://10.5.5.205:9002/auth";

            var requestBody = new
            {
                username = "zubcova_ma",
                password = "root"
            };

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
                // Сохраняем токен в кэше
                if (response.Headers.TryGetValues("Authorization", out var values))
                {
                    var token = values.FirstOrDefault();
                    _logger.LogInformation($"Received token for host {new Uri(apiUrl).Host}: {token}");
                    _memoryCache.Set($"Token_{new Uri(apiUrl).Host}", token, TimeSpan.FromMinutes(60));
                    return token;
                }
                _logger.LogWarning($"No Authorization header found in response from host {new Uri(apiUrl).Host}.");
                return responseContent;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при запросе ко второй системе");
                throw;
            }
        }
    }
}



// public const string WebHostv1Path = "/rs/api/";
//public const string WebHostv2Path = "/rs2/api/";
/*        private readonly HttpClient _httpClient;

        public TokenController()
        {
            var httpClientHandler = new HttpClientHandler();
            // Временно отключаем проверку сертификата, ТОЛЬКО ДЛЯ ОТЛАДКИ
            //   httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

            // Явно указываем поддерживаемые протоколы TLS
            //  httpClientHandler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13; 

            _httpClient = new HttpClient(httpClientHandler);
        }*/


/*    [HttpPost("getToken")]
    public async Task<IActionResult> GetToken()
    {
        try
        {
            // var apiUrl = "https://test.asu2.big3.ru/api/token-auth/";
            var apiUrl = "http://10.5.5.205:9002/auth";

            // Создаем тело запроса в виде JSON
            var requestBody = new
            {
                username = "zubcova_ma",
                password = "root"
            };
            *//*  var requestBody = new
              {
                  username = "kemerovo_test_api",
                  password = "D29CuAmR"
              };

            *//*
            var jsonBody = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");


            // Выполняем POST-запрос
            var response = await _httpClient.PostAsync(apiUrl, content);

            response.EnsureSuccessStatusCode(); // Проверяем, что статус код в диапазоне 200-299

            // Читаем ответ
            var responseContent = await response.Content.ReadAsStringAsync();

            // Десериализуем JSON, чтобы получить токен
            //  var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseContent);

            if (responseContent == null || string.IsNullOrEmpty(responseContent))
            {
                return BadRequest("Не удалось получить токен");
            }
            return Ok(responseContent);

        }
        catch (HttpRequestException ex)
        {
            // Обработка ошибок HTTP
            return BadRequest($"Ошибка HTTP: {ex.Message}");
        }
        catch (JsonException ex)
        {
            // Ошибка разбора JSON
            return BadRequest($"Ошибка JSON: {ex.Message}");
        }
        catch (Exception ex)
        {
            // Любые другие ошибки
            return BadRequest($"Непредвиденная ошибка: {ex.Message}");
        }
    }*/


