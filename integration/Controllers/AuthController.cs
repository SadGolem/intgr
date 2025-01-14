using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;

namespace integration.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly HttpClient _httpClient;

        public AuthController(AuthService authService, IHttpClientFactory clientFactory)
        {
            _authService = authService;
            _httpClient = clientFactory.CreateClient();
        }

        [HttpPost]
        public async Task<IActionResult> GetDataFromApi()
        {
            // 1. Получаем токен
            string token = await _authService.GetAuthTokenAsync("zubcova_ma", "root");

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest("Не удалось получить токен");
            }

            // 2. Используем токен в запросе к другому API (пример):
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            // Замените  "http://10.5.5.205:9003/api/data" на фактический адрес вашего API
            HttpResponseMessage apiResponse = await _httpClient.GetAsync("http://10.5.5.205:9003/api/data");
            apiResponse.EnsureSuccessStatusCode();
            string apiData = await apiResponse.Content.ReadAsStringAsync();


            return Ok(new { Message = "Данные получены успешно", Data = apiData });
        }
    }
}
