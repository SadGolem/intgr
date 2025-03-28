using integration.Context;

namespace integration.Services.ContractPosition.Storage;

public interface IContractPositionStorageService
{
    List<ContractPositionData> GetPosition();
    void SetPosition(ContractPositionData dates);
    void SetPositions(List<ContractPositionData> dates);
    void ClearList(ContractPositionData date);
    void ClearList();
}