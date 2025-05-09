using Azure.Provisioning.ContainerRegistry;

var builder = DistributedApplication.CreateBuilder(args);

// Add (or reference) the registry
var acr = builder.AddAzureContainerRegistry("my-acr");

// Wire an environment to that registry
builder.AddAzureContainerAppEnvironment("env")
       .WithAzureContainerRegistry(acr);

// (Optional) let a service push images
builder.AddProject("api", "../Api/Api.csproj")
       .WithRoleAssignments(acr, ContainerRegistryBuiltInRole.AcrPush);

builder.Build().Run();
