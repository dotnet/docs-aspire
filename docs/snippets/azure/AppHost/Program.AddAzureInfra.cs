using Aspire.Hosting.Azure;
using Azure.Provisioning;
using Azure.Provisioning.ContainerRegistry;

internal static partial class Program
{
    public static void AddAzureInfrastructure(IDistributedApplicationBuilder builder)
    {
        // <add>
        var acr = builder.AddAzureInfrastructure("acr", infra =>
        {
            var registry = new ContainerRegistryService("acr")
            {
                Sku = new()
                {
                    Name = ContainerRegistrySkuName.Standard
                },
            };
            infra.Add(registry);

            var output = new ProvisioningOutput("registryName", typeof(string))
            {
                Value = registry.Name
            };
            infra.Add(output);
        });

        builder.AddProject<Projects.WorkerService>("worker")
               .WithEnvironment(
                    "ACR_REGISTRY_NAME",
                    new BicepOutputReference("registryName", acr.Resource));
        // </add>
    }
}
