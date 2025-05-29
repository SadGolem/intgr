using integration.Context;

namespace integration.Services.ContractPosition.Storage;

public interface IContractPositionStorageService
{
    List<ContractPositionDataResponse> GetPosition();
    void SetPosition(ContractPositionDataResponse dates);
    void SetPositions(List<ContractPositionDataResponse> dates);
    void ClearList(ContractPositionDataResponse date);
    void ClearList();
}