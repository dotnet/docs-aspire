---
title: .NET Aspire SqlServer Entity Framework Core component
description: This article describes the .NET Aspire SQL Server Entity Framework Core component.
ms.topic: how-to
ms.date: 01/22/2024
---

# .NET Aspire SqlServer Entity Framework Core component

In this article, you learn how to use the .NET Aspire SqlServer Entity Framework Core component. The `Aspire.Microsoft.EntityFrameworkCore.SqlServer` library is used to:

- Registers [EntityFrameworkCore](/ef/core/) <xref:Microsoft.EntityFrameworkCore.DbContext> service for connecting to a SQL database.
- Automatically configures the following:
  - Connection pooling to efficiently managed HTTP requests and database connections
  - Health checks, logging and telemetry to improve app monitoring and diagnostics

## Prerequisites

- SQL database and connection string for accessing the database.

## Get started

To get started with the .NET Aspire SQL Server Entity Framework Core component, install the [Aspire.Microsoft.EntityFrameworkCore.SqlServer](https://www.nuget.org/packages/Aspire.Microsoft.EntityFrameworkCore.SqlServer) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Microsoft.EntityFrameworkCore.SqlServer --prerelease
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Microsoft.EntityFrameworkCore.SqlServer"
                  Version="[SelectVersion]" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

In the _Program.cs_ file of your component-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireSqlServerEFCoreSqlClientExtensions.AddSqlServerDbContext%2A> extension to register a `DbContext` for use via the dependency injection container.

```csharp
builder.AddSqlServerDbContext<YourDbContext>("sqldb");
```

To retrieve `YourDbContext` object from a service:

```csharp
public class ExampleService(YourDbContext client)
{
    // Use client...
}
```

## App host usage

In your app host project, register a SqlServer database and consume the connection using the following methods, such as <xref:Aspire.Hosting.SqlServerBuilderExtensions.AddSqlServer%2A>:

```csharp
var sql = builder.AddSqlServer("sql");
var sqldb = sql.AddDatabase("sqldb");

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(sqldb);
```

## Configuration

The .NET Aspire SQL Server Entity Framework Core component provides multiple configuration approaches and options to meet the requirements and conventions of your project.

### Use configuration providers

The .NET Aspire SQL Server Entity Framework Core component supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the <xref:Aspire.Microsoft.EntityFrameworkCore.SqlServer.MicrosoftEntityFrameworkCoreSqlServerSettings> from configuration files such as _appsettings.json_ by using the `Aspire:Microsoft:EntityFrameworkCore:SqlServer` key. If you have set up your configurations in the `Aspire:Microsoft:EntityFrameworkCore:SqlServer` section you can just call the method without passing any parameter.

The following is an example of an _appsettings.json_ file that configures some of the available options:

```json
{
  "Aspire": {
    "Microsoft": {
      "EntityFrameworkCore": {
        "SqlServer": {
          "ConnectionString": "YOUR_CONNECTIONSTRING",
          "DbContextPooling": true,
          "HealthChecks": false,
          "Tracing": false,
          "Metrics": true
        }
      }
    }
  }
}
```

### Use inline configurations

You can also pass the `Action<MicrosoftEntityFrameworkCoreSqlServerSettings>` delegate to set up some or all the options inline, for example to turn off the `Metrics`:

```csharp
builder.AddSqlServerDbContext<YourDbContext>(
    "sql",
    static settings =>
        settings.ConnectionString = "YOUR_CONNECTIONSTRING");
```

### Configure multiple DbContext connections

If you want to register more than one `DbContext` with different configuration, you can use `$"Aspire.Microsoft.EntityFrameworkCore.SqlServer:{typeof(TContext).Name}"` configuration section name. The json configuration would look like:

```json
{
  "Aspire": {
    "Microsoft": {
      "EntityFrameworkCore": {
          "SqlServer": {
            "ConnectionString": "YOUR_CONNECTIONSTRING",
            "DbContextPooling": true,
            "HealthChecks": false,
            "Tracing": false,
            "Metrics": true,
          "AnotherDbContext": {
            "ConnectionString": "AnotherDbContext_CONNECTIONSTRING",
            "Tracing": true
          }
        }
      }
    }
  }
}
```

Then calling the `AddSqlServerDbContext` method with `AnotherDbContext` type parameter would load the settings from `Aspire:Microsoft:EntityFrameworkCore:SqlServer:AnotherDbContext` section.

```csharp
builder.AddSqlServerDbContext<AnotherDbContext>("another-sql");
```

### Configuration options

Here are the configurable options with corresponding default values:

| Name | Description |
|--|--|
| `ConnectionString` | The connection string of the SQL Server database to connect to. |
| `DbContextPooling` | A boolean value that indicates whether the db context will be pooled or explicitly created every time it's requested |
| `MaxRetryCount` | The maximum number of retry attempts. Default value is 6, set it to 0 to disable the retry mechanism. |
| `HealthChecks` | A boolean value that indicates whether the database health check is enabled or not. |
| `Tracing` | A boolean value that indicates whether the OpenTelemetry tracing is enabled or not. |
| `Metrics` | A boolean value that indicates whether the OpenTelemetry metrics are enabled or not. |
| `Timeout` | The time in seconds to wait for the command to execute. |

[!INCLUDE [component-health-checks](../includes/component-health-checks.md)]

By default, the .NET Aspire Sql Server Entity Framework Core component handles the following:

- Adds the [`DbContextHealthCheck`](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/blob/master/src/HealthChecks.NpgSql/NpgSqlHealthCheck.cs), which calls EF Core's <xref:Microsoft.EntityFrameworkCore.Storage.IDatabaseCreator.CanConnectAsync%2A> method. The name of the health check is the name of the `TContext` type.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic

[!INCLUDE [component-observability-and-telemetry](../includes/component-observability-and-telemetry.md)]

### Logging

The .NET Aspire SQL Server Entity Framework Core component uses the following Log categories:

- `Microsoft.EntityFrameworkCore.ChangeTracking`
- `Microsoft.EntityFrameworkCore.Database.Command`
- `Microsoft.EntityFrameworkCore.Database.Connection`
- `Microsoft.EntityFrameworkCore.Database.Transaction`
- `Microsoft.EntityFrameworkCore.Infrastructure`
- `Microsoft.EntityFrameworkCore.Infrastructure`
- `Microsoft.EntityFrameworkCore.Migrations`
- `Microsoft.EntityFrameworkCore.Model`
- `Microsoft.EntityFrameworkCore.Model.Validation`
- `Microsoft.EntityFrameworkCore.Query`
- `Microsoft.EntityFrameworkCore.Update`

### Tracing

The .NET Aspire SQL Server Entity Framework Core component will emit the following Tracing activities using OpenTelemetry:

- "OpenTelemetry.Instrumentation.EntityFrameworkCore"

### Metrics

The .NET Aspire SQL Server Entity Framework Core component will emit the following metrics using OpenTelemetry:

- Microsoft.EntityFrameworkCore:
  - `ec_Microsoft_EntityFrameworkCore_active_db_contexts`
  - `ec_Microsoft_EntityFrameworkCore_total_queries`
  - `ec_Microsoft_EntityFrameworkCore_queries_per_second`
  - `ec_Microsoft_EntityFrameworkCore_total_save_changes`
  - `ec_Microsoft_EntityFrameworkCore_save_changes_per_second`
  - `ec_Microsoft_EntityFrameworkCore_compiled_query_cache_hit_rate`
  - `ec_Microsoft_Entity_total_execution_strategy_operation_failures`
  - `ec_Microsoft_E_execution_strategy_operation_failures_per_second`
  - `ec_Microsoft_EntityFramew_total_optimistic_concurrency_failures`
  - `ec_Microsoft_EntityF_optimistic_concurrency_failures_per_second`

- [Azure SQL Database documentation](/azure/azure-sql/)
- [.NET Aspire components](../fundamentals/components-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
