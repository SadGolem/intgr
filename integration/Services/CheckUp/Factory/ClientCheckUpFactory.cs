using integration.Context;
using integration.Services.CheckUp.Services;

namespace integration.Services.CheckUp.Factory;

public class ClientCheckUpFactory : ICheckUpFactory<ClientData> 
{

    public ICheckUpService<ClientData> Create()
    {
        return new ClientCheckUpService();
    }
}