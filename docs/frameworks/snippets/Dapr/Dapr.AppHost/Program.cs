using Aspire.Hosting.Dapr;

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder
    .AddProject<Projects.Dapr_ApiService>("apiservice")
    .WithDaprSidecar();

DaprSidecarOptions sidecarOptions = new()
{
    DaprGrpcPort = 50001,
    DaprHttpPort = 3500,
    MetricsPort = 9090
};

builder.AddProject<Projects.Dapr_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WithDaprSidecar(sidecarOptions);

builder.Build().Run();
