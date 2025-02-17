using integration.Context;
using integration.Factory.GET.Interfaces;
using integration.Factory.SET.Interfaces;
using integration.Services.Interfaces;

namespace integration.Factory.SET
{
    public class DataSetterServiceFactory : ISetterServiceFactory<Data>
    {
        public ISetterService<Data> Create()
        {
            throw new NotImplementedException();
        }

        ISetterService<Data> ISetterServiceFactory<Data>.Create()
        {
            throw new NotImplementedException();
        }
    }
}
