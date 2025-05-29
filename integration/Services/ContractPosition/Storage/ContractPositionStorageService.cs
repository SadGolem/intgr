using integration.Context;

namespace integration.Services.ContractPosition.Storage;

public class ContractPositionStorageService : IContractPositionStorageService
{
    public static List<ContractPositionDataResponse> ContractPositionList = new List<ContractPositionDataResponse>();
    public List<ContractPositionDataResponse> GetPosition()
    {
        return ContractPositionList;
    }

    public void SetPosition(ContractPositionDataResponse dates)
    {
        ContractPositionList.Add(dates);
    }

    public void SetPositions(List<ContractPositionDataResponse> dates)
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