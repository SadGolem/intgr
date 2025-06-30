using integration.Context;
using integration.Factory.GET.Interfaces;
using Microsoft.AspNetCore.Mvc;


namespace integration.Controllers.Apro
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmitterController : BaseSyncController<EmitterDataResponse>
    {
        public EmitterController(
            ILogger<EmitterController> logger,
            IGetterServiceFactory<EmitterDataResponse> serviceGetter)
            : base(logger, serviceGetter) { }
    
        public async Task<IActionResult> Sync()
        {
            return await base.Sync();
        }
        void ToGetMessage(string ex)
        {
            EmailMessageBuilder.PutInformation(EmailMessageBuilder.ListType.getemitter, ex);
        }
    }
}

        


