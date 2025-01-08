using Azure.Provisioning.ServiceBus;

internal static partial class Program
{
    public static void ConfigureServiceBusInfra(IDistributedApplicationBuilder builder)
    {
        // <configure>
        builder.AddAzureServiceBus("service-bus")
            .ConfigureInfrastructure(infra =>
            {
                var serviceBusNamespace = infra.GetProvisionableResources()
                                               .OfType<ServiceBusNamespace>()
                                               .Single();

                serviceBusNamespace.Sku = new ServiceBusSku
                {
                    Tier = ServiceBusSkuTier.Premium
                };
                serviceBusNamespace.Tags.Add("ExampleKey", "Example value");
            });
        // </configure>
    }
}
