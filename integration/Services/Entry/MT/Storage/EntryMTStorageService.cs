using integration.Context.MT;
using integration.Services.Entry.Storage;
using integration.Services.Storage.Interfaces;

namespace integration.Services.Entry.MT.Storage;

public class EntryMTStorageService : IEntryStorageService<EntryMTDataResponse>
{
    public static List<EntryMTDataResponse> EntryListMT = new List<EntryMTDataResponse>();

    List<EntryMTDataResponse> IStorageService<EntryMTDataResponse>.Get()
    {
        return EntryListMT;
    }

    public void Set(EntryMTDataResponse data, bool isNew)
    {
        throw new NotImplementedException();
    }

    public List<(EntryMTDataResponse, bool)> Get()
    {
        throw new NotImplementedException();
    }

    public void Set(EntryMTDataResponse data)
    {
        EntryListMT.Add(data);
    }
    public void Set(List<EntryMTDataResponse> datas)
    {
        foreach (var data in datas)
        {
            EntryListMT.Add(data);
        }
    }
    public void ClearList(EntryMTDataResponse date)
    {
        EntryListMT.Remove(date);
    }

    public void ClearList()
    {
        EntryListMT.Clear();
    }
}