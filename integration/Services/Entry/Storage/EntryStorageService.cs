namespace integration.Services.Entry.Storage;

public class EntryStorageService : IEntryStorageService
{
    public static List<EntryDataResponse> EntryList = new List<EntryDataResponse>();
    
    public List<EntryDataResponse> Get()
    {
        return EntryList;
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