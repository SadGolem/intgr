using integration.Context;
using integration.Helpers.Auth;
using integration.Services.Integration.Interfaces;
using Microsoft.Extensions.Options;

namespace integration.Services.Integration.Processors;

public class EmitterProcessor : IIntegrationProcessor<EmitterDataResponse>
{
    private readonly IApiClientService _apiClientService;
    private readonly string _baseUrl;
    private readonly ILogger<EmitterProcessor> _logger;

    public EmitterProcessor(
        IApiClientService apiClientService, 
        IOptions<AuthSettings> mtSettings,
        ILogger<EmitterProcessor> logger)
    {
        _apiClientService = apiClientService;
        _baseUrl = mtSettings.Value.MTconnect.BaseUrl;
        _logger = logger;
    }

    public async Task ProcessAsync(EmitterDataResponse entity)
    {
        var isNew = entity.ext_id == 0;
        var endpoint = isNew 
            ? "api/v2/garbage_maker/create_from_asupro" 
            : "api/v2/garbage_maker/update_from_asupro";
        
        var url = $"{_baseUrl}{endpoint}";
        var method = isNew ? HttpMethod.Post : HttpMethod.Patch;

        try
        {
            if (isNew)
            {
                var response = await _apiClientService.SendAsync<EmitterDataResponse, EmitterDataResponse>(
                    entity, url, method);
                
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
}
