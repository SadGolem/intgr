using integration.Services.CheckUp;
using integration.Structs;

namespace integration.Services.Integration;

public class IntegrationStructValidator
{
    private readonly IClientCheckUpService _clientCheckUp;
    private readonly IEmitterCheckUpService _emitterCheckUp;
    private readonly ILocationCheckUpService _locationCheckUp;
    private readonly IScheduleCheckUpService _scheduleCheckUp;

    public IntegrationStructValidator(
        IClientCheckUpService clientCheckUp,
        IEmitterCheckUpService emitterCheckUp,
        ILocationCheckUpService locationCheckUp, 
        IScheduleCheckUpService scheduleCheckUp)
    {
        _clientCheckUp = clientCheckUp;
        _emitterCheckUp = emitterCheckUp;
        _locationCheckUp = locationCheckUp;
        _scheduleCheckUp = scheduleCheckUp;
    }

    public async Task<bool> Validate(IntegrationStruct str)
    {
        var clientValid = _clientCheckUp.Check(str);
        if (!clientValid.Item1)
        {
            return false;
        }
        
        var emitterValid = _emitterCheckUp.Check(str);
        if (!emitterValid.Item1)
        {
            return false;
        }

        var locationValid = _locationCheckUp.Check(str);
        if (!locationValid.Item1)
        {
            await Message(clientValid.Item2, EmailMessageBuilder.ListType.getlocation);
            return false;
        }
        
        var scheduleValid = _scheduleCheckUp.Check(str);
        if (!scheduleValid.Item1)
        {
            return false;
        }
        
        return true;
    }
    
    public async Task Message(string ex, EmailMessageBuilder.ListType type)
    { 
        EmailMessageBuilder.PutInformation(type, ex);
    }
}