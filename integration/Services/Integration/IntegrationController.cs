using integration.Context;
using integration.Services.CheckUp;
using integration.Structs;

namespace integration.Services.Integration;

public class IntegrationController : IntegrationControllerBase where T : class
{
    private readonly ICheckUpFactory<T> _checkUpFactory; // Change to ClientDataResponse
    private readonly ILogger<IntegrationControllerBase> _logger; // Change to ClientDataResponse
    private readonly IIntegrationService _integrationService; // Change to ClientDataResponse

    public IntegrationController(
        ILogger<IntegrationControllerBase> logger,
        ICheckUpFactory<T> checkUpFactory, // Change to ClientDataResponse
        IIntegrationService integrationService)
        : base(logger, checkUpFactory<T>)
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