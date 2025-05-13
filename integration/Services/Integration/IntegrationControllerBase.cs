using integration.Context;
using integration.Services.CheckUp;
using integration.Structs;
using Microsoft.AspNetCore.Mvc;

namespace integration.Services.Integration;

[ApiController]
[Route("api/[controller]")]
public class IntegrationControllerBase : ControllerBase
{
    private readonly ICheckUpService<ClientData> _checkUpService; // Change to ClientData
    private readonly ILogger<IntegrationControllerBase> _logger; // Change to ClientData

    public IntegrationControllerBase(
        ILogger<IntegrationControllerBase> logger,
        ICheckUpFactory<ClientData> checkUpFactory) // Change to ClientData
    {
        _checkUpService = checkUpFactory.Create();
        _logger = logger;
    }

    [HttpGet("sync")]
    public async Task<IActionResult> Sync(IntegrationStruct _struct)
    {
        _logger.LogInformation("Starting sync...");
        try
        {
            await Check(_struct);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sync failed");
            return StatusCode(500, "Internal error");
        }
    }

    private async Task<IActionResult> Check(IntegrationStruct _struct)
    {
        var item = _checkUpService.Check(_struct);
        if (item.Item1)
            return Ok();

        Message(item.Item2);
        return StatusCode(500, "There are mistakes in the data");
    }

    public void Message(string ex)
    {
        EmailMessageBuilder.PutInformation(EmailMessageBuilder.ListType.setcontragent, ex);
    }
}