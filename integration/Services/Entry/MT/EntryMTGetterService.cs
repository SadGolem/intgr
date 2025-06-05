using integration.Context.MT;
using integration.Helpers;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Entry.Storage;
using integration.Services.Interfaces;
using integration.Services.Location;
using Microsoft.Extensions.Options;

namespace integration.Services.Entry.MT;

public class EntryMTGetterService : ServiceGetterBase<EntryMTDataResponse>, IGetterService<EntryMTDataResponse>
{
     private readonly string _connectionString;
    private readonly IOptions<AuthSettings> _apiSettings;
    private readonly ILogger<EntryMTGetterService> _logger;
    private IHttpClientFactory _httpClientFactory;
    private IEntryStorageService<EntryMTDataResponse> _storageService;

    public EntryMTGetterService(
        IHttpClientFactory httpClientFactory,
        ILogger<EntryMTGetterService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> apiSettings,
        IEntryStorageService<EntryMTDataResponse> storageService)
        : base(httpClientFactory, logger, authorizer, apiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _apiSettings = apiSettings;
        _storageService = storageService;
    }
    
    public async Task Get()
    {
        var endpoint = BuildEmitterEndpoint();
        var response = await Get(_httpClientFactory, endpoint);
        GetNewEntry(response);
    }
    

    private async Task GetNewEntry(List<EntryMTDataResponse> entry)
    {
        var lastUpdate = TimeManager.GetLastUpdateTime("entryMT");

        foreach (var data in entry)
        {
            try
            {
              //  var isNew = DetermineIfNew(data, lastUpdate);
                _storageService.Set(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing entry {LocationId}", data.idAPRO);
            }
        }
    }
    
    private string BuildEmitterEndpoint()
    {
        string basePath;
             basePath = _apiSettings.Value.APROconnect.BaseUrl +
                           (_apiSettings.Value.APROconnect.ApiClientSettings.EntryEndpoint);
             basePath = new ConnectingStringApro(_apiSettings, basePath).GetAproConnectSettings();
        return $"{basePath}";
    }
}