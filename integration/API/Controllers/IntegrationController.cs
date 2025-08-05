using integration.Services.Integration;
using integration.Structs;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class IntegrationController : ControllerBase
{
    private readonly ILogger<IntegrationController> _logger;
    private readonly IIntegrationService _integrationService;
    private readonly IIntegrationValidationService _validationService;

    public IntegrationController(
        ILogger<IntegrationController> logger,
        IIntegrationService integrationService,
        IIntegrationValidationService validationService)
    {
        _logger = logger;
        _integrationService = integrationService;
        _validationService = validationService;
    }

    [HttpPost("sync")]
    public async Task<IActionResult> Sync([FromBody] IntegrationStruct integrationStruct)
    {
        _logger.LogInformation("Starting integration sync...");
        try
        {
            // Проверка данных
            bool isValid = await _validationService.ValidateIntegrationDataAsync(integrationStruct);
            if (!isValid)
            {
                return BadRequest("Invalid integration data");
            }
            
            // Отправка данных
            await _integrationService.SendIntegrationDataAsync(integrationStruct);
            
            return Ok("Integration sync completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Integration sync error");
            return StatusCode(500, "Internal server error");
        }
    }
}