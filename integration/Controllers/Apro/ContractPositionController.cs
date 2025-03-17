using integration.Context;
using integration.Factory.GET.Interfaces;
using integration.Services.Interfaces;
using integration.Services.Location;
using Microsoft.AspNetCore.Mvc;

namespace integration.Controllers.Apro;

public class ContractPositionController : ControllerBase, IController
{
    private readonly ILogger<ContractPositionController> _logger;
    private readonly IGetterServiceFactory<ContractPositionData> _serviceGetter;
    private IGetterService<ContractPositionData> _getter;
    private ILocationIdService _locationIdService;
        
    public ContractPositionController(ILogger<ContractPositionController> logger, 
        IGetterServiceFactory<ContractPositionData> serviceGetter,
        ILocationIdService locationIdService
    )
    {
        _logger = logger;
        _serviceGetter = serviceGetter;
        _locationIdService = locationIdService;
    }
    public async Task<IActionResult> Sync()
    {
        try
        {
            await GetContractPositions();
            return Ok("");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, "Error during location sync.");
        }
    }
    public async Task GetContractPositions()
    {
        try
        {
            _getter = _serviceGetter.Create();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            await _getter.Get();
        }
    }
}