using integration.Context.MT;

namespace integration.Services.Location.fromMT.Storage;

public class LocationMtStatusStorageStorageService : ILocationMTStatusStorageService
{
    private static List<LocationMTDataResponse> locationMtStatusDatas = new List<LocationMTDataResponse>();
    
    public List<LocationMTDataResponse> Get()
    {
        return locationMtStatusDatas;
    }

    public void Set(LocationMTDataResponse data)
    {
        locationMtStatusDatas.Add(data);
    }

    public void Set(List<LocationMTDataResponse> datas)
    {
        locationMtStatusDatas = datas;
    }

    public void ClearList(LocationMTDataResponse data)
    {
        locationMtStatusDatas.Remove(data);
    }

    public void ClearList()
    {
        locationMtStatusDatas.Clear();
    }
}