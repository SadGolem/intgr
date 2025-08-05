using integration.Context;
using integration.Factory.GET.Interfaces;

public interface IClientManagerService
{
    Task SyncAsync();
}

public class ClientManagerService : IClientManagerService
{
    private readonly IGetterServiceFactory<ClientDataResponse> _serviceGetter;
    private readonly ILogger<ClientManagerService> _logger;

    public ClientManagerService(
        IGetterServiceFactory<ClientDataResponse> serviceGetter,
        ILogger<ClientManagerService> logger)
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
            _logger.LogError(ex, "Error in Client sync");
            throw;
        }
    }
}