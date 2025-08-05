using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class LocationController : ControllerBase
{
    private readonly ILocationManagerService _locationSyncService;
    private readonly ILogger<LocationController> _logger;

    public LocationController(
        ILocationManagerService locationSyncService,
        ILogger<LocationController> logger)
    {
        _locationSyncService = locationSyncService;
        _logger = logger;
    }

    [HttpPost("sync")]
    public async Task<IActionResult> Sync()
    {
        try
        {
            await _locationSyncService.SyncLocationsAsync();
            return Ok("Locations synced successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Location sync error");
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpPost("syncMT")]
    public async Task<IActionResult> SyncMT()
    {
        try
        {
            await _locationSyncService.GetFromMTAsync();
            await _locationSyncService.SetFromMTAsync();
            return Ok("Location MT sync completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Location MT sync error");
            return StatusCode(500, "Internal server error");
        }
    }
}