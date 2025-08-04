using integration.Context.Request.MT;
using integration.Factory.GET.Interfaces;
using integration.Factory.SET.Interfaces;

public interface IEntryManagerService
{
    Task SyncAsync();
    Task GetMTAsync();
    Task SetToMTAsync();
}

public class EntryManagerService : IEntryManagerService
{
    private readonly IGetterServiceFactory<EntryDataResponse> _serviceGetter;
    private readonly ISetterServiceFactory<EntryDataResponse> _serviceSetter;
    private readonly ISetterServiceFactory<EntryMTRequest> _setterFromMTToAproStatusServiceFactory;
    private readonly IGetterServiceFactory<EntryMTDataResponse> _getterServiceFactoryMT;
    private readonly ILogger<EntryManagerService> _logger;

    public EntryManagerService(
        IGetterServiceFactory<EntryDataResponse> serviceGetter,
        ISetterServiceFactory<EntryDataResponse> serviceSetter,
        ISetterServiceFactory<EntryMTRequest> setterFromMTToAproStatusServiceFactory,
        IGetterServiceFactory<EntryMTDataResponse> getterServiceFactoryMT,
        ILogger<EntryManagerService> logger)
    {
        _serviceGetter = serviceGetter;
        _serviceSetter = serviceSetter;
        _setterFromMTToAproStatusServiceFactory = setterFromMTToAproStatusServiceFactory;
        _getterServiceFactoryMT = getterServiceFactoryMT;
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
            _logger.LogError(ex, "Error in Entry sync");
            throw;
        }
    }

    public async Task GetMTAsync()
    {
        try
        {
            var service = _getterServiceFactoryMT.Create();
            await service.Get();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Entry get MT");
            throw;
        }
    }

    public async Task SetToMTAsync()
    {
        try
        {
            var service = _setterFromMTToAproStatusServiceFactory.Create(); 
            await service.Set();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Entry set to MT");
            throw;
        }
    }
}