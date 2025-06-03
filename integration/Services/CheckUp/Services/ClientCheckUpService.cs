using integration.Context;
using integration.Structs;

namespace integration.Services.CheckUp.Services;

public class ClientCheckUpService : IClientCheckUpService
{
    public (bool, string) Check(IntegrationStruct str)
    {
        List<ClientDataResponse> clientDatas = str.contragentList;

        foreach (var client in clientDatas)
        {
            
            if (!Check(client, str.location.id))
                return new (false, $"{client} not found");
        }
        return (true, "Clients found");
    }

    private bool Check(ClientDataResponse client, int idLocation)
    {
        if (client == null)
        {
            Message($"{idLocation} - client is not found");
            return false;
        }

        return true;
    }
    private void Message(string message)
    {
        EmailMessageBuilder.PutInformation(
            EmailMessageBuilder.ListType.getemitter, 
            message
        );
    }
}