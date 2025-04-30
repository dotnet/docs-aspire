---
ms.topic: include
---

To get started with the .NET Aspire SQL Server client integration, install the [📦 Aspire.Microsoft.Data.SqlClient](https://www.nuget.org/packages/Aspire.Microsoft.Data.SqlClient) NuGet package in the client-consuming project, that is, the project for the application that uses the SQL Server client. The SQL Server client integration registers a <xref:System.Data.SqlClient.SqlConnection> instance that you can use to interact with SQL Server.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Microsoft.Data.SqlClient
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Microsoft.Data.SqlClient"
                  Version="*" />
```

---

### Add SQL Server client

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireSqlServerSqlClientExtensions.AddSqlServerClient*> extension method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register a `SqlConnection` for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddSqlServerClient(connectionName: "database");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the SQL Server database resource in the app host project. In other words, when you call `AddDatabase` and provide a name of `database` that same name should be used when calling `AddSqlServerClient`. For more information, see [Add SQL Server resource and database resource](sql-app-host.md#add-sql-server-resource-and-database-resource).

You can then retrieve the <xref:Microsoft.Data.SqlClient.SqlConnection> instance using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(SqlConnection connection)
{
    // Use connection...
}
```

For more information on dependency injection, see [.NET dependency injection](/dotnet/core/extensions/dependency-injection).

### Add keyed SQL Server client

There might be situations where you want to register multiple `SqlConnection` instances with different connection names. To register keyed SQL Server clients, call the <xref:Microsoft.Extensions.Hosting.AspireSqlServerSqlClientExtensions.AddKeyedSqlServerClient*> method:

```csharp
builder.AddKeyedSqlServerClient(name: "mainDb");
builder.AddKeyedSqlServerClient(name: "loggingDb");
```

> [!IMPORTANT]
> When using keyed services, it's expected that your SQL Server resource configured two named databases, one for the `mainDb` and one for the `loggingDb`.

Then you can retrieve the `SqlConnection` instances using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(
    [FromKeyedServices("mainDb")] SqlConnection mainDbConnection,
    [FromKeyedServices("loggingDb")] SqlConnection loggingDbConnection)
{
    // Use connections...
}
```

For more information on keyed services, see [.NET dependency injection: Keyed services](/dotnet/core/extensions/dependency-injection#keyed-services).

### Configuration

The .NET Aspire SQL Server integration provides multiple options to configure the connection based on the requirements and conventions of your project.

#### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling the <xref:Microsoft.Extensions.Hosting.AspireSqlServerSqlClientExtensions.AddSqlServerClient*> method:

```csharp
builder.AddSqlServerClient(connectionName: "sql");
```

Then the connection string is retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "database": "Data Source=myserver;Initial Catalog=master"
  }
}
```

For more information on how to format this connection string, see the [ConnectionString](/dotnet/api/system.data.sqlclient.sqlconnection.connectionstring#remarks).

#### Use configuration providers

The .NET Aspire SQL Server integration supports <xref:Microsoft.Extensions.Configuration>. It loads the <xref:Aspire.Microsoft.Data.SqlClient.MicrosoftDataSqlClientSettings> from configuration by using the `Aspire:Microsoft:Data:SqlClient` key. The following snippet is an example of a _:::no-loc text="appsettings.json":::_ file that configures some of the options:

```json
{
  "Aspire": {
    "Microsoft": {
      "Data": {
        "SqlClient": {
          "ConnectionString": "YOUR_CONNECTIONSTRING",
          "DisableHealthChecks": false,
          "DisableMetrics": true
        }
      }
    }
  }
}
```

For the complete SQL Server client integration JSON schema, see [Aspire.Microsoft.Data.SqlClient/ConfigurationSchema.json](https://github.com/dotnet/aspire/blob/v8.2.2/src/Components/Aspire.Microsoft.Data.SqlClient/ConfigurationSchema.json).

#### Use inline delegates

Also you can pass the `Action<MicrosoftDataSqlClientSettings> configureSettings` delegate to set up some or all the options inline, for example to disable health checks from code:

```csharp
builder.AddSqlServerClient(
    "database",
    static settings => settings.DisableHealthChecks = true);
```

### Client integration health checks

By default, .NET Aspire integrations enable [health checks](../../fundamentals/health-checks.md) for all services. For more information, see [.NET Aspire integrations overview](../../fundamentals/integrations-overview.md).

The .NET Aspire SQL Server integration:

- Adds the health check when <xref:Aspire.Microsoft.Data.SqlClient.MicrosoftDataSqlClientSettings.DisableHealthChecks?displayProperty=nameWithType> is `false`, which attempts to connect to the SQL Server.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic.

[!INCLUDE [integration-observability-and-telemetry](../../includes/integration-observability-and-telemetry.md)]

#### Logging

The .NET Aspire SQL Server integration currently doesn't enable logging by default due to limitations of the <xref:Microsoft.Data.SqlClient>.

#### Tracing

The .NET Aspire SQL Server integration emits the following tracing activities using OpenTelemetry:

- `OpenTelemetry.Instrumentation.SqlClient`

#### Metrics

The .NET Aspire SQL Server integration will emit the following metrics using OpenTelemetry:

- Microsoft.Data.SqlClient.EventSource
  - `active-hard-connections`
  - `hard-connects`
  - `hard-disconnects`
  - `active-soft-connects`
  - `soft-connects`
  - `soft-disconnects`
  - `number-of-non-pooled-connections`
  - `number-of-pooled-connections`
  - `number-of-active-connection-pool-groups`
  - `number-of-inactive-connection-pool-groups`
  - `number-of-active-connection-pools`
  - `number-of-inactive-connection-pools`
  - `number-of-active-connections`
  - `number-of-free-connections`
  - `number-of-stasis-connections`
  - `number-of-reclaimed-connections`
