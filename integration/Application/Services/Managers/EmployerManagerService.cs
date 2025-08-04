using integration.Context.Response;
using integration.Factory.GET.Interfaces;

public interface IEmployerManagerService
{
    Task SyncAsync();
}

public class EmployerManagerService : IEmployerManagerService
{
    private readonly IGetterServiceFactory<EmployerDataResponse> _serviceGetter;
    private readonly ILogger<EmployerManagerService> _logger;

    public EmployerManagerService(
        IGetterServiceFactory<EmployerDataResponse> serviceGetter,
        ILogger<EmployerManagerService> logger)
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
            _logger.LogError(ex, "Error in Employer sync");
            throw;
        }
    }
}