using integration.Context.MT;

namespace integration.Services.Agre.Storage;

public class AgreStorageService: IAgreStorageService
{
    private static List<(AgreData, int)> _datas = new List<(AgreData, int)>();
    
    public List<(AgreData, int)> Get()
    {
        return _datas;
    }

    public void Set((AgreData, int) data)
    {
        _datas.Add(data);
    }

    public void Set(AgreData data, int id)
    {
        _datas.Add((data,id));
    }

    public void Set(List<(AgreData, int)> datas)
    {
        _datas = datas;
    }

    public void ClearList((AgreData, int) data)
    {
        throw new NotImplementedException();
    }
    
    public void ClearList()
    {
        _datas.Clear();
    }
}