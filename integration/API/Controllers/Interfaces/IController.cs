using Microsoft.AspNetCore.Mvc;

namespace integration.Controllers
{
    public interface IController
    {
         Task<IActionResult> Sync();
    }
}
