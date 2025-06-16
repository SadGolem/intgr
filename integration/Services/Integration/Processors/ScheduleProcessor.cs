using integration.Context;
using integration.Helpers.Auth;
using integration.Services.Integration.Interfaces;
using Microsoft.Extensions.Options;

namespace integration.Services.Integration.Processors;

public class ScheduleProcessor : IIntegrationProcessor<ScheduleDataResponse>
{
    private readonly IApiClientService _apiClientService;
    private readonly string _baseUrl;
    private readonly ILogger<ScheduleProcessor> _logger;

    public ScheduleProcessor(
        IApiClientService apiClientService, 
        IOptions<AuthSettings> mtSettings,
        ILogger<ScheduleProcessor> logger)
    {
        _apiClientService = apiClientService;
        _baseUrl = mtSettings.Value.MTconnect.BaseUrl;
        _logger = logger;
    }

    public async Task ProcessAsync(ScheduleDataResponse entity)
    {
        var isNew = entity.ext_id == 0;
        var endpoint = isNew 
            ? "api/v2/export_schedule/create_from_asupro" 
            : "api/v2/export_schedule/edit_from_asupro";
        
        var url = $"{_baseUrl}{endpoint}";
        var method = isNew ? HttpMethod.Post : HttpMethod.Patch;

        try
        {
            if (isNew)
            {
                var response = await _apiClientService.SendAsync<ScheduleDataResponse, ScheduleDataResponse>(
                    entity, url, method);
                
                entity.UpdateIntegrationId(response.ext_id);
            }
            else
            {
                await _apiClientService.SendAsync(entity, url, method);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                $"Error processing schedule with ID: {entity.ext_id}");
            throw;
        }
    }
}