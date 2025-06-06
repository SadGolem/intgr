using integration.Context;
using integration.Helpers;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Interfaces;
using integration.Services.Location;
using integration.Services.Storage;
using integration.Services.Storage.Interfaces;
using Microsoft.Extensions.Options;

namespace integration.Services.Client;

public class ClientSetterService : ServiceSetterBase<ClientDataResponse>, ISetterService<ClientDataResponse>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ClientSetterService> _logger;
    private IContractStorageService _contractStorageService;
    private IStorageService<ClientDataResponse>_storageService;
    private readonly IOptions<AuthSettings> _configuration;
    private ConnectingStringApro _aproConnect;

    public ClientSetterService(IHttpClientFactory httpClientFactory,
        ILogger<ClientSetterService> logger, IAuthorizer authorizer, IOptions<AuthSettings> configuration,
        IStorageService<ClientDataResponse> storageService) : base(httpClientFactory, logger, authorizer, configuration)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _storageService = storageService;
        _configuration = configuration;
    }

  /*   private async Task PostOrPatch(List<EmitterDataResponse> emitters)
    {
        var lastUpdate = TimeManager.GetLastUpdateTime("locations");

        foreach (var emitter in emitters)
        {
            if (!emitter.normative) continue;
            if (emitter.datetime_create > lastUpdate || emitter.datetime_update > lastUpdate)
            {
                if (emitter.datetime_create > lastUpdate) //здесь менять логику незлья, так как у них  апдейт чуть позже криеэйт
                {
                    await Set(emitter, true);
                }
                else if (emitter.datetime_update > lastUpdate)
                {
                    await Set(emitter, false);
                }
            }
            TimeManager.SetLastUpdateTime("emitter");
        }

    }

      private async Task<List<ClientDataResponse>> FetchEmitterData()
    {
        var locations = new List<EmitterDataResponse>();
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
            locations = await JsonSerializer.DeserializeAsync<List<EmitterDataResponse>>(
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

    private async Task Set(EmitterDataResponse emitter, bool isNew)
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

    private object MapLocationData(EmitterDataResponse emitter)
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

    private void GetData()
    {
        
    }

    private bool CheckEmitter(EmitterDataResponse emitter)
    {
        if (emitter.address == null || emitter.address == "")
        {
            ToSetMessage($"Address is empty {emitter.id}");
            return false;
        }
        return true;
    }

*/
  public Task PostAndPatch(List<(ClientDataResponse, bool)> data)
  {
      throw new NotImplementedException();
  }

  public Task Post(ClientDataResponse dataResponse)
  {
      throw new NotImplementedException();
  }

  public Task Patch(ClientDataResponse dataResponse)
  {
      throw new NotImplementedException();
  }

  public object MappingData(ClientDataResponse dataResponse)
  {
      throw new NotImplementedException();
  }

  public bool Check(ClientDataResponse dataResponse)
  {
      throw new NotImplementedException();
  }

  public Task Set()
  {
      throw new NotImplementedException();
  }
}