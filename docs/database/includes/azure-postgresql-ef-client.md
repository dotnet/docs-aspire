---
ms.topic: include
ms.custom: sfi-ropc-nochange
---

To get started with the Aspire PostgreSQL Entity Framework Core client integration, install the [📦 Aspire.Azure.Npgsql.EntityFrameworkCore.PostgreSQL](https://www.nuget.org/packages/) NuGet package in the client-consuming project, that is, the project for the application that uses the PostgreSQL client. The Aspire PostgreSQL Entity Framework Core client integration registers your desired `DbContext` subclass instances that you can use to interact with PostgreSQL.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Azure.Npgsql.EntityFrameworkCore.PostgreSQL
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Azure.Npgsql.EntityFrameworkCore.PostgreSQL"
                  Version="*" />
```

---

The PostgreSQL connection can be consumed using the client integration by calling the `AddAzureNpgsqlDbContext`:

```csharp
builder.AddAzureNpgsqlDbContext<YourDbContext>(connectionName: "postgresdb");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the PostgreSQL server resource in the AppHost project.

The preceding code snippet demonstrates how to use the <xref:Microsoft.Extensions.Hosting.AspireAzureEFPostgreSqlExtensions.AddAzureNpgsqlDbContext*> method to register an `YourDbContext` (that's [pooled for performance](/ef/core/performance/advanced-performance-topics)) instance that uses Azure authentication ([Microsoft Entra ID](/azure/postgresql/flexible-server/concepts-azure-ad-authentication)). This `"postgresdb"` connection name corresponds to a connection string configuration value.

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
> The connection string name that you pass to the <xref:Microsoft.Extensions.Configuration.ConfigurationExtensions.GetConnectionString*> method must match the name used when adding the PostgreSQL server resource in the AppHost project. For more information, see [Add PostgreSQL server resource](#add-postgresql-server-resource).

You have more flexibility when you create the database context in this way, for example:

- You can reuse existing configuration code for the database context without rewriting it for Aspire.
- You can use Entity Framework Core interceptors to modify database operations.
- You can choose not to use Entity Framework Core context pooling, which may perform better in some circumstances.

If you use this method, you can enhance the database context with Aspire retries, health checks, logging, and telemetry features by calling the `EnrichAzureNpgsqlDbContext` method:

```csharp
builder.EnrichAzureNpgsqlDbContext<YourDbContext>(
    configureSettings: settings =>
    {
        settings.DisableRetry = false;
        settings.CommandTimeout = 30;
    });
```

The `settings` parameter is an instance of the <xref:Aspire.Azure.Npgsql.EntityFrameworkCore.PostgreSQL.AzureNpgsqlEntityFrameworkCorePostgreSQLSettings> class.

You might also need to configure specific options of Npgsql, or register a <xref:System.Data.Entity.DbContext> in other ways. In this case, you do so by calling the <xref:Microsoft.Extensions.Hosting.AspireAzureEFPostgreSqlExtensions.EnrichAzureNpgsqlDbContext*> extension method, as shown in the following example:

```csharp
var connectionString = builder.Configuration.GetConnectionString("postgresdb");

builder.Services.AddDbContextPool<YourDbContext>(
    dbContextOptionsBuilder => dbContextOptionsBuilder.UseNpgsql(connectionString));

builder.EnrichAzureNpgsqlDbContext<YourDbContext>();
```

#### Configuration

The Aspire Azure PostgreSQL EntityFrameworkCore Npgsql integration provides multiple options to configure the database connection based on the requirements and conventions of your project.

##### Use a connection string

When using a connection string defined in the `ConnectionStrings` configuration section, you provide the name of the connection string when calling `AddAzureNpgsqlDataSource`:

```csharp
builder.AddAzureNpgsqlDbContext<YourDbContext>("postgresdb");
```

The connection string is retrieved from the `ConnectionStrings` configuration section, for example, consider the following JSON configuration:

```json
{
  "ConnectionStrings": {
    "postgresdb": "Host=myserver;Database=test"
  }
}
```

For more information on how to configure the connection string, see the [Npgsql connection string documentation](https://www.npgsql.org/doc/connection-string-parameters.html).

> [!NOTE]
> The username and password are automatically inferred from the credential provided in the settings.

##### Use configuration providers

The Aspire Azure PostgreSQL EntityFrameworkCore Npgsql integration supports <xref:Microsoft.Extensions.Configuration>. It loads the <xref:Aspire.Azure.Npgsql.EntityFrameworkCore.PostgreSQL.AzureNpgsqlEntityFrameworkCorePostgreSQLSettings> from configuration using the `Aspire:Npgsql:EntityFrameworkCore:PostgreSQL` key. For example, consider the following _appsettings.json_ file that configures some of the available options:

```json
{
  "Aspire": {
    "Npgsql": {
      "EntityFrameworkCore": {
        "PostgreSQL": {
          "DisableHealthChecks": true,
          "DisableTracing": true
        }
      }
    }
  }
}
```

##### Use inline delegates

You can configure settings in code, by passing the `Action<AzureNpgsqlEntityFrameworkCorePostgreSQLSettings> configureSettings` delegate to set up some or all the options inline, for example to disable health checks from code:

```csharp
builder.AddAzureNpgsqlDbContext<YourDbContext>(
    "postgresdb",
    settings => settings.DisableHealthChecks = true);
```

Alternatively, you can use the `EnrichAzureNpgsqlDbContext` extension method to configure the settings:

```csharp
builder.EnrichAzureNpgsqlDbContext<YourDbContext>(
    settings => settings.DisableHealthChecks = true);
```

Use the <xref:Aspire.Azure.Npgsql.EntityFrameworkCore.PostgreSQL.AzureNpgsqlEntityFrameworkCorePostgreSQLSettings.Credential?displayProperty=nameWithType> property to establish a connection. If no credential is configured, the <xref:Azure.Identity.DefaultAzureCredential> is used.

When the connection string contains a username and password, the credential is ignored.

##### Troubleshooting

In the rare case that the `Username` property isn't provided and the integration can't detect it using the application's Managed Identity, Npgsql throws an exception with a message similar to the following:

> Npgsql.PostgresException (0x80004005): 28P01: password authentication failed for user ...

In this case you can configure the `Username` property in the connection string and use `EnrichAzureNpgsqlDbContext`, passing the connection string in `UseNpgsql`:

```csharp
builder.Services.AddDbContextPool<YourDbContext>(
    options => options.UseNpgsql(newConnectionString));

builder.EnrichAzureNpgsqlDbContext<YourDbContext>();
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
builder.AddAzureNpgsqlDbContext<AnotherDbContext>();
```

[!INCLUDE [client-integration-health-checks](../../includes/client-integration-health-checks.md)]

By default, the Aspire PostgreSQL Entity Framework Core integrations handles the following:

- Adds the [`DbContextHealthCheck`](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/blob/master/src/HealthChecks.NpgSql/NpgSqlHealthCheck.cs), which calls EF Core's <xref:Microsoft.EntityFrameworkCore.Storage.IDatabaseCreator.CanConnectAsync%2A> method. The name of the health check is the name of the `TContext` type.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic

[!INCLUDE [integration-observability-and-telemetry](../../includes/integration-observability-and-telemetry.md)]

#### Logging

The Aspire PostgreSQL Entity Framework Core integration uses the following Log categories:

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

The Aspire PostgreSQL Entity Framework Core integration will emit the following tracing activities using OpenTelemetry:

- `Npgsql`

#### Metrics

The Aspire PostgreSQL Entity Framework Core integration will emit the following metrics using OpenTelemetry:

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
