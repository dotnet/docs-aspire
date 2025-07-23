using Examples.ContainerBuilding;

var builder = DistributedApplication.CreateBuilder(args);

// Add some example services that will need container images
// Note: In a real scenario, these would reference actual project files
var apiService = builder.AddProject<Projects.ApiService>("api")
    .WithEnvironment("ASPNETCORE_ENVIRONMENT", "Production");

var webApp = builder.AddProject<Projects.WebApp>("webapp")
    .WithReference(apiService)
    .WithEnvironment("API_BASE_URL", apiService.GetEndpoint("https"));

// Add a database that the API will use
var database = builder.AddPostgres("postgres")
    .WithDataVolume()
    .AddDatabase("appdb");

apiService.WithReference(database);

// Add the custom compute environment resource that demonstrates 
// container image building and progress reporting during publish/deploy
builder.AddComputeEnvironment("compute-env");

builder.Build().Run();