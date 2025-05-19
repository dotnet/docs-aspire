using Azure.Provisioning.ContainerRegistry;

internal static partial class Program
{
    internal static void ReferenceExisting(string[] args)
    {
        // <existing>
        var builder = DistributedApplication.CreateBuilder(args);

        var registryName = builder.AddParameter("registryName");
        var rgName = builder.AddParameter("rgName");

        // Add (or reference) the registry
        var acr = builder.AddAzureContainerRegistry("my-acr")
                         .PublishAsExisting(registryName, rgName);

        // Wire an environment to that registry
        builder.AddAzureContainerAppEnvironment("env")
               .WithAzureContainerRegistry(acr);

        builder.Build().Run();
        // </existing>
    }
}