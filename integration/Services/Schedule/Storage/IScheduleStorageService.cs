using integration.Context;

namespace integration.Services.Schedule;

public interface IScheduleStorageService
{
    List<ScheduleData> GetScheduls();
    void SetSchedules(ScheduleData dates);
    void SetSchedules(List<ScheduleData> dates);
    void ClearList(ScheduleData date);
    void ClearList();
}
