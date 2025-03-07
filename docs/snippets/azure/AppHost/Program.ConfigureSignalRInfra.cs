using Azure.Provisioning.SignalR;

internal static partial class Program
{
    public static void ConfigureSignalRInfra(IDistributedApplicationBuilder builder)
    {
        // <configure>
        builder.AddAzureSignalR("signalr")
            .ConfigureInfrastructure(infra =>
            {
                var signalRService = infra.GetProvisionableResources()
                                          .OfType<SignalRService>()
                                          .Single();

                signalRService.Sku.Name = "Premium_P1";
                signalRService.Sku.Capacity = 10;
                signalRService.PublicNetworkAccess = "Enabled";
                signalRService.Tags.Add("ExampleKey", "Example value");
            });
        // </configure>
    }
}
