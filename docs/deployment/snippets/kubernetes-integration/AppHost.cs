// <apphost>
var builder = DistributedApplication.CreateBuilder(args);

// Add Kubernetes environment
var k8s = builder.AddKubernetesEnvironment("k8s");

// Add your application resources
var cache = builder.AddRedis("cache");

var apiService = builder.AddProject<Projects.ApiService>("apiservice")
                        .WithReference(cache);

builder.AddProject<Projects.Web>("webfrontend")
       .WithExternalHttpEndpoints()
       .WithReference(cache)
       .WithReference(apiService);


builder.Build().Run();
// </apphost>

namespace Projects
{
    public class ApiService : IProjectMetadata { string IProjectMetadata.ProjectPath => "."; }
}
