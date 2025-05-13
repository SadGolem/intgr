using integration.Context;
using integration.Structs;

namespace integration.Services.CheckUp.Services;

public class ClientCheckUpService : ICheckUpService<ClientData>
{
    public (bool, string) Check(IntegrationStruct str)
    {
        List<ClientData> clientDatas = str.contragentList;

        foreach (var client in clientDatas)
        {
            if (!Check(client))
                return new (false, $"{client} not found");
        }
        return (true, "Clients found");
    }

    private bool Check(ClientData client)
    {
        if (client == null)
            
            return false;
            
        return true;
    }

}