var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddPostgresContainer("postgres")
    .AddDatabase("test");

var databaseConnection = builder.AddPostgresConnection("PostgreSqlConnection");

builder.AddProject<Projects.WorkerService>("workerservice")
    .WithReference(database)
    .WithReference(databaseConnection)
    .WithReplicas(3);

builder.Build().Run();
