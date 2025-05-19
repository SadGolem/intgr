using System.Text.Json;
using integration.Context;
using integration.HelpClasses;
using integration.Helpers.Interfaces;
using Microsoft.Extensions.Options;

namespace integration.Services.Location;

public class ServiceGetterBase<T> : ServiceBase
{
    private readonly ILogger _logger;

    public ServiceGetterBase(IHttpClientFactory httpClientFactory, ILogger<ServiceBase> logger, IAuthorizer authorizer, IOptions<AuthSettings> apiSettings) : base(httpClientFactory, logger, authorizer, apiSettings)
    {
        _logger = logger;
    }    public async Task<List<T>> Get(IHttpClientFactory _httpClientFactory, string _connect)
    {
        var client = _httpClientFactory.CreateClient();
        await Authorize(client, true);
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

    public override Task HandleErrorAsync(string errorMessage)
    {
        throw new NotImplementedException();
    }

  
}