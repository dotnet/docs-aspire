using Azure.Provisioning.KeyVault;

internal static partial class Program
{
    public static void ConfigureKeyVaultInfra(IDistributedApplicationBuilder builder)
    {
        // <configure>
        builder.AddAzureKeyVault("key-vault")
            .ConfigureInfrastructure(infra =>
            {
                var keyVault = infra.GetProvisionableResources()
                                    .OfType<KeyVaultService>()
                                    .Single();

                keyVault.Properties.Sku = new()
                {
                    Family = KeyVaultSkuFamily.A,
                    Name = KeyVaultSkuName.Premium,
                };
                keyVault.Properties.EnableRbacAuthorization = true;
                keyVault.Tags.Add("ExampleKey", "Example value");
            });
        // </configure>
    }
}
