using System.Text.Json;
using integration.Exceptions;
using integration.HelpClasses;
using integration.Services.Token.Interfaces;

namespace integration.Services.Token;

public class TokenService : ITokenService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TokenService> _logger;

    public TokenService(
        IHttpClientFactory httpClientFactory,
        ILogger<TokenService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<string> GetTokenAsync(AuthSettings settings)
    {
        try
        {
            using var client = _httpClientFactory.CreateClient();
            var response = await client.PostAsJsonAsync(
                settings.CallbackUrl, 
                new { settings.Login, settings.Password });

            if (!response.IsSuccessStatusCode)
            {
                throw new ApiAuthException(
                    $"Auth failed for {settings.CallbackUrl}. Status: {response.StatusCode}");
            }

            var responseData = await response.Content.ReadFromJsonAsync<TokenResponse>();
            return responseData?.Token ?? throw new ApiAuthException("Empty token received");
        }
        catch (HttpRequestException ex)
        {
            throw new ApiAuthException("HTTP request failed", ex);
        }
        catch (JsonException ex)
        {
            throw new ApiAuthException("Invalid token response format", ex);
        }
    }
    
    private record TokenResponse(string Token);
}
