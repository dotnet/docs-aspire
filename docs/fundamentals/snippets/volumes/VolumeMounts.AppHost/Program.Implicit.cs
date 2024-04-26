internal partial class Program
{
    private static IResourceBuilder<SqlServerDatabaseResource> Implicit(IDistributedApplicationBuilder builder)
    {
        // <implicit>
        var sql = builder.AddSqlServer("sql")
                         .WithDataVolume()
                         .AddDatabase("sqldb");
        // </implicit>

        return sql;
    }
}
