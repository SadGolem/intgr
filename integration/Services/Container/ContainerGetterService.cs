using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.ContractPosition.Storage;
using integration.Services.Interfaces;
using integration.Services.Location;
using Microsoft.Extensions.Options;


namespace integration.Services.Container;

public class ContainerGetterService: ServiceGetterBase<Context.Container>,
    IGetterService<Context.Container>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ContainerGetterService> _logger;
    private IContractStorageService _contractStorageService;
    private IContractPositionStorageService _contractPositionStorageService;
    private List<ContractPositionDataResponse> _contractPosition = new List<ContractPositionDataResponse>();

    private readonly string _aproConnect;
    

    public ContainerGetterService(IHttpClientFactory httpClientFactory,
        ILogger<ContainerGetterService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> apiSettings,
        IContractPositionStorageService contractPositionStorageService) : base(httpClientFactory, logger, authorizer, apiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _contractPositionStorageService = contractPositionStorageService;
        _aproConnect = apiSettings.Value.APROconnect.BaseUrl +
                       apiSettings.Value.APROconnect.ApiClientSettings.ContainersEndpoint;
    }

    public async Task Get()
    {
        await GetPositions();
        await GetContainers();
    }
    
    private async Task GetPositions()
    {
        _contractPosition = _contractPositionStorageService.Get();
    }

    private async Task GetContainers()
    {
        //_logger.LogInformation(_contractPositionStorageService.Get().First().waste_site.containers.Count().ToString());
        
        foreach (var con in _contractPosition)
        { 
            con.waste_site.containers = await Get(_aproConnect + con.waste_site.id, true);
        }
        _logger.LogInformation(_contractPositionStorageService.Get().First().waste_site.containers.Count().ToString());
        
        
    }
}