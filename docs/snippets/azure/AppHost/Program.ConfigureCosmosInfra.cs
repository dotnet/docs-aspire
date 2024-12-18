using Azure.Provisioning.CosmosDB;

internal static partial class Program
{
    public static void ConfigureCosmosInfra(IDistributedApplicationBuilder builder)
    {
        // <configure>
        builder.AddAzureCosmosDB("cosmos-db")
            .ConfigureInfrastructure(infra =>
            {
                var cosmosDbAccount = infra.GetProvisionableResources()
                                           .OfType<CosmosDBAccount>()
                                           .Single();

                cosmosDbAccount.Kind = CosmosDBAccountKind.MongoDB;
                cosmosDbAccount.ConsistencyPolicy = new()
                {
                    DefaultConsistencyLevel = DefaultConsistencyLevel.Strong,
                };
                cosmosDbAccount.Cors.Add(new CosmosDBAccountCorsPolicy()
                {
                    AllowedOrigins = "*",
                    AllowedMethods = "GET,POST",
                    AllowedHeaders = "*"
                });
                cosmosDbAccount.Tags.Add("ExampleKey", "Example value");
            });
        // </configure>
    }
}
