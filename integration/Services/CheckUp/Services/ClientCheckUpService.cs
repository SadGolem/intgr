using integration.Context;
using integration.Structs;

namespace integration.Services.CheckUp.Services;

public class ClientCheckUpService : ICheckUpService<ClientDataResponse>
{
    public (bool, string) Check(IntegrationStruct str)
    {
        List<ClientDataResponse> clientDatas = str.contragentList;

        foreach (var client in clientDatas)
        {
            if (!Check(client))
                return new (false, $"{client} not found");
        }
        return (true, "Clients found");
    }

    private bool Check(ClientDataResponse client)
    {
        if (client == null)
            
            return false;
            
        return true;
    }

}