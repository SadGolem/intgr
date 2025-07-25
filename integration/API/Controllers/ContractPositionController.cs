using integration.Context;
using integration.Factory.GET.Interfaces;
using integration.Services.Interfaces;
using integration.Services.Location;
using Microsoft.AspNetCore.Mvc;

namespace integration.Controllers.Apro;

public class ContractPositionController : ControllerBase, IController
{
    private readonly ILogger<ContractPositionController> _logger;
    private readonly IGetterServiceFactory<ContractPositionDataResponse> _serviceGetter;
    private readonly IGetterServiceFactory<Container> _serviceGetterContainer;
    private IGetterService<ContractPositionDataResponse> _getter;
    private IGetterService<Container> _getterContainer;
    private ILocationIdService _locationIdService;
        
    public ContractPositionController(ILogger<ContractPositionController> logger, 
        IGetterServiceFactory<ContractPositionDataResponse> serviceGetter,
        IGetterServiceFactory<Container> serviceGetterContainer,
        ILocationIdService locationIdService
    )
    {
        _logger = logger;
        _serviceGetter = serviceGetter;
        _serviceGetterContainer = serviceGetterContainer;
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
            _getterContainer = _serviceGetterContainer.Create();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        finally
        {
            await _getter.Get();
            await _getterContainer.Get();
        }
    }
}