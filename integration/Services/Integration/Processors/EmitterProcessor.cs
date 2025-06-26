using System.Text.RegularExpressions;
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
        try
        {
            var entityRequest = _mapper.Map<EmitterRequest>(entity);
        
            if (isNew)
            {
                string response = await _apiClientService.SendAndGetStringAsync(entityRequest, url, HttpMethod.Post);
                var mtId = ParseMtIdFromResponse(response);
            
                if (!mtId.HasValue)
                {
                    _logger.LogError($"Failed to parse MT ID for emitter: {entity.id}");
                }
            
                await UpdateAproEntity(entity.id, mtId.Value);
            }
            else
            {
                await _apiClientService.SendAsync(entityRequest, url, HttpMethod.Patch);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing emitter: {entity.ext_id}");

            throw;
        }
    }
    public int? ParseMtIdFromResponse(string response)
    {
        var match = Regex.Match(response, @"id is (\d+)$");
        return match.Success && int.TryParse(match.Groups[1].Value, out int id) 
            ? id 
            : null;
    }
    
    private async Task UpdateAproEntity(int aproId, int mtId)
    {
        try
        {
            string endpointPath = $"wf__contractpositionemitter__contract_position_takeout/{aproId}/";
        
            var aproEndpoint = _aproBaseUrl + endpointPath;
            var updateRequest = new { external_id = mtId };
        
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
