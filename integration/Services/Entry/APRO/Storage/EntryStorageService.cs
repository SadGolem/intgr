using integration.Services.Storage.Interfaces;

namespace integration.Services.Entry.Storage;

public class EntryStorageService : IEntryStorageService<EntryDataResponse>
{
    public static List<(EntryDataResponse, bool)> EntryListBoolAPRO = new List<(EntryDataResponse, bool)>();
    public static List<EntryDataResponse> EntryListAPRO = new List<EntryDataResponse>();
    
    public List<(EntryDataResponse, bool)> Get()
    {
        return EntryListBoolAPRO;
    }

    List<EntryDataResponse> IStorageService<EntryDataResponse>.Get()
    {
        throw new NotImplementedException();
    }

    public void Set(EntryDataResponse data, bool isNew)
    {
        EntryListBoolAPRO.Add((data, isNew));
    }
    
    public void Set(EntryDataResponse data)
    {
        EntryListAPRO.Add(data);
    }
    public void Set(List<EntryDataResponse> datas)
    {
        foreach (var data in datas)
        {
            EntryListAPRO.Add(data);
        }
    }

    public void ClearList(EntryDataResponse date)
    {
        EntryListAPRO.Remove(date);
    }

    public void ClearList()
    {
        EntryListAPRO.Clear();
    }
}