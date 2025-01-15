using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace integration.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {

        // public const string WebHostv1Path = "/rs/api/";
        //public const string WebHostv2Path = "/rs2/api/";
        private readonly HttpClient _httpClient;

        public AuthController()
        {
            var httpClientHandler = new HttpClientHandler();
            // Временно отключаем проверку сертификата, ТОЛЬКО ДЛЯ ОТЛАДКИ
            //   httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true;

            // Явно указываем поддерживаемые протоколы TLS
            //  httpClientHandler.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13; 

            _httpClient = new HttpClient(httpClientHandler);
        }


        [HttpPost("getToken")]
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
                /*  var requestBody = new
                  {
                      username = "kemerovo_test_api",
                      password = "D29CuAmR"
                  };
  
                */
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
        }
    }
}
