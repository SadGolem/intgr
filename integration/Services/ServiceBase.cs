using integration.Context;
using integration.HelpClasses;
using integration.Services.Interfaces;
using Microsoft.Net.Http;
namespace integration.Services
{
    public abstract class ServiceBase : IService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;
        private readonly ILogger<ServiceBase> _logger;
        private readonly IConfiguration _configuration;
        private readonly string _stringConnect;
        private ITokenService _tokenService;
        public ServiceBase (IHttpClientFactory httpClientFactory,HttpClient httpClient, ILogger<ServiceBase> logger, IConfiguration configuration, ITokenService tokenService)
        {
            _httpClientFactory = httpClientFactory;
            _httpClient = httpClient;
            _logger = logger;
            _configuration = configuration;
            _tokenService = tokenService;
            _stringConnect = configuration.GetSection("APROconnect").Get<AuthSettings>().CallbackUrl;
        }
        public virtual async Task Authorize(HttpClient httpClient, bool isAPRO)
        {
            var token = "";
            if (isAPRO)
                token = await _tokenService.GetCachedTokenAPRO();
            else
            {
                token = await _tokenService.GetCachedTokenMT();
            }

            httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }
        public abstract void Message(string ex);
    }
}
