using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using integration.Controllers.Apro;
using integration.Context;
namespace integration.Controllers.MT
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntryController : ControllerBase
    {
        private AuthSettings _mtConnectSettings;
        private readonly ILogger<EntryController> _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly TokenController _tokenController;

        public EntryController(ILogger<EntryController> logger, IMemoryCache memoryCache, IHttpClientFactory httpClientFactory, IConfiguration configuration, TokenController tokenController)
        {
            _logger = logger;
            _memoryCache = memoryCache;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _tokenController = tokenController;
            _mtConnectSettings = _configuration.GetSection("MTconnect").Get<AuthSettings>();
            _logger.LogInformation("DataController initialized.");
        }

        public async Task ProcessEntryData(EntryData wasteData)
        {
            try
            {
                var token = await TokenController._authorizer.GetCachedTokenMT();
                var client = _httpClientFactory.CreateClient();
                _logger.LogInformation($"Using token for request: {token}");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var apiUrl = _mtConnectSettings.CallbackUrl.Replace("auth", "api/v2/entry/create_from_bt");
                HttpResponseMessage response;
                if (wasteData.BtNumber > 0)
                {
                    apiUrl = _mtConnectSettings.CallbackUrl.Replace("auth", "api/v2/entry/update_from_bt");
                    response = await client.PatchAsJsonAsync(apiUrl, MapWasteDataToRequest(wasteData));
                }
                else
                {
                    response = await client.PostAsJsonAsync(apiUrl, MapWasteDataToRequest(wasteData));
                }

                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Successfully sent data for ID: {wasteData.BtNumber}. Response: {responseContent}");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"HTTP error while sending data with ID: {wasteData.BtNumber}");
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, $"Json Exception while sending data with ID: {wasteData.BtNumber}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unexpected Exception while sending data with ID: {wasteData.BtNumber}");
            }
        }

        private object MapWasteDataToRequest(EntryData wasteData)
        {
            return new
            {
                consumerName = wasteData.ConsumerName,
                btNumber = wasteData.BtNumber,
                creator = wasteData.AuthorName,
                status = wasteData.Status,
/*                idLocation = wasteData.idLocation,*/
                amount = 1,
                volume = wasteData.Volume,
                creationDate = wasteData.DateTimeCreate.ToString("yyyy-MM-dd"),
                planDateRO = wasteData.PlanDateRO,
                commentByRO = wasteData.CommentByRO,
                type = wasteData.Type, 
                idContainerType = wasteData.IdContainerType
            };
        }
    }

}

