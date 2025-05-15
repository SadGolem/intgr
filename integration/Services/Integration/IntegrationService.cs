using System.Text;
using System.Text.Json;
using integration.Context;
using integration.HelpClasses;
using integration.Structs;

namespace integration.Services.Integration;

public class IntegrationService : ServiceBase, IIntegrationService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private ILogger<IntegrationService> _logger;
    private IConfiguration _configuration;
    private IHttpClientFactory _httpClientFactory;
    private string _MTconnect = "";
    public IntegrationService (IHttpClientFactory httpClientFactory,HttpClient httpClient, ILogger<IntegrationService> logger, IConfiguration configuration) : base(httpClientFactory, httpClient,logger,configuration)
    {
        _httpClientFactory = httpClientFactory;
        _httpClient = httpClient;
        _logger = logger;
        _configuration = configuration;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
        _MTconnect = _configuration.GetSection("MTconnect").Get<AuthSettings>().CallbackUrl.Replace("/auth", "/");
    }

    public async Task SendIntegrationDataAsync(IntegrationStruct integrationData)
    {
        // 1. Обрабатываем контрагентов
        await ProcessEntitiesAsync(
            integrationData.contragentList,
            postUrl: "api/v2/consumer/create_from_asupro",
            patchUrl: "api/v2/consumer/update_from_asupro",
            methodSelector: c => c.ext_id == 0 ? HttpMethod.Post : HttpMethod.Patch);

        // 2. Обрабатываем эмиттеры
        await ProcessEntitiesAsync(
            integrationData.emittersList,
            postUrl: "api/v2/garbage_maker/create_from_asupro", // пример для эмиттеров
            patchUrl: "api/v2/garbage_maker/update_from_asupro",
            methodSelector: e => e.ext_id == 0 ? HttpMethod.Post : HttpMethod.Patch);

        /*// 3. Обрабатываем договоры
        await ProcessEntitiesAsync(
            integrationData.contractList,
            postUrl: "api/v2/contract/create_from_asupro",
            patchUrl: "api/v2/contract/update_from_asupro",
            methodSelector: c => c.id == 0 ? HttpMethod.Post : HttpMethod.Patch);*/

        // 4. Обрабатываем локацию
        if (integrationData.location != null)
        {
            await ProcessEntityAsync(
                integrationData.location,
                postUrl: "api/v2/location/create_from_asupro",
                patchUrl: "api/v2/location/update_from_asupro",
                methodSelector: l => l.ext_id == 0 ? HttpMethod.Post : HttpMethod.Patch);
        }

        // 5. Обрабатываем расписания
        await ProcessEntitiesAsync(
            integrationData.schedulesList,
            postUrl: "api/v2/export_schedule/create_from_asupro",
            patchUrl: "api/v2/export_schedule/edit_from_asupro",
            methodSelector: s => s.ext_id == 0 ? HttpMethod.Post : HttpMethod.Patch);
    }

    private async Task ProcessEntitiesAsync<T>(
        List<T> entities,
        string postUrl,
        string patchUrl,
        Func<T, HttpMethod> methodSelector) where T : class
    {
        if (entities == null || !entities.Any()) return;

        foreach (var entity in entities)
        {
            await ProcessEntityAsync(entity, postUrl, patchUrl, methodSelector);
        }
    }

    private async Task ProcessEntityAsync<T>(
        T entity,
        string postUrl,
        string patchUrl,
        Func<T, HttpMethod> methodSelector)
    {
        var method = methodSelector(entity);
        var url = BuildUrl(entity, method, postUrl, patchUrl);
        
        var json = JsonSerializer.Serialize(entity, _jsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        HttpResponseMessage response;
        await Authorize(_httpClient, false);
        
        if (method == HttpMethod.Post)
        {
            response = await _httpClient.PostAsync(url, content);
        }
        else if (method == HttpMethod.Patch)
        {
            response = await _httpClient.PatchAsync(url, content);
        }
        else
        {
            throw new NotSupportedException($"HTTP method {method} not supported");
        }

        response.EnsureSuccessStatusCode();
        
        if (method == HttpMethod.Post)
        {
            var responseContent = await response.Content.ReadFromJsonAsync<T>();
            UpdateEntityId(entity, responseContent);
        }
    }

    private string BuildUrl<T>(T entity, HttpMethod method, string postUrl, string patchUrl)
    {
        var methodName = method.Method.ToUpper(); // Получаем название метода в верхнем регистре

        return methodName switch
        {
            "POST" => _MTconnect + postUrl.TrimEnd('/'),
            "PATCH" => _MTconnect + $"{patchUrl.TrimEnd('/')}/{GetEntityId(entity)}",
            _ => throw new NotSupportedException($"Unsupported method: {method}")
        };
    }

    private int GetEntityId<T>(T entity)
    {
        return entity switch
        {
            ClientData c => c.idAsuPro,
            EmitterData e => e.id,
            ContractData c => c.id,
            LocationData l => l.id,
            ScheduleData s => s.id_oob,
            _ => throw new NotSupportedException($"Unsupported type: {typeof(T)}")
        };
    }

    private void UpdateEntityId<T>(T source, T response)
    {
        var sourceId = GetEntityId(source);
        if (sourceId > 0) return;

        var responseId = GetEntityId(response);
        switch (source)
        {
            case ClientData c:
                c.idAsuPro = responseId;
                break;
            case EmitterData e:
                e.id = responseId;
                break;
            case ContractData c:
                c.id = responseId;
                break;
            case LocationData l:
                l.id = responseId;
                break;
            case ScheduleData s:
                s.id_oob = responseId;
                break;
        }
    }

    public override void Message(string ex)
    {
        throw new NotImplementedException();
    }
}
