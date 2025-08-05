using integration.Context;

namespace integration.Structs;

public struct IntegrationStruct
{
    public int _idPosition;
    public List<EmitterDataResponse> emittersList;
    public List<ClientDataResponse> contragentList;
    public List<ScheduleDataResponse> schedulesList;
    public List<ContractDataResponse> contractList;
    public LocationDataResponse location;

    public IntegrationStruct(int idPosition, List<EmitterDataResponse> emittersList, List<ClientDataResponse> contragentList, 
        List<ScheduleDataResponse> schedulesList, List<ContractDataResponse> contractList, LocationDataResponse location)
    {
        _idPosition = idPosition;
        this.emittersList = emittersList;
        this.contragentList = contragentList;
        this.schedulesList = schedulesList;
        this.contractList = contractList;
        this.location = location;
    }
}