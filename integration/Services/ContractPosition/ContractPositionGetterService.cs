using integration.Context;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.ContractPosition.Storage;
using integration.Services.Interfaces;
using integration.Services.Location;
using Microsoft.Extensions.Options;

namespace integration.Services.ContractPosition;

public class ContractPositionGetterService(
    IHttpClientFactory httpClientFactory,
    ILogger<ContractPositionGetterService> logger,
    IAuthorizer authorizer,
    IOptions<AuthSettings> apiSettings,
    ILocationIdService locationIdService, 
    IContractPositionStorageService contractPositionStorageService )
    : ServiceGetterBase<ContractPositionDataResponse>(httpClientFactory, logger, authorizer, apiSettings),
        IGetterService<ContractPositionDataResponse>
{
    private readonly IHttpClientFactory _httpClientFactory = httpClientFactory;
   // private IConverterToStorageService _converterToStorageService = converterToStorageService;
    private IContractPositionStorageService _contractPositionStorageService = contractPositionStorageService;
    private List<int> _locationIdSList = new List<int>(); 
    private readonly string _aproConnect ="https://test.asu2.big3.ru/api/wf__contract_position_emitter__contract_position_takeout/?waste_site=1270125" +
                                          "&query={id,number,status{id,name},contract{id,name,status{id,name},root_id,participant{id,name,short_name," +
                                          " inn,kpp,ogrn,root_company ,waste_person, doc_type{name}}},waste_source{id,name,waste_source_category{name},address, " +
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
        List<ContractPositionDataResponse> postionsList = await Get(_httpClientFactory, _aproConnect.Replace("1270125", id.ToString())); //по позициям получаем всю инфу
            //_contractPositionStorageService.Set(postionsList); // тут мы отправляем данные в сторэйдж
        _contractPositionStorageService.Set(postionsList);
       
    }

    private void GetLocationsId()
    {
        _locationIdSList = locationIdService?.GetLocationIds() ?? _locationIdSList;
    }
    public void Message(string ex)
    {
        EmailMessageBuilder.PutInformation(EmailMessageBuilder.ListType.getall, ex);
    }
}