using integration.Context;

namespace integration.Services.Client.Storage;

public class ContractStorageService : IContractStorageService
{
    public static List<ContractDataResponseResponse> _contractDatas = new List<ContractDataResponseResponse>();
    public List<ContractDataResponseResponse> GetContracts()
    {
        return _contractDatas;
    }

    public void SetContracts(ContractDataResponseResponse dates)
    {
        _contractDatas.Add(dates);
    }

    public void SetContracts(List<ContractDataResponseResponse> dates)
    {
        _contractDatas = dates;
    }

    public void ClearList(ContractDataResponseResponse date)
    {
        _contractDatas.Remove(date);
    }

    public void ClearList()
    {
        _contractDatas.Clear();
    }
}