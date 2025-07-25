using integration.Context;
using integration.Structs;

namespace integration.Services.CheckUp.Services;

public class ClientCheckUpService : BaseCheckUpService, IClientCheckUpService
{
    public (bool, string) Check(IntegrationStruct str)
    {
        List<ClientDataResponse> clientDatas = str.contragentList;

        foreach (var client in clientDatas)
        {
            if (!Check(client, str.location.id))
                return new (false, $"{client} not found");
        }
        return (true, "");
    }

    private bool Check(ClientDataResponse client, int idLocation)
    {
        if (client == null) return false;
        if (string.IsNullOrEmpty(client.type_ka))
        {
            Message($"Location id {idLocation} - contract has not a type", EmailMessageBuilder.ListType.getlocation);
            return false;
        }

        if (string.IsNullOrEmpty(client.doc_type?.name))
        {
            Message($"Location id {idLocation} - contract has not a type", EmailMessageBuilder.ListType.getlocation);
            return false;
        }

        if (string.IsNullOrEmpty(client.address))
        {
            Message($"Location id {idLocation} - address not found", EmailMessageBuilder.ListType.getlocation);
            return false;
        }
        
        return true;
    }
}