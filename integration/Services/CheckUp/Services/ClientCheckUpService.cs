using integration.Context;
using integration.Structs;

namespace integration.Services.CheckUp.Services;

public class ClientCheckUpService : ICheckUpService<ClientData>
{
    public bool Check(IntegrationStruct str)
    {
        List<ClientData> clientDatas = str.contragentList;

        foreach (var client in clientDatas)
        {
            if (!Check(client))
                return false;
        }
        return true;
    }

    private bool Check(ClientData client)
    {
        if (client != null)
            return false;
            
        return true;
    }

}