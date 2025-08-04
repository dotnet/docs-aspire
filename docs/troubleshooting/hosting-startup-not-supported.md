---
title: HostingStartup is not supported with .NET Aspire integrations
description: Learn how to migrate from HostingStartup to the IHostApplicationBuilder pattern for use with .NET Aspire integrations.
ms.date: 08/04/2025
---

# HostingStartup is not supported with .NET Aspire integrations

.NET Aspire integrations require the use of <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder>, but `HostingStartup` only provides access to <xref:Microsoft.AspNetCore.Hosting.IWebHostBuilder>. This fundamental incompatibility means that you can't configure .NET Aspire integrations from within a `HostingStartup` implementation.

## Symptoms

When attempting to use .NET Aspire integrations within a HostingStartup implementation, you might encounter:

- **Compilation errors**: Aspire integration extension methods like `AddNpgsqlDbContext` or `AddRedis` are not available on `IWebHostBuilder`.
- **Runtime configuration issues**: Even if you access the underlying services, the proper configuration and service registration won't occur.
- **Missing telemetry and resilience**: Aspire's built-in observability, health checks, and resilience patterns won't be applied.

## Why HostingStartup doesn't work with .NET Aspire

.NET Aspire integrations extend <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to provide:

- Standardized configuration patterns
- Built-in health checks
- Telemetry and observability
- Resilience patterns
- Service discovery integration

The `HostingStartup` feature was designed for the older ASP.NET Core hosting model and only provides access to <xref:Microsoft.AspNetCore.Hosting.IWebHostBuilder>, which doesn't include these modern hosting capabilities.

## Migration strategies

### Option 1: Use IHostApplicationBuilder directly (Recommended)

Instead of using `HostingStartup`, configure your application directly in the `Program.cs` file using the modern hosting pattern:

**Before (HostingStartup pattern):**

```csharp
// MyDatabaseStartup.cs
public class MyDatabaseStartup : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // This won't work with Aspire integrations
            services.AddDbContext<MyDbContext>(options =>
                options.UseNpgsql(connectionString));
        });
    }
}
```

**After (IHostApplicationBuilder pattern):**

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Add service defaults first
builder.AddServiceDefaults();

// Now you can use Aspire integrations
builder.AddNpgsqlDbContext<MyDbContext>("postgres");

var app = builder.Build();

app.MapDefaultEndpoints();
app.Run();
```

### Option 2: Create configuration extensions

If you need modular configuration, create extension methods that work with `IHostApplicationBuilder`:

```csharp
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
```

### Option 3: Use feature flags or configuration-based service registration

For conditional service registration based on configuration:

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

// Conditional service registration based on configuration
var databaseProvider = builder.Configuration["DatabaseProvider"];
switch (databaseProvider)
{
    case "PostgreSQL":
        builder.AddNpgsqlDbContext<MyDbContext>("postgres");
        break;
    case "SqlServer":
        builder.AddSqlServerDbContext<MyDbContext>("sqlserver");
        break;
    default:
        throw new InvalidOperationException($"Unsupported database provider: {databaseProvider}");
}

var telemetryProvider = builder.Configuration["TelemetryProvider"];
switch (telemetryProvider)
{
    case "ApplicationInsights":
        builder.Services.AddApplicationInsightsTelemetry();
        break;
    case "OpenTelemetry":
        // OpenTelemetry is included with service defaults
        break;
}

var app = builder.Build();
```

### Option 4: Use dependency injection for plugin architecture

For more complex plugin scenarios, use dependency injection with interfaces:

```csharp
// IServicePlugin.cs
public interface IServicePlugin
{
    void ConfigureServices(IHostApplicationBuilder builder);
}

// DatabasePlugin.cs
public class DatabasePlugin : IServicePlugin
{
    public void ConfigureServices(IHostApplicationBuilder builder)
    {
        builder.AddNpgsqlDbContext<MyDbContext>("postgres");
    }
}

// Program.cs
var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();

// Register plugins
var plugins = new List<IServicePlugin>
{
    new DatabasePlugin(),
    // Add other plugins as needed
};

foreach (var plugin in plugins)
{
    plugin.ConfigureServices(builder);
}

var app = builder.Build();
```

## Best practices for modular configuration

1. **Use configuration-based decisions**: Instead of having separate startup classes, use configuration values to determine which services to register.

2. **Create extension methods**: Group related service registrations into extension methods on `IHostApplicationBuilder`.

3. **Leverage service defaults**: Always call `builder.AddServiceDefaults()` to get the full benefits of .NET Aspire's built-in features.

4. **Use the app host for orchestration**: For development scenarios, use the [.NET Aspire app host](../fundamentals/app-host-overview.md) to manage dependencies and configuration.

## Additional considerations

- **Service discovery**: .NET Aspire integrations automatically configure service discovery. If you were using HostingStartup for service-to-service communication, consider using Aspire's [service discovery features](../service-discovery/overview.md).

- **Configuration management**: Instead of hard-coding connection strings in HostingStartup, use .NET Aspire's configuration patterns with connection string names that map to resources in your app host.

- **Testing**: .NET Aspire provides [testing capabilities](../testing/overview.md) that work with the new hosting model.

For more information about .NET Aspire integrations and the hosting model, see [.NET Aspire integrations overview](../fundamentals/integrations-overview.md).