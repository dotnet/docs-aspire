using Azure.Provisioning.WebPubSub;

internal static partial class Program
{
    public static void ConfigureWebPubSubInfra(IDistributedApplicationBuilder builder)
    {
        // <configure>
        builder.AddAzureWebPubSub("web-pubsub")
            .ConfigureInfrastructure(infra =>
            {
                var webPubSubService = infra.GetProvisionableResources()
                                            .OfType<WebPubSubService>()
                                            .Single();

                webPubSubService.Sku.Name = "Standard_S5";
                webPubSubService.Sku.Capacity = 5;
                webPubSubService.Tags.Add("ExampleKey", "Example value");
            });
        // </configure>
    }
}
