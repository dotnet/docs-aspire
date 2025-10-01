using AspirePostgreSQLEFCore.Data;
using Microsoft.EntityFrameworkCore;

namespace AspirePostgreSQLEFCore.MigrationService;

public class Worker(
    IServiceProvider serviceProvider,
    IHostApplicationLifetime hostApplicationLifetime,
    ILogger<Worker> logger) : BackgroundService
{
    public const string ActivitySourceName = "Migrations";

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TicketContext>();

            await EnsureDatabaseAsync(dbContext, stoppingToken);
            await RunMigrationAsync(dbContext, stoppingToken);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database.");
            throw;
        }

        hostApplicationLifetime.StopApplication();
    }

    private static async Task EnsureDatabaseAsync(TicketContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            // Create the database if it doesn't exist
            // This is safe to run multiple times
            await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        });
    }

    private async Task RunMigrationAsync(TicketContext dbContext, CancellationToken cancellationToken)
    {
        var strategy = dbContext.Database.CreateExecutionStrategy();

        await strategy.ExecuteAsync(async () =>
        {
            logger.LogInformation("Running database migrations...");
            
            // Apply any pending migrations
            await dbContext.Database.MigrateAsync(cancellationToken);
            
            logger.LogInformation("Database migrations completed successfully.");
        });
    }
}