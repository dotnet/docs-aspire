---
title: .NET Aspire NATS integration
description: Learn how to use the .NET Aspire NATS integration to send logs and traces to a NATS Server.
ms.date: 10/11/2024
uid: messaging/nats-integration
---

# .NET Aspire NATS integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[NATS](https://nats.io) is a high-performance, secure, distributed messaging system. The .NET Aspire NATS integration enables you to connect to existing NATS instances, or create new instances from .NET with the [`docker.io/library/nats` container image](https://hub.docker.com/_/nats).

## Hosting integration

NATS hosting integration for .NET Aspire models a NATS server as the <xref:Aspire.Hosting.ApplicationModel.NatsServerResource> type. To access this type, install the [📦 Aspire.Hosting.Nats](https://www.nuget.org/packages/Aspire.Hosting.Nats) NuGet package in the [app host](xref:dotnet/aspire/app-host) project, then add it with the builder.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Nats
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Nats"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add NATS server resource

In your app host project, call <xref:Aspire.Hosting.NatsBuilderExtensions.AddNats*> on the `builder` instance to add a NATS server resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var nats = builder.AddNats("nats");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(nats);

// After adding all resources, run the app...
```

When .NET Aspire adds a container image to the app host, as shown in the preceding example with the `docker.io/library/nats` image, it creates a new NATS server instance on your local machine. A reference to your NATS server (the `nats` variable) is added to the `ExampleProject`.

The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method configures a connection in the `ExampleProject` named `"nats"`. For more information, see [Container resource lifecycle](../fundamentals/app-host-overview.md#container-resource-lifecycle).

> [!TIP]
> If you'd rather connect to an existing NATS server, call <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> instead. For more information, see [Reference existing resources](../fundamentals/app-host-overview.md#reference-existing-resources).

### Add NATS server resource with JetStream

To add the [NATS JetStream](https://docs.nats.io/nats-concepts/jetstream) to the NATS server resource, call the <xref:Aspire.Hosting.NatsBuilderExtensions.WithJetStream*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var nats = builder.AddNats("nats");
                  .WithJetStream();

builder.AddProject<Projects.ExampleProject>()
       .WithReference(nats);

// After adding all resources, run the app...
```

The NATS JetStream functionality provides a built-in persistence engine called JetStream which enables messages to be stored and replayed at a later time. You can optionally provide a `srcMountPath` parameter to specify the path to the JetStream data directory on the host machine (the provided mount path maps to the container's `-sd` argument).

### Add NATS server resource with data volume

To add a data volume to the NATS server resource, call the <xref:Aspire.Hosting.NatsBuilderExtensions.WithDataVolume*> method on the NATS server resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var nats = builder.AddNats("nats");
                  .WithDataVolume(isReadOnly: false);

builder.AddProject<Projects.ExampleProject>()
       .WithReference(nats);

// After adding all resources, run the app...
```

The data volume is used to persist the NATS server data outside the lifecycle of its container. The data volume is mounted at the `/var/lib/nats` path in the NATS server container. A name is generated at random unless you provide a set the `name` parameter. For more information on data volumes and details on why they're preferred over [bind mounts](#add-nats-server-resource-with-data-bind-mount), see [Docker docs: Volumes](https://docs.docker.com/engine/storage/volumes).

### Add NATS server resource with data bind mount

To add a data bind mount to the NATS server resource, call the <xref:Aspire.Hosting.NatsBuilderExtensions.WithDataBindMount*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var nats = builder.AddNats("nats");
                  .WithDataBindMount(
                      source: @"C:\NATS\Data",
                      isReadOnly: false);

builder.AddProject<Projects.ExampleProject>()
       .WithReference(nats);

// After adding all resources, run the app...
```

[!INCLUDE [data-bind-mount-vs-volumes](../includes/data-bind-mount-vs-volumes.md)]

Data bind mounts rely on the host machine's filesystem to persist the NATS server data across container restarts. The data bind mount is mounted at the `C:\NATS\Data` on Windows (or `/NATS/Data` on Unix) path on the host machine in the NATS server container. For more information on data bind mounts, see [Docker docs: Bind mounts](https://docs.docker.com/engine/storage/bind-mounts).

### Hosting integration health checks

The NATS hosting integration automatically adds a health check for the NATS server resource. The health check verifies that the NATS server is running and that a connection can be established to it.

The hosting integration relies on the [📦 AspNetCore.HealthChecks.Nats](https://www.nuget.org/packages/AspNetCore.HealthChecks.Nats) NuGet package.

## Client integration

To get started with the .NET Aspire NATS client integration, install the [📦 Aspire.NATS.Net](https://www.nuget.org/packages/Aspire.NATS.Net) NuGet package in the client-consuming project, that is, the project for the application that uses the NATS client. The NATS client integration registers an [INatsConnection](https://nats-io.github.io/nats.net/api/NATS.Client.Core.INatsConnection.html) instance that you can use to interact with NATS.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.NATS.Net
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.NATS.Net"
                  Version="*" />
```

---

### Add NATS client

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireNatsClientExtensions.AddNatsClient*> extension method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register an `INatsConnection` for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddNatsClient(connectionName: "nats");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the NATS server resource in the app host project. For more information, see [Add NATS server resource](#add-nats-server-resource).

You can then retrieve the `INatsConnection` instance using dependency injection. For example, to retrieve the client from a service:

```csharp
public class ExampleService(INatsConnection connection)
{
    // Use connection...
}
```

For more information on dependency injection, see [.NET dependency injection](/dotnet/core/extensions/dependency-injection).

### Add keyed NATS client

There might be situations where you want to register multiple `INatsConnection` instances with different connection names. To register keyed NATS clients, call the <xref:Microsoft.Extensions.Hosting.AspireNatsClientExtensions.AddKeyedNatsClient*> method:

```csharp
builder.AddKeyedNatsClient(name: "chat");
builder.AddKeyedNatsClient(name: "queue");
```

Then you can retrieve the `IConnection` instances using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(
    [FromKeyedServices("chat")] INatsConnection chatConnection,
    [FromKeyedServices("queue")] INatsConnection queueConnection)
{
    // Use connections...
}
```

For more information on keyed services, see [.NET dependency injection: Keyed services](/dotnet/core/extensions/dependency-injection#keyed-services).

### Configuration

The .NET Aspire NATS integration provides multiple options to configure the NATS connection based on the requirements and conventions of your project.

#### Use a connection string

Provide the name of the connection string when you call `builder.AddNatsClient`:

```csharp
builder.AddNatsClient(connectionName: "nats");
```

The connection string is retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "nats": "nats://nats:4222"
  }
}
```

See the [ConnectionString documentation](https://docs.nats.io/using-nats/developer/connecting#nats-url) for more information on how to format this connection string.

#### Use configuration providers

The .NET Aspire NATS integration supports <xref:Microsoft.Extensions.Configuration>. It loads the <xref:Aspire.NATS.Net.NatsClientSettings> from configuration by using the `Aspire:Nats:Client` key. The following snippet is an example of a _:::no-loc text="appsettings.json":::_ file that configures some of the options:

```json
{
  "Aspire": {
    "Nats": {
      "Client": {
        "ConnectionString": "nats://nats:4222",
        "DisableHealthChecks": true,
        "DisableTracing": true
      }
    }
  }
}
```

For the complete NATS client integration JSON schema, see [Aspire.NATS.Net/ConfigurationSchema.json](https://github.com/dotnet/aspire/blob/v8.2.1/src/Components/Aspire.NATS.Net/ConfigurationSchema.json).

#### Use inline delegates

Pass the `Action<NatsClientSettings> configureSettings` delegate to set up some or all the options inline, for example to disable health checks from code:

```csharp
builder.AddNatsClient(
    "nats",
    static settings => settings.DisableHealthChecks  = true);
```

### NATS in the .NET Aspire manifest

NATS isn't part of the .NET Aspire [deployment manifest](../deployment/manifest-format.md). It's recommended you set up a secure production NATS server outside of .NET Aspire.

### Client integration health checks

By default, .NET Aspire integrations enable [health checks](../fundamentals/health-checks.md) for all services. For more information, see [.NET Aspire integrations overview](../fundamentals/integrations-overview.md).

The .NET Aspire NATS integration handles the following:

- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic.

### Observability and telemetry

.NET Aspire integrations automatically set up Logging, Tracing, and Metrics configurations, which are sometimes known as *the pillars of observability*. For more information about integration observability and telemetry, see [.NET Aspire integrations overview](../fundamentals/integrations-overview.md). Depending on the backing service, some integrations may only support some of these features. For example, some integrations support logging and tracing, but not metrics. Telemetry features can also be disabled using the techniques presented in the [Configuration](#configuration) section.

### Logging

The .NET Aspire NATS integration uses the following log categories:

- `NATS`

### Tracing

The .NET Aspire NATS integration emits the following tracing activities:

- `NATS.Net`

## See also

- [NATS.Net quickstart](https://nats-io.github.io/nats.net/documentation/intro.html?tabs=core-nats)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
