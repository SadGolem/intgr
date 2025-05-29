using integration.Context;

public interface IContractStorageService
{
    List<ContractDataResponseResponse> GetContracts();
    void SetContracts(ContractDataResponseResponse dates);
    void SetContracts(List<ContractDataResponseResponse> dates);
    void ClearList(ContractDataResponseResponse date);
    void ClearList();
}