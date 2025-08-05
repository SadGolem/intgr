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
        private Timer? _timer;
        private const int _updateTime = 30;
        public MainSyncService(ILogger<MainSyncService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
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
                var tokenManager = scope.ServiceProvider.GetRequiredService<ITokenManagerService>();
                await tokenManager.GetTokensAsync();

                // var getterAPI = scope.ServiceProvider.GetRequiredService<GetterAPIService>();

                var contractPositionSync = scope.ServiceProvider.GetRequiredService<IContractPositionManagerService>();
                var contractSync = scope.ServiceProvider.GetRequiredService<IContractManagerService>();
                var emitterSync = scope.ServiceProvider.GetRequiredService<IEmitterManagerService>();
                var employerSync = scope.ServiceProvider.GetRequiredService<IEmployerManagerService>();
                var entrySync = scope.ServiceProvider.GetRequiredService<IEntryManagerService>();
                var clientSync = scope.ServiceProvider.GetRequiredService<IClientManagerService>();
                var agreSync = scope.ServiceProvider.GetRequiredService<IAgreManagerService>();
                var locationSync = scope.ServiceProvider.GetRequiredService<ILocationManagerService>();
                var scheduleSync = scope.ServiceProvider.GetRequiredService<IScheduleManagerService>();
                var converter = scope.ServiceProvider.GetRequiredService<IConverterToStorageService>();
                var storage = scope.ServiceProvider.GetRequiredService<IStorageService<IntegrationStruct>>();
                var integrationService = scope.ServiceProvider.GetRequiredService<IIntegrationService>();
                var validationService = scope.ServiceProvider.GetRequiredService<IIntegrationValidationService>();

                await GetEmployers(employerSync);
                //await GetMTLEntryStatus(entrySync);
                await GetMTAgre(agreSync);
                await GetLocation(locationSync);
                await GetContractPosition(contractPositionSync);
                await GetContract(contractSync);
                await GetClient(clientSync);
                await GetEmitter(emitterSync);
                await GetSchedule(scheduleSync);
                await SetStruct(converter);

                await StartEntry(entrySync); 
                await CheckAndSendIntegrationToAPRO(storage, integrationService);
                await GetMTLocationAndSendStatusAndPhotoToApro(locationSync);

                await SendToEmail();

                EmailMessageBuilder.ClearList();
            }
        }

        private async Task GetMTAgre(IAgreManagerService agreController)
        {
            try
            {
                await agreController.SyncAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while syncing agre.");
            }
        }

        private async Task SendToEmail()
        {
            await EmailSender.Send();
        }

        private async Task GetSchedule(IScheduleManagerService scheduleController)
        {
            try
            {
                await scheduleController.SyncAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while syncing employers.");
            }
        }
        private async Task GetEmployers(IEmployerManagerService employerController)
        {
            try
            {
                await employerController.SyncAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while syncing schedule.");
            }
        }
        private async Task GetMTLocationAndSendStatusAndPhotoToApro(ILocationManagerService locationController)
        {
            try
            {
                await locationController.GetFromMTAsync();
                await locationController.SetFromMTAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while syncing schedule.");
            }
        }
        private async Task GetMTLEntryStatus(IEntryManagerService entryController)
        {
            try
            {
                await entryController.GetMTAsync();
                await entryController.SetToMTAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while syncing schedule.");
            }
        }
        private async Task GetEmitter(IEmitterManagerService emitterController)
        {
            try
            {
                await emitterController.SyncAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while syncing emitter.");
            }
        }

        private async Task GetLocation(ILocationManagerService locationController)
        {
            try
            {
                await locationController.SyncLocationsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while syncing locations.");
            }
        }
        private async Task GetContractPosition(IContractPositionManagerService contractPositionController)
        {
            try
            {
                await contractPositionController.SyncAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while syncing contract position.");
            }
        }

        private async Task GetContract(IContractManagerService contractController)
        {
            try
            {
                await contractController.SyncAsync();
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
        private async Task GetClient(IClientManagerService clientController)
        {
            try
            {
                await clientController.SyncAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while syncing contragents.");
            }
        }

        private async Task CheckAndSendIntegrationToAPRO(IStorageService<IntegrationStruct> _storageService, IIntegrationService _multipartSetterController)

        {
            List<IntegrationStruct> _structs = _storageService.Get();
            foreach (var _struct in _structs)
            {
                try
                {
                    await _multipartSetterController.SendIntegrationDataAsync(_struct);
                    _logger.LogInformation("MultipartSetter was finished");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        private async Task StartEntry(IEntryManagerService entryController)
        {
            try
            {
                await entryController.SyncAsync();
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
