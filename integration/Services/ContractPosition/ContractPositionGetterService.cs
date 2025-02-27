using integration.Context;
using integration.Services.Interfaces;
using integration.Services.Location;
using integration.Services.Storage;
using integration.Structs;
using Newtonsoft.Json;

namespace integration.Services.ContractPosition;

public class ContractPositionGetterService(
    IHttpClientFactory httpClientFactory,
    HttpClient httpClient,
    ILogger<ContractPositionGetterService> logger,
    IConfiguration configuration,
    ILocationIdService locationIdService, IConverterToStorageService converterToStorageService)
    : ServiceGetterBase<ContractPositionData>(httpClientFactory, httpClient, logger, configuration),
        IGetterService<ContractPositionData>
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ILogger<ContractPositionGetterService> _logger = logger; 
    private readonly IConfiguration _configuration = configuration;
    private IConverterToStorageService _converterToStorageService = converterToStorageService;
    private List<int> _locationIdSList; 
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
        List<ContractPositionData> postionsList = await Get(_httpClientFactory, _aproConnect); //по позициям получаем всю инфу
        _converterToStorageService.Mapping(postionsList);

        /*
        //тут надо преобразовать данные ПРЕОБРАЗОВАНИЕ
        _storageService.SetNewStruct(CreateStruct());*/
        Message("");
    }

    private void GetLocationsId()
    {
        _locationIdSList = locationIdService?.GetLocationIds() ?? _locationIdSList;
    }
    public override void Message(string ex)
    {
        EmailMessageBuilder.PutInformation(EmailMessageBuilder.ListType.getall, ex);
    }
}