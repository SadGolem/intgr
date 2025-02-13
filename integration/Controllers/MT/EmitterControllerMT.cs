using integration.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Text;
using System.Text.Json;
using static integration.Context.Data;

namespace integration.Controllers.MT
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmitterControllerMT : ControllerBase
    {
        private AuthSettings _mtConnectSettings;
        private readonly ILogger<EmitterControllerMT> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly TokenController _tokenController;
        private string url = "api/v2/garbage_maker/create_from_asupro";
        public EmitterControllerMT(ILogger<EmitterControllerMT> logger, IHttpClientFactory httpClientFactory, IConfiguration configuration, TokenController tokenController)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _tokenController = tokenController;
            _mtConnectSettings = _configuration.GetSection("MTconnect").Get<AuthSettings>();
            _logger.LogInformation("DataController initialized.");
        }

        public async Task ProcessEntryPostData(EmitterData emitter)
        {
            try
            {
                var token = await TokenController._authorizer.GetCachedTokenMT();
                var client = _httpClientFactory.CreateClient();
                _logger.LogInformation($"Using token for request: {token}");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var apiUrl = _mtConnectSettings.CallbackUrl.Replace("auth", url);
                var emitter_body = MapWasteDataToRequest(emitter);
                var jsonBody = JsonSerializer.Serialize(emitter_body);

                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                using HttpResponseMessage response = await client.PostAsync(apiUrl, content);

                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                ToMessage($"Successfully sent data for ID: {emitter.id}. Response: {responseContent}");
                _logger.LogInformation($"Successfully sent data for ID: {emitter.id}. Response: {responseContent}");
            }
            catch (HttpRequestException ex)
            {
                ToMessage(ex + $" HTTP error while sending data with ID: {emitter.id}");
                _logger.LogError(ex, $"HTTP error while sending data with ID: {emitter.id}");
            }
            catch (JsonException ex)
            {
                ToMessage(ex + $" Json Exception while sending data with ID: {emitter.id}");
                _logger.LogError(ex, $"Json Exception while sending data with ID: {emitter.id}");
            }
            catch (Exception ex)
            {
                ToMessage(ex + $" Unexpected Exception while sending data with ID: {emitter.id}");
                _logger.LogError(ex, $"Unexpected Exception while sending data with ID: {emitter.id}");
            }
        }

        public async Task ProcessEntryPatchData(EmitterData emitter)
        {
            try
            {
                var token = await TokenController._authorizer.GetCachedTokenMT();
                var client = _httpClientFactory.CreateClient();
                _logger.LogInformation($"Using token for request: {token}");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                var apiUrl = _mtConnectSettings.CallbackUrl.Replace("auth", "api/v2/garbage_maker/update_from_asupro");
                HttpResponseMessage response;

                if (!CheckRequestBody(emitter))
                {
                    return;
                }

                var requestBody = MapWasteDataToRequest(emitter);
                response = await client.PatchAsJsonAsync(apiUrl, requestBody);

                response.EnsureSuccessStatusCode();
                var responseContent = await response.Content.ReadAsStringAsync();
                ToMessage($"Successfully sent data for ID: {emitter.id}. Response: {responseContent}");
                _logger.LogInformation($"Successfully sent data for ID: {emitter.id}. Response: {responseContent}");
            }
            catch (HttpRequestException ex)
            {
                ToMessage(ex + $" HTTP error while sending data with ID: {emitter.id}");
                _logger.LogError(ex, $"HTTP error while sending data with ID: {emitter.id}");
            }
            catch (JsonException ex)
            {
                ToMessage(ex + $" Json Exception while sending data with ID: {emitter.id}");
                _logger.LogError(ex, $"Json Exception while sending data with ID: {emitter.id}");
            }
            catch (Exception ex)
            {
                ToMessage(ex + $" Unexpected Exception while sending data with ID: {emitter.id}");
                _logger.LogError(ex, $"Unexpected Exception while sending data with ID: {emitter.id}");
            }
        }

        private object MapWasteDataToRequest(EmitterData wasteData)
        {
            return new
            {
                idBT = wasteData.id,
                idConsumer = wasteData.client.id,
                idConsumerType = 0,
                amount = 1,
                consumerAddress = "",
                accountingType = "Норматив",
                contractNumber = "",
                idLocation = 222,
                executorName = wasteData.author?.Name ?? "",
                idContract = 122,
                contractStatus = 0,
                addressBT = wasteData.address,
                usernameBT = wasteData.author?.Name ?? "",
            };
        }

        bool CheckRequestBody(EmitterData emitter)
        {
            if (emitter.author == null )
            {
                ToMessage($" Author is empty: {emitter.id}");
                return false;
            }
            else if (emitter.status == null)
            {
                ToMessage($" Status is empty or incorrect: {emitter.id}");
                return false;
            }
            else if (emitter.client == null)
            {
                ToMessage($" Client is empty or incorrect: {emitter.id}");
                return false;
            }
            else if (emitter.address == null )
            {
                ToMessage($" Address is empty or incorrect: {emitter.id}");
                return false;
            }
            else
            {
                return true;
            }
        }

        void ToMessage(string ex)
        {
            EmailMessageBuilder.PutInformation(EmailMessageBuilder.ListType.setemitter, ex);
        }
    }
}

