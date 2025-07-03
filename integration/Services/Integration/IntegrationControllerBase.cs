using integration.Context;
using integration.Services.CheckUp;
using integration.Structs;
using Microsoft.AspNetCore.Mvc;

namespace integration.Services.Integration;

[ApiController]
[Route("api/[controller]")]
public class IntegrationControllerBase : ControllerBase
{
    private readonly ICheckUpService<ClientDataResponse> _checkUpServiceClient; 
    private readonly ICheckUpService<EmitterDataResponse> _checkUpServiceEmitter; 
    private readonly ICheckUpService<LocationDataResponse> _checkUpServiceLocation; 
    private readonly ICheckUpService<ScheduleDataResponse> _checkUpServiceSchedule; 
    private readonly ILogger<ControllerBase> _logger; 

    public IntegrationControllerBase(
        ILogger<IntegrationControllerBase> logger) 
    {
        _logger = logger;
    }

    [HttpGet("sync")]
    public async Task<IActionResult> Sync(IntegrationStruct _struct)
    {
        _logger.LogInformation("Starting sync...");
        try
        {
            await Check(_struct);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Sync failed");
            return StatusCode(500, "Internal error");
        }
    }

    private async Task<IActionResult> Check(IntegrationStruct _struct)
    {
        var client = _checkUpServiceClient.Check(_struct);
        if (client.Item1)
        {
            var emitter = _checkUpServiceEmitter.Check(_struct);
            if (emitter.Item1)
            {
                var location = _checkUpServiceEmitter.Check(_struct);
                if (location.Item1)
                {
                    var schedule = _checkUpServiceEmitter.Check(_struct);
                    if (schedule.Item1)
                    {
                        return Ok();
                    }
                    else
                    {
                        Message(schedule.Item2, EmailMessageBuilder.ListType.setschedule);
                    }
                }
                else
                {
                    Message(location.Item2, EmailMessageBuilder.ListType.setlocation);
                }

            }
            else
            {
                Message(emitter.Item2, EmailMessageBuilder.ListType.setemitter);
            }
        }

        Message(client.Item2, EmailMessageBuilder.ListType.setcontragent);
        
        
        return StatusCode(500, "There are mistakes in the data");
    }

    public void Message(string ex, EmailMessageBuilder.ListType type)
    {
        EmailMessageBuilder.PutInformation(type, ex);
    }
}