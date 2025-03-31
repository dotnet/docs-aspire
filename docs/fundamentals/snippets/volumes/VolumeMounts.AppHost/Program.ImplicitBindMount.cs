internal partial class Program
{
    private static IResourceBuilder<SqlServerDatabaseResource> ImplicitBindMount(IDistributedApplicationBuilder builder)
    {
        // <implicitbindmount>
        var sql = builder.AddSqlServer("sql")
                         .WithDataBindMount(source: @"C:\SqlServer\Data")
                         .AddDatabase("sqldb");
        // </implicitbindmount>

        return sql;
    }
}
