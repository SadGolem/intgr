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
public class ClientController : ControllerBase, IController
{
    private readonly ILogger<ClientController> _logger;
    private readonly IGetterServiceFactory<ClientData> _serviceGetter;
    private IGetterService<ClientData> _getter;
    private ILocationIdService _locationIdService;
    public ClientController(ILogger<ClientController> logger, 
        IGetterServiceFactory<ClientData> serviceGetter,
        ILocationIdService locationIdService)
    {
        _logger = logger;
        _serviceGetter = serviceGetter;
        _locationIdService = locationIdService;
    }
    
    public async Task<IActionResult> Sync()
    {
        _logger.LogInformation("Starting manual contragents sync...");
        try
        {
            await GetClients();

            return Ok("Contragents synced successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during location sync.");
            return StatusCode(500, "Error during location sync.");
        }
    }

    private async Task GetClients()
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
