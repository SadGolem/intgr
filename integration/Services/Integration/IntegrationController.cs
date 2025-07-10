using integration.Structs;

namespace integration.Services.Integration;

public class IntegrationController : IntegrationControllerBase
{
    private readonly ILogger<IntegrationControllerBase> _logger; 
    private readonly IIntegrationService _integrationService; 

    public IntegrationController(
        ILogger<IntegrationControllerBase> logger,
        IIntegrationService integrationService)
        : base(logger)
    {
        _logger = logger;
        _integrationService = integrationService;
    }

    public async Task Sync(IntegrationStruct _struct)
    {
        _logger.LogInformation("Starting sync integration set...");
        try
        { 
            await _integrationService.SendIntegrationDataAsync(_struct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sync failed");
        }
    }
}