var builder = DistributedApplication.CreateBuilder(args);

var database = builder.AddPostgresContainer("PostgreSqlConnection")
    .AddDatabase("dbname");

builder.AddProject<Projects.WorkerService>("workerservice")
    .WithReference(database)
    .WithReplicas(5);

builder.Build().Run();
