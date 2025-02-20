using integration.Context;

namespace integration.Services.Interfaces
{
    public interface IGetterService<T> : IService
    {
        Task<List<(T, bool IsNew)>> GetSync();
        Task Get();
        Task<List<(T,bool isNew)>> FetchData(); 
    }

  
}
