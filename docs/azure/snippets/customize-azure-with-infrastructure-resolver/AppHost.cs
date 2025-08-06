using Aspire.Hosting.Azure;
using Azure.Provisioning;
using Azure.Provisioning.Primitives;
using Azure.Provisioning.CosmosDB;
using Microsoft.Extensions.DependencyInjection;

// <configure-azure-options>
var builder = DistributedApplication.CreateBuilder(args);

builder.Services.Configure<AzureProvisioningOptions>(options =>
{
    options.ProvisioningBuildOptions.InfrastructureResolvers.Insert(0, new FixedNameInfrastructureResolver());
});

builder.Build().Run();
// </configure-azure-options>

// <infrastructure-resolver>
internal sealed class FixedNameInfrastructureResolver : InfrastructureResolver
{
    public override void ResolveProperties(ProvisionableConstruct construct, ProvisioningBuildOptions options)
    {
        if (construct is CosmosDBAccount account)
        {
            account.Name = "ContosoCosmosDb";
        }

        base.ResolveProperties(construct, options);
    }
}
// </infrastructure-resolver>
