using integration.Context;
using integration.Structs;

namespace integration.Services.CheckUp.Services;

public class ScheduleCheckUpService: IScheduleCheckUpService
{
    public (bool, string) Check(IntegrationStruct str)
    {
        List<ScheduleDataResponse> schedules = str.schedulesList;

        foreach (var schedule in schedules)
        {
            if (!Check(schedule, str.location.id))
                return new (false, $"{schedule} not founded");
        }
        return (true, "Schedules founded");
    }

    private bool Check(ScheduleDataResponse schedule, int idLocation)
    {
        if (schedule == null)
        {
            Message($"{idLocation} - schedule is not found");
            return false;
        }

        if (schedule.containers == null)
        {
            Message($"{idLocation} - in schedule containers is not found");
            return false;
        }

        if (schedule.gr_w == "")
        {
            Message($"{idLocation} - schedule is not found");
            return false;
        }
        
        if (schedule.emitter == null)
        {
            Message($"{idLocation} - in schedule emitter is not found");
            return false;
        }
        return true;
    }
    
    private void Message(string message)
    {
        EmailMessageBuilder.PutInformation(
            EmailMessageBuilder.ListType.getschedule, 
            message
        );
    }
}