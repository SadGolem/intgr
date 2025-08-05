using integration.Context;
using integration.Factory.GET.Interfaces;

public interface IContractManagerService
{
    Task SyncAsync();
}

public class ContractManagerService : IContractManagerService
{
    private readonly IGetterServiceFactory<ContractDataResponse> _serviceGetter;
    private readonly ILogger<ContractManagerService> _logger;

    public ContractManagerService(
        IGetterServiceFactory<ContractDataResponse> serviceGetter,
        ILogger<ContractManagerService> logger)
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
            _logger.LogError(ex, "Error in Contract sync");
            throw;
        }
    }
}