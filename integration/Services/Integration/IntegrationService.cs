using System.Net;
using System.Text;
using System.Text.Json;
using integration.Context;
using integration.HelpClasses;
using integration.Helpers.Auth;
using integration.Structs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace integration.Services.Integration;


public class IntegrationService : IIntegrationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<IntegrationService> _logger;
    private readonly IApiClientService _apiClientService;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly string _mtBaseUrl;

    public IntegrationService(
        IHttpClientFactory httpClientFactory,
        ILogger<IntegrationService> logger,
        IOptions<AuthSettings> mtSettings,
        IApiClientService apiClientService)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _apiClientService = apiClientService;
        _mtBaseUrl = mtSettings.Value.MTconnect.CallbackUrl;
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    public async Task SendIntegrationDataAsync(IntegrationStruct integrationData)
    {
        try
        {
            await ProcessIntegrationData(integrationData);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing integration data");
            throw;
        }
    }

    private async Task ProcessIntegrationData(IntegrationStruct integrationData)
    {
        var tasks = new List<Task>
        {
            ProcessContragents(integrationData.contragentList),
            ProcessEmitters(integrationData.emittersList),
            ProcessLocation(integrationData.location),
            ProcessSchedules(integrationData.schedulesList)
        };

        await Task.WhenAll(tasks);
    }

    private async Task ProcessContragents(List<ClientData> contragents)
    {
        await ProcessEntitiesAsync(
            contragents,
            "api/v2/consumer/create_from_asupro",
            "api/v2/consumer/update_from_asupro",
            c => c.GetIntegrationExtId() == null ? HttpMethod.Post : HttpMethod.Patch);
    }

    private async Task ProcessEmitters(List<EmitterData> emitters)
    {
        await ProcessEntitiesAsync(
            emitters,
            "api/v2/garbage_maker/create_from_asupro",
            "api/v2/garbage_maker/update_from_asupro",
            e => e.ext_id == 0 ? HttpMethod.Post : HttpMethod.Patch);
    }

    private async Task ProcessLocation(LocationData location)
    {
        if (location == null) return;
        
        await ProcessEntityAsync(
            location,
            "api/v2/location/create_from_asupro",
            "api/v2/location/update_from_asupro",
            l => l.ext_id == 0 ? HttpMethod.Post : HttpMethod.Patch);
    }

    private async Task ProcessSchedules(List<ScheduleData> schedules)
    {
        await ProcessEntitiesAsync(
            schedules,
            "api/v2/export_schedule/create_from_asupro",
            "api/v2/export_schedule/edit_from_asupro",
            s => s.ext_id == 0 ? HttpMethod.Post : HttpMethod.Patch);
    }

    private async Task ProcessEntitiesAsync<T>(
        List<T> entities,
        string postEndpoint,
        string patchEndpoint,
        Func<T, HttpMethod> methodSelector) where T : class, IIntegratableEntity
    {
        if (entities == null || !entities.Any()) return;

        foreach (var entity in entities)
        {
            await ProcessEntityAsync(entity, postEndpoint, patchEndpoint, methodSelector);
        }
    }

    private async Task ProcessEntityAsync<T>(
        T entity,
        string postEndpoint,
        string patchEndpoint,
        Func<T, HttpMethod> methodSelector) where T : class, IIntegratableEntity
    {
        try
        {
            var method = methodSelector(entity);
            var endpoint = method == HttpMethod.Post 
                ? postEndpoint 
                : $"{patchEndpoint}/{entity.GetIntegrationExtId()}";

            var response = await _apiClientService.SendRequestAsync(
                entity,
                $"{_mtBaseUrl}{endpoint}",
                method);

            if (response.IsSuccessStatusCode && method == HttpMethod.Post)
            {
                var responseEntity = await response.Content.ReadFromJsonAsync<T>(_jsonOptions);
                entity.UpdateIntegrationId(responseEntity.GetIntegrationExtId());
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing {typeof(T).Name} with ID: {entity.GetIntegrationExtId()}");
            throw;
        }
    }
}

public interface IApiClientService
{
    Task<HttpResponseMessage> SendRequestAsync<T>(T entity, string url, HttpMethod method) where T : class;
}

public class ApiClientService : IApiClientService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ApiClientService> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiClientService(
        IHttpClientFactory httpClientFactory,
        ILogger<ApiClientService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    public async Task<HttpResponseMessage> SendRequestAsync<T>(T entity, string url, HttpMethod method) where T : class
    {
        using var client = _httpClientFactory.CreateClient();
        using var request = CreateRequestMessage(entity, url, method);
        
        _logger.LogInformation($"Sending {method} request to {url}");
        
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();
        
        return response;
    }

    private HttpRequestMessage CreateRequestMessage<T>(T entity, string url, HttpMethod method) where T : class
    {
        var json = JsonSerializer.Serialize(entity, _jsonOptions);
        return new HttpRequestMessage(method, url)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };
    }
}

public interface IIntegratableEntity
{
    int GetIntegrationExtId();
    void UpdateIntegrationId(int newId);
}
