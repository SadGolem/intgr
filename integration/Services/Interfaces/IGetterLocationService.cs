using integration.Context;

namespace integration.Services.Interfaces
{
    public interface IGetterLocationService<T> : IGetterService<T>
    {
        Task<List<(T, bool IsNew)>> GetSync();

    }

  
}
