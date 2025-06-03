using integration.Context;
using integration.Structs;

namespace integration.Services.CheckUp.Services;

public class EmitterCheckUpService : ICheckUpService<EmitterDataResponse>
{
    public (bool, string) Check(IntegrationStruct str)
    {
        List<EmitterDataResponse> emittersDatas = str.emittersList;

        foreach (var emitter in emittersDatas)
        {
            if (!Check(emitter, str.location.id))
                return new (false, $"{emitter} not founded");
        }
        return (true, "Emitters founded");
    }

    private bool Check(EmitterDataResponse emitter,  int idLocation)
    {
        if (emitter == null)
        {
            Message($"{idLocation} - in emitter is not found");
            return false;
        }

        if (emitter.client == null)
        {
            Message($"{idLocation} - in emitter client is not found");
            return false;
        }

        if (emitter.address == null)
        {
            Message($"{idLocation} - in emitter address is not found");
            return false;
        }

        if (emitter.idContract == null)
        {
            Message($"{idLocation} - in emitter idContract is not found");
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