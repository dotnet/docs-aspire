var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("redis");

builder.AddProject<Projects.api>("api")
    .WithReference(cache);

builder.AddComputeEnvironment("compute-env");

builder.Build().Run();