// <apphost>
var builder = DistributedApplication.CreateBuilder(args);

// Add a Docker Compose environment
var compose = builder.AddDockerComposeEnvironment("compose");

// Add your services to the Docker Compose environment
var cache = builder.AddRedis("cache")
                   .PublishAsDockerComposeService();

var apiService = builder.AddProject<Projects.ApiService>("apiservice")
                        .PublishAsDockerComposeService();

var webApp = builder.AddProject<Projects.WebApp>("webapp")
                    .WithReference(cache)
                    .WithReference(apiService)
                    .PublishAsDockerComposeService();

builder.Build().Run();
// </apphost>

namespace Projects
{
    public class WebApp : IProjectMetadata { string IProjectMetadata.ProjectPath => "."; }
    public class ApiService : IProjectMetadata { string IProjectMetadata.ProjectPath => "."; }
}
