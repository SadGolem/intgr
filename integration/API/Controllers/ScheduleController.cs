using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ScheduleController : ControllerBase
{
    private readonly IScheduleManagerService _scheduleSyncService;
    private readonly ILogger<ScheduleController> _logger;

    public ScheduleController(
        IScheduleManagerService scheduleSyncService,
        ILogger<ScheduleController> logger)
    {
        _scheduleSyncService = scheduleSyncService;
        _logger = logger;
    }

    [HttpPost("sync")]
    public async Task<IActionResult> Sync()
    {
        try
        {
            await _scheduleSyncService.SyncAsync();
            return Ok("Schedule sync completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Schedule sync error");
            return StatusCode(500, "Internal server error");
        }
    }
}