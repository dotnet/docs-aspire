---
title: .NET Aspire MySQL database integration
description: Learn how to use the .NET Aspire MySQL database integration, which includes both hosting and client integrations.
ms.date: 12/09/2024
uid: storage/mysql-integration
---

# .NET Aspire MySQL database integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[MySQL](https://www.mysql.com/) is an open-source Relational Database Management System (RDBMS) that uses Structured Query Language (SQL) to manage and manipulate data. It's employed in a many different environments, from small projects to large-scale enterprise systems and it's a popular choice to host data that underpins microservices in a cloud-native application. The .NET Aspire MySQL database integration enables you to connect to existing MySQL Server databases or create new instances from .NET with the [`mysql` container image](https://hub.docker.com/_/mysql).

## Hosting integration

[!INCLUDE [mysql-app-host](includes/mysql-app-host.md)]

> AJMTODO: Integrate the following into the previous include?

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var mysql = builder.AddMySql("mysql");
var mysqldb = mysql.AddDatabase("mysqldb");

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(mysqldb);
```

When you want to explicitly provide a root MySQL password, you can provide it as a parameter. Consider the following alternative example:

```csharp
var password = builder.AddParameter("password", secret: true);

var mysql = builder.AddMySql("mysql", password);
var mysqldb = mysql.AddDatabase("mysqldb");

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(mysqldb);
```

For more information, see [External parameters](../fundamentals/external-parameters.md).

> AJMTODO Other host integration stuff?

## Client integration




## Prerequisites

- MySQL database and connection string for accessing the database.

## Get started

To get started with the .NET Aspire MySQL database integration, install the [📦 Aspire.MySqlConnector](https://www.nuget.org/packages/Aspire.MySqlConnector) NuGet package.

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

## Example usage

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the `AddMySqlDataSource` extension to register a `MySqlDataSource` for use via the dependency injection container.

```csharp
builder.AddMySqlDataSource("mysqldb");
```

To retrieve your `MySqlDataSource` object, consider the following example service:

```csharp
public class ExampleService(MySqlDataSource dataSource)
{
    // Use dataSource...
}
```

After adding a `MySqlDataSource`, you can require the `MySqlDataSource` instance using DI.

## Configuration

The .NET Aspire MySQL database integration provides multiple configuration approaches and options to meet the requirements and conventions of your project.

### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddMySqlDataSource()`:

```csharp
builder.AddMySqlDataSource("mysql");
```

Then the connection string will be retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "mysql": "Server=mysql;Database=mysqldb"
  }
}
```

For more information on how to format this connection string, see [MySqlConnector: ConnectionString documentation](https://mysqlconnector.net/connection-options/).

### Use configuration providers

The .NET Aspire MySQL database supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the `MySqlConnectorSettings` from configuration files such as _:::no-loc text="appsettings.json":::_ by using the `Aspire:MySqlConnector` key. If you have set up your configurations in the `Aspire:MySqlConnector` section, you can just call the method without passing any parameter.

The following example shows an _:::no-loc text="appsettings.json":::_ file that configures some of the available options:

```json
{
  "Aspire": {
    "MySqlConnector": {
      "DisableHealthChecks": true,
      "DisableTracing": true
    }
  }
}
```

### Use inline configurations

You can also pass the `Action<MySqlConnectorSettings>` delegate to set up some or all the options inline, for example to disable health checks from code:

```csharp
builder.AddMySqlDataSource("mysql",
    static settings => settings.DisableHealthChecks  = true);
```

### Configuration options

Here are the configurable options with corresponding default values:

| Name                  | Description                                                                           |
|-----------------------|---------------------------------------------------------------------------------------|
| `ConnectionString`    | The connection string of the MySQL database database to connect to.                   |
| `DisableHealthChecks` | A boolean value that indicates whether the database health check is disabled or not.  |
| `DisableTracing`      | A boolean value that indicates whether the OpenTelemetry tracing is disabled or not.  |
| `DisableMetrics`      | A boolean value that indicates whether the OpenTelemetry metrics are disabled or not. |

[!INCLUDE [integration-health-checks](../includes/integration-health-checks.md)]

By default, the .NET Aspire MySQL database integration handles the following:

- Adds a `MySqlHealthCheck`, which verifies that a connection can be made commands can be run against the MySql database.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

## See also

- [MySQL database](https://mysqlconnector.net/)
- [.NET Aspire database containers sample](/samples/dotnet/aspire-samples/aspire-database-containers/)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
