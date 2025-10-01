var builder = DistributedApplication.CreateBuilder(args);

// Add PostgreSQL server with persistent data volume
var postgres = builder.AddPostgres("postgres")
    .WithDataVolume();

var ticketdb = postgres.AddDatabase("ticketdb");

// Add migration service
var migrations = builder.AddProject<Projects.AspirePostgreSQLEFCore_MigrationService>("migration")
    .WithReference(ticketdb);

// Add web application and ensure it waits for migrations to complete
builder.AddProject<Projects.AspirePostgreSQLEFCore>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(ticketdb)
    .WaitFor(migrations);

builder.Build().Run();