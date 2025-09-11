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
            Message($"У площадки с номером {str.location.id} - Не найден график вывоза", EmailMessageBuilder.ListType.getlocation);
            return (false, $"У площадки с номером {str.location.id} - Не найден график вывоза");
        }
        foreach (var schedule in schedules)
        {
            if (!Check(schedule, str.location.id))
                return new (false, $"График {schedule} не найден");
        }
        return (true, "");
    }

    private bool Check(ScheduleDataResponse schedule, int idLocation)
    {
        if (schedule == null)
        {
            Message($"У площадки с номером {idLocation} - Не найден график вывоза", EmailMessageBuilder.ListType.getlocation);
            return false;
        }

        if (schedule.containers == null)
        {
            Message($"У площадки с номером {idLocation} - в графике нет контейнера", EmailMessageBuilder.ListType.getlocation);
            return false;
        }

        if (schedule.gr_w == "")
        {
            Message($"У площадки с номером {idLocation} - Не найден график вывоза", EmailMessageBuilder.ListType.getlocation);
            return false;
        }
        
        if (schedule.emitter == null)
        {
            Message($"У площадки с номером {idLocation} - в графике нет эмиттера", EmailMessageBuilder.ListType.getlocation);
            return false;
        }
        return true;
    }
    
}