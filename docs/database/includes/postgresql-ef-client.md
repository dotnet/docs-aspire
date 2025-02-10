---
ms.topic: include
---

To get started with the .NET Aspire PostgreSQL Entity Framework Core client integration, install the [📦 Aspire.Npgsql.EntityFrameworkCore.PostgreSQL](https://www.nuget.org/packages/Aspire.Npgsql.EntityFrameworkCore.PostgreSQL) NuGet package in the client-consuming project, that is, the project for the application that uses the PostgreSQL client. The .NET Aspire PostgreSQL Entity Framework Core client integration registers your desired `DbContext` subclass instances that you can use to interact with PostgreSQL.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Npgsql.EntityFrameworkCore.PostgreSQL
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Npgsql.EntityFrameworkCore.PostgreSQL"
                  Version="*" />
```

---

### Add Npgsql database context

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireEFPostgreSqlExtensions.AddNpgsqlDbContext%2A> extension method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register your <xref:System.Data.Entity.DbContext> subclass for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddNpgsqlDbContext<YourDbContext>(connectionName: "postgresdb");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the PostgreSQL server resource in the app host project. For more information, see [Add PostgreSQL server resource](#add-postgresql-server-resource).

After adding `YourDbContext` to the builder, you can get the `YourDbContext` instance using dependency injection. For example, to retrieve your data source object from an example service define it as a constructor parameter and ensure the `ExampleService` class is registered with the dependency injection container:

```csharp
public class ExampleService(YourDbContext context)
{
    // Use context...
}
```

For more information on dependency injection, see [.NET dependency injection](/dotnet/core/extensions/dependency-injection).

### Enrich an Npgsql database context

You may prefer to use the standard Entity Framework method to obtain a database context and add it to the dependency injection container:

```csharp
builder.Services.AddDbContext<YourDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("postgresdb")
        ?? throw new InvalidOperationException("Connection string 'postgresdb' not found.")));
```

> [!NOTE]
> The connection string name that you pass to the <xref:Microsoft.Extensions.Configuration.ConfigurationExtensions.GetConnectionString*> method must match the name used when adding the PostgreSQL server resource in the app host project. For more information, see [Add PostgreSQL server resource](#add-postgresql-server-resource).

You have more flexibility when you create the database context in this way, for example:

- You can reuse existing configuration code for the database context without rewriting it for .NET Aspire.
- You can use Entity Framework Core interceptors to modify database operations.
- You can choose not to use Entity Framework Core context pooling, which may perform better in some circumstances.

If you use this method, you can enhance the database context with .NET Aspire-style retries, health checks, logging, and telemetry features by calling the <xref:Microsoft.Extensions.Hosting.AspireEFPostgreSqlExtensions.EnrichNpgsqlDbContext*> method:

```csharp
builder.EnrichNpgsqlDbContext<YourDbContext>(
    configureSettings: settings =>
    {
        settings.DisableRetry = false;
        settings.CommandTimeout = 30;
    });
```

The `settings` parameter is an instance of the <xref:Aspire.Npgsql.EntityFrameworkCore.PostgreSQL.NpgsqlEntityFrameworkCorePostgreSQLSettings> class.

### Configuration

The .NET Aspire PostgreSQL Entity Framework Core integration provides multiple configuration approaches and options to meet the requirements and conventions of your project.

#### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you provide the name of the connection string when calling the <xref:Microsoft.Extensions.Hosting.AspireEFPostgreSqlExtensions.AddNpgsqlDbContext*> method:

```csharp
builder.AddNpgsqlDbContext<MyDbContext>("pgdb");
```

The connection string is retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "pgdb": "Host=myserver;Database=test"
  }
}
```

The `EnrichNpgsqlDbContext` won't make use of the `ConnectionStrings` configuration section since it expects a `DbContext` to be registered at the point it's called.

