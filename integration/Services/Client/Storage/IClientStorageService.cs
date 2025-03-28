using integration.Context;

namespace integration.Services.Client.Storage;

public interface IClientStorageService
{
    List<ClientData> GetClients();
    void SetClient(ClientData client);
    void SetClients(List<ClientData> clients);
    void ClearList(ClientData client);
    void ClearList();
}