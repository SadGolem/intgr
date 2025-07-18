using AutoMapper;
using integration.Context.MT;
using integration.Context.Request;
using integration.Context.Request.MT;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Entry.MT;
using integration.Services.Interfaces;
using integration.Services.Location;
using Microsoft.Extensions.Options;

namespace integration.Services.Agre.Storage;

public class AgreSetterService : ServiceSetterBase<AgreRequest>, ISetterService<AgreRequest>
{
    private readonly string _connectionStringGetLocation;
    private readonly string _connectionString;
    private readonly IOptions<AuthSettings> _apiSettings;
    private readonly ILogger<AgreSetterService> _logger;
    private IHttpClientFactory _httpClientFactory;
    private IAgreStorageService _storageService;
    private List<(AgreData, int)> _data;
    private readonly IMapper _mapper;

    public AgreSetterService(IHttpClientFactory httpClientFactory,
        ILogger<AgreSetterService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> apiSettings,
        IAgreStorageService storageService,
        IMapper mapper) : base(httpClientFactory, logger, authorizer, apiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _apiSettings = apiSettings;
        _connectionString = _apiSettings.Value.APROconnect.BaseUrl
                            + _apiSettings.Value.APROconnect.ApiClientSettings.EntryEndpointPATCH;
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
        _data = _storageService.Get();
    }

    private async Task PostOrPatch()
    {
        foreach (var message in _data)
        {
            var requestBody = _mapper.Map<AgreData, AgreRequest>(message.Item1);

            await Patch(
                _httpClientFactory,
                $"{_connectionString}{message.Item2}/",
                requestBody,
                true
            );

        }
    }
}