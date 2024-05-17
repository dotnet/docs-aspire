using Aspire.Hosting.Dapr;

var builder = DistributedApplication.CreateBuilder(args);
builder.AddDapr();

DaprSidecarOptions sidecarOptions = new ()
{
    AppId = "apiservice-dapr",
    DaprGrpcPort = 50001,
    DaprHttpPort = 3500,
    MetricsPort = 9090
};

var apiService = builder
    .AddProject<Projects.Dapr_ApiService>("apiservice")
    .WithDaprSidecar(sidecarOptions);

builder.AddProject<Projects.Dapr_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService)
    .WithDaprSidecar("webfrontend-dapr");

builder.Build().Run();
