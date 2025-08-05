using AutoMapper;
using integration.Context;
using integration.Domain.Entities;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Infrastructure;
using integration.Services;
using integration.Services.CheckUp;
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
    private readonly ICheckUpService<ClientDataResponse> _checkUpServiceClient;
    private readonly ICheckUpService<EmitterDataResponse> _checkUpServiceEmitter;
    private readonly ICheckUpService<LocationDataResponse> _checkUpServiceLocation;
    private readonly ICheckUpService<ScheduleDataResponse> _checkUpServiceSchedule;
    private readonly AppDbContext _dbContext;
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
        ICheckUpService<ClientDataResponse> checkUpServiceClient,
        ICheckUpService<EmitterDataResponse> checkUpServiceEmitter,
        ICheckUpService<LocationDataResponse> checkUpServiceLocation,
        ICheckUpService<ScheduleDataResponse> checkUpServiceSchedule,
        AppDbContext dbContext,
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
        _checkUpServiceClient = checkUpServiceClient;
        _checkUpServiceEmitter = checkUpServiceEmitter;
        _checkUpServiceLocation = checkUpServiceLocation;
        _checkUpServiceSchedule = checkUpServiceSchedule;
        _dbContext = dbContext;
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

    public async Task<bool> ValidateIntegrationDataAsync(IntegrationStruct integrationStruct)
    {
        try
        {
            _logger.LogInformation("Validating integration data...");
            
            var clientValid = _checkUpServiceClient.Check(integrationStruct);
            if (!clientValid.Item1) return false;
            
            var emitterValid =  _checkUpServiceEmitter.Check(integrationStruct);
            if (!emitterValid.Item1) return false;
            
            var locationValid = _checkUpServiceLocation.Check(integrationStruct);
            if (!locationValid.Item1) return false;
            
            var scheduleValid = _checkUpServiceSchedule.Check(integrationStruct);
            
            _logger.LogInformation("Integration data validation completed");
            return scheduleValid.Item1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Validation error");
            return false;
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
    
    public async Task SaveIntegrationDataAsync(IntegrationStruct integrationData)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync();
        
        try
        {
            _logger.LogInformation("Saving integration data to database...");
            
            // Сохранение контрагентов
            if (integrationData.contragentList?.Any() == true)
            {
                var clientEntities = _mapper.Map<List<ClientEntity>>(integrationData.contragentList);
                await _dbContext.ClientsRecords.AddRangeAsync(clientEntities);
                _logger.LogInformation($"Saved {clientEntities.Count} clients");
            }

            // Сохранение локаций
            if (integrationData.location != null)
            {
                var locationEntity = _mapper.Map<LocationEntity>(integrationData.location);
                await _dbContext.LocationRecords.AddAsync(locationEntity);
                _logger.LogInformation("Saved location: {LocationId}", locationEntity.Id);
            }

            // Сохранение эмиттеров
            if (integrationData.emittersList?.Any() == true)
            {
                var emitterEntities = _mapper.Map<List<EmitterEntity>>(integrationData.emittersList);
                await _dbContext.EmitterRecords.AddRangeAsync(emitterEntities);
                _logger.LogInformation($"Saved {emitterEntities.Count} emitters");
            }

            // Сохранение расписаний
            if (integrationData.schedulesList?.Any() == true)
            {
                var scheduleEntities = _mapper.Map<List<ScheduleEntity>>(integrationData.schedulesList);
                await _dbContext.ScheduleRecords.AddRangeAsync(scheduleEntities);
                _logger.LogInformation($"Saved {scheduleEntities.Count} schedules");
            }
            
            // Сохранение изменений
            await _dbContext.SaveChangesAsync();
            await transaction.CommitAsync();
            
            _logger.LogInformation("All integration data saved successfully");
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error saving integration data to database");
            throw;
        }
    }
}