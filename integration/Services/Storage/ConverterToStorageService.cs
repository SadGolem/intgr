using integration.Context;
using integration.Services.Client.Storage;
using integration.Services.ContractPosition.Storage;
using integration.Services.Schedule;
using integration.Structs;

namespace integration.Services.Storage;

public interface IConverterToStorageService
{
    //IntegrationStruct Mapping(List<ContractPositionDataResponse> context);
    Task ToStorage();
}

public class ConverterToStorageService : IConverterToStorageService
{
    private List<int> contractPositionList;
    private IScheduleStorageService _scheduleStorage;
    private IContractStorageService _contractStorageService;
    private IClientStorageService _clientStorageService;
    private IStorageService _storageService;
    private IContractPositionStorageService _contractPositionStorageService;
    private List<ContractDataResponseResponse> contracts = new List<ContractDataResponseResponse>();
    private List<ScheduleDataResponse> schedules = new List<ScheduleDataResponse>();
    private List<ClientDataResponseResponse> clients = new List<ClientDataResponseResponse>();
    private List<ContractPositionDataResponse> contractPositions = new List<ContractPositionDataResponse>();
    
    public ConverterToStorageService(IScheduleStorageService scheduleStorage, IContractStorageService contractStorageService,
        IClientStorageService clientStorageService, IContractPositionStorageService contractPositionStorageService, IStorageService storageService)
    {
        _scheduleStorage = scheduleStorage;
        _contractStorageService = contractStorageService;
        _clientStorageService = clientStorageService;
        _contractPositionStorageService = contractPositionStorageService;
        _storageService = storageService;
    }

    public async Task GetAll()
    {
        contractPositions = _contractPositionStorageService.GetPosition();
        contracts = _contractStorageService.GetContracts();
        schedules = _scheduleStorage.GetScheduls();
        clients = _clientStorageService.GetClients();
    }
    
    public async Task ToStorage()
    {
        await GetAll();
        await Mapping();
    }

    private async Task Mapping()
    {
        foreach (var position in contractPositions)
        {
            _storageService.SetNewStruct(CreateStruct(position));
        }
    }

    private IntegrationStruct CreateStruct(ContractPositionDataResponse context)
    {
        int idLocation = context.waste_site.id;
        contracts = _contractStorageService.GetContracts();
        schedules = _scheduleStorage.GetScheduls();
        List<ContractDataResponseResponse> contractDatas = new List<ContractDataResponseResponse>();
        List<EmitterDataResponse> emitterDatas = new List<EmitterDataResponse>();
        List<ClientDataResponseResponse> clientDatas = new List<ClientDataResponseResponse>();
        List<ScheduleDataResponse> scheduleDatas = new List<ScheduleDataResponse>();
        LocationDataResponse locationDatasResponse = new LocationDataResponse();

        int idPos = context.id;
        contractDatas.Add(context.contract);
        emitterDatas.Add(context.waste_source);
        clientDatas.Add(context.contract.client);
        locationDatasResponse = context.waste_site;
        foreach (var schedule in scheduleDatas)
        {
            if (locationDatasResponse.id == schedule.location.id)
            {
                scheduleDatas.Add(schedule);
                break;
            }
        }

        foreach (var contract in contractDatas)
        {
            if (context.contract.root_id == contract.root_id)
            {
                contractDatas.Add(contract);
                break;
            }
        }

    return new IntegrationStruct(idLocation, emitterDatas, clientDatas, scheduleDatas, contractDatas, locationDatasResponse);
    }
}