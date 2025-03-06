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

                signalRService.Kind = CosmosDBAccountKind.MongoDB;
                signalRService.ConsistencyPolicy = new()
                {
                    DefaultConsistencyLevel = DefaultConsistencyLevel.Strong,
                };
                signalRService.Tags.Add("ExampleKey", "Example value");
            });
        // </configure>
    }
}
