using integration.Context;
using integration.Structs;
using Newtonsoft.Json;

namespace integration.Services.Storage;

public interface IConverterToStorageService
{
    IntegrationStruct Mapping(List<ContractPositionData> context);
}

public class ConverterToStorageService : IConverterToStorageService
{
    private List<int> contractPositionList;
    public IntegrationStruct Mapping(List<ContractPositionData> context)
    {
        MappingContractPosition(context);
        return new IntegrationStruct();
    }
    
    private void MappingContractPosition(List<ContractPositionData> context)
    {
        int idLocation = context.First().waste_site.id;
        List<ContractData> contractDatas = new List<ContractData>();
        List<EmitterData> emitterDatas = new List<EmitterData>();
        List<ClientData> clientDatas = new List<ClientData>();
        List<ScheduleData> scheduleDatas = new List<ScheduleData>();
        LocationData locationDatas = new LocationData();
        
        foreach (var pos in context)
        {
            int idPos = pos.id;
            contractDatas.Add(pos.contract);
            emitterDatas.Add(pos.waste_source);
            clientDatas.Add(pos.contract.client);
            locationDatas = pos.waste_site;
            //scheduleDatas.Add(pos.);
        }

        new IntegrationStruct(idLocation, emitterDatas, clientDatas, scheduleDatas, contractDatas, locationDatas);
    }
    
    private IntegrationStruct CreateStruct()
    {
        var data = new IntegrationStruct();

        return data;
    }
}