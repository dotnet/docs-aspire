var builder = DistributedApplication.CreateBuilder(args);

// By calling this API, you register that ACA environment as infrastructure 
// the AppHost owns. Any project or container resources you later publish 
// as Azure Container Apps automatically attach to this environment.
builder.AddAzureContainerAppEnvironment("aca-env");

// Omitted for brevity...but this is where you would add containers and/or 
// project resources.

builder.Build().Run();
