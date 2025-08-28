using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class EmployerController : ControllerBase
{
    private readonly IEmployerManagerService _employerSyncService;
    private readonly ILogger<EmployerController> _logger;

    public EmployerController(
        IEmployerManagerService employerSyncService,
        ILogger<EmployerController> logger)
    {
        _employerSyncService = employerSyncService;
        _logger = logger;
    }

    [HttpGet("get employers' emails")]
    public async Task<IActionResult> Sync()
    {
        try
        {
            await _employerSyncService.SyncAsync();
            return Ok("Employer sync completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Employer sync error");
            return StatusCode(500, "Internal server error");
        }
    }
}