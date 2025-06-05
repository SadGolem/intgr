using integration.Services.Storage.Interfaces;

namespace integration.Services.Entry.Storage;

public class EntryStorageService : IEntryStorageService
{
    public static List<(EntryDataResponse, bool)> EntryListBool = new List<(EntryDataResponse, bool)>();
    public static List<EntryDataResponse> EntryList = new List<EntryDataResponse>();
    
    public List<(EntryDataResponse, bool)> Get()
    {
        return EntryListBool;
    }

    List<EntryDataResponse> IStorageService<EntryDataResponse>.Get()
    {
        throw new NotImplementedException();
    }

    public void Set(EntryDataResponse data, bool isNew)
    {
        EntryListBool.Add((data, isNew));
    }
    
    public void Set(EntryDataResponse data)
    {
        EntryList.Add(data);
    }
    public void Set(List<EntryDataResponse> datas)
    {
        foreach (var data in datas)
        {
            EntryList.Add(data);
        }
    }

    public void ClearList(EntryDataResponse date)
    {
        EntryList.Remove(date);
    }

    public void ClearList()
    {
        EntryList.Clear();
    }
}