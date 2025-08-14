---
title: .NET Aspire MySQL database integration
description: Learn how to use the .NET Aspire MySQL database integration, which includes both hosting and client integrations.
ms.date: 02/07/2025
uid: storage/mysql-integration
ms.custom: sfi-ropc-nochange
---

# .NET Aspire MySQL integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[MySQL](https://www.mysql.com/) is an open-source Relational Database Management System (RDBMS) that uses Structured Query Language (SQL) to manage and manipulate data. It's employed in a many different environments, from small projects to large-scale enterprise systems and it's a popular choice to host data that underpins microservices in a cloud-native application. The .NET Aspire MySQL database integration enables you to connect to existing MySQL databases or create new instances from .NET with the [`mysql` container image](https://hub.docker.com/_/mysql).

## Hosting integration

[!INCLUDE [mysql-app-host](includes/mysql-app-host.md)]

## Client integration

To get started with the .NET Aspire MySQL database integration, install the [📦 Aspire.MySqlConnector](https://www.nuget.org/packages/Aspire.MySqlConnector) NuGet package in the client-consuming project, that is, the project for the application that uses the MySQL client. The MySQL client integration registers a `MySqlConnector.MySqlDataSource` instance that you can use to interact with the MySQL server.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.MySqlConnector
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.MySqlConnector"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add a MySQL data source

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireMySqlConnectorExtensions.AddMySqlDataSource*> extension method to register a `MySqlDataSource` for use via the dependency injection container. The method takes a `connectionName` parameter.

```csharp
builder.AddMySqlDataSource(connectionName: "mysqldb");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the MySQL database resource in the AppHost project. In other words, when you call `AddDatabase` and provide a name of `mysqldb` that same name should be used when calling `AddMySqlDataSource`. For more information, see [Add MySQL server resource and database resource](#add-mysql-server-resource-and-database-resource).

You can then retrieve the `MySqlConnector.MySqlDataSource` instance using dependency injection. For example, to retrieve the data source from an example service:

```csharp
public class ExampleService(MySqlDataSource dataSource)
{
    // Use dataSource...
}
```

For more information on dependency injection, see [.NET dependency injection](/dotnet/core/extensions/dependency-injection).

### Add keyed MySQL data source

There might be situations where you want to register multiple `MySqlDataSource` instances with different connection names. To register keyed MySQL data sources, call the <xref:Microsoft.Extensions.Hosting.AspireMySqlConnectorExtensions.AddKeyedMySqlDataSource*> method:

```csharp
builder.AddKeyedMySqlDataSource(name: "mainDb");
builder.AddKeyedMySqlDataSource(name: "loggingDb");
```

> [!IMPORTANT]
> When using keyed services, it's expected that your MySQL resource configured two named databases, one for the `mainDb` and one for the `loggingDb`.

Then you can retrieve the `MySqlDatSource` instances using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(
    [FromKeyedServices("mainDb")] MySqlDataSource mainDbConnection,
    [FromKeyedServices("loggingDb")] MySqlDataSource loggingDbConnection)
{
    // Use connections...
}
```

For more information on keyed services, see [.NET dependency injection: Keyed services](/dotnet/core/extensions/dependency-injection#keyed-services).

### Configuration

The .NET Aspire MySQL database integration provides multiple options to configure the connection based on the requirements and conventions of your project.

#### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling <xref:Microsoft.Extensions.Hosting.AspireMySqlConnectorExtensions.AddMySqlDataSource*> method:

```csharp
builder.AddMySqlDataSource(connectionName: "mysql");
```

Then the connection string is retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "mysql": "Server=mysql;Database=mysqldb"
  }
}
```

For more information on how to format this connection string, see [MySqlConnector: ConnectionString documentation](https://mysqlconnector.net/connection-options/).

#### Use configuration providers

The .NET Aspire MySQL database integration supports <xref:Microsoft.Extensions.Configuration>. It loads the <xref:Aspire.MySqlConnector.MySqlConnectorSettings> from configuration by using the `Aspire:MySqlConnector` key. The following snippet is an example of a _:::no-loc text="appsettings.json":::_ file that configures some of the options:

```json
{
  "Aspire": {
    "MySqlConnector": {
      "ConnectionString": "YOUR_CONNECTIONSTRING",
      "DisableHealthChecks": true,
      "DisableTracing": true
    }
  }
}
```

For the complete MySQL integration JSON schema, see [Aspire.MySqlConnector/ConfigurationSchema.json](https://github.com/dotnet/aspire/blob/main/src/Components/Aspire.MySqlConnector/ConfigurationSchema.json).

#### Use inline delegates

Also you can pass the `Action<MySqlConnectorSettings>` delegate to set up some or all the options inline, for example to disable health checks from code:

```csharp
builder.AddMySqlDataSource(
    "mysql",
    static settings => settings.DisableHealthChecks  = true);
```

### Client integration health checks

By default, .NET Aspire integrations enable [health checks](../fundamentals/health-checks.md) for all services. For more information, see [.NET Aspire integrations overview](../fundamentals/integrations-overview.md).

The .NET Aspire MySQL database integration:

- Adds the health check when <xref:Aspire.MySqlConnector.MySqlConnectorSettings.DisableHealthChecks?displayProperty=nameWithType> is `false`, which verifies that a connection can be made and commands can be run against the MySQL database.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic.

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

#### Logging

The .NET Aspire MySQL integration uses the following log categories:

- `MySqlConnector.ConnectionPool`
- `MySqlConnector.MySqlBulkCopy`
- `MySqlConnector.MySqlCommand`
- `MySqlConnector.MySqlConnection`
- `MySqlConnector.MySqlDataSource`

#### Tracing

The .NET Aspire MySQL integration emits the following tracing activities using OpenTelemetry:

- `MySqlConnector`

#### Metrics

The .NET Aspire MySQL integration will emit the following metrics using OpenTelemetry:

- MySqlConnector
  - `db.client.connections.create_time`
  - `db.client.connections.use_time`
  - `db.client.connections.wait_time`
  - `db.client.connections.idle.max`
  - `db.client.connections.idle.min`
  - `db.client.connections.max`
  - `db.client.connections.pending_requests`
  - `db.client.connections.timeouts`
  - `db.client.connections.usage`

## See also

- [MySQL database](https://mysqlconnector.net/)
- [.NET Aspire database containers sample](/samples/dotnet/aspire-samples/aspire-database-containers/)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
