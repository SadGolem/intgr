using System.Text.RegularExpressions;
using AutoMapper;
using integration.Context;
using integration.Context.Request;
using integration.Helpers.Auth;
using integration.Services.Integration.Interfaces;
using Microsoft.Extensions.Options;

namespace integration.Services.Integration.Processors;

public class ScheduleProcessor : IIntegrationProcessor<ScheduleDataResponse>
{
    private readonly IApiClientService _apiClientService;
    private readonly string _baseUrl;
    private readonly string _baseUrlApro;
    private readonly ILogger<ScheduleProcessor> _logger;
    private readonly IMapper _mapper;
    private readonly IAproClientService _aproClientService;

    public ScheduleProcessor(
        IApiClientService apiClientService, 
        IOptions<AuthSettings> mtSettings,
        ILogger<ScheduleProcessor> logger,
        IMapper mapper,
        IAproClientService aproClientService)
    {
        _apiClientService = apiClientService;
        _baseUrl = mtSettings.Value.MTconnect.BaseUrl;
        _baseUrlApro = mtSettings.Value.APROconnect.BaseUrl;
        _logger = logger;
        _mapper = mapper;
        _aproClientService = aproClientService;
    }

    public async Task ProcessAsync(ScheduleDataResponse entity)
    {
        var isNew = int.TryParse(entity.ext_id, out int result) ? result != 0 : true;
        
        var endpoint = isNew 
            ? "api/v2/export_schedule/create_from_asupro" 
            : "api/v2/export_schedule/edit_from_asupro";
        
        var url = $"{_baseUrl}{endpoint}";
        var method = isNew ? HttpMethod.Post : HttpMethod.Patch;
        
        try
        {
            var entityRequest = _mapper.Map<ScheduleDataResponse, ScheduleRequest>(entity);
            if (isNew)
            {
                var response = await _apiClientService.SendAndGetStringAsync(entityRequest, url, method);
                var mtId = ParseMtIdFromResponse(response);
                
                if (!mtId.HasValue)
                {
                    _logger.LogError($"Failed to parse MT ID for location: {entity.id}");
                }
                
                await UpdateAproEntity(entity.id, mtId.Value);
            }
            else
            {
                await _apiClientService.SendAsync(entityRequest, url, method);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                $"Error processing schedule with ID: {entity.id}");
            throw;
        }
    }

    private async Task UpdateAproEntity(int aproId, int mtId)
    {
        try
        {
            string endpointPath = $"wf__wastesitescheduleset__waste_site_schedule_set/{aproId}/";

            var aproEndpoint = _baseUrlApro + endpointPath;
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

    public int? ParseMtIdFromResponse(string response)
    {
        var match = Regex.Match(response, @"id (\d+)$");
        return match.Success && int.TryParse(match.Groups[1].Value, out int id)
            ? id
            : null;
    }
}