using Projects;

var builder = DistributedApplication.CreateBuilder(args);

// Add your application resources
var cache = builder.AddRedis("cache");

var apiService = builder.AddProject<Projects.ApiService>("apiservice")
                        .WithReference(cache);

builder.AddProject<Projects.Web>("webfrontend")
       .WithExternalHttpEndpoints()
       .WithReference(cache)
       .WithReference(apiService);

// Add Kubernetes environment
builder.AddKubernetesEnvironment("k8s");

builder.Build().Run();