using integration.Context;
using integration.Factory.GET.Interfaces;

public interface IScheduleManagerService
{
    Task SyncAsync();
}

public class ScheduleManagerService : IScheduleManagerService
{
    private readonly IGetterServiceFactory<ScheduleDataResponse> _serviceGetter;
    private readonly ILogger<ScheduleManagerService> _logger;

    public ScheduleManagerService(
        IGetterServiceFactory<ScheduleDataResponse> serviceGetter,
        ILogger<ScheduleManagerService> logger)
    {
        _serviceGetter = serviceGetter;
        _logger = logger;
    }

    public async Task SyncAsync()
    {
        try
        {
            var scheduleGetterService = _serviceGetter.Create();
            await scheduleGetterService.Get();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Schedule sync");
            throw;
        }
    }
}