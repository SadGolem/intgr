using integration.Context;
using integration.Factory.GET.Interfaces;
using integration.Factory.SET.Interfaces;
using integration.Services.Interfaces;

namespace integration.Factory.SET
{
    public class DataSetterServiceFactory : ISetterServiceFactory<DataResponse>
    {
        public ISetterService<DataResponse> Create()
        {
            throw new NotImplementedException();
        }

        ISetterService<DataResponse> ISetterServiceFactory<DataResponse>.Create()
        {
            throw new NotImplementedException();
        }
    }
}
