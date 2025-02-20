using integration.Context;
using integration.HelpClasses;
using integration.Services.Interfaces;
using integration.Services.Location;

namespace integration.Services.ContractPosition;

public class ContractPositionGetterService : ServiceGetterBase<ContractData>, IGetterService<ContractData>
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _httpClient;
    private readonly ILogger<ContractPositionGetterService> _logger; 
    private readonly IConfiguration _configuration;
    private readonly ILocationIdService _locationIdService;
    private List<int> _locationIdSList = new List<int>();
    private string _aproConnect = "https://asu2.big3.ru/api/wf__contract_position_emitter__contract_position_takeout/?waste_site=1225908&query={id,number,status{color,id,name},contract{id,name,status{color,id,name}},waste_source{id,name,waste_source_category{name},address},waste_site{participant{id,name},address},estimation_value,value,value_manual,date_end,date_start}&ordering=-id&approximate_count=1&status_id=153";
    
    public ContractPositionGetterService(IHttpClientFactory httpClientFactory, 
        HttpClient httpClient, 
        ILogger<ContractPositionGetterService> logger, 
        IConfiguration configuration,
        ILocationIdService locationIdService) : base(httpClientFactory, httpClient, logger, configuration)
    {
        _httpClientFactory = httpClientFactory;
        _httpClient = httpClient;
        _logger = logger;
        _configuration = configuration;
        _locationIdService = locationIdService;
    }

    public async Task Get()
    {
        GetLocationsId();
        try
        {
            foreach (var loc in _locationIdSList)
            { 
                GetPosition(loc);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    async void GetPosition(int id)
    {
        await Get(_httpClientFactory, _aproConnect.Replace("1225908", id.ToString()));
    }

    private void GetLocationsId()
    {
         _locationIdSList = _locationIdService?.GetLocationIds() ?? _locationIdSList;
    }

    public override void Message(string ex)
    {
        throw new NotImplementedException();
    }

    public Task<List<(ContractData, bool IsNew)>> GetSync()
    {
        throw new NotImplementedException();
    }

    public Task<List<(ContractData, bool isNew)>> FetchData()
    {
        throw new NotImplementedException();
    }
}