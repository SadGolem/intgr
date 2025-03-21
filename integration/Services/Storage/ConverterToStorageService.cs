using integration.Context;
using integration.Services.Client.Storage;
using integration.Services.Schedule;
using integration.Structs;

namespace integration.Services.Storage;

public interface IConverterToStorageService
{
    IntegrationStruct Mapping(List<ContractPositionData> context);
}

public class ConverterToStorageService : IConverterToStorageService
{
    private List<int> contractPositionList;
    private IScheduleStorageService _scheduleStorage;
    private IContractStorageService _contractStorageService;
    private List<ContractData> contracts = new List<ContractData>();
    private List<ScheduleData> schedules = new List<ScheduleData>();
    
    public ConverterToStorageService(IScheduleStorageService scheduleStorage, IContractStorageService contractStorageService)
    {
        _scheduleStorage = scheduleStorage;
        _contractStorageService = contractStorageService;
    }

    public IntegrationStruct Mapping(List<ContractPositionData> context)
    {
        return CreateStruct(context);
    }
    
    private IntegrationStruct CreateStruct(List<ContractPositionData> context)
    {
        int idLocation = context.First().waste_site.id;
        contracts = _contractStorageService.GetContracts();
        schedules = _scheduleStorage.GetScheduls();
        List<ContractData> contractDatas = new List<ContractData>();
        List<EmitterData> emitterDatas = new List<EmitterData>();
        List<ClientData> clientDatas = new List<ClientData>();
        List<ScheduleData> scheduleDatas = new List<ScheduleData>();
        LocationData locationDatas = new LocationData();
        
        foreach (var pos in context)
        {
            int idPos = pos.id;
            //contractDatas.Add(pos.contract);
            emitterDatas.Add(pos.waste_source);
            clientDatas.Add(pos.contract.client);
            locationDatas = pos.waste_site;
            foreach (var schedule in scheduleDatas)
            {
                if (locationDatas.id == schedule.location.id)
                {
                    scheduleDatas.Add(schedule);
                    break;
                }
            }
            foreach (var contract in contractDatas)
            {
                if (pos.contract.root_id == contract.root_id)
                {
                    contractDatas.Add(contract);
                    break;
                }
            }
        }

        return new IntegrationStruct(idLocation, emitterDatas, clientDatas, scheduleDatas, contractDatas, locationDatas);
    }
}