using integration.Context.Response;
using integration.Factory.GET.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace integration.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployerController: BaseSyncController<EmployerDataResponse>
{
    private string _aproConnectSettings;
    private readonly ILogger<EmployerController> _logger;
    private IGetterServiceFactory<EmployerDataResponse> _getterServiceFactory;
    
    public EmployerController(ILogger<BaseSyncController<EmployerDataResponse>> logger, 
        IGetterServiceFactory<EmployerDataResponse> serviceGetter) 
        : base(logger, serviceGetter)
    {
        _getterServiceFactory = serviceGetter;
    }
    
    public async Task<IActionResult> Sync()
    {
        await Get();
        return Ok();
    }
        
    private async Task Get()
    {
        var service = _getterServiceFactory.Create();
        await service.Get();
    }
}