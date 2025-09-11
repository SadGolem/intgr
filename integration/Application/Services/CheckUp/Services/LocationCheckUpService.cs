using integration.Context;
using integration.Structs;

namespace integration.Services.CheckUp.Services;

public class LocationCheckUpService : ILocationCheckUpService
{
    public (bool, string) Check(IntegrationStruct str)
    {
        LocationDataResponse location = str.location;
        if (!Check(location))
                return new (false, $"{location.id} not founded");
        
        return (true, "");
    }

    private bool Check(LocationDataResponse location)
    {
        if (location == null)
        {
            Message($"Площадка {location.id} - Не найдена", location.author?.id);
            return false;
        }

        if (location.address == "")
        {
            Message($"У площадки с номером {location.id} - Не указан адрес", location.author?.id);
            return false;
        }
        
        if (location.containers == null)
        {
            Message($"У площадки с номером {location.id} - нет контейнера", location.author?.id);
            return false;
        }
        
        if (location.containers.Count == 0)
        {
            Message($"У площадки с номером {location.id} - нет контейнера", location.author?.id);
            return false;
        }

        foreach (var container in location.containers)
        {
            if (container.type == null || container.type?.id == 0)
            {
                Message($"У площадки с номером {location.id} - контейнер {container.id} с некорректным типом", location.author?.id);
                return false;
            }
        }
        
        return true;
    }
    
    private void Message(string message, int? id)
    {
        EmailMessageBuilder.PutError(
            EmailMessageBuilder.ListType.getlocation, 
            message, id
        );
    }
}