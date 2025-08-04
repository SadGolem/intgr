using integration.Context;
using integration.Factory.GET.Interfaces;

public interface IEmitterManagerService
{
    Task SyncAsync();
}

public class EmitterManagerService : IEmitterManagerService
{
    private readonly IGetterServiceFactory<EmitterDataResponse> _serviceGetter;
    private readonly ILogger<EmitterManagerService> _logger;

    public EmitterManagerService(
        IGetterServiceFactory<EmitterDataResponse> serviceGetter,
        ILogger<EmitterManagerService> logger)
    {
        _serviceGetter = serviceGetter;
        _logger = logger;
    }

    public async Task SyncAsync()
    {
        try
        {
            var getter = _serviceGetter.Create();
            await getter.Get();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Emitter sync");
            throw;
        }
    }
}