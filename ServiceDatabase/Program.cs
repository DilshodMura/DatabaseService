using ServiceDatabase;

internal class Program
{
    private static void Main(string[] args)
    {
        var interval = TimeSpan.FromSeconds(1);
        var timer = new Timer(SyncronizeDatabases.SynchronizeData, null, TimeSpan.Zero, interval);
        //SyncronizeDatabases.SynchronizeData();
        Console.WriteLine("Test");

        // Wait for the batch job to run indefinitely
        Task.Delay(-1).Wait();
    }
}