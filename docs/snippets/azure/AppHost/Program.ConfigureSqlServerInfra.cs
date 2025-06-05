using Azure.Provisioning.Sql;

internal static partial class Program
{
    public static void ConfigureSqlServerInfra(IDistributedApplicationBuilder builder)
    {
        // <configure>
        var sql = builder.AddAzureSqlServer("sql-server")
            .ConfigureInfrastructure(infra =>
            {
                var sqlServer = infra.GetProvisionableResources()
                                    .OfType<SqlServer>()
                                    .Single();

                sqlServer.Tags.Add("Owner", "TeamAlpha");
                sqlServer.Tags.Add("Environment", "Production");
                sqlServer.Tags.Add("CostCenter", "Engineering");
            });

        // Configure database with additional tags
        sql.AddDatabase("inventory-db")
            .ConfigureInfrastructure(infra =>
            {
                var database = infra.GetProvisionableResources()
                                   .OfType<SqlDatabase>()
                                   .Single();

                database.Tags.Add("Owner", "me");
                database.Tags.Add("DataClassification", "Internal");
            });
        // </configure>
    }
}