using integration.Context;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Client.Storage;
using integration.Services.ContractPosition.Storage;
using integration.Services.Emitter.Storage;
using integration.Services.Interfaces;
using integration.Services.Location;
using Microsoft.Extensions.Options;

namespace integration.Services.Emitter;

public class EmitterGetterService : ServiceGetterBase<EmitterDataResponse>,
    IGetterService<EmitterDataResponse>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<EmitterGetterService> _logger;
    private readonly APROconnectSettings _apiSettings;
    private List<int> _locationIdSList;
    private List<ClientDataResponse> _clients;
    private IContractPositionStorageService _positionStorage;
    private IClientStorageService _clientStorage;
    private IEmitterStorageService _emitterStorage;

    private List<int> ids = new List<int>();
    
    public EmitterGetterService(IHttpClientFactory httpClientFactory, 
        ILogger<EmitterGetterService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> apiSettings,
        IContractPositionStorageService positionStorage,
        IEmitterStorageService storageService) : base(httpClientFactory, logger, authorizer, apiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _apiSettings = apiSettings.Value.APROconnect;

        _positionStorage = positionStorage;
        _emitterStorage = storageService;
    }
    public async Task Get()
    {
        try
        {
            // ?/*var clientIds = await GetClientIdentifiers();
            // var clients = await ProcessClientsAsync(clientIds);*/
            GetClientIdentifiers();
            FetchEmitterDataAsync();


        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Client synchronization failed");
            throw;
        }


    }

    private async Task GetClientIdentifiers()
    {
        var positions = _positionStorage.Get();
        foreach (var pos in positions)
        {
            ids.Add(pos.id);
        }
    }

    private async Task FetchEmitterDataAsync()
    {
        try
        {
            foreach (var id in ids)
            {
                var endpoint = BuildEmitterEndpoint(id.ToString());
                var response = await Get(_httpClientFactory, endpoint);
                var emitter = response.FirstOrDefault();
                emitter.amount = _positionStorage.GetPosOn_ID(id).value;
                emitter.contractNumber = _positionStorage.GetPosOn_ID(id).number;
                emitter.location_mt_id = _positionStorage.GetPosOn_ID(id).waste_site.ext_id == null ? 0 : _positionStorage.GetPosOn_ID(id).waste_site.ext_id;
                emitter.executorName = _positionStorage.GetPosOn_ID(id)?.contract?.assignee?.name;
                emitter.idContract = _positionStorage.GetPosOn_ID(id).contract.id;
                emitter.contractStatus = _positionStorage.GetPosOn_ID(id).contract?.status?.Name;
                
                _emitterStorage.Set(emitter);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to fetch emitter");
        }
    }

    private string BuildEmitterEndpoint(string root_id)
    {
        var basePath = _apiSettings.BaseUrl + (_apiSettings.ApiClientSettings.EmitterEndopint);

        return $"{basePath}{root_id}";
    }
    private record ClientIdentifier(int Id, string DocumentType)
    {
        public bool IsLegalEntity => DocumentType == "Юридические лица";
    }
}
