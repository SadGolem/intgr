using integration.Context;

namespace integration.Services.ContractPosition.Storage;

public class ContractPositionStorage : IContractPositionStorage
{
    public static List<ContractPositionData> ContractPositionList = new List<ContractPositionData>();
    public List<ContractPositionData> GetPosition()
    {
        return ContractPositionList;
    }

    public void SetPosition(ContractPositionData dates)
    {
        ContractPositionList.Add(dates);
    }

    public void SetPositions(List<ContractPositionData> dates)
    {
        foreach (var data in dates)
        {
            ContractPositionList.Add(data);
        }
    }

    public void ClearList(ContractPositionData date)
    {
        ContractPositionList.Remove(date);
    }

    public void ClearList()
    {
        ContractPositionList.Clear();
    }
}