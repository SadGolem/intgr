using integration.Context;
using integration.Structs;

namespace integration.Services.CheckUp.Services;

public class ScheduleCheckUpService: BaseCheckUpService,IScheduleCheckUpService
{
    public (bool, string) Check(IntegrationStruct str)
    {
        List<ScheduleDataResponse> schedules = str.schedulesList;
        if (schedules.Count == 0)
        {
            Message($"Location id {str.location.id} - Schedules not found", EmailMessageBuilder.ListType.getlocation);
            return (false, $"Location id {str.location.id} - Schedules not found");
        }
        foreach (var schedule in schedules)
        {
            if (!Check(schedule, str.location.id))
                return new (false, $"{schedule} not found");
        }
        return (true, "");
    }

    private bool Check(ScheduleDataResponse schedule, int idLocation)
    {
        if (schedule == null)
        {
            Message($"{idLocation} - schedule not found", EmailMessageBuilder.ListType.getlocation);
            return false;
        }

        if (schedule.containers == null)
        {
            Message($"{idLocation} - in schedule containers not" +
                    $" found", EmailMessageBuilder.ListType.getlocation);
            return false;
        }

        if (schedule.gr_w == "")
        {
            Message($"{idLocation} - schedule not found", EmailMessageBuilder.ListType.getlocation);
            return false;
        }
        
        if (schedule.emitter == null)
        {
            Message($"{idLocation} - in schedule emitter not found", EmailMessageBuilder.ListType.getlocation);
            return false;
        }
        return true;
    }
    
}