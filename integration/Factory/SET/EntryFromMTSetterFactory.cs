using AutoMapper;
using integration.Context.Request.MT;
using integration.Factory.SET.Interfaces;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Entry;
using integration.Services.Entry.MT;
using integration.Services.Entry.Storage;
using integration.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace integration.Factory.SET;

public class EntryFromMTSetterFactory:  ISetterServiceFactory<EntryMTRequest>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<EntryFromMTSetterService> _logger;
    private readonly IAuthorizer _authorizer;
    private readonly IOptions<AuthSettings> _apiSettings;
    private readonly IEntryStorageService<EntryMTDataResponse> _storageService;
    private readonly IMapper _mapper;

    public EntryFromMTSetterFactory(
        IHttpClientFactory httpClientFactory,
        ILogger<EntryFromMTSetterService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> apiSettings,
        IEntryStorageService<EntryMTDataResponse> storageService,
        IMapper mapper)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _authorizer = authorizer;
        _apiSettings = apiSettings;
        _storageService = storageService;
        _mapper = mapper;
    }

    public ISetterService<EntryMTRequest> Create()
    {
        return new EntryFromMTSetterService(_httpClientFactory, _logger, _authorizer, _apiSettings, _storageService, _mapper);
    }
}