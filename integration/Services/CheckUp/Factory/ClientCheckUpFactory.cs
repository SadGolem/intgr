using integration.Context;
using integration.Services.CheckUp.Services;

namespace integration.Services.CheckUp.Factory;

public class ClientCheckUpFactory : ICheckUpFactory<ClientDataResponse> 
{

    public ICheckUpService<ClientDataResponse> Create()
    {
        return new ClientCheckUpService();
    }
}