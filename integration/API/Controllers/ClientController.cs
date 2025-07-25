using integration.Context;
using Microsoft.AspNetCore.Mvc;
using integration.Controllers;
using integration.Factory.GET.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class ClientController : BaseSyncController<ClientDataResponse>
{
    public ClientController(
        ILogger<ClientController> logger,
        IGetterServiceFactory<ClientDataResponse> serviceGetter)
        : base(logger, serviceGetter) { }
    
    public async Task<IActionResult> Sync()
    {
        return await base.Sync();
    }
}
