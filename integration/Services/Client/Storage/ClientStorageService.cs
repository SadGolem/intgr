using integration.Context;

namespace integration.Services.Client.Storage;

public class ClientStorageService : IClientStorageService
{
    private static List<ClientDataResponseResponse> ClientDatas = new List<ClientDataResponseResponse>();
    public List<ClientDataResponseResponse> GetClients()
    {
        return ClientDatas;
    }

    public void SetClient(ClientDataResponseResponse client)
    {
        ClientDatas.Add(client);
    }

    public void SetClients(List<ClientDataResponseResponse> clients)
    {
        ClientDatas = clients;
    }

    public void ClearList(ClientDataResponseResponse client)
    {
        ClientDatas.Remove(client);
    }

    public void ClearList()
    {
        ClientDatas.Clear();
    }
}