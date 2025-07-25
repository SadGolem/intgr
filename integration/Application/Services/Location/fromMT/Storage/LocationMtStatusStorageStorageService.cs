using integration.Context.MT;

namespace integration.Services.Location.fromMT.Storage;

public class LocationMtStatusStorageStorageService : ILocationMTStatusStorageService
{
    private static List<LocationData> locationMtStatusDatas = new List<LocationData>();
    
    public List<LocationData> Get()
    {
        return locationMtStatusDatas;
    }

    public void Set(LocationData data)
    {
        locationMtStatusDatas.Add(data);
    }

    public void Set(List<LocationData> datas)
    {
        locationMtStatusDatas = datas;
    }

    public void ClearList(LocationData data)
    {
        locationMtStatusDatas.Remove(data);
    }

    public void ClearList()
    {
        locationMtStatusDatas.Clear();
    }
}