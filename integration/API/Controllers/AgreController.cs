using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AgreController : ControllerBase
{
    private readonly IAgreManagerService _agreSyncService;
    private readonly ILogger<AgreController> _logger;

    public AgreController(
        IAgreManagerService agreSyncService,
        ILogger<AgreController> logger)
    {
        _agreSyncService = agreSyncService;
        _logger = logger;
    }

    [HttpPost("sync")]
    public async Task<IActionResult> Sync()
    {
        try
        {
            await _agreSyncService.SyncAsync();
            return Ok("Agre sync completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Agre sync error");
            return StatusCode(500, "Internal server error");
        }
    }
}