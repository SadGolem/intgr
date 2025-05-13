using integration.Context;
using integration.Services.CheckUp;
using integration.Structs;

namespace integration.Services.Integration;

public class IntegrationController : IntegrationControllerBase
{
    private readonly ICheckUpFactory<ClientData> _checkUpFactory; // Change to ClientData
    private readonly ILogger<IntegrationControllerBase> _logger; // Change to ClientData
    private readonly IIntegrationService _integrationService; // Change to ClientData

    public IntegrationController(
        ILogger<IntegrationControllerBase> logger,
        ICheckUpFactory<ClientData> checkUpFactory, // Change to ClientData
        IIntegrationService integrationService)
        : base(logger, checkUpFactory)
    {
        _checkUpFactory = checkUpFactory;
        _logger = logger;
        _integrationService = integrationService;
    }

    public async Task Sync(IntegrationStruct _struct)
    {
        _logger.LogInformation("Starting sync integration set...");
        try
        {
            if (Check(_struct).Result.Item1)
            {
                await _integrationService.SendIntegrationDataAsync(_struct);
            }
            else
            {
                throw new Exception("Clients not found");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sync failed");
        }
    }
    private async Task<(bool, string)> Check(IntegrationStruct _struct)
    {
        var _checkUpService = _checkUpFactory.Create();
        return _checkUpService.Check(_struct);
    }
}