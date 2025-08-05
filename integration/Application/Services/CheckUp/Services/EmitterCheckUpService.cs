using integration.Context;
using integration.Structs;

namespace integration.Services.CheckUp.Services;

public class EmitterCheckUpService : BaseCheckUpService, IEmitterCheckUpService
{
    public (bool, string) Check(IntegrationStruct str)
    {
        List<EmitterDataResponse> emittersDatas = str.emittersList;

        foreach (var emitter in emittersDatas)
        {
            if (!Check(emitter, str.location.id))
                return new (false, $"{emitter} not founded");
        }
        return (true, "");
    }

    private bool Check(EmitterDataResponse emitter,  int idLocation)
    {
        if (emitter == null)
        {
            Message($"{idLocation} - in emitter not found", EmailMessageBuilder.ListType.getlocation);
            return false;
        }
        
        /*if (emitter.container == null || emitter.container.Count == 0)
        {
            Message($"{idLocation} - containers not found", EmailMessageBuilder.ListType.getlocation);
            return false;
        }*/

        if (emitter.participant_id == null)
        {
            Message($"{idLocation} - in emitter client not found", EmailMessageBuilder.ListType.getlocation);
            return false;
        }

        if (emitter.WasteSource.address == null)
        {
            Message($"{idLocation} - in emitter address not found", EmailMessageBuilder.ListType.getlocation);
            return false;
        }

        if (emitter.idContract == null)
        {
            Message($"{idLocation} - in emitter idContract not found", EmailMessageBuilder.ListType.getlocation);
            return false;
        }

        return true;
    }
    
}