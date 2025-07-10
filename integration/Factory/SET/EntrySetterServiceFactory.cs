using AutoMapper;
using integration.Factory.SET.Interfaces;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Entry;
using integration.Services.Entry.Storage;
using integration.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace integration.Factory.SET;

public class EntrySetterServiceFactory :  ISetterServiceFactory<EntryDataResponse>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<EntrySetterService> _logger;
    private readonly IAuthorizer _authorizer;
    private readonly IOptions<AuthSettings> _apiSettings;
    private readonly IEntryStorageService<EntryDataResponse> _storageService;
    private readonly IMapper _mapper;

    public EntrySetterServiceFactory(
        IHttpClientFactory httpClientFactory,
        ILogger<EntrySetterService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> apiSettings,
        IEntryStorageService<EntryDataResponse> storageService,
        IMapper mapper)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _authorizer = authorizer;
        _apiSettings = apiSettings;
        _storageService = storageService;
        _mapper = mapper;
    }

    public ISetterService<EntryDataResponse> Create()
    {
        return new EntrySetterService(_httpClientFactory, _logger, _authorizer, _apiSettings, _storageService, _mapper);
    }
}