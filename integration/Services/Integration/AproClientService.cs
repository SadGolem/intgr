using System.Text;
using System.Text.Json;

public interface IAproClientService
{
    Task PatchAsync(string url, object data);
}

public class AproClientService : IAproClientService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AproClientService> _logger;

    public AproClientService(HttpClient httpClient, ILogger<AproClientService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task PatchAsync(string url, object data)
    {
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        var response = await _httpClient.PatchAsync(url, content);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError($"PATCH to {url} failed: {response.StatusCode}. Response: {errorContent}");
            throw new HttpRequestException($"Request failed with status {response.StatusCode}");
        }
    }
}