using integration.Context;
using integration.Services.Location;

namespace integration.Services.Schedule;

public class ScheduleStorageService : IScheduleStorageService
{
    public static List<ScheduleData> _SchedulesList;
    private readonly ILocationIdService _locationIdService;
    private List<int> ids = new List<int>();

    public ScheduleStorageService(ILocationIdService locationIdService)
    {
        _locationIdService = locationIdService;
    }

    public List<ScheduleData> GetScheduls()
    {
        return _SchedulesList;
    }

    public void SetSchedules(ScheduleData dates)
    {
        throw new NotImplementedException();
    }

    public void SetSchedules(List<ScheduleData> datas)
    {
        ids = _locationIdService.GetLocationIds(); 
        foreach (var data in datas)
        {
            if (ids.Contains(data.location.id)) //если площадка должна измениться то график ее добавляется
                _SchedulesList.Add(data);
        }
    }

    public void ClearList(ScheduleData data)
    {
        _SchedulesList.Remove(data);
    }
    
    public void ClearList()
    {
        _SchedulesList.Clear();
    }
}