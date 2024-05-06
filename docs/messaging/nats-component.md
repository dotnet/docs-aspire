---
title: .NET Aspire NATS component
description: Learn how to use the .NET Aspire NATS component to send logs and traces to a NATS Server.
ms.topic: how-to
ms.date: 04/09/2024
---

# .NET Aspire NATS component

In this article, you learn how to use the .NET Aspire NATS component to send logs and traces to a NATS Server. The component supports persistent logs and traces across application restarts via configuration.

## Prerequisites

- [Install the NATS server](https://docs.nats.io/running-a-nats-service/introduction/installation)
- The URL to access the server.

## Get started

To get started with the .NET Aspire NATS component, install the [Aspire.NATS.Net](https://www.nuget.org/packages/Aspire.NATS.Net) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.NATS.Net --prerelease
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.NATS.Net"
                  Version="[SelectVersion]" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

<!-- TODO: <xref:Microsoft.Extensions.Hosting.AspireNatsClientExtensions.AddNatsClient%2A>  -->

In the _Program.cs_ file of your projects, call the `AddNatsClient` extension method to register an `INatsConnection` to send logs and traces to NATS and the .NET Aspire Dashboard. The method takes a connection name parameter.

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

## Configuration

The .NET Aspire NATS component provides multiple options to configure the NATS connection based on the requirements and conventions of your project.

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

The .NET Aspire NATS component supports [Microsoft.Extensions.Configuration](/dotnet/api/microsoft.extensions.configuration). It loads the `NatsClientSettings` from configuration using the `Aspire:Nats:Client` key. Example `appsettings.json` that configures some of the options:

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

## AppHost extensions

In your AppHost project, install the `Aspire.Hosting.Nats` library with [NuGet](https://www.nuget.org):

```dotnetcli
dotnet add package Aspire.Hosting.Nats
```

Then, in the _Program.cs_ file of `AppHost`, register a NATS server and consume the connection using the following methods:

```csharp
var nats = builder.AddNats("nats");

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(nats);
```

The `WithReference` method configures a connection in the `MyService` project named `nats`. In the _Program.cs_ file of `MyService`, the NATS connection can be consumed using:

```csharp
builder.AddNatsClient("nats");
```

### Persistent logs and traces

Register NATS with a data directory in your **.AppHost** project to retain NATS's data and configuration across application restarts.

```csharp
var NATS = builder.AddNATS("NATS", NATSDataDirectory: "./NATSdata");
```

The directory specified must already exist.

### NATS in the .NET Aspire manifest

NATS isn't part of the .NET Aspire deployment manifest. It's recommended you set up a secure production NATS server outside of .NET Aspire.

[!INCLUDE [component-health-checks](../includes/component-health-checks.md)]

The .NET Aspire NATS component handles the following:

- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic.

[!INCLUDE [component-observability-and-telemetry](../includes/component-observability-and-telemetry.md)]

### Logging

The .NET Aspire NATS component uses the following log categories:

- `NATS`

### Tracing

The .NET Aspire NATS component emits the following tracing activities:

- `NATS.Net`

## See also

- [NATS.Net quickstart](https://nats-io.github.io/nats.net.v2/documentation/intro.html?tabs=core-nats)
- [NATS component README](https://github.com/dotnet/aspire/tree/main/src/Components/README.md)
