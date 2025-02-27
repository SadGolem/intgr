using integration.Context;

namespace integration.Structs;

public struct IntegrationStruct
{
    public int _idLocation;
    public List<EmitterData> emittersList;
    public List<ClientData> contragentList;
    public List<ScheduleData> schedulesList;
    public List<ContractData> contractList;
    public LocationData location;

    public IntegrationStruct(int idLocation, List<EmitterData> emittersList, List<ClientData> contragentList, 
        List<ScheduleData> schedulesList, List<ContractData> contractList, LocationData location)
    {
        _idLocation = idLocation;
        this.emittersList = emittersList;
        this.contragentList = contragentList;
        this.schedulesList = schedulesList;
        this.contractList = contractList;
        this.location = location;
    }
}