using integration.Context;

namespace integration.Services.Client.Storage;

public class ContractStorageService : IContractStorageService
{
    public static List<ContractData> _contractDatas = new List<ContractData>();
    public List<ContractData> GetContracts()
    {
        return _contractDatas;
    }

    public void SetContracts(ContractData dates)
    {
        _contractDatas.Add(dates);
    }

    public void SetContracts(List<ContractData> dates)
    {
        _contractDatas = dates;
    }

    public void ClearList(ContractData date)
    {
        _contractDatas.Remove(date);
    }

    public void ClearList()
    {
        _contractDatas.Clear();
    }
}