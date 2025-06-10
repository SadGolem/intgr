using AutoMapper;
using integration.Context.Request;
using integration.Helpers;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Entry.Storage;
using integration.Services.Interfaces;
using integration.Services.Location;
using Microsoft.Extensions.Options;

namespace integration.Services.Entry;

public class EntrySetterService : ServiceSetterBase<EntryDataResponse>, ISetterService<EntryDataResponse>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<EntrySetterService> _logger;
    private IAuthorizer _authorizer;
    private string _apiClientSettingsCreate;
    private string _apiClientSettingsUpdate;
    private ConnectingStringApro _aproConnect;
    private IEntryStorageService<EntryDataResponse> _storageService;
    private IMapper _mapper;
    private List<(EntryDataResponse, bool)> _entriesData;
    
    public EntrySetterService(IHttpClientFactory httpClientFactory,
        ILogger<EntrySetterService> logger, IAuthorizer authorizer,
        IOptions<AuthSettings> apiSettings, IEntryStorageService<EntryDataResponse> storageService, IMapper mapper) : base(httpClientFactory, logger, authorizer, apiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _authorizer = authorizer;
        _apiClientSettingsCreate = apiSettings.Value.MTconnect.BaseUrl+apiSettings.Value.MTconnect.ApiClientSettings.EntryEndpointCreate;
        _apiClientSettingsUpdate = apiSettings.Value.MTconnect.BaseUrl+apiSettings.Value.MTconnect.ApiClientSettings.EntryEndpointUpdate;
        _storageService = storageService;
        _mapper = mapper;
    }

    public async Task Set()
    {
        GetEntries();
        PostOrPatch();
    }

    private async Task GetEntries()
    {
        _entriesData  = _storageService.Get();
    }

    private async Task PostOrPatch()
    {
        foreach (var entry in _entriesData)
        {
            var responce = _mapper.Map<EntryDataResponse, EntryRequest>(entry.Item1);
            if (entry.Item2)
                await Post(_httpClientFactory,_apiClientSettingsCreate, _mapper.Map<EntryDataResponse,EntryRequest>(entry.Item1));
            else
            {
                await Patch(_httpClientFactory, _apiClientSettingsUpdate,
                    _mapper.Map<EntryDataResponse, EntryRequest>(entry.Item1));
            }
        }
    }

    public Task PostAndPatch(List<(EntryDataResponse, bool)> data)
    {
        throw new NotImplementedException();
    }
}