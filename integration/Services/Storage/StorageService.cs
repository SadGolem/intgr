using integration.Services.Storage.Interfaces;
using integration.Structs;

namespace integration.Services.Storage;

public class StorageService : IIntegrationStructStorageService
{
    public static List<IntegrationStruct> integrationDataList;

    public StorageService()
    {
        integrationDataList = new List<IntegrationStruct>();
    }

    public List<IntegrationStruct> Get()
    {
        return integrationDataList;
    }

    public void Set(IntegrationStruct data)
    {
        integrationDataList.Add(data);
    }

    public void Set(List<IntegrationStruct> datas)
    {
        throw new NotImplementedException();
    }

    public void ClearList()
    {
        integrationDataList.Clear();
    }

    public void ClearList(IntegrationStruct data)
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