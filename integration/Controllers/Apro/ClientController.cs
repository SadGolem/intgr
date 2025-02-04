using integration.Context;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using integration;

[ApiController]
[Route("api/[controller]")]
public class ClientController : ControllerBase
{
    private readonly string _aproConnectSettings;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ClientController> _logger;
    private string url = "";

    public ClientController(
        IHttpClientFactory httpClientFactory,
        ILogger<ClientController> logger,
        IConfiguration configuration
        ) // Inject rather than build
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        // The Apro endpoint can be injected directly instead of creating a new instance here.
        ConnectngStringApro connectngStringApro = new ConnectngStringApro(configuration, url);
        _aproConnectSettings = connectngStringApro.GetAproConnectSettings();
    }

    [HttpGet]
    public async Task<IActionResult> GetEntriesData()
    {
        _logger.LogInformation("Starting manual entry sync...");
        try
        {
            var entries = await FetchAndProcessEntries();
            if (entries == null)
            {
                return StatusCode(500, "Error during locations sync.");
            }
            return Ok(entries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during location sync.");
            return StatusCode(500, "Error during location sync.");
        }
    }

    private async Task<SyncResult> FetchAndProcessEntries()
    {
        _logger.LogInformation($"Fetching locations from {_aproConnectSettings}...");
        List<EntryData> entries;
        try
        {
            entries = await FetchEntryData();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during locations fetch");
            return null;
        }

        _logger.LogInformation($"Received {entries.Count} locations");
        var lastUpdate = LastUpdateTextFileManager.GetLastUpdateTime("entry");
        var newEntries = new List<EntryData>();
        var updateEntries = new List<EntryData>();

        foreach (var entry in entries)
        {
            if (entry.DateTimeUpdate > lastUpdate)
            {
                updateEntries.Add(entry);
            }
            else if (entry.DateTimeCreate > lastUpdate)
            {
                newEntries.Add(entry);
            }
        }
        return new SyncResult(newEntries, updateEntries);
    }

    private async Task<List<EntryData>> FetchEntryData()
    {
        var entries = new List<EntryData>();
        var token = await TokenController._authorizer.GetCachedTokenAPRO();
        using var httpClient = _httpClientFactory.CreateClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        try
        {
            var response = await httpClient.GetAsync(_aproConnectSettings);

            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            entries = JsonSerializer.Deserialize<List<EntryData>>(content);
            LastUpdateTextFileManager.SetLastUpdateTime("entry");
            return entries;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, $"HTTP Error fetching entries. Status code {ex.StatusCode}, URL: {_aproConnectSettings} ");
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching or processing entries from: {_aproConnectSettings}");
            throw;
        }
    }
}

public record SyncResult(List<EntryData> NewEntries, List<EntryData> UpdateEntries);