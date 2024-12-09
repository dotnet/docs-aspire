using Azure.Provisioning.Storage;
using Azure.Core;

internal static partial class Program
{
    public static void ConfigureStorageInfra(IDistributedApplicationBuilder builder)
    {
        // <configure>
        builder.AddAzureStorage("storage")
            .ConfigureInfrastructure(infra =>
            {
                var resources = infra.GetProvisionableResources();

                var storageAccount = resources.OfType<StorageAccount>()
                                              .FirstOrDefault(r => r.BicepIdentifier is "storage")
                    ?? throw new InvalidOperationException("""
                        Could not find configured storage account with name 'storage'
                        """);

                storageAccount.AccessTier = StorageAccountAccessTier.Cool;
                storageAccount.Sku = new StorageSku { Name = StorageSkuName.PremiumZrs };
                storageAccount.Location = AzureLocation.CentralUS;
                storageAccount.Tags.Add("Environment", "Production");
                storageAccount.EnableHttpsTrafficOnly = true;
            });
        // </configure>
    }
}