For more information, see the [ConnectionString](https://www.npgsql.org/doc/connection-string-parameters.html).

#### Use configuration providers

The .NET Aspire PostgreSQL Entity Framework Core integration supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the <xref:Aspire.Npgsql.EntityFrameworkCore.PostgreSQL.NpgsqlEntityFrameworkCorePostgreSQLSettings> from configuration files such as _:::no-loc text="appsettings.json":::_ by using the `Aspire:Npgsql:EntityFrameworkCore:PostgreSQL` key. If you have set up your configurations in the `Aspire:Npgsql:EntityFrameworkCore:PostgreSQL` section you can just call the method without passing any parameter.

The following example shows an _:::no-loc text="appsettings.json":::_ file that configures some of the available options:

```json
{
  "Aspire": {
    "Npgsql": {
      "EntityFrameworkCore": {
        "PostgreSQL": {
          "ConnectionString": "Host=myserver;Database=postgresdb",
          "DisableHealthChecks": true,
          "DisableTracing": true
        }
      }
    }
  }
}
```

For the complete PostgreSQL Entity Framework Core client integration JSON schema, see [Aspire.Npgsql.EntityFrameworkCore.PostgreSQL/ConfigurationSchema.json](https://github.com/dotnet/aspire/blob/v9.0.0/src/Components/Aspire.Npgsql.EntityFrameworkCore.PostgreSQL/ConfigurationSchema.json).

#### Use inline delegates

You can also pass the `Action<NpgsqlEntityFrameworkCorePostgreSQLSettings>` delegate to set up some or all the options inline, for example to set the `ConnectionString`:

```csharp
builder.AddNpgsqlDbContext<YourDbContext>(
    "pgdb",
    static settings => settings.ConnectionString = "<YOUR CONNECTION STRING>");
```

#### Configure multiple DbContext classes

If you want to register more than one <xref:Microsoft.EntityFrameworkCore.DbContext> with different configuration, you can use `$"Aspire:Npgsql:EntityFrameworkCore:PostgreSQL:{typeof(TContext).Name}"` configuration section name. The json configuration would look like:

```json
{
  "Aspire": {
    "Npgsql": {
      "EntityFrameworkCore": {
        "PostgreSQL": {
          "ConnectionString": "<YOUR CONNECTION STRING>",
          "DisableHealthChecks": true,
          "DisableTracing": true,
          "AnotherDbContext": {
            "ConnectionString": "<ANOTHER CONNECTION STRING>",
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

[!INCLUDE [integration-health-checks](../../includes/integration-health-checks.md)]

By default, the .NET Aspire PostgreSQL Entity Framework Core integrations handles the following:

- Adds the [`DbContextHealthCheck`](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/blob/master/src/HealthChecks.NpgSql/NpgSqlHealthCheck.cs), which calls EF Core's <xref:Microsoft.EntityFrameworkCore.Storage.IDatabaseCreator.CanConnectAsync%2A> method. The name of the health check is the name of the `TContext` type.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic

[!INCLUDE [integration-observability-and-telemetry](../../includes/integration-observability-and-telemetry.md)]

#### Logging

The .NET Aspire PostgreSQL Entity Framework Core integration uses the following Log categories:

- `Microsoft.EntityFrameworkCore.ChangeTracking`
- `Microsoft.EntityFrameworkCore.Database.Command`
- `Microsoft.EntityFrameworkCore.Database.Connection`
- `Microsoft.EntityFrameworkCore.Database.Transaction`
- `Microsoft.EntityFrameworkCore.Migrations`
- `Microsoft.EntityFrameworkCore.Infrastructure`
- `Microsoft.EntityFrameworkCore.Migrations`
- `Microsoft.EntityFrameworkCore.Model`
- `Microsoft.EntityFrameworkCore.Model.Validation`
- `Microsoft.EntityFrameworkCore.Query`
- `Microsoft.EntityFrameworkCore.Update`

#### Tracing

The .NET Aspire PostgreSQL Entity Framework Core integration will emit the following tracing activities using OpenTelemetry:

- `Npgsql`

#### Metrics

The .NET Aspire PostgreSQL Entity Framework Core integration will emit the following metrics using OpenTelemetry:

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

- Npgsql:
  - `ec_Npgsql_bytes_written_per_second`
  - `ec_Npgsql_bytes_read_per_second`
  - `ec_Npgsql_commands_per_second`
  - `ec_Npgsql_total_commands`
  - `ec_Npgsql_current_commands`
  - `ec_Npgsql_failed_commands`
  - `ec_Npgsql_prepared_commands_ratio`
  - `ec_Npgsql_connection_pools`
  - `ec_Npgsql_multiplexing_average_commands_per_batch`
  - `ec_Npgsql_multiplexing_average_write_time_per_batch`
