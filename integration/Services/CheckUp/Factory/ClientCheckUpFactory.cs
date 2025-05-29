using integration.Context;
using integration.Services.CheckUp.Services;

namespace integration.Services.CheckUp.Factory;

public class ClientCheckUpFactory : ICheckUpFactory<ClientDataResponseResponse> 
{

    public ICheckUpService<ClientDataResponseResponse> Create()
    {
        return new ClientCheckUpService();
    }
}