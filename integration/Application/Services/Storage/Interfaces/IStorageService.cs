namespace integration.Services.Storage.Interfaces;

public interface IStorageService<T>
{
    List<T> Get();
    void Set(T data);
    void Set(List<T> datas);
    void ClearList(T data);
    void ClearList();
}