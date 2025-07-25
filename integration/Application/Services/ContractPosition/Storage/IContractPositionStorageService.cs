using integration.Services.Storage.Interfaces;

namespace integration.Services.ContractPosition.Storage;

public interface IContractPositionStorageService : IStorageService<ContractPositionDataResponse>
{
   // public ContractPositionDataResponse GetPosOnRoot_ID(string root_id);
    public ContractPositionDataResponse GetPosOn_ID(int id);
}