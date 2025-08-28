using integration.Context;
using Microsoft.AspNetCore.Mvc;
using integration.Controllers;
using integration.Factory.GET.Interfaces;
using integration.Services.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class ContractController : BaseSyncController<ContractDataResponse>
{
    private readonly ILogger<ContractController> _logger;
    private readonly IGetterServiceFactory<ContractDataResponse> _serviceGetter;
    private IGetterService<ContractDataResponse> _getter;
    public ContractController(ILogger<ContractController> logger, 
        IGetterServiceFactory<ContractDataResponse> serviceGetter
      ) : base(logger, serviceGetter) {}

    [HttpGet("get contracts")] // This endpoint can be used for manual triggers
    public async Task<IActionResult> Sync()
    {
        return await base.Sync();
    }
    
}
