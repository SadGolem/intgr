using integration.Context;
using integration.Factory.GET.Interfaces;
using integration.Services.Interfaces;

namespace integration.Factory.GET
{
    public class DataGetterServiceFactory : IGetterServiceFactory<DataResponse>
    {
        public IGetterService<DataResponse> Create()
        {
            throw new NotImplementedException();
        }
    }
}
