---
title: .NET Aspire MongoDB database component
description: This article describes the .NET Aspire MongoDB database component.
ms.topic: how-to
ms.date: 01/22/2024
---

# .NET Aspire MongoDB database component

In this article, you learn how to use the .NET Aspire MongoDB database component. The `Aspire.MongoDB.Driver` library:

- Registers a [IMongoClient](https://www.mongodb.com/docs/drivers/csharp/current/quick-start/#add-mongodb-as-a-dependency) in the DI container for connecting MongoDB database.
- Automatically configures the following:
  - Health checks, logging and telemetry to improve app monitoring and diagnostics

## Prerequisites

- MongoDB database and connection string for accessing the database.

## Get started

To get started with the .NET Aspire MongoDB database component, install the [Aspire.MongoDB.Driver](https://www.nuget.org/packages/Aspire.MongoDB.Driver) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.MongoDB.Driver --prerelease
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.MongoDB.Driver"
                  Version="[SelectVersion]" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

In the _Program.cs_ file of your component-consuming project, call the `AddMongoDBClient` extension to register a `IMongoClient` for use via the dependency injection container.

```csharp
builder.AddMongoDBClient("mongodb");
```

To retrieve your `IMongoClient` object, consider the following example service:

```csharp
public class ExampleService(IMongoClient mongoClient)
{
    // Use mongoClient...
}
```

After adding a `IMongoClient`, you can require the `IMongoClient` instance using DI.

## App host usage

In your app host project, register a MongoDB database and consume the connection using the following methods:

```csharp
var mongo = builder.AddMongoDB("mongo");
var mongodb = mongo.AddDatabase("mongodb");

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(mongodb);
```

## Configuration

The .NET Aspire MongoDB database component provides multiple configuration approaches and options to meet the requirements and conventions of your project.

### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddMongoDBClient()`:

```csharp
builder.AddMongoDBClient("MongoConnection");
```

And then the connection string will be retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "MongoConnection": "mongodb://server:port/test",
  }
}
```

For more information on how to format this connection string, see [MongoDB: ConnectionString documentation](https://www.mongodb.com/docs/v3.0/reference/connection-string).

### Use configuration providers

The .NET Aspire MongoDB database component supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the `MySqlConnectorSettings` from configuration files such as _appsettings.json_ by using the `Aspire:MongoDB:Driver` key. If you have set up your configurations in the `Aspire:MongoDB:Driver` section, you can just call the method without passing any parameter.

The following example shows an _appsettings.json_ file that configures some of the available options:

```json
{
  "Aspire": {
    "MongoDB": {
      "Driver": {
        "ConnectionString": "mongodb://server:port/test",
        "DisableHealthChecks": false,
        "HealthCheckTimeout": 10000,
        "DisableTracing": false
      },
    }
  }
}
```

### Use inline configurations

You can also pass the `Action<MongoDBSettings>` delegate to set up some or all the options inline:

```csharp
builder.AddMongoDBClient("mongodb",
    static settings => settings.ConnectionString = "mongodb://server:port/test");
```

### Configuration options

Here are the configurable options with corresponding default values:

| Name                  | Description                                                                           |
|-----------------------|---------------------------------------------------------------------------------------|
| `ConnectionString`    | The connection string of the MongoDB database database to connect to.                 |
| `DisableHealthChecks` | A boolean value that indicates whether the database health check is disabled or not.  |
| `HealthCheckTimeout`  | An `int?` value that indicates the MongoDB health check timeout in milliseconds.      |
| `DisableTracing`      | A boolean value that indicates whether the OpenTelemetry tracing is disabled or not.  |

[!INCLUDE [component-health-checks](../includes/component-health-checks.md)]

By default, the .NET Aspire MongoDB database component handles the following:

- Adds a health check when enabled that verifies that a connection can be made commands can be run against the MongoDB database within a certain amount of time.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic

[!INCLUDE [component-observability-and-telemetry](../includes/component-observability-and-telemetry.md)]

### Logging

The .NET Aspire MongoDB database component uses standard .NET logging, and you'll see log entries from the following categories:

- `MongoDB[.*]`: Any log entries from the MongoDB namespace.

### Tracing

The .NET Aspire MongoDB database component will emit the following Tracing activities using OpenTelemetry:

- "MongoDB.Driver.Core.Extensions.DiagnosticSources"

### Metrics

The .NET Aspire MongoDB database component doesn't currently expose any OpenTelemetry metrics.

## See also

- [MongoDB database](https://www.mongodb.com/docs/drivers/csharp/current/quick-start)
- [.NET Aspire components](../fundamentals/components-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
