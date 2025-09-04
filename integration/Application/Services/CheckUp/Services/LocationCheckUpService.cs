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
            Message($"{location.id} - not found", location.author?.id);
            return false;
        }

        if (location.address == "")
        {
            Message($"{location.id} - address not found", location.author?.id);
            return false;
        }
        
        if (location.containers == null)
        {
            Message($"{location.id} - containers not found", location.author?.id);
            return false;
        }
        
        if (location.containers.Count == 0)
        {
            Message($"{location.id} - containers not found", location.author?.id);
            return false;
        }

        foreach (var container in location.containers)
        {
            if (container.type == null || container.type?.id == 0)
            {
                Message($"{location.id} - container id {container.id} type not correct", location.author?.id);
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