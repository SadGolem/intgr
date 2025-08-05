using System.Text;
using System.Text.Json;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using Microsoft.Extensions.Options;

namespace integration.Services;

public abstract class HttpServiceBase : ServiceBase
{
    private readonly ILogger _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    protected HttpServiceBase(
        IHttpClientFactory httpClientFactory,
        ILogger<HttpServiceBase> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> apiSettings,
        JsonSerializerOptions? jsonOptions = null)
        : base(httpClientFactory, logger, authorizer, apiSettings)
    {
        _logger = logger;
        _jsonOptions = jsonOptions ?? new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    protected async Task<HttpResponseMessage> SendAsync(
        string url,
        HttpMethod method,
        object? data = null,
        bool requireAuthorization = true)
    {
        using var client = await GetHttpClient(requireAuthorization);
        using var request = CreateRequest(url, method, data);

        try
        {
            var response = await client.SendAsync(request);
            await EnsureSuccessResponse(response, url);
            return response;
        }
        catch (Exception ex) when (HandleException(ex, url, method, data))
        {
            throw;
        }
    }

    private HttpRequestMessage CreateRequest(string url, HttpMethod method, object? data)
    {
        var request = new HttpRequestMessage(method, url);

        if (data != null)
        {
            var jsonBody = JsonSerializer.Serialize(data, _jsonOptions);
            request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        return request;
    }

    protected async Task EnsureSuccessResponse(HttpResponseMessage response, string url)
    {
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            var ex = new HttpRequestException($"Response status code does not indicate success: {(int)response.StatusCode}.");
            ex.Data.Add("StatusCode", response.StatusCode);
            ex.Data.Add("ResponseContent", content);
            throw ex;
        }
    }

    private bool HandleException(Exception ex, string url, HttpMethod method, object? data)
    {
        var errorType = ex switch
        {
            HttpRequestException => "HTTP request error",
            JsonException => "JSON serialization error",
            _ => "Unexpected error"
        };

        _logger.LogError(ex, $"{errorType} during {method} request to {url}. DataResponse: {data ?? "null"}");
        return false;
    }

    protected virtual async Task<HttpClient> GetHttpClient(bool requireAuthorization)
    {
        return requireAuthorization 
            ? await Authorize(false) 
            : _httpClientFactory.CreateClient();
    }
}