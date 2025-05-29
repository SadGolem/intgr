using integration.Context;
using integration.Helpers.Auth;
using integration.Services.Integration.Interfaces;
using Microsoft.Extensions.Options;

namespace integration.Services.Integration.Processors;

public class LocationProcessor : IIntegrationProcessor<LocationDataResponse>
{
    private readonly IApiClientService _apiClientService;
    private readonly string _baseUrl;
    private readonly ILogger<LocationProcessor> _logger;

    public LocationProcessor(
        IApiClientService apiClientService, 
        IOptions<AuthSettings> mtSettings,
        ILogger<LocationProcessor> logger)
    {
        _apiClientService = apiClientService;
        _baseUrl = mtSettings.Value.MTconnect.BaseUrl;
        _logger = logger;
    }

    public async Task ProcessAsync(LocationDataResponse entity)
    {
        if (entity == null) return;
        
        var isNew = entity.ext_id == 0;
        var endpoint = isNew 
            ? "api/v2/location/create_from_asupro" 
            : "api/v2/location/update_from_asupro";
        
        var url = $"{_baseUrl}{endpoint}";
        var method = isNew ? HttpMethod.Post : HttpMethod.Patch;

        try
        {
            if (isNew)
            {
                var response = await _apiClientService.SendAsync<LocationDataResponse, LocationDataResponse>(
                    entity, url, method);
                
                entity.UpdateIntegrationId(response.id);
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
}