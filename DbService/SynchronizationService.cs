using DbService.Interfaces;

namespace DbService
{
    public class SynchronizationService : ISynchronizationService
    {
        private readonly SyncHelper _syncService;
        public SynchronizationService(
        
        ILogger<SyncHelper> logger)
        {
            _syncService = new SyncHelper(logger);
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
