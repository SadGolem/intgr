using integration.Services.Integration;
using integration.Services.Storage;
using integration.Services.Storage.Interfaces;
using integration.Structs;

namespace integration
{
    public class MainSyncService : BackgroundService
    {
        private readonly ILogger<MainSyncService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostApplicationLifetime _appLifetime;

        public MainSyncService(
            ILogger<MainSyncService> logger,
            IServiceProvider serviceProvider,
            IHostApplicationLifetime appLifetime)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _appLifetime = appLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("DataSyncService started (single-run).");

            try
            {
                await DoWorkOnce(stoppingToken);
                _logger.LogInformation("DataSyncService finished all tasks. Shutting down...");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DataSyncService terminated with an error.");
            }
            finally
            {
                _appLifetime.StopApplication();
            }
        }

        private async Task DoWorkOnce(CancellationToken ct)
        {
            using var scope = _serviceProvider.CreateScope();

            var tokenManager = scope.ServiceProvider.GetRequiredService<ITokenManagerService>();
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


            await tokenManager.GetTokensAsync();
            await GetEmployers(employerSync, ct);
            await GetMTLEntryStatus(entrySync, ct);
            await GetMTAgre(agreSync, ct);
            await GetLocation(locationSync, ct);
            await GetContractPosition(contractPositionSync, ct);
            await GetContract(contractSync, ct);
            await GetClient(clientSync, ct);
            await GetEmitter(emitterSync, ct);
            await GetSchedule(scheduleSync, ct);
            await SetStruct(converter, ct);

            await StartEntry(entrySync, ct);
            await CheckAndSendIntegrationToAPRO(storage, integrationService, ct);
            //await StartPhoto(locationSync, ct);

            await SendToEmail();
            EmailMessageBuilder.ClearList();
        }

        // Ниже — те же методы, только пробрасываем CancellationToken (на будущее — полезно для корректной отмены)

        private async Task GetMTAgre(IAgreManagerService agreController, CancellationToken ct)
        {
            try { await agreController.SyncAsync(); }
            catch (Exception ex) { _logger.LogError(ex, "Error while syncing agre."); }
        }

        private Task SendToEmail() => EmailSender.Send();

        private async Task GetSchedule(IScheduleManagerService scheduleController, CancellationToken ct)
        {
            try { await scheduleController.SyncAsync(); }
            catch (Exception ex) { _logger.LogError(ex, "Error while syncing schedule."); }
        }

        private async Task GetEmployers(IEmployerManagerService employerController, CancellationToken ct)
        {
            try { await employerController.SyncAsync(); }
            catch (Exception ex) { _logger.LogError(ex, "Error while syncing employers."); }
        }

        private async Task StartPhoto(ILocationManagerService locationController, CancellationToken ct)
        {
            try
            {
                await locationController.GetFromMTAsync();
                await locationController.SetFromMTAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while syncing photo.");
            }
        }

        private async Task GetMTLEntryStatus(IEntryManagerService entryController, CancellationToken ct)
        {
            try
            {
                await entryController.GetMTAsync();
                await entryController.SetToMTAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while syncing entry status.");
            }
        }

        private async Task GetEmitter(IEmitterManagerService emitterController, CancellationToken ct)
        {
            try { await emitterController.SyncAsync(); }
            catch (Exception ex) { _logger.LogError(ex, "Error while syncing emitter."); }
        }

        private async Task GetLocation(ILocationManagerService locationController, CancellationToken ct)
        {
            try { await locationController.SyncLocationsAsync(); }
            catch (Exception ex) { _logger.LogError(ex, "Error while syncing locations."); }
        }

        private async Task GetContractPosition(IContractPositionManagerService contractPositionController, CancellationToken ct)
        {
            try { await contractPositionController.SyncAsync(); }
            catch (Exception ex) { _logger.LogError(ex, "Error while syncing contract position."); }
        }

        private async Task GetContract(IContractManagerService contractController, CancellationToken ct)
        {
            try { await contractController.SyncAsync(); }
            catch (Exception ex) { _logger.LogError(ex, "Error while syncing contracts."); }
        }

        private Task SetStruct(IConverterToStorageService converter, CancellationToken ct) => converter.ToStorage();

        private async Task GetClient(IClientManagerService clientController, CancellationToken ct)
        {
            try { await clientController.SyncAsync(); }
            catch (Exception ex) { _logger.LogError(ex, "Error while syncing contragents."); }
        }

        private async Task CheckAndSendIntegrationToAPRO(
            IStorageService<IntegrationStruct> storageService,
            IIntegrationService multipartSetterController,
            CancellationToken ct)
        {
            var structs = storageService.Get();
            foreach (var s in structs)
            {
                try
                {
                    ct.ThrowIfCancellationRequested();
                    await multipartSetterController.SendIntegrationDataAsync(s);
                    _logger.LogInformation("MultipartSetter finished");
                }
                catch (OperationCanceledException) { throw; }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error while sending integration");
                    throw;
                }
            }
        }

        private async Task StartEntry(IEntryManagerService entryController, CancellationToken ct)
        {
            try { await entryController.SyncAsync(); }
            catch (Exception ex) { _logger.LogError(ex, "An error occurred while syncing data."); }
        }
    }
}
