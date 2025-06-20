using System.Text.RegularExpressions;
using AutoMapper;
using integration.Context;
using integration.Context.Request;
using integration.Helpers.Auth;
using integration.Services.Integration;
using integration.Services.Integration.Interfaces;
using integration.Services.Integration.Processors;
using Microsoft.Extensions.Options;

public class ContragentProcessor : BaseProcessor, IIntegrationProcessor<ClientDataResponse>
{
    private readonly IApiClientService _apiClientService;
    private readonly IAproClientService _aproClientService; // Новый сервис для работы с АСУ ПРО
    private readonly string _baseUrl;
    private readonly string _aproBaseUrl; // Базовый URL АСУ ПРО
    private readonly ILogger<ContragentProcessor> _logger;
    private readonly IMapper _mapper;

    public ContragentProcessor(
        IApiClientService apiClientService,
        IAproClientService aproClientService,
        IOptions<AuthSettings> apiSettings, 
        ILogger<ContragentProcessor> logger,
        IMapper mapper)
    {
        _apiClientService = apiClientService;
        _aproClientService = aproClientService;
        _baseUrl = apiSettings.Value.MTconnect.BaseUrl;
        _aproBaseUrl = apiSettings.Value.APROconnect.BaseUrl;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task ProcessAsync(ClientDataResponse entity)
    {
        var isNew = entity.ext_id == null;
        var endpoint = isNew 
            ? "api/v2/consumer/create_from_asupro" 
            : "api/v2/consumer/update_from_asupro";
        
        var url = $"{_baseUrl}{endpoint}";
        var method = isNew ? HttpMethod.Post : HttpMethod.Patch;
        var entityRequest = _mapper.Map<ClientRequest>(entity);

        try
        {
            if (isNew)
            {
                string response = await _apiClientService.SendAndGetStringAsync<ClientRequest>(
                    entityRequest, url, method);
            
                var mtId = ParseMtIdFromResponse(response);
                await UpdateAproEntity(entity.idAsuPro, mtId.Value, entity.doc_type.name == "Юридические лица");
            }
            else
            {
                await _apiClientService.SendAsync(entityRequest, url, method);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                $"Error processing contragent with ID: {entity.ext_id}");
            throw;
        }
    }
    
    private async Task UpdateAproEntity(int aproId, int mtId, bool isLegalEntity)
    {
        try
        {
            string endpointPath = isLegalEntity 
                ? $"wf__participant__legal_entity/{aproId}/" 
                : $"wf__participant__fl/{aproId}/";
        
            var aproEndpoint = _aproBaseUrl + endpointPath;
            var updateRequest = new { ext_id = mtId };
        
            await _aproClientService.PatchAsync(aproEndpoint, updateRequest);
        
            _logger.LogInformation($"Updated ASU PRO {aproId} ({(isLegalEntity ? "Юр.лицо" : "Физ.лицо")}) with MT ID {mtId}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating ASU PRO entity {aproId}");
            throw;
        }
    }
}