using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class EntryController : ControllerBase
{
    private readonly IEntryManagerService _entrySyncService;
    private readonly ILogger<EntryController> _logger;

    public EntryController(
        IEntryManagerService entrySyncService,
        ILogger<EntryController> logger)
    {
        _entrySyncService = entrySyncService;
        _logger = logger;
    }

    [HttpPost("sync")]
    public async Task<IActionResult> Sync()
    {
        try
        {
            await _entrySyncService.SyncAsync();
            return Ok("Entry sync completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Entry sync error");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("syncMT")]
    private async Task<IActionResult> SyncMT()
    {
        try
        {
            await _entrySyncService.GetMTAsync();
            await _entrySyncService.SetToMTAsync();
            return Ok("Entry MT sync completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Entry MT sync error");
            return StatusCode(500, "Internal server error");
        }
    }
}