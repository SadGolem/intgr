using System.Net.Http.Headers;
using integration.HelpClasses;
using integration.Helpers.Interfaces;
using integration.Services.Interfaces;
using Microsoft.Extensions.Options;
namespace integration.Services
{
    public abstract class ServiceBase : IService
    {
        protected readonly IHttpClientFactory _httpClientFactory;
        protected readonly ILogger<ServiceBase> _logger;
        protected readonly IAuthorizer _authorizer;
        protected readonly string _apiBaseUrl;

        protected ServiceBase(
            IHttpClientFactory httpClientFactory,
            ILogger<ServiceBase> logger,
            IAuthorizer authorizer,
            IOptions<AuthSettings> apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _authorizer = authorizer;
            _apiBaseUrl = apiSettings.Value.CallbackUrl;
        }

        protected ServiceBase(IHttpClientFactory httpClientFactory, ILogger<ServiceBase> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public virtual async Task<HttpClient> CreateAuthorizedClientAsync(AuthType authType = AuthType.APRO)
        {
            var client = _httpClientFactory.CreateClient();
            var token = await GetTokenAsync(authType);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        public void Message(string ex)
        {
            throw new NotImplementedException();
        }

        private async Task<string> GetTokenAsync(AuthType authType)
        {
            return authType switch
            {
                AuthType.APRO => await _authorizer.GetCachedTokenAPROAsync(),
                AuthType.MT => await _authorizer.GetCachedTokenMTAsync(),
                _ => throw new ArgumentOutOfRangeException(nameof(authType))
            };
        }

        public abstract Task HandleErrorAsync(string errorMessage);

        protected async Task Authorize(HttpClient client, bool useCache)
        {
            await GetTokenAsync(AuthType.APRO);
        }
    }

    public enum AuthType
    {
        APRO,
        MT
    }
}