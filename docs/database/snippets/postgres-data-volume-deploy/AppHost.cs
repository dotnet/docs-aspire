var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddAzurePostgresFlexibleServer("postgres")
                      .RunAsContainer(ConfPostgresContainer);

builder.Build().Run();

static void ConfPostgresContainer(IResourceBuilder<PostgresServerResource> container)
{
    container.WithDataVolume();
}