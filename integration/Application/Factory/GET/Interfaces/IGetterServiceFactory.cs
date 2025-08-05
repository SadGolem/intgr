using integration.Services.Interfaces;

namespace integration.Factory.GET.Interfaces
{
    public interface IGetterServiceFactory<T> where T : class
    {
        IGetterService<T> Create();
    }
}

