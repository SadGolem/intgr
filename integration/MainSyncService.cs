using integration.Controllers;
using integration.Controllers.Apro;
using integration.Controllers.MT;
using integration.Services.Storage;

namespace integration
{
    public class MainSyncService : IHostedService, IDisposable
    {
        private readonly ILogger<MainSyncService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private  IConverterToStorageService _converterToStorageService;
        private Timer? _timer;
        private const int _updateTime = 30;

        public MainSyncService(ILogger<MainSyncService> logger, IServiceProvider serviceProvider, IConverterToStorageService converterToStorageService)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _converterToStorageService = converterToStorageService;
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

              //  var wasteSiteEntryController = scope.ServiceProvider.GetRequiredService<WasteSiteEntryController>();
            //    var entryController = scope.ServiceProvider.GetRequiredService<EntryController>();
            
                await GetLocation(locationController);
                await GetContractPosition(contractPositionController);
                await GetSchedule(scheduleController);
                await GetContract(contractController);
                await GetClient(contragentController);
                await SetStruct(_converterToStorageService);
               // await StartEntry(wasteSiteEntryController, entryController);
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

        private async Task GetLocation( LocationController locationController)
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
        
        private async Task StartEmitter(ContractController contractController)
        {
            /*try
            {
                await contractController.SyncEmitter();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while syncing emitters.");
            }*/
        }
    
        private async Task StartEntry(WasteSiteEntryController wasteSiteEntryController, EntryController entryController)
        {
            try
            {
                Console.WriteLine(wasteSiteEntryController.GetType());
                var newWasteData = await wasteSiteEntryController.GetEntriesData();
                TimeManager.SetLastUpdateTime("entry");
                if (WasteSiteEntryController.newEntry.Any() || WasteSiteEntryController.updateEntry.Any())
                {
                    _logger.LogInformation($"Found {WasteSiteEntryController.newEntry.Count()} new/updated records to sync");
                    foreach (var wasteData in WasteSiteEntryController.newEntry)
                    {
                        await ProcessWasteData(wasteData, entryController, true);
                    }
                    foreach (var wasteUpdateData in WasteSiteEntryController.updateEntry)
                    {
                        await ProcessWasteData(wasteUpdateData, entryController, false);
                    }
                }
                else
                {
                    _logger.LogInformation("No new/updated records found.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while syncing data.");
            }
        }

        private async Task ProcessWasteData(EntryData wasteData, EntryController entryController, bool isNew)
        {
            try
            {
                if (wasteData.BtNumber == 0)
                {
                    _logger.LogError($"No ID found: {wasteData.BtNumber}");
                    return;
                }
                if (isNew)
                    await entryController.ProcessEntryPostData(wasteData);
                else { await entryController.ProcessEntryPatchData(wasteData); }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing data with id: {wasteData.BtNumber}");
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
