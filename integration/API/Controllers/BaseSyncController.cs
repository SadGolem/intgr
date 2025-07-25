using integration.Factory.GET.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace integration.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseSyncController<T> : ControllerBase where T : class
    {
        protected readonly ILogger<BaseSyncController<T>> _logger;
        protected readonly IGetterServiceFactory<T> _serviceGetter;

        protected BaseSyncController(
            ILogger<BaseSyncController<T>> logger,
            IGetterServiceFactory<T> serviceGetter)
        {
            _logger = logger;
            _serviceGetter = serviceGetter;
        }

        [HttpPost("sync")]
        public virtual async Task<IActionResult> Sync()
        {
            _logger.LogInformation($"Starting {typeof(T).Name} sync...");
            try
            {
                var service = _serviceGetter.Create();
                await service.Get();
                return Ok($"{typeof(T).Name} synced successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error during {typeof(T).Name} sync");
                return StatusCode(500, $"Error during {typeof(T).Name} sync");
            }
        }
    }
}