namespace DbService.Interfaces
{
    public interface ISynchronizationService
    {
        public Task StartAsync(CancellationToken cancellation);
        public Task StopAsync(CancellationToken cancellation);
    }
}
