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
        
        return (true, "Location founded");
    }

    private bool Check(LocationDataResponse location)
    {
        if (location == null)
        {
            Message($"{location.id} - is not found");
            return false;
        }

        if (location.address == "")
        {
            Message($"{location.id} - address is not found");
            return false;
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