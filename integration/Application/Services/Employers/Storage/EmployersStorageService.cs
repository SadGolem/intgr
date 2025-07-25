using integration.Context.Response;

namespace integration.Services.Employers.Storage;

public class EmployersStorageService : IEmployersStorageService
{
    private static List<EmployerDataResponse> _employerData;
    public List<EmployerDataResponse> Get()
    {
        return _employerData;
    }

    public void Set(EmployerDataResponse data)
    {
        _employerData.Add(data);
    }

    public void Set(List<EmployerDataResponse> datas)
    {
        _employerData = datas;
    }

    public void ClearList(EmployerDataResponse data)
    {
        _employerData.Remove(data);
    }

    public void ClearList()
    {
        _employerData.Clear();
    }
}