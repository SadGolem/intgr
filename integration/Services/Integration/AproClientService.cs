using System.Text;
using System.Text.Json;
using integration;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services;
using integration.Services.Location;
using Microsoft.Extensions.Options;

public interface IAproClientService
{
    Task PatchAsync(string url, object data);
}

public class AproClientService :ServiceSetterBase<AproClientService>, IAproClientService
{
    private readonly ILogger<AproClientService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IAuthorizer _authorizer;
    private readonly AuthSettings _apiSettings;

    public AproClientService(IHttpClientFactory httpClientFactory, ILogger<AproClientService> logger, IAuthorizer authorizer, IOptions<AuthSettings> apiSettings) : base(httpClientFactory, logger, authorizer, apiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _authorizer = authorizer;
        _apiSettings = apiSettings.Value;
    }

    public async Task PatchAsync(string url, object data)
    {
        /*var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json"); 
        using var _httpClient = await Authorize(true);*/

        await Patch(_httpClientFactory, url, data, true);

    }
}