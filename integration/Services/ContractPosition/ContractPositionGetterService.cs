using integration.Context;
using integration.Services.ContractPosition.Storage;
using integration.Services.Interfaces;
using integration.Services.Location;
using integration.Services.Storage;

namespace integration.Services.ContractPosition;

public class ContractPositionGetterService(
    IHttpClientFactory httpClientFactory,
    HttpClient httpClient,
    ILogger<ContractPositionGetterService> logger,
    IConfiguration configuration,
    ILocationIdService locationIdService, IContractPositionStorage contractPositionStorage,  IStorageService storageService )
    : ServiceGetterBase<ContractPositionData>(httpClientFactory, httpClient, logger, configuration),
        IGetterService<ContractPositionData>
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
    private readonly ILogger<ContractPositionGetterService> _logger = logger; 
    private readonly IConfiguration _configuration = configuration;
   // private IConverterToStorageService _converterToStorageService = converterToStorageService;
    private IContractPositionStorage _contractPositionStorage = contractPositionStorage;
    private IStorageService _storageService = storageService;
    private List<int> _locationIdSList; 
    private readonly string _aproConnect ="https://test.asu2.big3.ru/api/wf__contract_position_emitter__contract_position_takeout/?waste_site=1270125" +
                                          "&query={id,number,status{id,name},contract{id,name,status{id,name},root_id,participant{id,name,short_name," +
                                          " inn,kpp,ogrn,root_company ,waste_person}},waste_source{id,name,waste_source_category{name},address, " +
                                          "normative_unit_value_exist, participant{id,name},status{id,name}, author{name}},waste_site{id,participant{id}," +
                                          "address, author{name}, lat,lon,status{id},datetime_create, datetime_update}" +
                                          ",estimation_value,value,value_manual,date_end,date_start}&ordering=-id&approximate_count=1&status_id=153";

    public async Task Get()
    { 
        GetLocationsId();
        try
        {
            foreach (var loc in _locationIdSList)
            {
                await GetPosition(loc);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    async Task GetPosition(int id)
    {
        List<ContractPositionData> postionsList = await Get(_httpClientFactory, _aproConnect.Replace("2085591", id.ToString())); //по позициям получаем всю инфу
            //_contractPositionStorage.SetPositions(postionsList); // тут мы отправляем данные в сторэйдж
        _contractPositionStorage.SetPositions(postionsList);
        _storageService.SetNewStruct(postionsList);
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