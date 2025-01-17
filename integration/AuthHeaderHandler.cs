using Microsoft.Extensions.Caching.Memory;
using System.Net.Http.Headers;

namespace integration
{
    public class AuthHeaderHandler : DelegatingHandler
    {
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<AuthHeaderHandler> _logger;

        public AuthHeaderHandler(IMemoryCache memoryCache, ILogger<AuthHeaderHandler> logger)
        {
            _memoryCache = memoryCache;
            _logger = logger;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string? token = null;
            if (request.RequestUri != null)
            {
                token = _memoryCache.Get<string>($"Token_{request.RequestUri.Host}");
            }

            if (!string.IsNullOrEmpty(token))
            {
                _logger.LogInformation($"Adding Bearer token to request {request.RequestUri}, token: {token}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                _logger.LogWarning($"No token found for {request.RequestUri}.");
            }


            return await base.SendAsync(request, cancellationToken);
        }
    }
}

