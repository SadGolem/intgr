using integration.Context.MT;
using integration.Context.Request;
using integration.Factory.GET.Interfaces;
using integration.Factory.SET.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace integration.Controllers.Apro;
[ApiController]
[Route("api/[controller]")]
public class AgreController: BaseSyncController<AgreMTDataResponse>
{
    private string _aproConnectSettings;
    private readonly ILogger<EntryController> _logger;
    private ISetterServiceFactory<AgreRequest> _setterServiceFactory;
    
    public AgreController(ILogger<BaseSyncController<AgreMTDataResponse>> logger,
        IGetterServiceFactory<AgreMTDataResponse> serviceGetter,
        ISetterServiceFactory<AgreRequest> serviceSetter
        ) : base(logger, serviceGetter)
    {
        _setterServiceFactory = serviceSetter;
    }
    
    public async Task<IActionResult> Sync()
    {
        await base.Sync();
        await Set();
        return Ok();
    }
    
    private async Task TryPostAndPatch()
    {
        var service = _setterServiceFactory.Create(); 
        await service.Set();
    }

    public async Task Set()
    {
        await TryPostAndPatch();
    }
}