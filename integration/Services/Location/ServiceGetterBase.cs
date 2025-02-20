using System.Text;
using System.Text.Json;
using integration.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace integration.Services.Location;

public class ServiceGetterBase<T> : ServiceBase
{
    private readonly ILogger _logger;
    public ServiceGetterBase(IHttpClientFactory httpClientFactory, HttpClient httpClient, ILogger<ServiceBase> logger, IConfiguration configuration) : base(httpClientFactory, httpClient, logger, configuration)
    {
        _logger = logger;
    }
    public async Task<string> Get(IHttpClientFactory _httpClientFactory, string _connect)
    {
        var client = _httpClientFactory.CreateClient();
        await Authorize(client, false);
        try
        {
            HttpResponseMessage response;
            response = await client.GetAsync(_connect);

            response.EnsureSuccessStatusCode();  
            var responseContent = await response.Content.ReadAsStringAsync();
            return "Succesess getted" + responseContent;
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
    public override void Message(string ex)
    {
        throw new NotImplementedException();
    }
}