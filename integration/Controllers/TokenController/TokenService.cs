using integration.HelpClasses;

namespace integration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

public class TokenService : ITokenService
{
    private readonly IMemoryCache _memoryCache;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TokenService> _logger;
    private readonly AuthSettings _mtConnectSettings;  // Same AuthSettings class as in TokenController
    private readonly AuthSettings _aproConnectSettings;  // Same AuthSettings class as in TokenController

    public TokenService(IMemoryCache memoryCache, IConfiguration configuration, ILogger<TokenService> logger)
    {
        _memoryCache = memoryCache;
        _configuration = configuration;
        _logger = logger;
        _mtConnectSettings = _configuration.GetSection("MTconnect").Get<AuthSettings>();
        _aproConnectSettings = _configuration.GetSection("APROconnect").Get<AuthSettings>();
    }

    public async Task<string> GetCachedTokenMT()
    {
        return await GetCachedToken(true);
    }
    public async Task<string> GetCachedTokenAPRO()
    {
        return await GetCachedToken(false);
    }

    private async Task<string> GetCachedToken(bool isMT)
    {
        AuthSettings connection;
        connection = isMT ? _mtConnectSettings : _aproConnectSettings;

        try
        {
            var today = DateTime.UtcNow.Date;
            var cacheKey = $"Token_{new Uri(connection.CallbackUrl).Host}_{today:yyyyMMdd}";

            if (_memoryCache.TryGetValue(cacheKey, out string token))
            {
                _logger.LogInformation("Retrieved token from cache in TokenService.");
                return token;
            }
            else
            {
                _logger.LogWarning("Token not found in cache in TokenService.  Consider refreshing.");
                return null; // Or throw an exception, depending on your needs
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving token from cache in TokenService.");
            return null; // Or throw an exception
        }
    }
}

