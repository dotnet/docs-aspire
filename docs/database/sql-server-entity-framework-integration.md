---
title: .NET Aspire SQL Server Entity Framework Core integration
description: Learn how to use the .NET Aspire SQL Server Entity Framework integration, which includes both hosting and client integrations.
ms.date: 12/02/2024
uid: database/sql-server-ef-core-integration
---

# .NET Aspire SQL Server Entity Framework Core integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[SQL Server](https://www.microsoft.com/sql-server) is a relational database management system developed by Microsoft. The .NET Aspire SQL Server Entity Framework Core integration enables you to connect to existing SQL Server instances or create new instances from .NET with the [`mcr.microsoft.com/mssql/server` container image](https://hub.docker.com/_/microsoft-mssql-server).

## Hosting integration

[!INCLUDE [sql-app-host](includes/sql-app-host.md)]

### Hosting integration health checks

The SQL Server hosting integration automatically adds a health check for the SQL Server resource. The health check verifies that the SQL Server is running and that a connection can be established to it.

The hosting integration relies on the [📦 AspNetCore.HealthChecks.SqlServer](https://www.nuget.org/packages/AspNetCore.HealthChecks.SqlServer) NuGet package.

## Client integration

To get started with the .NET Aspire SQL Server Entity Framework Core integration, install the [📦 Aspire.Microsoft.EntityFrameworkCore.SqlServer](https://www.nuget.org/packages/Aspire.Microsoft.EntityFrameworkCore.SqlServer) NuGet package in the client-consuming project, that is, the project for the application that uses the SQL Server Entity Framework Core client.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Microsoft.EntityFrameworkCore.SqlServer
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Microsoft.EntityFrameworkCore.SqlServer"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add SQL Server database context

In the :::no-loc text="Program.cs"::: file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireSqlServerEFCoreSqlClientExtensions.AddSqlServerDbContext%2A> extension method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register a <xref:Microsoft.EntityFrameworkCore.DbContext> for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddSqlServerDbContext<ExampleDbContext>(connectionName: "database");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the SQL Server database resource in the app host project. In other words, when you call `AddDatabase` and provide a name of `database` that same name should be used when calling `AddSqlServerDbContext`. For more information, see [Add SQL Server resource and database resource](#add-sql-server-resource-and-database-resource).

To retrieve `ExampleDbContext` object from a service:

```csharp
public class ExampleService(ExampleDbContext context)
{
    // Use context...
}
```

For more information on dependency injection, see [.NET dependency injection](/dotnet/core/extensions/dependency-injection).

### Add SQL Server database context with enrichment

To enrich the `DbContext` with additional services, such as automatic retries, health checks, logging and telemetry, call the <xref:Microsoft.Extensions.Hosting.AspireSqlServerEFCoreSqlClientExtensions.EnrichSqlServerDbContext*> method:

```csharp
builder.EnrichSqlServerDbContext<ExampleDbContext>(
    connectionName: "database",
    configureSettings: settings =>
    {
        settings.DisableRetry = false;
        settings.CommandTimeout = 30 // seconds
    });
```

The `settings` parameter is an instance of the <xref:Aspire.Microsoft.EntityFrameworkCore.SqlServer.MicrosoftEntityFrameworkCoreSqlServerSettings> class.

### Configuration

The .NET Aspire SQL Server Entity Framework Core integration provides multiple configuration approaches and options to meet the requirements and conventions of your project.

#### Use connection string

When using a connection string from the `ConnectionStrings` configuration section, you provide the name of the connection string when calling `builder.AddSqlServerDbContext<TContext>()`:

```csharp
builder.AddSqlServerDbContext<ExampleDbContext>("sql");
```

The connection string is retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "sql": "Data Source=myserver;Initial Catalog=master"
  }
}
```

The `EnrichSqlServerDbContext` won't make use of the `ConnectionStrings` configuration section since it expects a `DbContext` to be registered at the point it's called.

For more information, see the [ConnectionString](/dotnet/api/system.data.sqlclient.sqlconnection.connectionstring#remarks).

#### Use configuration providers

The .NET Aspire SQL Server Entity Framework Core integration supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the <xref:Aspire.Microsoft.EntityFrameworkCore.SqlServer.MicrosoftEntityFrameworkCoreSqlServerSettings> from configuration files such as _:::no-loc text="appsettings.json":::_ by using the `Aspire:Microsoft:EntityFrameworkCore:SqlServer` key. If you have set up your configurations in the `Aspire:Microsoft:EntityFrameworkCore:SqlServer` section you can just call the method without passing any parameter.

The following is an example of an _:::no-loc text="appsettings.json":::_ file that configures some of the available options:

```json
{
  "Aspire": {
    "Microsoft": {
      "EntityFrameworkCore": {
        "SqlServer": {
          "ConnectionString": "YOUR_CONNECTIONSTRING",
          "DbContextPooling": true,
          "DisableHealthChecks": true,
          "DisableTracing": true,
          "DisableMetrics": false
        }
      }
    }
  }
}
```

#### Use inline configurations

You can also pass the `Action<MicrosoftEntityFrameworkCoreSqlServerSettings>` delegate to set up some or all the options inline, for example to turn off the metrics:

```csharp
builder.AddSqlServerDbContext<YourDbContext>(
    "sql",
    static settings =>
        settings.DisableMetrics = true);
```

#### Configure multiple DbContext connections

If you want to register more than one `DbContext` with different configuration, you can use `$"Aspire.Microsoft.EntityFrameworkCore.SqlServer:{typeof(TContext).Name}"` configuration section name. The json configuration would look like:

```json
{
  "Aspire": {
    "Microsoft": {
      "EntityFrameworkCore": {
          "SqlServer": {
            "ConnectionString": "YOUR_CONNECTIONSTRING",
            "DbContextPooling": true,
            "DisableHealthChecks": true,
            "DisableTracing": true,
            "DisableMetrics": false,
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

Then calling the `AddSqlServerDbContext` method with `AnotherDbContext` type parameter would load the settings from `Aspire:Microsoft:EntityFrameworkCore:SqlServer:AnotherDbContext` section.

```csharp
builder.AddSqlServerDbContext<AnotherDbContext>("another-sql");
```

#### Configuration options

Here are the configurable options with corresponding default values:

| Name                  | Description                                                                                                          |
|-----------------------|----------------------------------------------------------------------------------------------------------------------|
| `ConnectionString`    | The connection string of the SQL Server database to connect to.                                                      |
| `DbContextPooling`    | A boolean value that indicates whether the db context will be pooled or explicitly created every time it's requested |
| `MaxRetryCount`       | The maximum number of retry attempts. Default value is 6, set it to 0 to disable the retry mechanism.                |
| `DisableHealthChecks` | A boolean value that indicates whether the database health check is disabled or not.                                 |
| `DisableTracing`      | A boolean value that indicates whether the OpenTelemetry tracing is disabled or not.                                 |
| `DisableMetrics`      | A boolean value that indicates whether the OpenTelemetry metrics are disabled or not.                                |
| `Timeout`             | The time in seconds to wait for the command to execute.                                                              |

[!INCLUDE [client-integration-health-checks](../../includes/client-integration-health-checks.md)]

By default, the .NET Aspire Sql Server Entity Framework Core integration handles the following:

- Adds the [`DbContextHealthCheck`](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/blob/master/src/HealthChecks.NpgSql/NpgSqlHealthCheck.cs), which calls EF Core's <xref:Microsoft.EntityFrameworkCore.Storage.IDatabaseCreator.CanConnectAsync%2A> method. The name of the health check is the name of the `TContext` type.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

#### Logging

The .NET Aspire SQL Server Entity Framework Core integration uses the following Log categories:

- `Microsoft.EntityFrameworkCore.ChangeTracking`
- `Microsoft.EntityFrameworkCore.Database.Command`
- `Microsoft.EntityFrameworkCore.Database.Connection`
- `Microsoft.EntityFrameworkCore.Database.Transaction`
- `Microsoft.EntityFrameworkCore.Infrastructure`
- `Microsoft.EntityFrameworkCore.Migrations`
- `Microsoft.EntityFrameworkCore.Model`
- `Microsoft.EntityFrameworkCore.Model.Validation`
- `Microsoft.EntityFrameworkCore.Query`
- `Microsoft.EntityFrameworkCore.Update`

#### Tracing

The .NET Aspire SQL Server Entity Framework Core integration will emit the following Tracing activities using OpenTelemetry:

- "OpenTelemetry.Instrumentation.EntityFrameworkCore"

#### Metrics

The .NET Aspire SQL Server Entity Framework Core integration will emit the following metrics using OpenTelemetry:

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

## See also

- [Azure SQL Database documentation](/azure/azure-sql/)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
