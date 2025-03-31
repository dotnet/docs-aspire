internal partial class Program
{
    private static IResourceBuilder<SqlServerDatabaseResource> WithBindMount(IDistributedApplicationBuilder builder)
    {
        // <bindmount>
        var sql = builder.AddSqlServer("sql")
                         .WithBindMount(source: @"C:\SqlServer\Data", target: "/var/opt/mssql")
                         .AddDatabase("sqldb");
        // </bindmount>

        return sql;
    }
}
