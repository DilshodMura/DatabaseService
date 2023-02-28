using DbService.Interfaces;
using ExternalDb;
using InternalDb;

namespace DbService
{
    public class SynchronizationService : ISynchronizationService
    {
        private readonly SyncService _syncService;
        public SynchronizationService(
        
        ILogger<SyncService> logger)
        {
            _syncService = new SyncService(logger);
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _syncService.StartAsync(cancellationToken);
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _syncService.StopAsync(cancellationToken);
        }
    }
}
