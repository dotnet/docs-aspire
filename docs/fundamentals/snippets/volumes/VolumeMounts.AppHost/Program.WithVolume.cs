internal partial class Program
{
    private static IResourceBuilder<SqlServerDatabaseResource> WithVolume(IDistributedApplicationBuilder builder)
    {
        // <mount>
        var sql = builder.AddSqlServer("sql")
                         .WithVolume(target: "/var/opt/mssql")
                         .AddDatabase("sqldb");
        // </mount>

        return sql;
    }
}
