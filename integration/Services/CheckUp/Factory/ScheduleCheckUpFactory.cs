using integration.Context;
using integration.Services.CheckUp.Services;

namespace integration.Services.CheckUp.Factory;

public class ScheduleCheckUpFactory: ICheckUpFactory<ScheduleDataResponse> 
{
    public ICheckUpService<ScheduleDataResponse> Create()
    {
        return new ScheduleCheckUpService();
    }
}