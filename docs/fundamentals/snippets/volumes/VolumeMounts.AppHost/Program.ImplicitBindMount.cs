internal partial class Program
{
    private static IResourceBuilder<SqlServerDatabaseResource> ImplicitBindMount(IDistributedApplicationBuilder builder)
    {
        // <implicit>
        var sql = builder.AddSqlServer("sql")
                         .WithDataBindMount(source: @"C:\SqlServer\Data")
                         .AddDatabase("sqldb");
        // </implicit>

        return sql;
    }
}
