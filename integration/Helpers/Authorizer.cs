using integration.HelpClasses;
using Microsoft.Extensions.Caching.Memory;

namespace integration
{
    public class Authorizer
    {
        private AuthSettings _mtConnectSettings;
        private readonly ILogger _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly TokenController _tokenController;
        private AuthSettings _aproConnectSettings;

        public Authorizer(ILogger logger, IMemoryCache memoryCache, IConfiguration configuration, TokenController tokenController)
        {
            _logger = logger;
            _memoryCache = memoryCache;
            _configuration = configuration;
            _aproConnectSettings = _configuration.GetSection("APROconnect").Get<AuthSettings>();
            _mtConnectSettings = _configuration.GetSection("MTconnect").Get<AuthSettings>();
            _tokenController = tokenController;
        }

        public async Task<string> GetCachedTokenMT()
        {
            var cacheKey = $"Token_{new Uri(_mtConnectSettings.CallbackUrl).Host}";
            if (_memoryCache.TryGetValue(cacheKey, out string cachedToken))
            {
                _logger.LogInformation($"Returning cached token: {cachedToken}");
                return cachedToken;
            }
          //  _logger.LogInformation("Getting new token.");
            await _tokenController.GetTokens();
            var token = TokenController.tokens.First().Key;
            _logger.LogInformation($"Got new token: {token}");
            return token;
        }

        public async Task<string> GetCachedTokenAPRO()
        {
            var cacheKey = $"Token_{new Uri(_aproConnectSettings.CallbackUrl).Host}";
            if (_memoryCache.TryGetValue(cacheKey, out string cachedToken))
            {
                _logger.LogInformation($"Returning cached token: {cachedToken}");
                return cachedToken;
            }

            _logger.LogInformation("Getting new token.");
            await _tokenController.GetTokens();
            var token = TokenController.tokens.First().Value;
            _logger.LogInformation($"Got new token: {token}");
            return token;
        }
    }
}
