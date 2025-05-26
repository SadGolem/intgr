using integration.Context;
using integration.HelpClasses;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.Client.Storage;
using integration.Services.ContractPosition.Storage;
using integration.Services.Interfaces;
using integration.Services.Location;
using Microsoft.Extensions.Options;

namespace integration.Services.Client;

public class ContractGetterService

    : ServiceGetterBase<ContractData>,
        IGetterService<ContractData>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<ContractGetterService> _logger;
    private IContractStorageService _contractStorageService;
    // private IConverterToStorageService _converterToStorageService = converterToStorageService;
    private IContractPositionStorageService _contractPositionStorageService;
    private List<int> _locationIdSList;

    private readonly string _aproConnect =
        "https://test.asu2.big3.ru/api/wf__contract__contract_takeout/?query={id,name,status{id,name},contract_type{name}," +
        " root_id,participant{id,name,short_name, inn,kpp, ogrn, root_company ,waste_person,doc_type{name}}, " +
        "v_order}&v_order=0&root_id=";

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
    }
    public async Task Get()
    {
        //сначала получить uuid 
        await GetContractsToList();
        //затем вывести все договоры 
        //передать список в сторэйдж
    }

    private async Task GetContractsToList()
    {
        List<ContractPositionData> contractsPosList = _contractPositionStorageService.GetPosition();

        foreach (var con in contractsPosList)
        {
            root_ids.Add(con.contract.root_id);
        }

        await GetContractsDataFromAPRO();
    }

    private async Task GetContractsDataFromAPRO()
    {
        List<ContractData> contractsList = new List<ContractData>();
        foreach (var id in root_ids)
        {
            try
            {
                contractsList = await Get(_httpClientFactory, _aproConnect + id);
                if (contractsList.Count() > 0)
                    _contractStorageService.SetContracts(contractsList.First());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

 
}