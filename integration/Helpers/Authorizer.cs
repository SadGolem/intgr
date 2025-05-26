using integration.HelpClasses;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Token.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace integration
{
    public class Authorizer : IAuthorizer
    {
        private readonly ILogger<Authorizer> _logger;
        private readonly IMemoryCache _cache;
        private readonly ITokenService _tokenService;
        private readonly APROconnectSettings _aproSettings;
        private readonly MTconnectSettings _mtSettings;

        public Authorizer(
            ILogger<Authorizer> logger,
            IMemoryCache cache,
            ITokenService tokenService,
            IOptions<AuthSettings> authSettings
            )
        {
            _logger = logger;
            _cache = cache;
            _tokenService = tokenService;
            _aproSettings = authSettings.Value.APROconnect;
            _mtSettings = authSettings.Value.MTconnect;
        }

        public async Task<string> GetCachedTokenMTAsync()
            => await GetTokenAsync(_mtSettings);

        public async Task<string> GetCachedTokenAPROAsync()
            => await GetTokenAsync(_aproSettings);

        private async Task<string> GetTokenAsync(IAuth settings)
        {
            var cacheKey = $"Token_{new Uri(settings.CallbackUrl).Host}";

            if (_cache.TryGetValue(cacheKey, out string token))
            {
                _logger.LogDebug("Returning cached token for {Host}", cacheKey);
                return token;
            }

            _logger.LogInformation("Requesting new token for {Host}", cacheKey);

            var newToken = await _tokenService.GetTokenAsync(settings);
            _cache.Set(cacheKey, newToken, TimeSpan.FromMinutes(55));

            return newToken;
        }
    }
}
