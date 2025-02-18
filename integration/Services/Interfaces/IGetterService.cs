using integration.Context;

namespace integration.Services.Interfaces
{
    public interface IGetterService<T> : IService
    {
        Task<List<T>> GetSync();
        Task<List<T>> FetchData(); 
    }

  
}
