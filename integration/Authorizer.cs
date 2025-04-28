using integration.Context;
using integration.Controllers.Apro;
using integration.Controllers.MT;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Http;

namespace integration
{
    public class Authorizer
    {
        private AuthSettings _mtConnectSettings;
        private readonly ILogger _logger;
        private readonly IMemoryCache _memoryCache;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenController;
        
        private AuthSettings _aproConnectSettings;

        public Authorizer(ILogger logger, IMemoryCache memoryCache, IConfiguration configuration, ITokenService tokenController)
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
          //  _logger.LogInformation("Getting new token.");
            string token = await _tokenController.GetCachedTokenMT();
            _logger.LogInformation($"Got new token: {token}");
            return token;
        }

        public async Task<string> GetCachedTokenAPRO()
        {
            
            _logger.LogInformation("Getting new token.");
            string token = await _tokenController.GetCachedTokenAPRO();
            _logger.LogInformation($"Got new token: {token}");
            return token;
        }
    }
}
