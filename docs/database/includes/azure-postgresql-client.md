---
ms.topic: include
ms.custom: sfi-ropc-nochange
---

To get started with the .NET Aspire Azure PostgreSQL client integration, install the [📦 Aspire.Azure.Npgsql](https://www.nuget.org/packages/Aspire.Azure.Npgsql) NuGet package in the client-consuming project, that is, the project for the application that uses the PostgreSQL client. The PostgreSQL client integration registers an [NpgsqlDataSource](https://www.npgsql.org/doc/api/Npgsql.NpgsqlDataSource.html) instance that you can use to interact with PostgreSQL.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Azure.Npgsql
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Azure.Npgsql"
                  Version="*" />
```

---

<!-- TODO: Add xref to AddAzureNpgsqlDataSource when available -->

The PostgreSQL connection can be consumed using the client integration by calling the `AddAzureNpgsqlDataSource`:

```csharp
builder.AddAzureNpgsqlDataSource(connectionName: "postgresdb");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the PostgreSQL server resource in the AppHost project.

The preceding code snippet demonstrates how to use the `AddAzureNpgsqlDataSource` method to register an `NpgsqlDataSource` instance that uses Azure authentication ([Microsoft Entra ID](/azure/postgresql/flexible-server/concepts-azure-ad-authentication)). This `"postgresdb"` connection name corresponds to a connection string configuration value.

After adding `NpgsqlDataSource` to the builder, you can get the `NpgsqlDataSource` instance using dependency injection. For example, to retrieve your data source object from an example service define it as a constructor parameter and ensure the `ExampleService` class is registered with the dependency injection container:

```csharp
public class ExampleService(NpgsqlDataSource dataSource)
{
    // Use dataSource...
}
```

For more information on dependency injection, see [.NET dependency injection](/dotnet/core/extensions/dependency-injection).

### Add keyed Azure Npgsql client

<!-- TODO: Add xref to AddKeyedAzureNpgsqlDataSource when available -->

There might be situations where you want to register multiple `NpgsqlDataSource` instances with different connection names. To register keyed Npgsql clients, call the `AddKeyedAzureNpgsqlDataSource` method:

```csharp
builder.AddKeyedAzureNpgsqlDataSource(name: "sales_db");
builder.AddKeyedAzureNpgsqlDataSource(name: "inventory_db");
```

Then you can retrieve the `NpgsqlDataSource` instances using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(
    [FromKeyedServices("sales_db")] NpgsqlDataSource salesDataSource,
    [FromKeyedServices("inventory_db")] NpgsqlDataSource inventoryDataSource)
{
    // Use data sources...
}
```

For more information on keyed services, see [.NET dependency injection: Keyed services](/dotnet/core/extensions/dependency-injection#keyed-services).

#### Configuration

The .NET Aspire Azure Npgsql integration provides multiple options to configure the database connection based on the requirements and conventions of your project.

##### Use a connection string

When using a connection string defined in the `ConnectionStrings` configuration section, you provide the name of the connection string when calling `AddAzureNpgsqlDataSource`:

```csharp
builder.AddAzureNpgsqlDataSource("postgresdb");
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

<!-- TODO: Add xref to AzureNpgsqlSettings when available -->

The .NET Aspire Azure Npgsql integration supports <xref:Microsoft.Extensions.Configuration>. It loads the `AzureNpgsqlSettings` from configuration using the `Aspire:Azure:Npgsql` key. For example, consider the following _appsettings.json_ file that configures some of the available options:

```json
{
  "Aspire": {
    "Npgsql": {
      "DisableHealthChecks": true,
      "DisableTracing": true
    }
  }
}
```

##### Use inline delegates

You can configure settings in code, by passing the `Action<AzureNpgsqlSettings> configureSettings` delegate to set up some or all the options inline, for example to disable health checks from code:

```csharp
builder.AddAzureNpgsqlDataSource(
    "postgresdb",
    settings => settings.DisableHealthChecks = true);
```

<!-- TODO: Add xref to AzureNpgsqlSettings.Credential when available -->

Use the `AzureNpgsqlSettings.Credential` property to establish a connection. If no credential is configured, the <xref:Azure.Identity.DefaultAzureCredential> is used. When the connection string contains a username and password, the credential is ignored.

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
