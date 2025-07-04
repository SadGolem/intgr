using integration.Context;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.ContractPosition.Storage;
using integration.Services.Interfaces;
using integration.Services.Location;
using Microsoft.Extensions.Options;

namespace integration.Services.Client;

public class ContractGetterService

    : ServiceGetterBase<ContractDataResponse>,
        IGetterService<ContractDataResponse>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ContractGetterService> _logger;
    private IContractStorageService _contractStorageService;
    private IContractPositionStorageService _contractPositionStorageService;
    private List<int> _locationIdSList;

    private readonly string _aproConnect;

    private List<string> root_ids = new List<string>();

    public ContractGetterService(IHttpClientFactory httpClientFactory,
        ILogger<ContractGetterService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> apiSettings,
        IContractPositionStorageService contractPositionStorageService,
        IContractStorageService contractStorageService) : base(httpClientFactory, logger, authorizer, apiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _contractPositionStorageService = contractPositionStorageService;
        _contractStorageService = contractStorageService;
        _aproConnect = apiSettings.Value.APROconnect.BaseUrl +
                       apiSettings.Value.APROconnect.ApiClientSettings.ContractEndpoint;
    }

    public async Task Get()
    {
        await GetContractsToList();
    }

    private async Task GetContractsToList()
    {
        List<ContractPositionDataResponse> contractsPosList = _contractPositionStorageService.Get();

        foreach (var con in contractsPosList)
        {
            if (!root_ids.Contains(con.contract.root_id))
                root_ids.Add(con.contract.root_id);
        }

        await GetContractsDataFromAPRO();
    }

    private async Task GetContractsDataFromAPRO()
    {
        List<ContractDataResponse> contractsList = new List<ContractDataResponse>();
        foreach (var id in root_ids)
        {
            try
            {
                contractsList = await Get(_aproConnect + id, true);
                if (contractsList.Count() > 0) 
                    _contractStorageService.Set(contractsList.First());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}