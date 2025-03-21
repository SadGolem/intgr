using integration.Context;

public interface IContractStorageService
{
    List<ContractData> GetContracts();
    void SetContracts(ContractData dates);
    void SetContracts(List<ContractData> dates);
    void ClearList(ContractData date);
    void ClearList();
}