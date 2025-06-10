using integration.Context;
using integration.Helpers;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Entry.Storage;
using integration.Services.Interfaces;
using integration.Services.Location;
using integration.Utilities;
using Microsoft.Extensions.Options;
using Type = integration.Context.Type;

namespace integration.Services.Entry;

public class EntryGetterService : ServiceGetterBase<EntryDataResponse>, IGetterService<EntryDataResponse>
{
    private readonly string _connectionString;
    private readonly IOptions<AuthSettings> _apiSettings;
    private readonly ILogger<EntryGetterService> _logger;
    private IHttpClientFactory _httpClientFactory;
    private IEntryStorageService<EntryDataResponse> _storageService;

    public EntryGetterService(
        IHttpClientFactory httpClientFactory,
        ILogger<EntryGetterService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> apiSettings,
        IEntryStorageService<EntryDataResponse> storageService)
        : base(httpClientFactory, logger, authorizer, apiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _apiSettings = apiSettings;
        _storageService = storageService;
    }

    public async Task Get()
    {
        var endpoint = BuildEmitterEndpoint(false);
        var response = await Get(_httpClientFactory, endpoint);
        await ProcessEntries(response);
    }

    public async Task<Capacity> GetCapacity(EntryDataResponse entry)
    {
        try
        {
            var endpoint = BuildEmitterEndpoint(true) + entry.BtNumber;
            var response = await Get(_httpClientFactory, endpoint);
        
            if (response == null || !response.Any())
            {
                _logger.LogWarning("Пустой ответ при получении capacity для {BtNumber}", entry.BtNumber);
                return null;
            }
        
            var capacity = response.FirstOrDefault()?.Capacity;
            return capacity;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении capacity для {BtNumber}", entry.BtNumber);
            return new Capacity { volume = 1 }; 
        }
    }

    private async Task ProcessEntries(List<EntryDataResponse> entries)
    {
        var lastUpdate = TimeManager.GetLastUpdateTime("entry");
        bool anyEntryProcessed = false;
        bool shouldUpdateTime = false;

        foreach (var data in entries)
        {
            try
            {
                var status = GetEntryStatus(data, lastUpdate);
                if (status == EntryStatus.Outdated)
                {
                    _logger.LogDebug("Пропуск записи {BtNumber}: не обновлялась", data.BtNumber);
                    continue;
                }

                if (DetermineIsHasNotAAgreement(data))
                {
                    _logger.LogInformation("Пропуск записи {BtNumber}: отсутствует соглашение", data.BtNumber);
                    continue;
                }

                data.Capacity = await GetCapacity(data);
                ///// убрать потом!!!
                data.Capacity = new Capacity();
                data.Capacity.type = new Types();
                data.Capacity.volume = 1;
                data.Capacity.id = 1;
                data.Capacity.type.id = 4;
            
            if (data.Capacity is null)
                {
                    _logger.LogInformation("Пропуск записи {BtNumber}: отсутствует соглашение", data.BtNumber);
                    Message($"Entry id {data.BtNumber} is not has a capacity." );
                    continue;
                }
                
                data.idContainerType = ContainerFinder.FindContainerId(data.Capacity.id, data.Capacity.type.id);
                data.statusString = StatusCoder.ToCorrectStatusEntryToMT(data);
                bool isNew = status == EntryStatus.New;
                _storageService.Set(data, isNew);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка обработки записи {BtNumber}", data.BtNumber);
            }
        }

        TimeManager.SetLastUpdateTime("entry");
    }

    private EntryStatus GetEntryStatus(EntryDataResponse entry, DateTime lastUpdate)
    {
        if (entry.datetime_create > lastUpdate)
            return EntryStatus.New;

        if (entry.datetime_update > lastUpdate)
            return EntryStatus.Updated;

        return EntryStatus.Outdated;
    }

    private enum EntryStatus
    {
        New, 
        Updated, 
        Outdated 
    }

    private bool DetermineIsHasNotAAgreement(EntryDataResponse entry)
    {
        if (entry.agreement is null)
        {
            Message($"Entry id {entry.BtNumber} is not has a agreement.");
            return true;
        }

        return false;
    }

    private string BuildEmitterEndpoint(bool isCapacity)
    {
        string basePath;
        if (!isCapacity)
        {
            basePath = _apiSettings.Value.APROconnect.ApiClientSettings.EntryEndpoint;
            basePath = new ConnectingStringApro(_apiSettings, basePath).GetAproConnectSettings();
        }
        else
        {
            basePath = _apiSettings.Value.APROconnect.BaseUrl +
                       (_apiSettings.Value.APROconnect.ApiClientSettings.EntryEndpointCapacity);
        }

        return $"{basePath}";
    }

    void Message(string ex)
    {
        EmailMessageBuilder.PutInformation(EmailMessageBuilder.ListType.getentry, ex);
    }
}