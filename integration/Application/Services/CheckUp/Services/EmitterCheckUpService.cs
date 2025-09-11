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
                return new (false, $"Эмиттер {emitter} не найден");
        }
        return (true, "");
    }

    private bool Check(EmitterDataResponse emitter,  int idLocation)
    {
        if (emitter == null)
        {
            Message($"В площадке с номером {idLocation} - нет эмиттера", EmailMessageBuilder.ListType.getlocation);
            return false;
        }
        
        /*if (emitter.container == null || emitter.container.Count == 0)
        {
            Message($"{idLocation} - containers not found", EmailMessageBuilder.ListType.getlocation);
            return false;
        }*/

        if (emitter.participant_id == null)
        {
            Message($"В площадке с номером {idLocation} - в эмиттере нет контрагента", EmailMessageBuilder.ListType.getlocation);
            return false;
        }

        if (emitter.WasteSource.address == null)
        {
            Message($"В площадке с номером {idLocation} - в эмиттере отстутвует адрес", EmailMessageBuilder.ListType.getlocation);
            return false;
        }

        if (emitter.idContract == null)
        {
            Message($"В площадке с номером {idLocation} - в эмиттере не найден договор", EmailMessageBuilder.ListType.getlocation);
            return false;
        }

        return true;
    }
    
}