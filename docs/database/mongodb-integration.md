---
title: .NET Aspire MongoDB database integration
description: This article describes the .NET Aspire MongoDB database integration.
ms.topic: how-to
ms.date: 08/12/2024
---

# .NET Aspire MongoDB database integration

In this article, you learn how to use the .NET Aspire MongoDB database integration. The `Aspire.MongoDB.Driver` library:

- Registers a [IMongoClient](https://www.mongodb.com/docs/drivers/csharp/current/quick-start/#add-mongodb-as-a-dependency) in the DI container for connecting to a MongoDB database.
- Automatically configures the following:
  - Health checks, logging and telemetry to improve app monitoring and diagnostics
- It supports both a local MongoDB Database and a [MongoDB Atlas](https://mdb.link/atlas) database deployed in the cloud.

## Prerequisites

- MongoDB database and connection string for accessing the database.

## Get started

To get started with the .NET Aspire MongoDB database integration, install the [Aspire.MongoDB.Driver](https://www.nuget.org/packages/Aspire.MongoDB.Driver) NuGet package in the client-consuming project, i.e., the project for the application that uses the MongoDB database client.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.MongoDB.Driver
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.MongoDB.Driver"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the `AddMongoDBClient` extension to register a `IMongoClient` for use via the dependency injection container.

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

To model the MongoDB resource in the app host, install the [Aspire.Hosting.MongoDB](https://www.nuget.org/packages/Aspire.Hosting.MongoDB) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.MongoDB
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.MongoDB"
                  Version="*" />
```

---

In your app host project, register the MongoDB database and consume the connection method and consume the service using the following methods:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var mongo = builder.AddMongoDB("mongo");
var mongodb = mongo.AddDatabase("mongodb");

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(mongodb);
```

If using MongoDB Atlas, you don't need the call to `.AddDatabase("mongodb");` as the database creation will automatically be handled by Atlas.

## Configuration

The .NET Aspire MongoDB database integration provides multiple configuration approaches and options to meet the requirements and conventions of your project.

### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddMongoDBClient()`:

```csharp
builder.AddMongoDBClient("MongoConnection");
```

And then the connection string will be retrieved from the `ConnectionStrings` configuration section:

Consider the following MongoDB example JSON configuration:

```json
{
  "ConnectionStrings": {
    "MongoConnection": "mongodb://server:port/test",
  }
}
```

Consider the following MongoDB Atlas example JSON configuration:

```json
{
  "ConnectionStrings": {
    "MongoConnection": "mongodb+srv://username:password@server.mongodb.net/",
  }
}
```

For more information on how to format this connection string, see [MongoDB: ConnectionString documentation](https://www.mongodb.com/docs/v3.0/reference/connection-string).

### Use configuration providers

The .NET Aspire MongoDB database integration supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the `MongoDBSettings` from configuration files such as _:::no-loc text="appsettings.json":::_ by using the `Aspire:MongoDB:Driver` key.

The following example shows an _:::no-loc text="appsettings.json":::_ file that configures some of the available options:

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

[!INCLUDE [integration-health-checks](../includes/integration-health-checks.md)]

By default, the .NET Aspire MongoDB database integration handles the following:

- Adds a health check when enabled that verifies that a connection can be made commands can be run against the MongoDB database within a certain amount of time.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

### Logging

The .NET Aspire MongoDB database integration uses standard .NET logging, and you'll see log entries from the following categories:

- `MongoDB[.*]`: Any log entries from the MongoDB namespace.

### Tracing

The .NET Aspire MongoDB database integration will emit the following Tracing activities using OpenTelemetry:

- "MongoDB.Driver.Core.Extensions.DiagnosticSources"

### Metrics

The .NET Aspire MongoDB database integration doesn't currently expose any OpenTelemetry metrics.

## See also

- [MongoDB database](https://www.mongodb.com/docs/drivers/csharp/current/quick-start)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
