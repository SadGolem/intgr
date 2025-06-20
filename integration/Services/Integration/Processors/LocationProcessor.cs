using AutoMapper;
using integration.Context;
using integration.Context.Request;
using integration.Helpers.Auth;
using integration.Services.Integration.Interfaces;
using Microsoft.Extensions.Options;

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

        var isNew = entity.ext_id == null;
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
                await UpdateAproEntity(entity.id, mtId.Value);
                entity.ext_id = mtId.Value.ToString();
            }
            else
            {
                await _apiClientService.SendAsync(entity, url, method);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                $"Error processing location with ID: {entity.ext_id}");
            throw;
        }
    }
    
    private async Task UpdateAproEntity(int aproId, int mtId)
    {
        try
        {
            string endpointPath = $"wf__waste_site__waste_site//{aproId}/";
        
            var aproEndpoint = _aproBaseUrl + endpointPath;
            var updateRequest = new { ext_id = mtId };
        
            await _aproClientService.PatchAsync(aproEndpoint, updateRequest);
            
            _logger.LogInformation($"Updated ASU PRO {aproId} with MT ID {mtId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating ASU PRO entity {aproId}");
            throw;
        }
    }
}