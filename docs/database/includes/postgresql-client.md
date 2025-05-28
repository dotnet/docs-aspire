---
ms.topic: include
---

To get started with the .NET Aspire PostgreSQL client integration, install the [📦 Aspire.Npgsql](https://www.nuget.org/packages/Aspire.Npgsql) NuGet package in the client-consuming project, that is, the project for the application that uses the PostgreSQL client. The PostgreSQL client integration registers an [NpgsqlDataSource](https://www.npgsql.org/doc/api/Npgsql.NpgsqlDataSource.html) instance that you can use to interact with PostgreSQL.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Npgsql
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Npgsql" Version="*" />
```

---

### Add Npgsql client

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspirePostgreSqlNpgsqlExtensions.AddNpgsqlDataSource%2A> extension method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register an `NpgsqlDataSource` for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddNpgsqlDataSource(connectionName: "postgresdb");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the PostgreSQL server resource in the app host project. For more information, see [Add PostgreSQL server resource](#add-postgresql-server-resource).

After adding `NpgsqlDataSource` to the builder, you can get the `NpgsqlDataSource` instance using dependency injection. For example, to retrieve your data source object from an example service define it as a constructor parameter and ensure the `ExampleService` class is registered with the dependency injection container:

```csharp
public class ExampleService(NpgsqlDataSource dataSource)
{
    // Use dataSource...
}
```

For more information on dependency injection, see [.NET dependency injection](/dotnet/core/extensions/dependency-injection).

### Add keyed Npgsql client

There might be situations where you want to register multiple `NpgsqlDataSource` instances with different connection names. To register keyed Npgsql clients, call the <xref:Microsoft.Extensions.Hosting.AspirePostgreSqlNpgsqlExtensions.AddKeyedNpgsqlDataSource*> method:

```csharp
builder.AddKeyedNpgsqlDataSource(name: "chat");
builder.AddKeyedNpgsqlDataSource(name: "queue");
```

Then you can retrieve the `NpgsqlDataSource` instances using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(
    [FromKeyedServices("chat")] NpgsqlDataSource chatDataSource,
    [FromKeyedServices("queue")] NpgsqlDataSource queueDataSource)
{
    // Use data sources...
}
```

For more information on keyed services, see [.NET dependency injection: Keyed services](/dotnet/core/extensions/dependency-injection#keyed-services).

### Configuration

The .NET Aspire PostgreSQL integration provides multiple configuration approaches and options to meet the requirements and conventions of your project.

#### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling the <xref:Microsoft.Extensions.Hosting.AspirePostgreSqlNpgsqlExtensions.AddNpgsqlDataSource*> method:

```csharp
builder.AddNpgsqlDataSource("postgresdb");
```

Then the connection string will be retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "postgresdb": "Host=myserver;Database=postgresdb"
  }
}
```

For more information, see the [ConnectionString](https://www.npgsql.org/doc/connection-string-parameters.html).

#### Use configuration providers

The .NET Aspire PostgreSQL integration supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the <xref:Aspire.Npgsql.NpgsqlSettings> from _:::no-loc text="appsettings.json":::_ or other configuration files by using the `Aspire:Npgsql` key. Example _:::no-loc text="appsettings.json":::_ that configures some of the options:

The following example shows an _:::no-loc text="appsettings.json":::_ file that configures some of the available options:

```json
{
  "Aspire": {
    "Npgsql": {
      "ConnectionString": "Host=myserver;Database=postgresdb",
      "DisableHealthChecks": false,
      "DisableTracing": true,
      "DisableMetrics": false
    }
  }
}
```

For the complete PostgreSQL client integration JSON schema, see [Aspire.Npgsql/ConfigurationSchema.json](https://github.com/dotnet/aspire/blob/v9.1.0/src/Components/Aspire.Npgsql/ConfigurationSchema.json).

#### Use inline delegates

You can also pass the `Action<NpgsqlSettings> configureSettings` delegate to set up some or all the options inline, for example to disable health checks:

```csharp
builder.AddNpgsqlDataSource(
    "postgresdb",
     static settings => settings.DisableHealthChecks = true);
```

[!INCLUDE [client-integration-health-checks](../../includes/client-integration-health-checks.md)]

- Adds the [`NpgSqlHealthCheck`](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/blob/master/src/HealthChecks.NpgSql/NpgSqlHealthCheck.cs), which verifies that commands can be successfully executed against the underlying Postgres database.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic

[!INCLUDE [integration-observability-and-telemetry](../../includes/integration-observability-and-telemetry.md)]

#### Logging

The .NET Aspire PostgreSQL integration uses the following log categories:

- `Npgsql.Connection`
- `Npgsql.Command`
- `Npgsql.Transaction`
- `Npgsql.Copy`
- `Npgsql.Replication`
- `Npgsql.Exception`

#### Tracing

The .NET Aspire PostgreSQL integration will emit the following tracing activities using OpenTelemetry:

- `Npgsql`

#### Metrics

The .NET Aspire PostgreSQL integration will emit the following metrics using OpenTelemetry:

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
