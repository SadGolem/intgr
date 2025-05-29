using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using integration.Controllers.Apro;
using integration.Context;
using integration.HelpClasses;
using integration.Helpers.Auth;

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

     /*
        public async Task ProcessEntryPostData(EntryDataResponse wasteData)
        {
            try
            {
                var token = await TokenController._authorizer.GetCachedTokenMT();
                var client = _httpClientFactory.CreateClient();
                _logger.LogInformation($"Using token for request: {token}");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var apiUrl = _mtConnectSettings.CallbackUrl.Replace("auth", "api/v2/entry/create_from_bt");
                var entry = MapWasteDataToRequest(wasteData);
                var jsonBody = JsonSerializer.Serialize(entry);

                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                using HttpResponseMessage response = await client.PostAsync(apiUrl,content) ;

                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                ToMessage($"Successfully sent data for ID: {wasteData.BtNumber}. Response: {responseContent}");
                _logger.LogInformation($"Successfully sent data for ID: {wasteData.BtNumber}. Response: {responseContent}");
            }
            catch (HttpRequestException ex)
            {
                ToMessage(ex + $" HTTP error while sending data with ID: {wasteData.BtNumber}");
                _logger.LogError(ex, $"HTTP error while sending data with ID: {wasteData.BtNumber}");
            }
            catch (JsonException ex)
            {
                ToMessage(ex + $" Json Exception while sending data with ID: {wasteData.BtNumber}");
                _logger.LogError(ex, $"Json Exception while sending data with ID: {wasteData.BtNumber}");
            }
            catch (Exception ex)
            {
                ToMessage(ex + $" Unexpected Exception while sending data with ID: {wasteData.BtNumber}");
                _logger.LogError(ex, $"Unexpected Exception while sending data with ID: {wasteData.BtNumber}");
            }
        }
       */
        /*
        public async Task ProcessEntryPatchData(EntryDataResponse wasteData)
        {
            try
            {
                var token = await TokenController._authorizer.GetCachedTokenMT();
                var client = _httpClientFactory.CreateClient();
                _logger.LogInformation($"Using token for request: {token}");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var apiUrl = _mtConnectSettings.CallbackUrl.Replace("auth", "api/v2/entry/update_from_bt");
                HttpResponseMessage response;

                if (!CheckRequestBody(wasteData))
                {
                    return;
                }

                var requestBody = MapWasteDataToRequest(wasteData);
                response = await client.PatchAsJsonAsync(apiUrl, requestBody);
                
                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                ToMessage($"Successfully sent data for ID: {wasteData.BtNumber}. Response: {responseContent}");
                _logger.LogInformation($"Successfully sent data for ID: {wasteData.BtNumber}. Response: {responseContent}");
            }
            catch (HttpRequestException ex)
            {
                ToMessage(ex + $" HTTP error while sending data with ID: {wasteData.BtNumber}");
                _logger.LogError(ex, $"HTTP error while sending data with ID: {wasteData.BtNumber}");
            }
            catch (JsonException ex)
            {
                ToMessage(ex + $" Json Exception while sending data with ID: {wasteData.BtNumber}");
                _logger.LogError(ex, $"Json Exception while sending data with ID: {wasteData.BtNumber}");
            }
            catch (Exception ex)
            {
                ToMessage(ex + $" Unexpected Exception while sending data with ID: {wasteData.BtNumber}");
                _logger.LogError(ex, $"Unexpected Exception while sending data with ID: {wasteData.BtNumber}");
            }
        }
        */

        private object MapWasteDataToRequest(EntryDataResponse wasteDataResponse)
        {
            return new
            {
                consumerName = wasteDataResponse.ConsumerName?.name ?? "",
                idBT = wasteDataResponse.BtNumber,
                creator = wasteDataResponse.AuthorName,
                status = StatusCoder.ToCorrectStatus(wasteDataResponse),
                idLocation = wasteDataResponse.location?.id ?? 0,
                amount = wasteDataResponse.Containers?.Count,
                volume = StatusCoder.ToCorrectCapacity(wasteDataResponse.Capacity.id),
                creationDate = wasteDataResponse.datetime_create.ToString("yyyy-MM-dd"),
                planDateRO = wasteDataResponse.PlanDateRO,
                commentByRO = wasteDataResponse.CommentByRO ?? "",
                type = wasteDataResponse.EntryType, //тип заявки
                idContainerType = StatusCoder.ToCorrectContainer(wasteDataResponse),
                /*idMtUser = _configuration.GetSection("idMtUser")*/
            };
        }

        bool CheckRequestBody(EntryDataResponse wasteDataResponse)
        {
            if (wasteDataResponse.ConsumerName == null || wasteDataResponse.ConsumerName?.name == "")
            {
                ToMessage($" ConsumerName is empty: {wasteDataResponse.BtNumber}");
                return false;
            }
            else if (wasteDataResponse.Status == null || (wasteDataResponse.Status.Id != 302 && wasteDataResponse.Status.Id != 282 && wasteDataResponse.Status.Id != 179))
            {
                ToMessage($" Status is empty or incorrect: {wasteDataResponse.BtNumber}");
                return false;
            }
            else if (wasteDataResponse.Containers == null || wasteDataResponse.Containers.Count == 0)
            {
                ToMessage($" Containers is empty or incorrect: {wasteDataResponse.BtNumber}");
                return false;
            }
            else if (wasteDataResponse.Capacity == null || wasteDataResponse.Capacity?.id == 0)
            {
                ToMessage($" Volume is empty or incorrect: {wasteDataResponse.BtNumber}");
                return false;
            }
            else
            {
                return true;
            }
        }

        void ToMessage(string ex)
        {
            EmailMessageBuilder.PutInformation(EmailMessageBuilder.ListType.setentry, ex);
        }
    }
}

