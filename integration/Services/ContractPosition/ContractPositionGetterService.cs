using integration.Context;
using integration.Services.Interfaces;
using integration.Services.Location;

namespace integration.Services.ContractPosition;

public class ContractPositionGetterService(
    IHttpClientFactory httpClientFactory,
    HttpClient httpClient,
    ILogger<ContractPositionGetterService> logger,
    IConfiguration configuration,
    ILocationIdService locationIdService)
    : ServiceGetterBase<ContractData>(httpClientFactory, httpClient, logger, configuration),
        IGetterService<ContractData>
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ILogger<ContractPositionGetterService> _logger = logger; 
    private readonly IConfiguration _configuration = configuration;
    private List<int> _locationIdSList = new List<int>(); 
    private readonly string _aproConnect ="https://asu2.big3.ru/api/wf__contract_position_emitter__contract" +
                                 "_position_takeout/?waste_site=2085591&query={id,number,status{color,id,name},contract{id,name,status{color,id,name}},waste_source{id,name,waste_source_category{name},address},waste_site{participant{id,name},address}," +
                                 "estimation_value,value,value_manual,date_end,date_start}&ordering=-id&approximate_count=1&status_id=153";

    public async Task Get()
    { 
        GetLocationsId();
        try
        {
            /*foreach (var loc in _locationIdSList)
            {*/
                await GetPosition(2085591);
            /*}*/
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    async Task GetPosition(int id)
    {
        //await Get(_httpClientFactory, _aproConnect.Replace("1225908", id.ToString()));
        await Get(_httpClientFactory, _aproConnect);
    }
    private void GetLocationsId()
    {
        _locationIdSList = locationIdService?.GetLocationIds() ?? _locationIdSList;
    }
    public override void Message(string ex)
    {
        throw new NotImplementedException();
    }
}