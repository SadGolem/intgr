using integration.Context;
using integration.Services.Factory.Interfaces;
using integration.Services.Interfaces;

namespace integration.Services.Factory
{
    public class DataGetterServiceFactory : IGetterServiceFactory<Data>
    {
        public IGetterService<Data> Create()
        {
            throw new NotImplementedException();
        }
    }
}
