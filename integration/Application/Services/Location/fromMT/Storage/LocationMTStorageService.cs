using integration.Context.MT;

namespace integration.Services.Location.fromMT.Storage;

public class LocationMTStorageService : ILocationMTStorageService
{
    private static List<LocationMTPhotoDataResponse> locationMtDatas = new List<LocationMTPhotoDataResponse>();
    public List<LocationMTPhotoDataResponse> Get()
    {
        return locationMtDatas;
    }

    public void Set(LocationMTPhotoDataResponse photoData)
    {
        locationMtDatas.Add(photoData);
    }

    public void Set(List<LocationMTPhotoDataResponse> datas)
    {
        locationMtDatas = datas;
    }

    public void ClearList(LocationMTPhotoDataResponse photoData)
    {
        locationMtDatas.Remove(photoData);
    }

    public void ClearList()
    {
        locationMtDatas.Clear();
    }
}