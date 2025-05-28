internal partial class Program
{
    private static IResourceBuilder<SqlServerDatabaseResource> WithVolume(IDistributedApplicationBuilder builder)
    {
        // <volume>
        var sql = builder.AddSqlServer("sql")
                         .WithVolume(target: "/var/opt/mssql")
                         .AddDatabase("sqldb");
        // </volume>

        return sql;
    }
}
