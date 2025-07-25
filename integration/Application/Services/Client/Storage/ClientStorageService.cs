using integration.Context;

namespace integration.Services.Client.Storage;

public class ClientStorageService : IClientStorageService
{
    private static List<ClientDataResponse> ClientDatas = new List<ClientDataResponse>();
    public List<ClientDataResponse> Get()
    {
        return ClientDatas;
    }

    public void Set(ClientDataResponse client)
    {
        ClientDatas.Add(client);
    }

    public void Set(List<ClientDataResponse> clients)
    {
        ClientDatas = clients;
    }

    public void ClearList(ClientDataResponse client)
    {
        ClientDatas.Remove(client);
    }

    public void ClearList()
    {
        ClientDatas.Clear();
    }
}