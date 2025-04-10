internal partial class Program
{
    private static IResourceBuilder<SqlServerDatabaseResource> ImplicitVolume(IDistributedApplicationBuilder builder)
    {
        // <implicitvolume>
        var sql = builder.AddSqlServer("sql")
                         .WithDataVolume()
                         .AddDatabase("sqldb");
        // </implicitvolume>

        return sql;
    }
}
