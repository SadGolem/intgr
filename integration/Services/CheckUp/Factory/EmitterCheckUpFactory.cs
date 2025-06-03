using integration.Context;
using integration.Services.CheckUp.Services;

namespace integration.Services.CheckUp.Factory;

public class EmitterCheckUpFactory : ICheckUpFactory<EmitterDataResponse> 
{
    public ICheckUpService<EmitterDataResponse> Create()
    {
        return new EmitterCheckUpService();
    }
}