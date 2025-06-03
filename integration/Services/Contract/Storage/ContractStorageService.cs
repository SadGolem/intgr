using integration.Context;

namespace integration.Services.Client.Storage;

public class ContractStorageService : IContractStorageService
{
    public static List<ContractDataResponse> _contractDatas = new List<ContractDataResponse>();
    public List<ContractDataResponse> Get()
    {
        return _contractDatas;
    }

    public void Set(ContractDataResponse dates)
    {
        _contractDatas.Add(dates);
    }

    public void Set(List<ContractDataResponse> dates)
    {
        _contractDatas = dates;
    }

    public void ClearList(ContractDataResponse date)
    {
        _contractDatas.Remove(date);
    }

    public void ClearList()
    {
        _contractDatas.Clear();
    }
}