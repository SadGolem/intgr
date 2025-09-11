using integration.Context;
using integration.Structs;
using Microsoft.IdentityModel.Tokens;

namespace integration.Services.CheckUp.Services;

public class ClientCheckUpService : BaseCheckUpService, IClientCheckUpService
{
    public (bool, string) Check(IntegrationStruct str)
    {
        
        List<ClientDataResponse> clientDatas = str.contragentList;
        if (clientDatas.IsNullOrEmpty())  return (false, "Контрагент не найден");
        foreach (var client in clientDatas)
        {
            if (!Check(client, str.location.id))
                return new (false, $"{client} не найден");
        }
        return (true, "");
    }

    private bool Check(ClientDataResponse client, int idLocation)
    {
        if (client == null) return false;
        if (string.IsNullOrEmpty(client.type_ka))
        {
            Message($"Площадка с номером {idLocation} - У договора не указан тип", EmailMessageBuilder.ListType.getlocation);
            return false;
        }

        if (string.IsNullOrEmpty(client.doc_type?.name))
        {
            Message($"Площадка с номером {idLocation} - У договора не указан тип", EmailMessageBuilder.ListType.getlocation);
            return false;
        }

        if (string.IsNullOrEmpty(client.address))
        {
            Message($"Площадка с номером {idLocation} - Не указан адрес площадки", EmailMessageBuilder.ListType.getlocation);
            return false;
        }
        
        return true;
    }
}