using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Caching.Memory;
namespace integration.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DataController : ControllerBase
    {
        private readonly ILogger<DataController> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpClientFactory _httpClientFactory;

        public DataController(ILogger<DataController> logger, IMemoryCache memoryCache, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _memoryCache = memoryCache;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("create_entry")]
        [Authorize]
        public async Task<IActionResult> CreateEntry(
             [FromQuery] string consumerName,
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
             [FromQuery] int idContainerType)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("AuthenticatedClient");
                client.DefaultRequestHeaders.Add("Authorization", "Basic " + token);
                var apiUrl = "http://10.5.5.205:9002/api/v2/entry/create_from_bt";
                var requestBody = new
                {
                    consumerName = consumerName,
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
                    idContainerType = idContainerType
                };


                var jsonBody = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                using var response = await client.PostAsync(apiUrl, content);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Response>(responseContent);

                return Ok(result);

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
        public class Response
        {
            public string result { get; set; }
        }
    }
}

