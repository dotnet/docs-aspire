var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddPostgresContainer("postgresql")
    .AddDatabase("PostgreSqlConnection");

builder.AddProject<Projects.WorkerService>("workerservice")
    .WithReference(database)
    .WithReplicas(3);

builder.Build().Run();
