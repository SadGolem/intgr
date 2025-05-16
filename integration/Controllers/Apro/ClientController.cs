using integration.Context;
using Microsoft.AspNetCore.Mvc;
using integration.Controllers;
using integration.Factory.GET.Interfaces;
using integration.Services.Interfaces;
using integration.Services.Location;

[ApiController]
[Route("api/[controller]")]
public class ClientController : BaseSyncController<ClientData>
{
    public ClientController(
        ILogger<ClientController> logger,
        IGetterServiceFactory<ClientData> serviceGetter)
        : base(logger, serviceGetter) { }
    
    public async Task<IActionResult> Sync()
    {
        return await base.Sync();
    }
}
