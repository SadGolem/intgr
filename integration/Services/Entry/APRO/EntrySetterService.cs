using AutoMapper;
using integration.Context.Request;
using integration.Helpers;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Entry.Storage;
using integration.Services.Interfaces;
using integration.Services.Location;
using integration.Services.Storage.Interfaces;
using Microsoft.Extensions.Options;

namespace integration.Services.Entry;

public class EntrySetterService : ServiceSetterBase<EntryDataResponse>, ISetterService<EntryDataResponse>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<EntrySetterService> _logger;
    private IAuthorizer _authorizer;
    private string _apiClientSettings;
    private ConnectingStringApro _aproConnect;
    private IEntryStorageService _storageService;
    private IMapper _mapper;
    private List<(EntryDataResponse, bool)> _entriesData;
    
    public EntrySetterService(IHttpClientFactory httpClientFactory,
        ILogger<EntrySetterService> logger, IAuthorizer authorizer,
        IOptions<AuthSettings> apiSettings, IEntryStorageService storageService) : base(httpClientFactory, logger, authorizer, apiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _authorizer = authorizer;
        _apiClientSettings = apiSettings.Value.MTconnect.ApiClientSettings.EntryEndpoint;
        _storageService = storageService;
    }

    public async Task PostAndPatch()
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
            if (entry.Item2)
                await Post(_httpClientFactory,_apiClientSettings, _mapper.Map<EntryDataResponse,EntryRequest>(entry.Item1));
            else
            {
                await Patch(_httpClientFactory, _apiClientSettings,
                    _mapper.Map<EntryDataResponse, EntryRequest>(entry.Item1));
            }
        }
    }

    public Task PostAndPatch(List<(EntryDataResponse, bool)> data)
    {
        throw new NotImplementedException();
    }
}