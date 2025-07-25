using integration.Services.Interfaces;

namespace integration.Factory.SET.Interfaces
{
    public interface ISetterServiceFactory<T> where T : class
    {
        ISetterService<T> Create();
    }
}
