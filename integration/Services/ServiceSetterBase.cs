using System.Text;
using System.Text.Json;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace integration.Services.Location;

public class ServiceSetterBase<T> : ServiceBase
{
    private readonly ILogger _logger;

    public async Task Post(IHttpClientFactory _httpClientFactory, string _connect, object mappedData, bool isApro)
    {
        var client = await Authorize(isApro);
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
    public async Task<string> Post( // Изменён возвращаемый тип на Task<string>
        IHttpClientFactory _httpClientFactory, 
        string _connect, 
        HttpContent content, // Переименовано mappedData -> content
        bool isApro)
    {
        var client = await Authorize(isApro);
        try
        {
            HttpResponseMessage response = await client.PostAsync(_connect, content);
            response.EnsureSuccessStatusCode();  
            return await response.Content.ReadAsStringAsync();
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, $"HTTP error while posting data");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unexpected Exception while posting data");
            throw;
        }
    }
    public async Task Patch(IHttpClientFactory _httpClientFactory, string _connect, object mappedData, bool isApro)
    {
        var client = await Authorize(isApro);
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
    public void Message(string ex)
    {
        throw new NotImplementedException();
    }

    public ServiceSetterBase(IHttpClientFactory httpClientFactory, ILogger<ServiceBase> logger, IAuthorizer authorizer, IOptions<AuthSettings> apiSettings) : base(httpClientFactory, logger, authorizer, apiSettings)
    {
    }
}