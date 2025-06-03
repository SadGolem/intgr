using integration.Context;
using integration.Services.CheckUp.Services;

namespace integration.Services.CheckUp.Factory;

public class LocationCheckUpFactory : ICheckUpFactory<LocationDataResponse> 
{
    public ICheckUpService<LocationDataResponse> Create()
    {
        return new LocationCheckUpService();
    }
}