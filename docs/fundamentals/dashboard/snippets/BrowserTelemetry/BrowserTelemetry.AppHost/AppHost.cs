var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.BrowserTelemetry_Web>("web")
    .WithExternalHttpEndpoints();

builder.Build().Run();
