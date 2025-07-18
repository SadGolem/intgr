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
    private ISetterServiceFactory<AgreMTRequest> _setterServiceFactory;
    
    public AgreController(ILogger<BaseSyncController<AgreMTDataResponse>> logger,
        IGetterServiceFactory<AgreMTDataResponse> serviceGetter,
        ISetterServiceFactory<AgreMTRequest> serviceSetter
        ) : base(logger, serviceGetter)
    {
        _setterServiceFactory = serviceSetter;
    }
    
    public async Task<IActionResult> Sync()
    {
        await base.Sync();
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
    
    public void Message(string ex)
    {
        EmailMessageBuilder.PutInformation(EmailMessageBuilder.ListType.getentry, ex);
    }
}