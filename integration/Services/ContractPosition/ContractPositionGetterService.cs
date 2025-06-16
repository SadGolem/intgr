using integration.Context;
using integration.Helpers.Auth;
using integration.Helpers.Interfaces;
using integration.Services.ContractPosition.Storage;
using integration.Services.Interfaces;
using integration.Services.Location;
using Microsoft.Extensions.Options;

namespace integration.Services.ContractPosition;

public class ContractPositionGetterService : ServiceGetterBase<ContractPositionDataResponse>, IGetterService<ContractPositionDataResponse>
{
    private readonly IHttpClientFactory _httpClientFactory;
   // private IConverterToStorageService _converterToStorageService = converterToStorageService;
    private IContractPositionStorageService _contractPositionStorageService;
    private List<int> _locationIdSList = new List<int>();
    private ILocationIdService _locationIdService;
    private string _aproConnect;
    public ContractPositionGetterService(IHttpClientFactory httpClientFactory,
        ILogger<ContractPositionGetterService> logger,
        IAuthorizer authorizer,
        IOptions<AuthSettings> apiSettings,
        ILocationIdService locationIdService, 
        IContractPositionStorageService contractPositionStorageService) : base(httpClientFactory, logger, authorizer, apiSettings)
    {
        _httpClientFactory = httpClientFactory;
        _contractPositionStorageService = contractPositionStorageService;
        _aproConnect = apiSettings.Value.APROconnect.BaseUrl +
                       apiSettings.Value.APROconnect.ApiClientSettings.ContractPositionEndpoint;
        _locationIdService = locationIdService;
    }
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
        _locationIdSList = _locationIdService?.GetLocationIds() ?? _locationIdSList;
    }
    public void Message(string ex)
    {
        EmailMessageBuilder.PutInformation(EmailMessageBuilder.ListType.getall, ex);
    }
}