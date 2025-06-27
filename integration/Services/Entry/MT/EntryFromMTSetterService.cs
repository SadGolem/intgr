using AutoMapper;
using integration.Context.MT;
using integration.Context.Request.MT;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Entry.Storage;
using integration.Services.Interfaces;
using integration.Services.Location;
using Microsoft.Extensions.Options;

namespace integration.Services.Entry.MT;

public class EntryFromMTSetterService: ServiceSetterBase<EntryMTRequest>, ISetterService<EntryMTRequest>
{
    private readonly string _connectionString;
    private readonly IOptions<AuthSettings> _apiSettings;
    private readonly ILogger<EntryFromMTSetterService> _logger;
    private IHttpClientFactory _httpClientFactory;
    private IEntryStorageService<EntryMTDataResponse> _storageService;
    private List<(EntryMTDataResponse, bool)> _entriesData;
    private readonly IMapper _mapper;
    
    public EntryFromMTSetterService(IHttpClientFactory httpClientFactory, 
        ILogger<EntryFromMTSetterService> logger, 
        IAuthorizer authorizer,
        IOptions<AuthSettings> apiSettings,
        IEntryStorageService<EntryMTDataResponse> storageService,
        IMapper mapper) : base(httpClientFactory, logger, authorizer, apiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _apiSettings = apiSettings;
        _connectionString = _apiSettings.Value.APROconnect.BaseUrl + _apiSettings.Value.APROconnect.ApiClientSettings.EntryEndpointPATCH;
        _mapper = mapper;
        _storageService = storageService;
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
            foreach (var entryData in entry.Item1.Data)
            {
                var responce = _mapper.Map<EntryData, EntryMTRequest>(entryData);

                await Patch(_httpClientFactory, _connectionString + entryData.id + "/",
                    responce, true);
            }
        }
    }
}