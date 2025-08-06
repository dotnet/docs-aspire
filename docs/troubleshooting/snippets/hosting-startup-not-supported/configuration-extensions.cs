// DatabaseConfiguration.cs
public static class DatabaseConfiguration
{
    public static IHostApplicationBuilder AddDatabaseServices(
        this IHostApplicationBuilder builder)
    {
        // Configure your database based on environment or configuration
        var connectionName = builder.Configuration["DatabaseProvider"] switch
        {
            "PostgreSQL" => "postgres",
            "SqlServer" => "sqlserver",
            _ => throw new InvalidOperationException("Unsupported database provider")
        };

        builder.AddNpgsqlDbContext<MyDbContext>(connectionName);
        
        return builder;
    }
}

// Program.cs
var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.AddDatabaseServices(); // Your modular configuration
var app = builder.Build();