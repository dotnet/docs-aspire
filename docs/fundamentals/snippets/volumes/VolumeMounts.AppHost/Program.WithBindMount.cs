internal partial class Program
{
    private static IResourceBuilder<SqlServerDatabaseResource> WithDataMount(IDistributedApplicationBuilder builder)
    {
        // <mount>
        var sql = builder.AddSqlServer("sql")
                         .WithBindMount("VolumeMount.AppHost-sql-data", "/var/opt/mssql")
                         .AddDatabase("sqldb");
        // </mount>

        return sql;
    }
}
