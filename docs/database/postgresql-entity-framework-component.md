---
title: .NET Aspire PostgreSQL Entity Framework Core component
description: This article describes the .NET Aspire PostgreSQL Entity Framework Core component.
ms.topic: how-to
ms.date: 01/22/2024
---

# .NET Aspire PostgreSQL Entity Framework Core component

In this article, you learn how to use the .NET Aspire PostgreSQL Entity Framework Core component. The `Aspire.Npgsql.EntityFrameworkCore.PostgreSQL` library is used to register a <xref:Microsoft.EntityFrameworkCore.DbContext> service for connecting to a PostgreSQL database. It also enables corresponding health checks, logging and telemetry.

PostgreSQL is a powerful, open source, object-relational database system. The .NET Aspire PostgreSQL Entity Framework component streamlines essential database context and connection configurations for you by handling the following concerns:

- Registers [EntityFrameworkCore](/ef/core/)  in the DI container for connecting to PostgreSQL database.
- Automatically configures the following:
  - Connection pooling to efficiently managed HTTP requests and database connections
  - Automatic retries to increase app resiliency
  - Health checks, logging and telemetry to improve app monitoring and diagnostics

## Prerequisites

- Azure subscription: [create one for free](https://azure.microsoft.com/free/)
- Azure Postgresql Database: learn more about how to [create an Azure Database for PostgreSQL](/azure/postgresql/flexible-server/quickstart-create-server-portal).

## Get started

To get started with the .NET Aspire PostgreSQL Entity Framework Core component, install the [Aspire.Npgsql.EntityFrameworkCore.PostgreSQL](https://www.nuget.org/packages/Aspire.Npgsql.EntityFrameworkCore.PostgreSQL) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Npgsql.EntityFrameworkCore.PostgreSQL --prerelease
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Npgsql.EntityFrameworkCore.PostgreSQL"
                  Version="[SelectVersion]" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

In the _Program.cs_ file of your component-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireEFPostgreSqlExtensions.AddNpgsqlDbContext%2A> extension to register a <xref:System.Data.Entity.DbContext> for use via the dependency injection container.

```csharp
builder.AddNpgsqlDbContext<YourDbContext>("postgresdb");
```

You can then retrieve the `YourDbContext` instance using dependency injection. For example, to retrieve the client from a service:

```csharp
public class ExampleService(YourDbContext context)
{
    // Use context...
}
```

## App host usage

In your app host project, register the PostgreSQL component and consume the `DbContext` using the following methods:

```csharp
var postgres = builder.AddPostgres("postgres");
var postgresdb = postgres.AddDatabase("postgresdb");

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(postgresdb);
```

## Configuration

The .NET Aspire PostgreSQL Entity Framework Core component provides multiple configuration approaches and options to meet the requirements and conventions of your project.

### Use configuration providers

The .NET Aspire PostgreSQL Entity Framework Core component supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the <xref:Aspire.Npgsql.EntityFrameworkCore.PostgreSQL.NpgsqlEntityFrameworkCorePostgreSQLSettings> from configuration files such as _appsettings.json_ by using the `Aspire:Npgsql:EntityFrameworkCore:PostgreSQL` key. If you have set up your configurations in the `Aspire:Npgsql:EntityFrameworkCore:PostgreSQL` section you can just call the method without passing any parameter.

The following example shows an _appsettings.json_ file that configures some of the available options:

```json
{
  "Aspire": {
    "Npgsql": {
      "EntityFrameworkCore": {
        "PostgreSQL": {
          "ConnectionString": "YOUR_CONNECTIONSTRING",
          "DbContextPooling": true,
          "DisableHealthChecks": true,
          "DisableTracing": true
        }
      }
    }
  }
}
```

### Use inline delegates

You can also pass the `Action<NpgsqlEntityFrameworkCorePostgreSQLSettings>` delegate to set up some or all the options inline, for example to set the `ConnectionString`:

```csharp
builder.AddNpgsqlDbContext<YourDbContext>(
    "db",
    static settings => settings.ConnectionString = "YOUR_CONNECTIONSTRING");
```

### Configure multiple DBContext classes

If you want to register more than one <xref:Microsoft.EntityFrameworkCore.DbContext> with different configuration, you can use `$"Aspire:Npgsql:EntityFrameworkCore:PostgreSQL:{typeof(TContext).Name}"` configuration section name. The json configuration would look like:

```json
{
  "Aspire": {
    "Npgsql": {
      "EntityFrameworkCore": {
        "PostgreSQL": {
          "ConnectionString": "YOUR_CONNECTIONSTRING",
          "DbContextPooling": true,
          "DisableHealthChecks": true,
          "DisableTracing": true,
          "AnotherDbContext": {
            "ConnectionString": "AnotherDbContext_CONNECTIONSTRING",
            "DisableTracing": false
          }
        }
      }
    }
  }
}
```

Then calling the <xref:Microsoft.Extensions.Hosting.AspireEFPostgreSqlExtensions.AddNpgsqlDbContext%2A> method with `AnotherDbContext` type parameter would load the settings from `Aspire:Npgsql:EntityFrameworkCore:PostgreSQL:AnotherDbContext` section.

```csharp
builder.AddNpgsqlDbContext<AnotherDbContext>();
```

### Configuration options

Here are the configurable options with corresponding default values:

| Name                  | Description                                                                                            |
|-----------------------|--------------------------------------------------------------------------------------------------------|
| `ConnectionString`    | The connection string of the SQL Server database to connect to.                                        |
| `MaxRetryCount`       | The maximum number of retry attempts. Default value is 6, set it to 0 to disable the retry mechanism.  |
| `DisableHealthChecks` | A boolean value that indicates whether the database health check is disabled or not.                   |
| `DisableTracing`      | A boolean value that indicates whether the OpenTelemetry tracing is disabled or not.                   |
| `DisableMetrics`      | A boolean value that indicates whether the OpenTelemetry metrics are disabled or not.                  |

[!INCLUDE [component-health-checks](../includes/component-health-checks.md)]

By default, the .NET Aspire PostgreSQL Entity Framework Core components handles the following:

- Adds the [`DbContextHealthCheck`](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/blob/master/src/HealthChecks.NpgSql/NpgSqlHealthCheck.cs), which calls EF Core's <xref:Microsoft.EntityFrameworkCore.Storage.IDatabaseCreator.CanConnectAsync%2A> method. The name of the health check is the name of the `TContext` type.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic

[!INCLUDE [component-observability-and-telemetry](../includes/component-observability-and-telemetry.md)]

### Logging

The .NET Aspire PostgreSQL Entity Framework Core component uses the following Log categories:

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

The .NET Aspire PostgreSQL Entity Framework Core component will emit the following Tracing activities using OpenTelemetry:

- "Npgsql"

### Metrics

The .NET Aspire PostgreSQL Entity Framework Core component will emit the following metrics using OpenTelemetry:

- Microsoft.EntityFrameworkCore:
  - ec_Microsoft_EntityFrameworkCore_active_db_contexts
  - ec_Microsoft_EntityFrameworkCore_total_queries
  - ec_Microsoft_EntityFrameworkCore_queries_per_second
  - ec_Microsoft_EntityFrameworkCore_total_save_changes
  - ec_Microsoft_EntityFrameworkCore_save_changes_per_second
  - ec_Microsoft_EntityFrameworkCore_compiled_query_cache_hit_rate
  - ec_Microsoft_Entity_total_execution_strategy_operation_failures
  - ec_Microsoft_E_execution_strategy_operation_failures_per_second
  - ec_Microsoft_EntityFramew_total_optimistic_concurrency_failures
  - ec_Microsoft_EntityF_optimistic_concurrency_failures_per_second

- Npgsql:
  - ec_Npgsql_bytes_written_per_second
  - ec_Npgsql_bytes_read_per_second
  - ec_Npgsql_commands_per_second
  - ec_Npgsql_total_commands
  - ec_Npgsql_current_commands
  - ec_Npgsql_failed_commands
  - ec_Npgsql_prepared_commands_ratio
  - ec_Npgsql_connection_pools
  - ec_Npgsql_multiplexing_average_commands_per_batch
  - ec_Npgsql_multiplexing_average_write_time_per_batch

## See also

- [Azure Database for PostgreSQL documentation](/azure/postgresql/)
- [.NET Aspire components](../fundamentals/components-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
