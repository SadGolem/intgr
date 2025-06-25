using System.Net;
using AutoMapper;
using integration.Context;
using integration.Context.Request;
using integration.Exceptions;
using integration.Helpers.Auth;
using integration.Services.Integration.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace integration.Services.Integration.Processors;

public class LocationProcessor : BaseProcessor, IIntegrationProcessor<LocationDataResponse>
{
    private readonly IApiClientService _apiClientService;
    private readonly IAproClientService _aproClientService;
    private readonly string _baseUrl;
    private readonly string _aproBaseUrl;
    private readonly ILogger<LocationProcessor> _logger;
    private readonly IMapper _mapper;

    public LocationProcessor(
        IApiClientService apiClientService, 
        IAproClientService aproClientService,
        IOptions<AuthSettings> mtSettings,
        ILogger<LocationProcessor> logger,
        IMapper mapper)
    {
        _apiClientService = apiClientService;
        _aproClientService = aproClientService;
        _baseUrl = mtSettings.Value.MTconnect.BaseUrl;
        _aproBaseUrl = mtSettings.Value.APROconnect.BaseUrl;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task ProcessAsync(LocationDataResponse entity)
{
    if (entity == null) return;

    bool isNew = entity.ext_id is null or "";
    
    var endpoint = isNew 
        ? "api/v2/location/create_from_asupro" 
        : "api/v2/location/update_from_asupro";
    
    var url = $"{_baseUrl}{endpoint}";
    var method = isNew ? HttpMethod.Post : HttpMethod.Patch;
    var entityRequest = _mapper.Map<LocationRequest>(entity);

    try
    {
        if (isNew)
        {
            string response = await _apiClientService.SendAndGetStringAsync<LocationRequest>(
                entityRequest, url, method);
            var mtId = ParseMtIdFromResponse(response);
            await UpdateAproEntity(entity.id, mtId.Id);
            entity.ext_id = mtId.Id.ToString();
        }
        else
        {
            await _apiClientService.SendAsync(entityRequest, url, method);
        }
    }
    catch (HttpRequestException ex) when (ex.Message.Contains("400"))
    {
        // Получаем доступ к содержимому ответа об ошибке
        string errorContent = await GetErrorContentAsync(ex);
        await HandleApiErrorAsync(errorContent, entity.id);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, $"Error processing location with ID: {entity.ext_id}");
        throw;
    }
}

private async Task<string> GetErrorContentAsync(HttpRequestException ex)
{
    if (ex.Data.Contains("ResponseContent"))
    {
        return ex.Data["ResponseContent"]?.ToString();
    }
    return "{}";
}

private async Task HandleApiErrorAsync(string errorContent, int entityId)
{
    try
    {
        var errorResponse = JObject.Parse(errorContent);
        var errorMessage = errorResponse["errorMessage"]?.ToString();
        
        if (errorMessage != null && 
            errorMessage.Contains("Duplicate entry") && 
            errorMessage.Contains("IDX_locations_adres"))
        {
            _logger.LogError($"Duplicate location address detected. Entity ID: {entityId}. Error: {errorMessage}");
            Message($"Duplicate location address detected. Entity ID: {entityId}. Error: {errorMessage}");
        }
        else
        {
            _logger.LogError($"API error occurred. Entity ID: {entityId}. Error: {errorMessage}");
            Message($"API error occurred. Entity ID: {entityId}. Error: {errorMessage}");
        }
    }
    catch
    {
        _logger.LogError($"Unparsable error content. Entity ID: {entityId}. Raw content: {errorContent}");
        throw;
    }
}
    
    private async Task UpdateAproEntity(int aproId, int mtId)
    {
        try
        {
            string endpointPath = $"wf__waste_site__waste_site/{aproId}/";
        
            var aproEndpoint = _aproBaseUrl + endpointPath;
            var updateRequest = new { ext_id_2 = mtId };
        
            await _aproClientService.PatchAsync(aproEndpoint, updateRequest);
            
            _logger.LogInformation($"Updated ASU PRO {aproId} with MT ID {mtId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating ASU PRO entity {aproId}");
            throw;
        }
    }
    
     public void Message(string message)
        {
            EmailMessageBuilder.PutInformation(
                EmailMessageBuilder.ListType.getlocation, 
                message
            );
        }
}