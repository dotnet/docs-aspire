using Azure.Provisioning.Storage;

internal static partial class Program
{
    public static void ConfigureInfrastructure(IDistributedApplicationBuilder builder)
    {
        // <infra>
        var sku = builder.AddParameter("storage-sku");

        var storage = builder.AddAzureStorage("storage")
            .ConfigureInfrastructure(infra =>
            {
                var resources = infra.GetProvisionableResources();

                var storageAccount = resources.OfType<StorageAccount>().Single();

                storageAccount.Sku = new StorageSku
                {
                    Name = sku.AsProvisioningParameter(infra)
                };
            });
        // </infra>
    }
}
