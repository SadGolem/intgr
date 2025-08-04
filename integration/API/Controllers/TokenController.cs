using integration.Exceptions;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class TokenController : ControllerBase
{
    private readonly ILogger<TokenController> _logger;
    private readonly ITokenManagerService _tokenManager;

    public TokenController(
        ILogger<TokenController> logger,
        ITokenManagerService tokenManager)
    {
        _logger = logger;
        _tokenManager = tokenManager;
    }

    [HttpPost("getTokens")]
    public async Task<IActionResult> GetTokens()
    {
        try
        {
            var (aproToken, mtToken) = await _tokenManager.GetTokensAsync();
            return Ok(new { TokenAPRO = aproToken, TokenMT = mtToken });
        }
        catch (ApiAuthException ex)
        {
            return StatusCode(500, new { Error = "Authorization failed", Details = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, new { Error = "Internal server error" });
        }
    }
}