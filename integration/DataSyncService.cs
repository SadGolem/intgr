using integration.Context;
using integration.Controllers;
using integration.Controllers.Apro;
using integration.Controllers.MT;

namespace integration
{
    public class DataSyncService : IHostedService, IDisposable
    {
        private readonly ILogger<DataSyncService> _logger;
        private readonly IServiceProvider _serviceProvider;
        private Timer? _timer;

        public DataSyncService(ILogger<DataSyncService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("DataSyncService is starting.");
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(30));
            return Task.CompletedTask;
        }

        private async void DoWork(object? state)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var tokenController = scope.ServiceProvider.GetService<TokenController>();
                tokenController.GetTokens();
                var wasteSiteEntryController = scope.ServiceProvider.GetRequiredService<WasteSiteEntryController>();
                var entryController = scope.ServiceProvider.GetRequiredService<EntryController>();
                var locationController = scope.ServiceProvider.GetRequiredService<LocationController>();


                /*try
                {
                    var newWasteData = await wasteSiteEntryController.GetNewWasteData();
                    if (newWasteData.Count > 0)
                    {
                        _logger.LogInformation($"Found {newWasteData.Count} new/updated records to sync");
                        foreach (var wasteData in newWasteData)
                        {
                            await ProcessWasteData(wasteData, entryController);
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
                }*/
                try
                {
                    await locationController.SyncLocations();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while syncing locations.");
                }

            }
        }

        private async Task ProcessWasteData(WasteData wasteData, EntryController entryController)
        {
            try
            {
                if (wasteData.idBT == 0)
                {
                    _logger.LogError($"No ID found: {wasteData.idBT}");
                    return;
                }

                await entryController.ProcessEntryData(wasteData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing data with id: {wasteData.idBT}");
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
