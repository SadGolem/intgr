using integration.Context.MT;
using integration.Factory.GET.Interfaces;
using integration.Factory.SET.Interfaces;
using Microsoft.AspNetCore.Mvc;
using integration.Services.Interfaces;

namespace integration.Controllers.Apro
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntryController : BaseSyncController<EntryDataResponse>, ISetterService<EntryDataResponse>
    {
        private string _aproConnectSettings;
        private readonly ILogger<EntryController> _logger;
        private ISetterServiceFactory<EntryDataResponse> _setterServiceFactory;
        private IGetterServiceFactory<EntryMTDataResponse> _getterServiceFactoryMT;
        public static List<EntryDataResponse> newEntry = new List<EntryDataResponse>();
        public static List<EntryDataResponse> updateEntry = new List<EntryDataResponse>();

        public EntryController(
            ILogger<EntryController> logger,
            IGetterServiceFactory<EntryDataResponse> serviceGetter,
            ISetterServiceFactory<EntryDataResponse> serviceSetter, IGetterServiceFactory<EntryMTDataResponse> serviceGetterMT)
            : base(logger, serviceGetter)
        {
            _setterServiceFactory = serviceSetter;
            _getterServiceFactoryMT = serviceGetterMT;
        }
        
        public async Task<IActionResult> Sync()
        {
            await Get();
            return Ok();
        }
        
        private async Task Get()
        {
            await base.Sync();
        }
        
        public async Task GetMT()
        {
            var service = _getterServiceFactoryMT.Create();
            await service.Get();
        }

        private async Task TryPostAndPatch()
        {
            var service = _setterServiceFactory.Create(); 
            await service.Set();
        }

        public async Task Set()
        {
            await TryPostAndPatch();
        }
        public void Message(string ex)
        {
            EmailMessageBuilder.PutInformation(EmailMessageBuilder.ListType.getentry, ex);
        }
    }
}