using Azure.Provisioning.CognitiveServices;

internal static partial class Program
{
    public static void ConfigureAIFoundryInfra(IDistributedApplicationBuilder builder)
    {
        // <configure>
        builder.AddAzureAIFoundry("foundry")
            .ConfigureInfrastructure(infra =>
            {
                var resources = infra.GetProvisionableResources();
                var account = resources.OfType<CognitiveServicesAccount>().Single();

                account.Sku = new CognitiveServicesSku
                {
                    Tier = CognitiveServicesSkuTier.Enterprise,
                    Name = "E0"
                };
                account.Tags.Add("ExampleKey", "Example value");
            });
        // </configure>
    }
}