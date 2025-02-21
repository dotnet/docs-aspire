using Azure.Provisioning.EventHubs;

internal static partial class Program
{
    public static void ConfigureEventHubsInfra(IDistributedApplicationBuilder builder)
    {
        // <configure>
        builder.AddAzureEventHubs("event-hubs")
            .ConfigureInfrastructure(infra =>
            {
                var eventHubs = infra.GetProvisionableResources()
                                     .OfType<EventHubsNamespace>()
                                     .Single();

                eventHubs.Sku = new EventHubsSku()
                {
                    Name = EventHubsSkuName.Premium,
                    Tier = EventHubsSkuTier.Premium,
                    Capacity = 7,
                };
                eventHubs.PublicNetworkAccess = EventHubsPublicNetworkAccess.SecuredByPerimeter;
                eventHubs.Tags.Add("ExampleKey", "Example value");
            });
        // </configure>
    }
}
