using integration.Context;

namespace integration.Services.Location;

public interface ILocationIdService
{
    List<int> GetLocationIds();
    int? GetAuthor(int author);
    void SetLocationIds(int ids, int? idAuthor);
    void SetLocation(List<(LocationDataResponse,bool)> loc);
}

public class LocationIdService : ILocationIdService
{
    private static List<int> _locationIds = new List<int>();
    private static Dictionary<int, int?> _locationAuthorsUpdate = new Dictionary<int, int?>();
    private static List<(LocationDataResponse,bool)> _locations = new List<(LocationDataResponse,bool)>();

    public List<int> GetLocationIds()
    {
        return _locationIds;
    }
    public int? GetAuthor(int idLoc)
    {
        foreach (var loc in _locationAuthorsUpdate)
        {
            if (loc.Key == idLoc)
            {
                return loc.Value;
            }
        }

        throw new Exception("Нет такой площадки");
    }
    public void SetLocationIds(List<int> ids)
    {
        _locationIds = ids;
   }
    
    public void SetLocationIds(int id, int? author)
    {
        _locationIds.Add(id);
        _locationAuthorsUpdate.Add(id, author);
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