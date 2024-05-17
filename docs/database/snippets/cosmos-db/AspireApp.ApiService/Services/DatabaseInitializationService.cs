namespace AspireApp.ApiService.Services;

public sealed class DatabaseInitializationService(
    CosmosClient client,
    ILogger<DatabaseInitializationService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var (databaseCreated, database) = await EnsureDatabaseCreatedAsync(stoppingToken);
        var (containerCreated, container) = await EnsureContainerCreatedAsync(database, stoppingToken);

        if (databaseCreated || containerCreated)
        {
            await SeedDatabaseAsync(container);
        }
    }

    private async Task<(bool Created, Database Database)> EnsureDatabaseCreatedAsync(
        CancellationToken cancellationToken)
    {
        var response = await client.CreateDatabaseIfNotExistsAsync(
            "programming", cancellationToken: cancellationToken);

        var created = response.StatusCode is HttpStatusCode.Created;

        logger.LogInformation(
            "The programming database {Message}.",
            created ? "was created" : "already existed");

        return (created, response.Database);
    }

    private async Task<(bool Created, Container Container)> EnsureContainerCreatedAsync(
        Database database, CancellationToken cancellationToken)
    {
        var response = await database.CreateContainerIfNotExistsAsync(
            new ContainerProperties
            {
                Id = "languages",
                PartitionKeyPath = "/id"
            },
            cancellationToken: cancellationToken);

        var created = response.StatusCode is HttpStatusCode.Created;

        logger.LogInformation(
            "The language container {Message}.",
            created ? "was created" : "already existed");

        return (created, response.Container);
    }

    private async Task SeedDatabaseAsync(Container container)
    {
        logger.LogInformation("Seeding database...");

        await container.UpsertItemAsync(
            new ProgrammingLanguage("1", "C#", "C# (pronounced \"See Sharp\") is a modern, object-oriented, and type-safe programming language.", new DateOnly(2000, 7, 11)), new PartitionKey("1"));

        await container.UpsertItemAsync(
            new ProgrammingLanguage("2", "F#", "F# is a universal programming language for writing succinct, robust and performant code.", new DateOnly(2005, 5, 1)), new PartitionKey("2"));

        await container.UpsertItemAsync(
            new ProgrammingLanguage("3", "VB.NET", "Visual Basic is an object-oriented programming language developed by Microsoft.", new DateOnly(2002, 2, 13)), new PartitionKey("3"));
    }
}
