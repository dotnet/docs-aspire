---
title: .NET Aspire NATS integration
description: Learn how to use the .NET Aspire NATS integration to send logs and traces to a NATS Server.
ms.topic: how-to
ms.date: 08/12/2024
---

# .NET Aspire NATS integration

In this article, you learn how to use the .NET Aspire NATS integration to send logs and traces to a NATS Server. The integration supports persistent logs and traces across application restarts via configuration.

## Prerequisites

- [Install the NATS server](https://docs.nats.io/running-a-nats-service/introduction/installation)
- The URL to access the server.

## Get started

To get started with the .NET Aspire NATS integration, install the [Aspire.NATS.Net](https://www.nuget.org/packages/Aspire.NATS.Net) NuGet package in the client-consuming project, i.e., the project for the application that uses the NATS client.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.NATS.Net
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.NATS.Net"
                  Version="[SelectVersion]" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

In the _:::no-loc text="Program.cs":::_ file of your projects, call the `AddNatsClient` extension method to register an `INatsConnection` to send logs and traces to NATS and the .NET Aspire Dashboard. The method takes a connection name parameter.

```csharp
builder.AddNatsClient("nats");
```

You can then retrieve the `INatsConnection` instance using dependency injection. For example, to retrieve the client from a service:

```csharp
public class ExampleService(INatsConnection client)
{
    // Use client...
}
```

## App host usage

To model the Nats resource in the app host, install the [Aspire.Hosting.Nats](https://www.nuget.org/packages/Aspire.Hosting.Nats) NuGet package in the [app host](xref:aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Nats
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Nats"
                  Version="[SelectVersion]" />
```

---

Then, in the _:::no-loc text="Program.cs":::_ file of `AppHost`, register a NATS server and consume the connection using the following methods:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var nats = builder.AddNats("nats");

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(nats);
```

The `WithReference` method configures a connection in the `MyService` project named `nats`. In the _:::no-loc text="Program.cs":::_ file of `MyService`, the NATS connection can be consumed using:

```csharp
builder.AddNatsClient("nats");
```

## Configuration

The .NET Aspire NATS integration provides multiple options to configure the NATS connection based on the requirements and conventions of your project.

### Use a connection string

Provide the name of the connection string when you call `builder.AddNatsClient()`:

```csharp
builder.AddNatsClient("myConnection");
```

The connection string is retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "myConnection": "nats://nats:4222"
  }
}
```

See the [ConnectionString documentation](https://docs.nats.io/using-nats/developer/connecting#nats-url) for more information on how to format this connection string.

### Use configuration providers

The .NET Aspire NATS integration supports [Microsoft.Extensions.Configuration](/dotnet/api/microsoft.extensions.configuration). It loads the `NatsClientSettings` from configuration using the `Aspire:Nats:Client` key. Example _:::no-loc text="appsettings.json":::_ that configures some of the options:

```json
{
  "Aspire": {
    "Nats": {
      "Client": {
        "DisableHealthChecks": true
      }
    }
  }
}
```

### Use inline delegates

Pass the `Action<NatsClientSettings> configureSettings` delegate to set up some or all the options inline, for example to disable health checks from code:

```csharp
builder.AddNatsClient("nats", settings => settings.DisableHealthChecks  = true);
```

### Persistent logs and traces

Register NATS with a data directory in your **.AppHost** project to retain NATS's data and configuration across application restarts.

```csharp
var NATS = builder.AddNATS("NATS", NATSDataDirectory: "./NATSdata");
```

The directory specified must already exist.

### NATS in the .NET Aspire manifest

NATS isn't part of the .NET Aspire deployment manifest. It's recommended you set up a secure production NATS server outside of .NET Aspire.

[!INCLUDE [integration-health-checks](../includes/integration-health-checks.md)]

The .NET Aspire NATS integration handles the following:

- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic.

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

### Logging

The .NET Aspire NATS integration uses the following log categories:

- `NATS`

### Tracing

The .NET Aspire NATS integration emits the following tracing activities:

- `NATS.Net`

## See also

- [NATS.Net quickstart](https://nats-io.github.io/nats.net/documentation/intro.html?tabs=core-nats)
- [NATS integration README](https://github.com/dotnet/aspire/tree/main/src/Components/README.md)
