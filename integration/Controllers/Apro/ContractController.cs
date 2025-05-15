using System.Diagnostics.Contracts;
using integration.Context;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using integration;
using System.Text;
using integration.Controllers;
using integration.Factory.GET.Interfaces;
using integration.HelpClasses;
using integration.Services.Interfaces;
using integration.Services.Location;

[ApiController]
[Route("api/[controller]")]
public class ContractController : ControllerBase, IController
{
    private readonly ILogger<ContractController> _logger;
    private readonly IGetterServiceFactory<ContractData> _serviceGetter;
    private IGetterService<ContractData> _getter;
    private ILocationIdService _locationIdService;
    public ContractController(ILogger<ContractController> logger, 
        IGetterServiceFactory<ContractData> serviceGetter,
        ILocationIdService locationIdService)
    {
        _logger = logger;
        _serviceGetter = serviceGetter;
        _locationIdService = locationIdService;
    }

    [HttpGet("syncEmitters")] // This endpoint can be used for manual triggers
    public async Task<IActionResult> Sync()
    {
        _logger.LogInformation("Starting manual contragents sync...");
        try
        {
            await GetContracts();

            return Ok("Contragents synced successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during location sync.");
            return StatusCode(500, "Error during location sync.");
        }
    }

    private async Task GetContracts()
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
