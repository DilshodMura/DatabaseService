namespace DbService.Interfaces
{
    public interface ISynchronizationService
    {
        /// <summary>
        /// Starting synchranization
        /// </summary>
        public Task StartAsync(CancellationToken cancellation);

        /// <summary>
        /// Stopping synchranization
        /// </summary>
        public Task StopAsync(CancellationToken cancellation);
    }
}
