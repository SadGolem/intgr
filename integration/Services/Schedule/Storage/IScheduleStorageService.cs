using integration.Context;

namespace integration.Services.Schedule;

public interface IScheduleStorageService
{
    List<ScheduleDataResponse> GetScheduls();
    void SetSchedules(ScheduleDataResponse dates);
    void SetSchedules(List<ScheduleDataResponse> dates);
    void ClearList(ScheduleDataResponse date);
    void ClearList();
}
