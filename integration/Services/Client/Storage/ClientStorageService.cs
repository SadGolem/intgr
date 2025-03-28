using integration.Context;

namespace integration.Services.Client.Storage;

public class ClientStorageService : IClientStorageService
{
    private static List<ClientData> ClientDatas = new List<ClientData>();
    public List<ClientData> GetClients()
    {
        return ClientDatas;
    }

    public void SetClient(ClientData client)
    {
        ClientDatas.Add(client);
    }

    public void SetClients(List<ClientData> clients)
    {
        ClientDatas = clients;
    }

    public void ClearList(ClientData client)
    {
        ClientDatas.Remove(client);
    }

    public void ClearList()
    {
        ClientDatas.Clear();
    }
}