using System.Text.Json;
using integration.Context;
namespace integration.Services.Location;

public class ServiceGetterBase<T> : ServiceBase
{
    private readonly ILogger _logger;
    public ServiceGetterBase(IHttpClientFactory httpClientFactory, HttpClient httpClient, ILogger<ServiceBase> logger, IConfiguration configuration) : base(httpClientFactory, httpClient, logger, configuration)
    {
        _logger = logger;
    }
    public async Task<List<ContractPositionData>> Get(IHttpClientFactory _httpClientFactory, string _connect)
    {
        var client = _httpClientFactory.CreateClient();
        await Authorize(client, true);
        try
        {
            var response = await client.GetAsync(_connect);

            response.EnsureSuccessStatusCode();  
            var responseContentString = await response.Content.ReadAsStringAsync();
            var responseContent = JsonSerializer.Deserialize<List<ContractPositionData>>(responseContentString);
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
    public override void Message(string ex)
    {
        throw new NotImplementedException();
    }
}