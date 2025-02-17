using integration.Context;
using integration.Factory.GET.Interfaces;
using integration.Services.Interfaces;

namespace integration.Factory.GET
{
    public class DataGetterServiceFactory : IGetterServiceFactory<Data>
    {
        public IGetterService<Data> Create()
        {
            throw new NotImplementedException();
        }
    }
}
