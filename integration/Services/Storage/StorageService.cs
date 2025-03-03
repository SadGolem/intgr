using integration.Structs;

namespace integration.Services.Storage;

public interface IStorageService
{
    List<IntegrationStruct> GetStructIds();
    void SetNewStruct(IntegrationStruct data);
    void SetNewStruct(List<IntegrationStruct> data);
    void DeleteStruct(IntegrationStruct data);
    void Clear();
}

public class StorageService : IStorageService
{
    public static List<IntegrationStruct> integrationDataList;

    public StorageService()
    {
        integrationDataList = new List<IntegrationStruct>();
    }

    public List<IntegrationStruct> GetStructIds()
    {
        return integrationDataList;
    }
    
    public void SetNewStruct(IntegrationStruct data)
    {
        integrationDataList.Add(data);
    }

    public void SetNewStruct(List<IntegrationStruct> data)
    {
        integrationDataList = data;
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