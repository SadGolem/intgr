using AutoMapper;
using integration.Context;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services;
using integration.Services.Integration;
using integration.Services.Integration.Interfaces;
using integration.Structs;
using Microsoft.Extensions.Options;

public class IntegrationService : ServiceBase, IIntegrationService
{
    private readonly ILogger<IntegrationService> _logger;
    private readonly IApiClientService _apiClientService;
    private readonly string _mtBaseUrl;
    private readonly IIntegrationProcessor<ClientDataResponse> _contragentProcessor;
    private readonly IIntegrationProcessor<EmitterDataResponse> _emitterProcessor;
    private readonly IIntegrationProcessor<LocationDataResponse> _locationProcessor;
    private readonly IIntegrationProcessor<ScheduleDataResponse> _scheduleProcessor;
    private IMapper _mapper;

    public IntegrationService(
        IHttpClientFactory httpClientFactory,
        ILogger<IntegrationService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> mtSettings,
        IApiClientService apiClientService,
        IIntegrationProcessor<ClientDataResponse> contragentProcessor,
        IIntegrationProcessor<EmitterDataResponse> emitterProcessor,
        IIntegrationProcessor<LocationDataResponse> locationProcessor,
        IIntegrationProcessor<ScheduleDataResponse> scheduleProcessor,
        IMapper mapper) 
        : base(httpClientFactory, logger, authorizer, mtSettings)
    {
        _logger = logger;
        _apiClientService = apiClientService;
        _mtBaseUrl = mtSettings.Value.MTconnect.BaseUrl;
        _contragentProcessor = contragentProcessor;
        _emitterProcessor = emitterProcessor;
        _locationProcessor = locationProcessor;
        _scheduleProcessor = scheduleProcessor;
        _mapper = mapper;
    }

    public async Task SendIntegrationDataAsync(IntegrationStruct integrationData)
    {
        using (_logger.BeginScope("Processing integration batch"))
        {
            try
            {
                await ProcessIntegrationData(integrationData);
                Message($"{integrationData.location.id} SUCCESS");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing integration data");
                throw;
            }
        }
    }

    private async Task ProcessIntegrationData(IntegrationStruct integrationData)
    {
        var tasks = new List<Task>
        {
            ProcessCollectionAsync(integrationData.contragentList, _contragentProcessor),
            ProcessEntityAsync(integrationData.location, _locationProcessor),
            ProcessCollectionAsync(integrationData.emittersList, _emitterProcessor),
            ProcessCollectionAsync(integrationData.schedulesList, _scheduleProcessor)
        };

        await Task.WhenAll(tasks);
    }

    private async Task ProcessCollectionAsync<T>(List<T>? items, IIntegrationProcessor<T> processor) 
        where T : class, IIntegratableEntity
    {
        if (items == null || items.Count == 0) return;
        
        await Task.WhenAll(items.Select(item => 
            processor.ProcessAsync(item)));
        
    }

    private async Task ProcessEntityAsync<T>(T? entity, IIntegrationProcessor<T> processor) 
        where T : class, IIntegratableEntity
    {
        if (entity == null) return;
        await processor.ProcessAsync(entity);
    }
}