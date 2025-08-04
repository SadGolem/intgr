using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class EmitterController : ControllerBase
{
    private readonly IEmitterManagerService _emitterSyncService;
    private readonly ILogger<EmitterController> _logger;

    public EmitterController(
        IEmitterManagerService emitterSyncService,
        ILogger<EmitterController> logger)
    {
        _emitterSyncService = emitterSyncService;
        _logger = logger;
    }

    [HttpPost("sync")]
    public async Task<IActionResult> Sync()
    {
        try
        {
            await _emitterSyncService.SyncAsync();
            return Ok("Emitter sync completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Emitter sync error");
            return StatusCode(500, "Internal server error");
        }
    }
}