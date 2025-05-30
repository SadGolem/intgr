using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using integration.Exceptions;
using integration.Helpers.Auth;
using integration.Services.Token.Interfaces;
using Microsoft.Extensions.Options;

namespace integration
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly ILogger<TokenController> _logger;
        private readonly ITokenService _tokenService;
        private readonly IMemoryCache _cache;
        private readonly APROconnectSettings _aproSettings;
        private readonly MTconnectSettings _mtSettings;

        public TokenController(
            ILogger<TokenController> logger,
            ITokenService tokenService,
            IMemoryCache cache,
            IOptions<AuthSettings> aproSettings,
            IOptions<AuthSettings> mtSettings)
        {
            _logger = logger;
            _tokenService = tokenService;
            _cache = cache;
            _aproSettings = aproSettings.Value.APROconnect;
            _mtSettings = mtSettings.Value.MTconnect;
        }

        [HttpPost("getTokens")]
        public async Task<IActionResult> GetTokens()
        {
            try
            {
                var (aproToken, mtToken) = await GetAndCacheTokensAsync();
                return Ok(new { TokenAPRO = aproToken, TokenMT = mtToken });
            }
            catch (ApiAuthException ex)
            {
                _logger.LogError(ex, "Authorization error");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Error = "Authorization failed",
                    Details = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error");
                return StatusCode(StatusCodes.Status500InternalServerError, new
                {
                    Error = "Internal server error"
                });
            }
        }

        private async Task<(string aproToken, string mtToken)> GetAndCacheTokensAsync()
        {
            var aproToken = await GetAndCacheTokenAsync(_aproSettings);
            var mtToken = await GetAndCacheTokenAsync(_mtSettings);
            return (aproToken, mtToken);
        }

        private async Task<string> GetAndCacheTokenAsync(IAuth settings)
        {
            var cacheKey = GetCacheKey(settings.CallbackUrl);
            if (_cache.TryGetValue(cacheKey, out string token))
            {
                _logger.LogInformation("Using cached token for {Service}", cacheKey);
                return token;
            }

            var newToken = await _tokenService.GetTokenAsync(settings);
            _cache.Set(cacheKey, newToken, TimeSpan.FromMinutes(55));

            return newToken;
        }

        private static string GetCacheKey(string url) => $"Token_{new Uri(url).Host}";
    }
}