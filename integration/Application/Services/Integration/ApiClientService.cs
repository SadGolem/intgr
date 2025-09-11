using System.Text.Json;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using Microsoft.Extensions.Options;

namespace integration.Services.Integration;
public interface IApiClientService
{
        Task<TResponse> SendAsync<TRequest, TResponse>(TRequest request, string url, HttpMethod method)
            where TRequest : class
            where TResponse : class;
    
        Task SendAsync<TRequest>(TRequest request, string url, HttpMethod method)
            where TRequest : class;
        
        Task<string> SendAndGetStringAsync<TRequest>(TRequest request, string url, HttpMethod method)
            where TRequest : class;

}
public class ApiClientService : HttpServiceBase, IApiClientService
{
    public ApiClientService(
        IHttpClientFactory httpClientFactory,
        ILogger<ApiClientService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> apiSettings)
        : base(httpClientFactory, logger, authorizer, apiSettings)
    {
    }

    public async Task<TResponse> SendAsync<TRequest, TResponse>(
        TRequest request, 
        string url, 
        HttpMethod method)
        where TRequest : class
        where TResponse : class
    {
        var response = await base.SendAsync(url, method, request);
        var content = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TResponse>(content) 
               ?? throw new InvalidOperationException("Deserialization returned null");
    }

    public async Task SendAsync<TRequest>(TRequest request, string url, HttpMethod method)
        where TRequest : class
    {
        await base.SendAsync(url, method, request);
    }

    public async Task<string> SendAndGetStringAsync<TRequest>(TRequest request, string url, HttpMethod method)
        where TRequest : class
    {
        var response = await base.SendAsync(url, method, request);
        return await response.Content.ReadAsStringAsync();
    }
}