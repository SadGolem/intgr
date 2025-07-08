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
    private readonly ILogger<ScheduleProcessor> _logger;
    private readonly IMapper _mapper;

    public ScheduleProcessor(
        IApiClientService apiClientService, 
        IOptions<AuthSettings> mtSettings,
        ILogger<ScheduleProcessor> logger,
        IMapper mapper)
    {
        _apiClientService = apiClientService;
        _baseUrl = mtSettings.Value.MTconnect.BaseUrl;
        _logger = logger;
        _mapper = mapper;
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
}