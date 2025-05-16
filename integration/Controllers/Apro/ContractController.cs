using System.Diagnostics.Contracts;
using integration.Context;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using integration;
using System.Text;
using integration.Controllers;
using integration.Factory;
using integration.Factory.GET.Interfaces;
using integration.HelpClasses;
using integration.Services.Interfaces;
using integration.Services.Location;

[ApiController]
[Route("api/[controller]")]
public class ContractController : BaseSyncController<ContractData>
{
    private readonly ILogger<ContractController> _logger;
    private readonly IGetterServiceFactory<ContractData> _serviceGetter;
    private IGetterService<ContractData> _getter;
    public ContractController(ILogger<ContractController> logger, 
        IGetterServiceFactory<ContractData> serviceGetter
      ) : base(logger, serviceGetter) {}

    [HttpGet("syncEmitters")] // This endpoint can be used for manual triggers
    public async Task<IActionResult> Sync()
    {
        return await base.Sync();
    }
    
}
