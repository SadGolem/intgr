using integration.Context.MT;
using integration.Services.Entry.Storage;
using integration.Services.Storage.Interfaces;

namespace integration.Services.Entry.MT.Storage;

public class EntryMTStorageService : IEntryStorageService<EntryMTDataResponse>
{
    public static List<(EntryMTDataResponse, bool)> EntryListMT = new List<(EntryMTDataResponse, bool)>();
    

    public void Set(EntryMTDataResponse data, bool isNew)
    {
        throw new NotImplementedException();
    }

    public List<(EntryMTDataResponse, bool)> Get()
    {
        return EntryListMT;
    }

    List<EntryMTDataResponse> IStorageService<EntryMTDataResponse>.Get()
    {
        throw new NotImplementedException();
    }

    public void Set(EntryMTDataResponse data)
    {
        EntryListMT.Add((data, false));
    }
    public void Set(List<EntryMTDataResponse> datas)
    {
        foreach (var data in datas)
        {
            EntryListMT.Add((data, false));
        }
    }
    public void ClearList(EntryMTDataResponse date)
    {
        EntryListMT.Remove((date, false));
    }

    public void ClearList()
    {
        EntryListMT.Clear();
    }
}