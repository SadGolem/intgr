using integration.Context;
using integration.Services.Interfaces;

namespace integration.Services.Factory.Interfaces
{
    public interface ILocationGetterServiceFactory
    {
        IGetterService<LocationData> Create();
    }
}
