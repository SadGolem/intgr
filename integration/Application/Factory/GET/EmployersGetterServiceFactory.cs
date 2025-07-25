using integration.Context.Response;
using integration.Factory.GET.Interfaces;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Employers;
using integration.Services.Employers.Storage;
using integration.Services.Interfaces;
using integration.Services.Storage;
using Microsoft.Extensions.Options;

namespace integration.Factory.GET;

public class EmployersGetterServiceFactory: IGetterServiceFactory<EmployerDataResponse>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<EmployerGetterService> _logger;
    private readonly IAuthorizer _authorizer;
    private readonly IOptions<AuthSettings> _configuration;
    private readonly IConverterToStorageService _converterToStorageService;
    private readonly IEmployersStorageService _storageService;

    public EmployersGetterServiceFactory(
        IHttpClientFactory httpClientFactory,
        ILogger<EmployerGetterService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> configuration,
        IEmployersStorageService storageService)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _authorizer = authorizer;
        _configuration = configuration;
        _storageService = storageService;
    }
    public IGetterService<EmployerDataResponse> Create()
    {
        return new EmployerGetterService(_httpClientFactory, _logger, _authorizer, _configuration, _storageService);
    }
}