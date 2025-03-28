using integration.Context;
using integration.Structs;

namespace integration.Services.Storage;

public interface IStorageService
{
    List<IntegrationStruct> GetStructIds();
    //void SetNewStruct(List<ContractPositionData> data);
    void DeleteStruct(IntegrationStruct data);
    void Clear();
}

public class StorageService : IStorageService
{
    public static List<IntegrationStruct> integrationDataList;
    private IConverterToStorageService _converterToStorageService;

    public StorageService(IConverterToStorageService converterToStorageService)
    {
        integrationDataList = new List<IntegrationStruct>();
        _converterToStorageService = converterToStorageService;
    }

    /*  public void SetNewStruct(List<ContractPositionData> contractPositionDatas)
    {
        IntegrationStruct structs = _converterToStorageService.Mapping(contractPositionDatas);
        NewStruct(structs);
    }*/

    public List<IntegrationStruct> GetStructIds()
    {
        return integrationDataList;
    }
    
    private void NewStruct(IntegrationStruct data)
    {
        integrationDataList.Add(data);
    }
    public void DeleteStruct(IntegrationStruct data)
    {
        integrationDataList.Remove(data);
    }
    public void Clear()
    {
        integrationDataList.Clear();
    }
}