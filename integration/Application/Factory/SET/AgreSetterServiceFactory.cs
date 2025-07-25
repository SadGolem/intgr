using AutoMapper;
using integration.Context;
using integration.Context.MT;
using integration.Context.Request;
using integration.Factory.SET.Interfaces;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Agre.Storage;
using integration.Services.Interfaces;
using integration.Services.Storage.Interfaces;
using Microsoft.Extensions.Options;

namespace integration.Factory.SET;

public class AgreSetterServiceFactory : ISetterServiceFactory<AgreRequest>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AgreSetterService> _logger;
    private readonly IAuthorizer _authorizer;
    private readonly IOptions<AuthSettings> _configuration;
    private readonly IMapper _mapper;
    
    private IAgreStorageService _storageService;
    
    public AgreSetterServiceFactory(
        IHttpClientFactory httpClientFactory,
        ILogger<AgreSetterService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> configuration,
        IAgreStorageService storageService,
        IMapper mapper)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _authorizer = authorizer;
        _configuration = configuration;
        _storageService = storageService;
        _mapper = mapper;
    }
    public ISetterService<AgreRequest> Create()
    {
        return new AgreSetterService(_httpClientFactory, _logger, _authorizer, _configuration, _storageService, _mapper);
    }
}