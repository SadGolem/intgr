using integration.Context;

namespace integration.Services.Client.Storage;

public interface IClientStorageService
{
    List<ClientDataResponseResponse> GetClients();
    void SetClient(ClientDataResponseResponse client);
    void SetClients(List<ClientDataResponseResponse> clients);
    void ClearList(ClientDataResponseResponse client);
    void ClearList();
}