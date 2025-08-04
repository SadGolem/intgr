using integration.Context;
using integration.Factory.GET.Interfaces;

public interface IContractPositionManagerService
{
    Task SyncAsync();
}

public class ContractPositionManagerService : IContractPositionManagerService
{
    private readonly IGetterServiceFactory<ContractPositionDataResponse> _serviceGetter;
    private readonly IGetterServiceFactory<Container> _serviceGetterContainer;
    private readonly ILogger<ContractPositionManagerService> _logger;

    public ContractPositionManagerService(
        IGetterServiceFactory<ContractPositionDataResponse> serviceGetter,
        IGetterServiceFactory<Container> serviceGetterContainer,
        ILogger<ContractPositionManagerService> logger)
    {
        _serviceGetter = serviceGetter;
        _serviceGetterContainer = serviceGetterContainer;
        _logger = logger;
    }

    public async Task SyncAsync()
    {
        try
        {
            var getter = _serviceGetter.Create();
            var containerGetter = _serviceGetterContainer.Create();


            await getter.Get();
            await containerGetter.Get();

        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ContractPosition sync");
            throw;
        }
    }
}