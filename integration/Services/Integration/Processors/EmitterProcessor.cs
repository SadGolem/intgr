using AutoMapper;
using integration.Context;
using integration.Context.Request;
using integration.Helpers.Auth;
using integration.Services.Integration.Interfaces;
using Microsoft.Extensions.Options;

namespace integration.Services.Integration.Processors;

public class EmitterProcessor : BaseProcessor, IIntegrationProcessor<EmitterDataResponse>
{
    private readonly IApiClientService _apiClientService;
    private readonly IAproClientService _aproClientService;
    private readonly string _baseUrl;
    private readonly string _aproBaseUrl;
    private readonly ILogger<EmitterProcessor> _logger;
    private readonly IMapper _mapper;
    

    public EmitterProcessor(
        IApiClientService apiClientService, 
        IAproClientService aproClientService,
        IOptions<AuthSettings> mtSettings,
        ILogger<EmitterProcessor> logger,
        IMapper mapper
        )
    {
        _apiClientService = apiClientService;
        _aproClientService = aproClientService;
        _baseUrl = mtSettings.Value.MTconnect.BaseUrl;
        _aproBaseUrl = mtSettings.Value.APROconnect.BaseUrl;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task ProcessAsync(EmitterDataResponse entity)
    {
        var isNew = entity.ext_id == 0;
        var endpoint = isNew 
            ? "api/v2/waste_generator/create_from_asupro" 
            : "api/v2/waste_generator/update_from_asupro";
        
        var url = $"{_baseUrl}{endpoint}";
        var method = isNew ? HttpMethod.Post : HttpMethod.Patch;
        var entityRequest = _mapper.Map<EmitterRequest>(entity);
        try
        {
            if (isNew)
            {
                string response = await _apiClientService.SendAndGetStringAsync(
                    entityRequest, url, method);
                var mtId = ParseMtIdFromResponse(response);
                await UpdateAproEntity(entity.id, mtId.Value);
                }
            else
            {
                await _apiClientService.SendAsync(entity, url, method);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                $"Error processing emitter with ID: {entity.ext_id}");
            throw;
        }
    }
    
    private async Task UpdateAproEntity(int aproId, int mtId)
    {
        try
        {
            string endpointPath = $"wf__contractpositionemitter__contract_position_takeout/{aproId}/";
        
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
