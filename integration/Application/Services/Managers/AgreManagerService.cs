using integration.Context.MT;
using integration.Context.Request;
using integration.Factory.GET.Interfaces;
using integration.Factory.SET.Interfaces;

public interface IAgreManagerService
{
    Task SyncAsync();
}

public class AgreManagerService : IAgreManagerService
{
    private readonly IGetterServiceFactory<AgreMTDataResponse> _serviceGetter;
    private readonly ISetterServiceFactory<AgreRequest> _serviceSetter;
    private readonly ILogger<AgreManagerService> _logger;

    public AgreManagerService(
        IGetterServiceFactory<AgreMTDataResponse> serviceGetter,
        ISetterServiceFactory<AgreRequest> serviceSetter,
        ILogger<AgreManagerService> logger)
    {
        _serviceGetter = serviceGetter;
        _serviceSetter = serviceSetter;
        _logger = logger;
    }

    public async Task SyncAsync()
    {
        try
        {
            var getter = _serviceGetter.Create();
            await getter.Get();
            
            var setter = _serviceSetter.Create(); 
            await setter.Set();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Agre sync");
            throw;
        }
    }
}