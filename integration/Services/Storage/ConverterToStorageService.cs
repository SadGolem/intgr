using integration.Context;
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

    public ConverterToStorageService(IScheduleStorageService scheduleStorage)
    {
        _scheduleStorage = scheduleStorage;
    }

    public IntegrationStruct Mapping(List<ContractPositionData> context)
    {
        return CreateStruct(context);
        
    }
    
    private IntegrationStruct CreateStruct(List<ContractPositionData> context)
    {
        int idLocation = context.First().waste_site.id;
        List<ContractData> contractDatas = new List<ContractData>();
        List<EmitterData> emitterDatas = new List<EmitterData>();
        List<ClientData> clientDatas = new List<ClientData>();
        List<ScheduleData> scheduleDatas = _scheduleStorage.GetScheduls();
        LocationData locationDatas = new LocationData();
        
        foreach (var pos in context)
        {
            int idPos = pos.id;
            contractDatas.Add(pos.contract);
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
        }

        return new IntegrationStruct(idLocation, emitterDatas, clientDatas, scheduleDatas, contractDatas, locationDatas);
    }
}