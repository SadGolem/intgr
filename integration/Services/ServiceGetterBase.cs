using System.Text.Json;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using Microsoft.Extensions.Options;

namespace integration.Services.Location;

public class ServiceGetterBase<T> : ServiceBase
{
    private readonly ILogger _logger;

    public ServiceGetterBase(IHttpClientFactory httpClientFactory,
        ILogger<ServiceBase> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> apiSettings) : base(httpClientFactory, logger, authorizer, apiSettings)
    {
        _logger = logger;
    }

    public async Task<List<T>> Get(IHttpClientFactory _httpClientFactory, string _connect)
    {
        var client = await Authorize(true);
        try
        {
            var response = await client.GetAsync(_connect);

            response.EnsureSuccessStatusCode();
            var responseContentString = await response.Content.ReadAsStringAsync();
            var responseContent = JsonSerializer.Deserialize<List<T>>(responseContentString);
            return responseContent;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, $"HTTP error while getting datas");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unexpected Exception while getting datas");
            throw;
        }
    }

    public void Message(string ex)
    {
        throw new NotImplementedException();
    }
}