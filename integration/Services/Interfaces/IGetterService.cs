using integration.Context;

namespace integration.Services.Interfaces
{
    public interface IGetterService<T> : IService
    {
        Task Get();
    }

  
}
