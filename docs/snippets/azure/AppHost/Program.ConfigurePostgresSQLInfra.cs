using Azure.Provisioning.PostgreSql;

internal static partial class Program
{
    public static void ConfigurePostgreSQLInfra(IDistributedApplicationBuilder builder)
    {
        // <configure>
        builder.AddAzurePostgresFlexibleServer("postgres")
            .ConfigureInfrastructure(infra =>
            {
                var flexibleServer = infra.GetProvisionableResources()
                                          .OfType<PostgreSqlFlexibleServer>()
                                          .Single();

                flexibleServer.Sku = new PostgreSqlFlexibleServerSku
                {
                    Tier = PostgreSqlFlexibleServerSkuTier.Burstable,
                };
                flexibleServer.HighAvailability = new PostgreSqlFlexibleServerHighAvailability
                {
                    Mode = PostgreSqlFlexibleServerHighAvailabilityMode.ZoneRedundant,
                    StandbyAvailabilityZone = "2",
                };
                flexibleServer.Tags.Add("ExampleKey", "Example value");
            });
        // </configure>
    }
}
