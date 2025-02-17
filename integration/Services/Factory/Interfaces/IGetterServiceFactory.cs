using integration.Services.Interfaces;

namespace integration.Services.Factory.Interfaces
{
    public interface IGetterServiceFactory<T> where T : class
    {
        IGetterService<T> Create();
    }
}

