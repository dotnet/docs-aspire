internal partial class Program
{
    private static IResourceBuilder<SqlServerDatabaseResource> ImplicitVolume(IDistributedApplicationBuilder builder)
    {
        // <implicit>
        var sql = builder.AddSqlServer("sql")
                         .WithDataVolume()
                         .AddDatabase("sqldb");
        // </implicit>

        return sql;
    }
}
