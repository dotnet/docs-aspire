---
title: .NET Aspire SQL Server integration
description: This article describes the .NET Aspire SQL Server integration.
ms.topic: how-to
ms.date: 08/12/2024
---

# .NET Aspire SQL Server integration

In this article, you learn how to use the .NET Aspire SQL Server integration. The `Aspire.Microsoft.Data.SqlClient` library:

- Registers a scoped <xref:Microsoft.Data.SqlClient.SqlConnection?displayProperty=fullName> factory in the DI container for connecting Azure SQL, MS SQL database.
- Automatically configures the following:
  - Connection pooling to efficiently managed HTTP requests and database connections
  - Automatic retries to increase app resiliency
  - Health checks, logging and telemetry to improve app monitoring and diagnostics

## Prerequisites

- An [Azure SQL Database](/azure/azure-sql/database) or [SQL Server](/sql/sql-server) database and the connection string for accessing the database.

## Get started

To get started with the .NET Aspire SQL Server integration, install the [Aspire.Microsoft.Data.SqlClient](https://www.nuget.org/packages/Aspire.Microsoft.Data.SqlClient) NuGet package in the client-consuming project, i.e., the project for the application that uses the SQL Server client.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Microsoft.Data.SqlClient
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Microsoft.Data.SqlClient"
                  Version="[SelectVersion]" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireSqlServerSqlClientExtensions.AddSqlServerClient%2A> extension to register a <xref:System.Data.SqlClient.SqlConnection> for use via the dependency injection container.

```csharp
builder.AddSqlServerClient("sqldb");
```

To retrieve your `SqlConnection` object an example service:

```csharp
public class ExampleService(SqlConnection client)
{
    // Use client...
}
```

After adding a `SqlConnection`, you can get the scoped [SqlConnection](/dotnet/api/microsoft.data.sqlclient.sqlconnection) instance using DI.

## App host usage

[!INCLUDE [sql-app-host](includes/sql-app-host.md)]

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql");
var sqldb = sql.AddDatabase("sqldb");

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(sqldb);
```

When you want to explicitly provide a root SQL password, you can provide it as a parameter. Consider the following alternative example:

```csharp
var password = builder.AddParameter("password", secret: true);

var sql = builder.AddSqlServer("sql", password);
var sqldb = sql.AddDatabase("sqldb");

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(sqldb);
```

For more information, see [External parameters](../fundamentals/external-parameters.md).

## Configuration

The .NET Aspire SQL Server integration provides multiple configuration approaches and options to meet the requirements and conventions of your project.

### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you provide the name of the connection string when calling `AddSqlServerClient`:

```csharp
builder.AddSqlServerClient("myConnection");
```

The connection string is retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "myConnection": "Data Source=myserver;Initial Catalog=master"
  }
}
```

For more information, see the [ConnectionString](/dotnet/api/system.data.sqlclient.sqlconnection.connectionstring#remarks).

### Use configuration providers

The .NET Aspire SQL Server supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the `MicrosoftDataSqlClientSettings` from configuration files such as _:::no-loc text="appsettings.json":::_ by using the `Aspire:SqlServer:SqlClient` key. If you have set up your configurations in the `Aspire:SqlServer:SqlClient` section, you can just call the method without passing any parameter.

The following example shows an _:::no-loc text="appsettings.json":::_ file that configures some of the available options:

```json
{
  "Aspire": {
    "SqlServer": {
      "SqlClient": {
        "ConnectionString": "YOUR_CONNECTIONSTRING",
        "DisableHealthChecks": false,
        "DisableMetrics": true
      }
    }
  }
}
```

### Use inline configurations

You can also pass the `Action<MicrosoftDataSqlClientSettings>` delegate to set up some or all the options inline, for example to turn off the `DisableMetrics`:

```csharp
builder.AddSqlServerClient(
    static settings => settings.DisableMetrics = true);
```

### Configuring connections to multiple databases

If you want to add more than one `SqlConnection` you could use named instances. The json configuration would look like:

```json
{
  "Aspire": {
    "SqlServer": {
      "SqlClient": {
        "INSTANCE_NAME": {
          "ServiceUri": "YOUR_URI",
          "DisableHealthChecks": true
        }
      }
    }
  }
}
```

To load the named configuration section from the json config call the `AddSqlServerClient` method by passing the `INSTANCE_NAME`.

```csharp
builder.AddSqlServerClient("INSTANCE_NAME");
```

### Configuration options

Here are the configurable options with corresponding default values:

| Name                  | Description                                                                           |
|-----------------------|---------------------------------------------------------------------------------------|
| `ConnectionString`    | The connection string of the SQL Server database to connect to.                       |
| `DisableHealthChecks` | A boolean value that indicates whether the database health check is disabled or not.  |
| `DisableTracing`      | A boolean value that indicates whether the OpenTelemetry tracing is disabled or not.  |
| `DisableMetrics`      | A boolean value that indicates whether the OpenTelemetry metrics are disabled or not. |

[!INCLUDE [integration-health-checks](../includes/integration-health-checks.md)]

By default, the .NET Aspire Sql Server integration handles the following:

- Adds the [`SqlServerHealthCheck`](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/blob/master/src/HealthChecks.NpgSql/NpgSqlHealthCheck.cs), which verifies that a connection can be made commands can be run against the SQL Database.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

### Logging

The .NET Aspire SQL Server integration currently doesn't enable logging by default due to limitations of the `SqlClient`.

### Tracing

The .NET Aspire SQL Server integration will emit the following Tracing activities using OpenTelemetry:

- "OpenTelemetry.Instrumentation.SqlClient"

### Metrics

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

## See also

- [Azure SQL Database](/azure/azure-sql/database)
- [SQL Server](/sql/sql-server)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
