using integration.Services.Storage.Interfaces;

namespace integration.Services.Entry.Storage;

public interface IEntryStorageService<T> : IStorageService<T> where T : class
{
    public List<(T, bool)> Get();
    public void Set(T data, bool isNew);
}