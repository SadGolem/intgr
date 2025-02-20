using System.Text;
using System.Text.Json;
using integration.Services.Interfaces;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace integration.Services.Location;

public class ServiceSetterBase<T> : ServiceBase
{
    private readonly ILogger _logger;
    public ServiceSetterBase(IHttpClientFactory httpClientFactory, HttpClient httpClient, ILogger<ServiceBase> logger, IConfiguration configuration) : base(httpClientFactory, httpClient, logger, configuration)
    {
        _logger = logger;
    }
    public async Task Post(IHttpClientFactory _httpClientFactory, string _connect, object mappedData)
    {
        var client = _httpClientFactory.CreateClient();
        await Authorize(client, false);
        try
        {
            var jsonBody = JsonSerializer.Serialize(mappedData);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            HttpResponseMessage response;
            response = await client.PostAsync(_connect, content);

            response.EnsureSuccessStatusCode();  
            var responseContent = await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, $"HTTP error while posting data with id: {mappedData}");
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, $"Json Exception while posting data with id: {mappedData}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unexpected Exception while posting data with id: {mappedData}");
            throw;
        }
    }
    
    public async Task Patch(IHttpClientFactory _httpClientFactory, string _connect, object mappedData)
    {
        var client = _httpClientFactory.CreateClient();
        await Authorize(client, false);
        try
        {
            var jsonBody = JsonSerializer.Serialize(mappedData);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            HttpResponseMessage response;
            response = await client.PatchAsync(_connect, content);

            response.EnsureSuccessStatusCode();  
            var responseContent = await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, $"HTTP error while patching data with id: {mappedData}");
            throw;
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, $"Json Exception while patching data with id: {mappedData}");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unexpected Exception while patching data with id: {mappedData}");
            throw;
        }
    }
    public override void Message(string ex)
    {
        throw new NotImplementedException();
    }
}