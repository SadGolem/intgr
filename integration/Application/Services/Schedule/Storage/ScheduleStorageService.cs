using integration.Context;
using integration.Services.Location;

namespace integration.Services.Schedule;

public class ScheduleStorageService : IScheduleStorageService
{
    public static List<ScheduleDataResponse> _SchedulesList = new List<ScheduleDataResponse>();
    private readonly ILocationIdService _locationIdService;
    private List<int> ids = new List<int>();

    public ScheduleStorageService(ILocationIdService locationIdService)
    {
        _locationIdService = locationIdService;
    }

    public List<ScheduleDataResponse> Get()
    {
        return _SchedulesList;
    }
    public void Set(ScheduleDataResponse dates)
    {
        _SchedulesList.Add(dates);
    }

    public void Set(List<ScheduleDataResponse> datas)
    {
        ids = _locationIdService.GetLocationIds(); 
        foreach (var data in datas)
        {
            if (ids.Contains(data.location.id)) //если площадка должна измениться то график ее добавляется
                _SchedulesList.Add(data);
        }
    }
    public void ClearList(ScheduleDataResponse dataResponse)
    {
        _SchedulesList.Remove(dataResponse);
    }
    public void ClearList()
    {
        _SchedulesList.Clear();
    }
}