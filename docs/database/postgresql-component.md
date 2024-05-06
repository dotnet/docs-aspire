---
title: .NET Aspire PostgreSQL component
description: This article describes the .NET Aspire PostgreSQL component.
ms.date: 01/22/2024
ms.topic: how-to
---

# .NET Aspire PostgreSQL component

In this article, you learn how to use the .NET Aspire PostgreSQL component. The `Aspire.Npgsql` library is used to register a [NpgsqlDataSource](https://www.npgsql.org/doc/api/Npgsql.NpgsqlDataSource.html) in the DI container for connecting to a PostgreSQL database. It also enables corresponding health checks, logging and telemetry.

## Get started

To get started with the .NET Aspire PostgreSQL component, install the [Aspire.Npgsql](https://www.nuget.org/packages/Aspire.Npgsql) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Npgsql --prerelease
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Npgsql" Version="[SelectVersion]" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

In the _Program.cs_ file of your component-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspirePostgreSqlNpgsqlExtensions.AddNpgsqlDataSource%2A> extension to register an `NpgsqlDataSource` for use via the dependency injection container.

```csharp
builder.AddNpgsqlDataSource("postgresdb");
```

After adding `NpgsqlDataSource` to the builder, you can get the `NpgsqlDataSource` instance using dependency injection. For example, to retrieve your context object from service:

```csharp
public class ExampleService(NpgsqlDataSource dataSource)
{
    // Use dataSource...
}
```

## App host usage

In your app host project, register and consume the PostgreSQL component using the following methods, such as <xref:Aspire.Hosting.PostgresBuilderExtensions.AddPostgres%2A>:

```csharp
var postgres = builder.AddPostgres("postgres");
var postgresdb = postgres.AddDatabase("postgresdb");

var exampleProject = builder.AddProject<Projects.ExampleProject>()
                            .WithReference(postgresdb);
```

## Configuration

The .NET Aspire PostgreSQL component provides multiple configuration approaches and options to meet the requirements and conventions of your project.

### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling <xref:Microsoft.Extensions.Hosting.AspirePostgreSqlNpgsqlExtensions.AddNpgsqlDataSource%2A>:

```csharp
builder.AddNpgsqlDataSource("NpgsqlConnection");
```

And then the connection string will be retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "NpgsqlConnection": "Host=myserver;Database=postgresdb"
  }
}
```

### Use configuration providers

The .NET Aspire PostgreSQL component supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the <xref:Aspire.Npgsql.NpgsqlSettings> from _appsettings.json_ or other configuration files by using the `Aspire:Npgsql` key. Example `appsettings.json` that configures some of the options:

The following example shows an _appsettings.json_ file that configures some of the available options:

```json
{
  "Aspire": {
    "Npgsql": {
      "HealthChecks": false,
      "Tracing": false
    }
  }
}
```

### Use inline delegates

You can also pass the `Action<NpgsqlSettings> configureSettings` delegate to set up some or all the options inline, for example to set the `ConnectionString`:

```csharp
builder.AddNpgsqlDataSource(
    "postgresdb",
     settings => settings.HealthChecks = false);
```

[!INCLUDE [component-health-checks](../includes/component-health-checks.md)]

- Adds the [`NpgSqlHealthCheck`](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/blob/master/src/HealthChecks.NpgSql/NpgSqlHealthCheck.cs), which verifies that commands can be successfully executed against the underlying Postgres Database.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic

[!INCLUDE [component-observability-and-telemetry](../includes/component-observability-and-telemetry.md)]

### Logging

The .NET Aspire PostgreSQL component uses the following Log categories:

- `Npgsql.Connection`
- `Npgsql.Command`
- `Npgsql.Transaction`
- `Npgsql.Copy`
- `Npgsql.Replication`
- `Npgsql.Exception`

### Tracing

The .NET Aspire PostgreSQL component will emit the following Tracing activities using OpenTelemetry:

- "Npgsql"

### Metrics

The .NET Aspire PostgreSQL component will emit the following metrics using OpenTelemetry:

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

## See also

- [PostgreSQL docs](https://www.npgsql.org/doc/api/Npgsql.html)
- [.NET Aspire components](../fundamentals/components-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
