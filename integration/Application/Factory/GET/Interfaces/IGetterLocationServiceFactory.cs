using integration.Services.Interfaces;

namespace integration.Factory.GET.Interfaces
{
    public interface IGetterLocationServiceFactory<T> where T : class
    {
        IGetterLocationService<T> Create();
    }
}

