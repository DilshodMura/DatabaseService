using DbService.Interfaces;

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

        /// <summary>
        /// Starting synchronization
        /// </summary>
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _syncService.StartAsync(cancellationToken);
        }

        /// <summary>
        /// Stopping synchronization
        /// </summary>
        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _syncService.StopAsync(cancellationToken);
        }
    }
}
