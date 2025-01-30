using integration.Controllers;
using integration.Controllers.Apro;
using integration.Controllers.MT;
using System.Formats.Tar;
using System.Security.Cryptography.X509Certificates;

namespace integration
{
    public class DataSyncService : IHostedService, IDisposable
    {
        private readonly ILogger<DataSyncService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private Timer? _timer;
        private const int _updateTime = 30;

        public DataSyncService(ILogger<DataSyncService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("DataSyncService is starting.");
            EmailMessageBuilder.ClearList();
            _timer = new Timer(async (state) => await DoWork(state), null, TimeSpan.Zero, TimeSpan.FromMinutes(_updateTime));
            return Task.CompletedTask;
        }

        private async Task DoWork(object? state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var tokenController = scope.ServiceProvider.GetService<TokenController>();
                await tokenController.GetTokens();
                var locationController = scope.ServiceProvider.GetRequiredService<LocationController>();
                var wasteSiteEntryController = scope.ServiceProvider.GetRequiredService<WasteSiteEntryController>();
                var entryController = scope.ServiceProvider.GetRequiredService<EntryController>();

                await StartLocation(locationController);
                await StartEntry(wasteSiteEntryController, entryController);
                await SendAsync();
            }
        }


        private async Task SendAsync()
        {
            await EmailSender.Send();
        }

        private async Task StartLocation(LocationController locationController)
        {
            try
            {
                await locationController.SyncLocations();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while syncing locations.");
            }
        }

        private async Task StartEntry(WasteSiteEntryController wasteSiteEntryController, EntryController entryController)
        {
            try
            {
                var newWasteData = await wasteSiteEntryController.GetEntriesData();
                LastUpdateTextFileManager.SetLastUpdateTime("entry");
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
