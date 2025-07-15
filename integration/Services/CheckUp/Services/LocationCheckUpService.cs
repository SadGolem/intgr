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
            Message($"{location.id} - not found");
            return false;
        }

        if (location.address == "")
        {
            Message($"{location.id} - address not found");
            return false;
        }
        
        if (location.containers.Count == 0)
        {
            Message($"{location.id} - containers not found");
            return false;
        }

        foreach (var container in location.containers)
        {
            if (container.type == null || container.type?.id == 0)
            {
                Message($"{location.id} - container id {container.id} type not correct");
                return false;
            }
        }
        
        return true;
    }
    
    private void Message(string message)
    {
        EmailMessageBuilder.PutInformation(
            EmailMessageBuilder.ListType.getlocation, 
            message
        );
    }
}