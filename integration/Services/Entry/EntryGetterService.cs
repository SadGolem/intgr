using System.Data.Common;
using integration.Helpers;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Entry.Storage;
using integration.Services.Interfaces;
using integration.Services.Location;
using Microsoft.Extensions.Options;

namespace integration.Services.Entry;

public class EntryGetterService : ServiceGetterBase<EntryDataResponse>, IGetterService<EntryDataResponse>
{
    private readonly string _connectionString;
    private readonly IOptions<AuthSettings> _apiSettings;
    private readonly ILogger<EntryGetterService> _logger;
    private IHttpClientFactory _httpClientFactory;
    private IEntryStorageService _storageService;

    public EntryGetterService(
        IHttpClientFactory httpClientFactory,
        ILogger<EntryGetterService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> apiSettings,
        IEntryStorageService storageService)
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
        var response = await Get(_httpClientFactory, _connectionString);
        GetNewEntry(response);
    }

    private void GetNewEntry(List<EntryDataResponse> entry)
    {
        var lastUpdate = TimeManager.GetLastUpdateTime("entry");

        foreach (var data in entry)
        {
            try
            {
                var isNew = DetermineIfNew(data, lastUpdate);
                _storageService.Set(data, isNew);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing entry {LocationId}", data.BtNumber);
            }
        }
    }

    private bool DetermineIfNew(EntryDataResponse entry, DateTime lastUpdate)
    {
        if (entry.datetime_create > lastUpdate)
        {
            return true;
        }
            
        if (entry.datetime_update > lastUpdate)
        {
            return false;
        }
            
        return false;
    }
    
    private string BuildEmitterEndpoint()
    {
        var basePath = _apiSettings.Value.APROconnect.BaseUrl + (_apiSettings.ApiClientSettings.EntryEndopint);
        string connectionString = new ConnectingStringApro(_apiSettings,basePath).GetAproConnectSettings();
        
       
        return $"{basePath}";
    }
}