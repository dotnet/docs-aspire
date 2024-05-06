---
title: .NET Aspire MySQL database component
description: This article describes the .NET Aspire MySQL database component.
ms.topic: how-to
ms.date: 01/22/2024
---

# .NET Aspire MySQL database component

In this article, you learn how to use the .NET Aspire MySQL database component. The `Aspire.MySqlConnector` library:

- Registers a [MySqlDataSource](https://mysqlconnector.net/api/mysqlconnector/mysqldatasourcetype) in the DI container for connecting MySQL database.
- Automatically configures the following:
  - Health checks, logging and telemetry to improve app monitoring and diagnostics

## Prerequisites

- MySQL database and connection string for accessing the database.

## Get started

To get started with the .NET Aspire MySQL database component, install the [Aspire.MySqlConnector](https://www.nuget.org/packages/Aspire.MySqlConnector) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.MySqlConnector --prerelease
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.MySqlConnector"
                  Version="[SelectVersion]" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

In the _Program.cs_ file of your component-consuming project, call the `AddMySqlDataSource` extension to register a `MySqlDataSource` for use via the dependency injection container.

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

## App host usage

In your app host project, register a MySql database and consume the connection using the following methods:

```csharp
var mysql = builder.AddMySql("mysql");
var mysqldb = mysql.AddDatabase("mysqldb");

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(mysqldb);
```

## Configuration

The .NET Aspire MySQL database component provides multiple configuration approaches and options to meet the requirements and conventions of your project.

### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddMySqlDataSource()`:

```csharp
builder.AddMySqlDataSource("MySqConnection");
```

Then the connection string will be retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "MySqConnection": "Server=mysql;Database=mysqldb"
  }
}
```

For more information on how to format this connection string, see [MySqlConnector: ConnectionString documentation](https://mysqlconnector.net/connection-options/).

### Use configuration providers

The .NET Aspire MySQL database supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the `MySqlConnectorSettings` from configuration files such as _appsettings.json_ by using the `Aspire:MySqlConnector` key. If you have set up your configurations in the `Aspire:MySqlConnector` section, you can just call the method without passing any parameter.

The following example shows an _appsettings.json_ file that configures some of the available options:

```json
{
  "Aspire": {
    "MySqlConnector": {
      "HealthChecks": false,
      "Tracing": false
    }
  }
}
```

### Use inline configurations

You can also pass the `Action<MySqlConnectorSettings>` delegate to set up some or all the options inline, for example to disable health checks from code:

```csharp
builder.AddMySqlDataSource("mysql",
    static settings => settings.HealthChecks = false);
```

### Configuration options

Here are the configurable options with corresponding default values:

| Name               | Description                                                                          |
|--------------------|--------------------------------------------------------------------------------------|
| `ConnectionString` | The connection string of the MySQL database database to connect to.                  |
| `HealthChecks`     | A boolean value that indicates whether the database health check is enabled or not.  |
| `Tracing`          | A boolean value that indicates whether the OpenTelemetry tracing is enabled or not.  |
| `Metrics`          | A boolean value that indicates whether the OpenTelemetry metrics are enabled or not. |

[!INCLUDE [component-health-checks](../includes/component-health-checks.md)]

By default, the .NET Aspire MySQL database component handles the following:

- Adds a `MySqlHealthCheck`, which verifies that a connection can be made commands can be run against the MySql database.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic

[!INCLUDE [component-observability-and-telemetry](../includes/component-observability-and-telemetry.md)]

## See also

- [MySQL database](https://mysqlconnector.net/)
- [.NET Aspire components](../fundamentals/components-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
