using integration.Context.MT;

namespace integration.Services.Location.fromMT.Storage;

public class LocationMTStorageService : ILocationMTStorageService
{
    private static List<LocationMTDataResponse> locationMtDatas = new List<LocationMTDataResponse>();
    public List<LocationMTDataResponse> Get()
    {
        return locationMtDatas;
    }

    public void Set(LocationMTDataResponse data)
    {
        locationMtDatas.Add(data);
    }

    public void Set(List<LocationMTDataResponse> datas)
    {
        locationMtDatas = datas;
    }

    public void ClearList(LocationMTDataResponse data)
    {
        locationMtDatas.Remove(data);
    }

    public void ClearList()
    {
        locationMtDatas.Clear();
    }
}