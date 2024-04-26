internal partial class Program
{
    private static IResourceBuilder<SqlServerDatabaseResource> ExplicitStable(IDistributedApplicationBuilder builder)
    {
        // <explicit-stable>
        var sqlPassword = builder.AddParameter("sql-password", secret: true);

        var sql = builder.AddSqlServer("sql", password: sqlPassword)
                         .WithDataVolume()
                         .AddDatabase("sqldb");
        // </explicit-stable>

        return sql;
    }
}
