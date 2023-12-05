using Npgsql;

namespace WorkerService;

public sealed class Worker(
    ILogger<Worker> logger,
    NpgsqlDataSource dataSource) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            // Use the dataSource

            await Task.Delay(1_000, stoppingToken);
        }
    }
}
