using integration.Context;
using integration.Helpers.Auth;
using integration.Services.Integration.Interfaces;
using Microsoft.Extensions.Options;

namespace integration.Services.Integration.Processors;

public class ContragentProcessor : IIntegrationProcessor<ClientDataResponseResponse>
{
    private readonly IApiClientService _apiClientService;
    private readonly string _baseUrl;
    private readonly ILogger<ContragentProcessor> _logger;

    public ContragentProcessor(
        IApiClientService apiClientService, 
        IOptions<AuthSettings> mtSettings,
        ILogger<ContragentProcessor> logger)
    {
        _apiClientService = apiClientService;
        _baseUrl = mtSettings.Value.MTconnect.BaseUrl;
        _logger = logger;
    }

    public async Task ProcessAsync(ClientDataResponseResponse entity)
    {
        var isNew = entity.GetIntegrationExtId() == 0;
        var endpoint = isNew 
            ? "api/v2/consumer/create_from_asupro" 
            : "api/v2/consumer/update_from_asupro";
        
        var url = $"{_baseUrl}{endpoint}";
        var method = isNew ? HttpMethod.Post : HttpMethod.Patch;

        try
        {
            if (isNew)
            {
                var response = await _apiClientService.SendAsync<ClientDataResponseResponse, ClientDataResponseResponse>(
                    entity, url, method);
                
                entity.UpdateIntegrationId(response.GetIntegrationExtId());
            }
            else
            {
                await _apiClientService.SendAsync(entity, url, method);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                $"Error processing contragent with ID: {entity.GetIntegrationExtId()}");
            throw;
        }
    }
}