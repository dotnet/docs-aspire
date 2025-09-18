using Azure.Provisioning.AppContainers;

var builder = DistributedApplication.CreateBuilder(args);

var acaEnv = builder.AddAzureContainerAppEnvironment("my-container-env");

acaEnv.ConfigureInfrastructure(config =>
{
    var resources = config.GetProvisionableResources();
    var containerEnvironment = resources.OfType<ContainerAppManagedEnvironment>().FirstOrDefault();

    // Set a custom name for the environment
    containerEnvironment.Name = "my-custom-aca-environment";
    
    // Set the location
    containerEnvironment.Location = "East US";
    
    // Add tags for metadata and organization
    containerEnvironment.Tags.Add("Environment", "Production");
    containerEnvironment.Tags.Add("Project", "MyAspireApp");
    containerEnvironment.Tags.Add("Owner", "Development Team");
});

// Omitted for brevity...

builder.Build().Run();