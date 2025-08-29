using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class ClientController : ControllerBase
{
    private readonly IClientManagerService _clientSyncService;
    private readonly ILogger<ClientController> _logger;

    public ClientController(
        IClientManagerService clientSyncService,
        ILogger<ClientController> logger)
    {
        _clientSyncService = clientSyncService;
        _logger = logger;
    }

    [HttpGet("get contragent")]
    public async Task<IActionResult> Sync()
    {
        try
        {
            await _clientSyncService.SyncAsync();
            return Ok("Client sync completed");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Client sync error");
            return StatusCode(500, "Internal server error");
        }
    }
}