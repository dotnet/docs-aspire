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
                var storageAccount = infra.GetProvisionableResources()
                                          .OfType<StorageAccount>()
                                          .Single();

                storageAccount.AccessTier = StorageAccountAccessTier.Cool;
                storageAccount.Sku = new StorageSku { Name = StorageSkuName.PremiumZrs };
                storageAccount.Tags.Add("ExampleKey", "Example value");
            });
        // </configure>
    }
}
