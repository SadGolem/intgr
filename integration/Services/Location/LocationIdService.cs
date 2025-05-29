using integration.Context;

namespace integration.Services.Location;

public interface ILocationIdService
{
    List<int> GetLocationIds();
    void SetLocationIds(List<int> ids);
    void SetLocationIds(int ids);
    void SetLocation(List<(LocationDataResponse,bool)> loc);
    void ClearList(List<int> ids);
}

public class LocationIdService : ILocationIdService
{
    private static List<int> _locationIds = new List<int>();
    private static List<(LocationDataResponse,bool)> _locations = new List<(LocationDataResponse,bool)>();

    public List<int> GetLocationIds()
    {
        return _locationIds;
    }

    public void SetLocationIds(List<int> ids)
    {
        _locationIds = ids;
    }
    
    public void SetLocationIds(int id)
    {
        _locationIds.Add(id);
    }
    public void SetLocation(List<(LocationDataResponse,bool)> loc)
    {
        _locations = loc;
    }

    public void ClearList(List<int> ids)
    {
        _locationIds.Clear();
    }
}