using integration.Controllers;
using integration.Controllers.Apro;
using integration.Services.Integration;
using integration.Services.Storage;
using integration.Services.Storage.Interfaces;
using integration.Structs;
namespace integration
{
    public class MainSyncService : IHostedService, IDisposable
    {
        private readonly ILogger<MainSyncService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private  IConverterToStorageService _converterToStorageService;
        private  IStorageService<IntegrationStruct> _storageService;
        private IntegrationController _multipartSetterController;
        private Timer? _timer;
        private const int _updateTime = 30;

        public MainSyncService(ILogger<MainSyncService> logger, IServiceProvider serviceProvider, 
            IConverterToStorageService converterToStorageService, IStorageService<IntegrationStruct> storageService, IntegrationController multipartSetterController)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _converterToStorageService = converterToStorageService;
            _storageService = storageService;
            _multipartSetterController = multipartSetterController;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("DataSyncService is starting.");
           // EmailMessageBuilder.ClearList();
            _timer = new Timer(async (state) => await DoWork(state), null, TimeSpan.Zero, TimeSpan.FromMinutes(_updateTime));
            return Task.CompletedTask;
        }

        private async Task DoWork(object? state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var tokenController = scope.ServiceProvider.GetService<TokenController>();
                await tokenController.GetTokens();

               // var getterAPI = scope.ServiceProvider.GetRequiredService<GetterAPIService>();
                
                var locationController = scope.ServiceProvider.GetRequiredService<LocationController>();
                var scheduleController = scope.ServiceProvider.GetRequiredService<ScheduleController>();
                var contractPositionController = scope.ServiceProvider.GetRequiredService<ContractPositionController>();
                var contractController = scope.ServiceProvider.GetRequiredService<ContractController>();
                var contragentController = scope.ServiceProvider.GetRequiredService<ClientController>();
                var emitterController = scope.ServiceProvider.GetRequiredService<EmitterController>(); 
                var entryController = scope.ServiceProvider.GetRequiredService<EntryController>();

                //await GetMTLEntryStatus(entryController);
                await GetLocation(locationController);
                await GetContractPosition(contractPositionController);
                await GetContract(contractController);
                await GetClient(contragentController);
                await GetEmitter(emitterController);
                await GetSchedule(scheduleController);
                await SetStruct(_converterToStorageService);
                
                //await StartEntry(entryController); 
                await CheckAndSendIntegrationToAPRO();
                //await GetMTLocationAndSendStatusAndPhotoToApro(locationController);
                
                await SendAsync();
                
                EmailMessageBuilder.ClearList();
            }
        }

        private async Task SendAsync()
        {
            await EmailSender.Send();
        }

        private async Task GetSchedule(ScheduleController scheduleController)
        {
            try
            {
                await scheduleController.Sync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while syncing schedule.");
            }
        }
        private async Task GetMTLocationAndSendStatusAndPhotoToApro(LocationController locationController)
        {
            try
            {
                await locationController.Get();
                await locationController.Set();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while syncing schedule.");
            }
        }
        private async Task GetMTLEntryStatus(EntryController entryController)
        {
            try
            {
                await entryController.GetMT();
                await entryController.SetToMT();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while syncing schedule.");
            }
        }
        private async Task GetEmitter(EmitterController emitterController)
        {
            try
            {
                await emitterController.Sync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while syncing emitter.");
            }
        }

        private async Task GetLocation(LocationController locationController)
        {
            try
            {
                await locationController.Sync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while syncing locations.");
            }
        }
        private async Task GetContractPosition(ContractPositionController contractPositionController)
        {
            try
            {
                await contractPositionController.Sync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while syncing contract position.");
            }
        }

        private async Task GetContract(ContractController contractController)
        {
            try
            {
                await contractController.Sync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while syncing contracts.");
            }
        }
        
        private async Task SetStruct(IConverterToStorageService converter)
        {
            await converter.ToStorage();
        }
        private async Task GetClient(ClientController clientController)
        {
            try
            {
                await clientController.Sync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while syncing contragents.");
            }
        }
        
        private async Task CheckAndSendIntegrationToAPRO()
        
        {
            List<IntegrationStruct> _structs = _storageService.Get();
            foreach (var _struct in _structs)
            {
                try
                {
                    await _multipartSetterController.Sync(_struct);
                    _logger.LogInformation("MultipartSetter was finished");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
                
            }
           
        }
        private async Task StartEntry(EntryController entryController)
        {
            try
            {
                await entryController.Sync();
                await entryController.Set();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while syncing data.");
            }
        }
        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("DataSyncService is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
