using System.Text;
using System.Text.Json;
using integration.Exceptions;
using integration.Helpers.Auth;
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

    public async Task<string> GetTokenAsync(IAuth settings)
    {
        try
        {
            using var client = _httpClientFactory.CreateClient();

            var requestBody = new
            {
                username = settings.Login,
                password = settings.Password
            };

            // Serialize to JSON and create HTTP content
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(requestBody),
                Encoding.UTF8,
                "application/json"
            );

            var response = await client.PostAsync(
                settings.CallbackUrl,
                jsonContent // Pass the HttpContent here
            );

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
