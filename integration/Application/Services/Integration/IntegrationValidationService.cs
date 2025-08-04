using integration.Context;
using integration.Services.CheckUp;
using integration.Structs;

public interface IIntegrationValidationService
{
    Task<bool> ValidateIntegrationDataAsync(IntegrationStruct integrationStruct);
}

public class IntegrationValidationService : IIntegrationValidationService
{
    private readonly ICheckUpService<ClientDataResponse> _checkUpServiceClient;
    private readonly ICheckUpService<EmitterDataResponse> _checkUpServiceEmitter;
    private readonly ICheckUpService<LocationDataResponse> _checkUpServiceLocation;
    private readonly ICheckUpService<ScheduleDataResponse> _checkUpServiceSchedule;
    private readonly ILogger<IntegrationValidationService> _logger;

    public IntegrationValidationService(
        ICheckUpService<ClientDataResponse> checkUpServiceClient,
        ICheckUpService<EmitterDataResponse> checkUpServiceEmitter,
        ICheckUpService<LocationDataResponse> checkUpServiceLocation,
        ICheckUpService<ScheduleDataResponse> checkUpServiceSchedule,
        ILogger<IntegrationValidationService> logger)
    {
        _checkUpServiceClient = checkUpServiceClient;
        _checkUpServiceEmitter = checkUpServiceEmitter;
        _checkUpServiceLocation = checkUpServiceLocation;
        _checkUpServiceSchedule = checkUpServiceSchedule;
        _logger = logger;
    }

    public async Task<bool> ValidateIntegrationDataAsync(IntegrationStruct integrationStruct)
    {
        try
        {
            _logger.LogInformation("Validating integration data...");
            
            var clientValid =  _checkUpServiceClient.Check(integrationStruct);
            if (!clientValid.Item1) return false;
            
            var emitterValid =  _checkUpServiceEmitter.Check(integrationStruct);
            if (!emitterValid.Item1) return false;
            
            var locationValid =  _checkUpServiceLocation.Check(integrationStruct);
            if (!locationValid.Item1) return false;
            
            var scheduleValid =  _checkUpServiceSchedule.Check(integrationStruct);
            
            _logger.LogInformation("Integration data validation completed");
            return scheduleValid.Item1;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Validation error");
            return false;
        }
    }
}