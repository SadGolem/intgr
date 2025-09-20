using System.Net.Http.Headers;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using Microsoft.Extensions.Options;
namespace integration.Services
{
    public abstract class ServiceBase
    {
        protected readonly IHttpClientFactory _httpClientFactory;
        protected readonly ILogger<ServiceBase> _logger;
        protected readonly IAuthorizer _authorizer;
        protected readonly IOptions<AuthSettings> _apiBaseUrl;

        protected ServiceBase(IHttpClientFactory httpClientFactory, ILogger<ServiceBase> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }
        
        protected ServiceBase(
            IHttpClientFactory httpClientFactory,
            ILogger<ServiceBase> logger,
            IAuthorizer authorizer,
            IOptions<AuthSettings> apiSettings)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _authorizer = authorizer;
            _apiBaseUrl = apiSettings;
        }



        public virtual async Task<HttpClient> CreateAuthorizedClientAsync(AuthType authType)
        {
            var client = _httpClientFactory.CreateClient();
            var token = await GetTokenAsync(authType);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        public void Message(string ex, int? ownerID)
        {
            EmailMessageBuilder.PutError(EmailMessageBuilder.ListType.getlocation, ex, ownerID);
        }
        public void MessageSuccesess(string ex,int idloc,  int? ownerID)
        {
            EmailMessageBuilder.PutSuccess(EmailMessageBuilder.ListType.getlocation,idloc, ownerID);
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
        protected async Task<HttpClient> Authorize(bool isApro)
        {
            if (isApro)
            {
                return await CreateAuthorizedClientAsync(AuthType.APRO);
            }
            return await CreateAuthorizedClientAsync(AuthType.MT);
        }
        
    }

    public enum AuthType
    {
        APRO,
        MT
    }
}