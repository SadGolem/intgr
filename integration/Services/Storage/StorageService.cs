using integration.Context;
using integration.Structs;

namespace integration.Services.Storage;

public interface IStorageService
{
    List<IntegrationStruct> GetStructs();
    void SetNewStruct(IntegrationStruct data);
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

    public List<IntegrationStruct> GetStructs()
    {
        return integrationDataList;
    }

    public void SetNewStruct(IntegrationStruct data)
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

    private void Message(string m)
    {
        EmailMessageBuilder.PutInformation(EmailMessageBuilder.ListType.getall,m);
    }
}