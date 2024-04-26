internal partial class Program
{
    private static IResourceBuilder<SqlServerDatabaseResource> WithDataMount(IDistributedApplicationBuilder builder)
    {
        // <with-data-mount>
        var sql = builder.AddSqlServer("sql")
                         .WithBindMount("VolumeMount.AppHost-sql-data", "/var/opt/mssql")
                         .AddDatabase("sqldb");
        // </with-data-mount>

        return sql;
    }
}
