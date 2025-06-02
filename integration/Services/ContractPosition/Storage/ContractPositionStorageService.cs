using integration.Context;

namespace integration.Services.ContractPosition.Storage;

public class ContractPositionStorageService : IContractPositionStorageService
{
    public static List<ContractPositionDataResponse> ContractPositionList = new List<ContractPositionDataResponse>();
    public List<ContractPositionDataResponse> Get()
    {
        return ContractPositionList;
    }

    public void Set(ContractPositionDataResponse dates)
    {
        ContractPositionList.Add(dates);
    }
    public ContractPositionDataResponse GetPosOnRoot_ID(string root_id)
    {
        foreach (var pos in ContractPositionList)
        {
            if (pos.contract.root_id == root_id)
                return pos;
        }

        return null;
    }

    public void Set(List<ContractPositionDataResponse> dates)
    {
        foreach (var data in dates)
        {
            ContractPositionList.Add(data);
        }
    }

    public void ClearList(ContractPositionDataResponse date)
    {
        ContractPositionList.Remove(date);
    }

    public void ClearList()
    {
        ContractPositionList.Clear();
    }
}