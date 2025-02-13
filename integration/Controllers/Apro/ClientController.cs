using integration.Context;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using integration;
using System.Text;
using integration.HelpClasses;

[ApiController]
[Route("api/[controller]")]
public class ClientController : ControllerBase
{

    private readonly HttpClient _httpClient;
    private readonly ILogger<ClientController> _logger;
    private readonly AuthSettings _sourceApiUrl;
    private readonly AuthSettings _destinationApiUrl;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _mtConnectCreate;
    private readonly string _mtConnectPatch;
    private readonly string url = "wf__waste_site__waste_site/?query={id,datetime_create, datetime_update,lon,  lat, address, status_id}";
    private readonly string _aproConnect = "wf__waste_site__waste_site/?query={id,datetime_create, datetime_update,lon,  lat, address, status_id}";

    public ClientController(HttpClient httpClient, ILogger<ClientController> logger, IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClient;
        _logger = logger;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
        _sourceApiUrl = _configuration.GetSection("APROconnect").Get<AuthSettings>();/*"https://test.asu2.big3.ru/api/wf__waste_site__waste_site/?query={id,datetime_create, datetime_update,lon,  lat, address}"; */// можно вынести в конфигурацию
        _destinationApiUrl = _configuration.GetSection("MTconnect").Get<AuthSettings>();
        _mtConnectCreate = _destinationApiUrl.CallbackUrl.Replace("auth", "api/v2/garbage_maker/create_from_asupro");
        _mtConnectPatch= _destinationApiUrl.CallbackUrl.Replace("auth", "api/v2/garbage_maker/update_from_asupro");
        ConnectngStringApro _connectngStringApro = new ConnectngStringApro(_configuration, url);
        _aproConnect = _connectngStringApro.GetAproConnectSettings();
    }

    [HttpGet("syncEmitters")] // This endpoint can be used for manual triggers
    public async Task<IActionResult> SyncEmitter()
    {
        _logger.LogInformation("Starting manual location sync...");
        try
        {
            await FetchtLocations();

            return Ok("Locations synced successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during location sync.");
            return StatusCode(500, "Error during location sync.");
        }
    }

    private async Task FetchtLocations()
    {
        _logger.LogInformation($"Fetching emitters from {_sourceApiUrl}...");
        List<EmitterData> locations = new List<EmitterData>();
        try
        {
            locations = await FetchEmitterData();

            await PostOrPatch(locations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during locations fetch");
            return;
        }
        _logger.LogInformation($"Received {locations.Count} locations");
    }

    private async Task PostOrPatch(List<EmitterData> emitters)
    {
        var lastUpdate = LastUpdateTextFileManager.GetLastUpdateTime("locations");

        foreach (var emitter in emitters)
        {
            if (!emitter.normative) continue;
            if (emitter.datetime_create > lastUpdate || emitter.datetime_update > lastUpdate)
            {
                if (emitter.datetime_create > lastUpdate) //здесь менять логику незлья, так как у них  апдейт чуть позже криеэйт
                {
                    await PostAndPatch(emitter, true);
                }
                else if (emitter.datetime_update > lastUpdate)
                {
                    await PostAndPatch(emitter, false);
                }
            }
            LastUpdateTextFileManager.SetLastUpdateTime("emitter");
        }

    }

    private async Task<List<EmitterData>> FetchEmitterData()
    {
        var locations = new List<EmitterData>();
       // var token = await TokenController._authorizer.GetCachedTokenAPRO();
       // _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        try
        {
            var response = await _httpClient.GetAsync(_aproConnect);

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            locations = await JsonSerializer.DeserializeAsync<List<EmitterData>>(
               await response.Content.ReadAsStreamAsync(), options);
            ToGetMessage("Got emitters: " + content);

            return locations;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, $"Error during GET request to {_aproConnect}");
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, $"Error during JSON deserialization of response from {_aproConnect}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unexpected error while fetching data from {_aproConnect}");
            throw;
        }
    }

    private async Task PostAndPatch(EmitterData emitter, bool isNew)
    {
        var client = _httpClientFactory.CreateClient();
       // var token = await TokenController._authorizer.GetCachedTokenAPRO();
        //client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        try
        {
            if (!CheckEmitter(emitter)) return;
            var mappedLocation = MapLocationData(emitter);
            var jsonBody = JsonSerializer.Serialize(mappedLocation);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            HttpResponseMessage response;
            if (isNew)
                response = await client.PostAsync(_mtConnectCreate, content);
            else
            {
                response = await client.PatchAsync(_mtConnectPatch, content);
            }

            response.EnsureSuccessStatusCode();
            var responseContent = await response.Content.ReadAsStringAsync();
            ToSetMessage($"Successfully posted location with id: {emitter.id}. Response: {responseContent}");
            _logger.LogInformation($"Successfully posted location with id: {emitter.id}. Response: {responseContent}");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, $"HTTP error while posting location with id: {emitter.id}");
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, $"Json Exception while posting location with id: {emitter.id}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unexpected Exception while posting location with id: {emitter.id}");
            throw;
        }
    }

    private object MapLocationData(EmitterData emitter)
    {
        return new
        {
            idBT = emitter.id,
            idConsumer = emitter.client.id,
            idConsumerType = 0,
            amount = 1,
            consumerAddress = "",
            accountingType = "Норматив",
            contractNumber = "",
            idLocation = 222,
            executorName = emitter.author?.Name ?? "",
            idContract = 122,
            contractStatus = 0,
            addressBT = emitter.address,
            usernameBT = emitter.author?.Name ?? "",
        };
    }

    private bool CheckEmitter(EmitterData emitter)
    {
        if (emitter.address == null || emitter.address == "")
        {
            ToSetMessage($"Address is empty {emitter.id}");
            return false;
        }
        return true;
    }

    void ToSetMessage(string ex)
    {
        EmailMessageBuilder.PutInformation(EmailMessageBuilder.ListType.setemitter, ex);
    }

    void ToGetMessage(string ex)
    {
        EmailMessageBuilder.PutInformation(EmailMessageBuilder.ListType.getemitter, ex);
    }
}
