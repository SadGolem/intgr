using integration.Services.Storage.Interfaces;

namespace integration.Services.Entry.Storage;

public interface IEntryStorageService : IStorageService<EntryDataResponse>
{
    public List<(EntryDataResponse, bool)> Get();
    public void Set(EntryDataResponse data, bool isNew);
}