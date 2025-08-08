---
title: .NET Aspire Community Toolkit EventStore integration
description: Learn how to use the .NET Aspire EventStore hosting and client integration to run the EventStore container and accessing it via the EventStore client.
ms.date: 08/07/2025
ms.custom: sfi-ropc-nochange
---

# .NET Aspire Community Toolkit EventStore integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[!INCLUDE [banner](includes/banner.md)]

In this article, you learn how to use the .NET Aspire EventStore hosting integration to run [EventStore](https://eventstore.com) container and accessing it via the [EventStore](https://github.com/EventStore/EventStore-Client-Dotnet) client.

## Hosting integration

To run the EventStore container, install the [📦 CommunityToolkit.Aspire.Hosting.EventStore][hosting-nuget-link] NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package CommunityToolkit.Aspire.Hosting.EventStore
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="CommunityToolkit.Aspire.Hosting.EventStore"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add EventStore resource

In the AppHost project, register and consume the EventStore integration using the `AddEventStore` extension method to add the EventStore container to the application builder.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var eventstore = builder.AddEventStore("eventstore");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(eventstore);

// After adding all resources, run the app...
```

When .NET Aspire adds a container image to the AppHost, as shown in the preceding example with the `docker.io/eventstore/eventstore` image, it creates a new EventStore instance on your local machine. A reference to your EventStore resource (the `eventstore` variable) is added to the `ExampleProject`.

For more information, see [Container resource lifecycle](../fundamentals/orchestrate-resources.md#container-resource-lifecycle).

### Add EventStore resource with data volume

To add a data volume to the EventStore resource, call the `Aspire.Hosting.EventStoreBuilderExtensions.WithDataVolume` method on the EventStore resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var eventstore = builder.AddEventStore("eventstore")
                        .WithDataVolume();

builder.AddProject<Projects.ExampleProject>()
       .WithReference(eventstore);

// After adding all resources, run the app...
```

The data volume is used to persist the EventStore data outside the lifecycle of its container. The data volume is mounted at the `/var/lib/eventstore` path in the EventStore container and when a `name` parameter isn't provided, the name is generated at random. For more information on data volumes and details on why they're preferred over [bind mounts](#add-eventstore-resource-with-data-bind-mount), see [Docker docs: Volumes](https://docs.docker.com/engine/storage/volumes).

### Add EventStore resource with data bind mount

To add a data bind mount to the EventStore resource, call the `Aspire.Hosting.EventStoreBuilderExtensions.WithDataBindMount` method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var eventstore = builder.AddEventStore("eventstore")
                        .WithDataBindMount(source: @"C:\EventStore\Data");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(eventstore);

// After adding all resources, run the app...
```

[!INCLUDE [data-bind-mount-vs-volumes](../includes/data-bind-mount-vs-volumes.md)]

Data bind mounts rely on the host machine's filesystem to persist the EventStore data across container restarts. The data bind mount is mounted at the `C:\EventStore\Data` on Windows (or `/EventStore/Data` on Unix) path on the host machine in the EventStore container. For more information on data bind mounts, see [Docker docs: Bind mounts](https://docs.docker.com/engine/storage/bind-mounts).

### Add EventStore resource with log volume

To add a log volume to the EventStore resource, call the <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.WithVolume*> extension method on the EventStore resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var eventstore = builder.AddEventStore("eventstore")
                        .WithVolume(name: "eventstore_logs", target: "/var/log/eventstore");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(eventstore);

// After adding all resources, run the app...
```

The data volume is used to persist the EventStore logs outside the lifecycle of its container. The data volume must be mounted at the `/var/log/eventstore` target path in the EventStore container and when a `name` parameter isn't provided, the name is generated at random. For more information on data volumes and details on why they're preferred over [bind mounts](#add-eventstore-resource-with-log-bind-mount), see [Docker docs: Volumes](https://docs.docker.com/engine/storage/volumes).

For more information about EventStore logs location, see [EventStore Resources: Logs](https://developers.eventstore.com/server/v24.10/diagnostics/logs.html#logs-location).

### Add EventStore resource with log bind mount

To add a log bind mount to the EventStore resource, call the <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.WithBindMount*> extension method on the EventStore resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var eventstore = builder.AddEventStore("eventstore")
                        .WithBindMount(@"C:\EventStore\Logs", "/var/log/eventstore");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(eventstore);

// After adding all resources, run the app...
```

[!INCLUDE [data-bind-mount-vs-volumes](../includes/data-bind-mount-vs-volumes.md)]

Data bind mounts rely on the host machine's filesystem to persist the EventStore logs across container restarts. The data bind mount is mounted at the `C:\EventStore\Logs` on Windows (or `/EventStore/Logs` on Unix) path on the host machine in the EventStore container. The target path must be set to the log folder used by the EventStore container (`/var/log/eventstore`).

For more information about EventStore logs location, see [EventStore Resources: Logs](https://developers.eventstore.com/server/v24.10/diagnostics/logs.html#logs-location).

For more information on data bind mounts, see [Docker docs: Bind mounts](https://docs.docker.com/engine/storage/bind-mounts).

## Client integration

To get started with the .NET Aspire EventStore client integration, install the [📦 CommunityToolkit.Aspire.EventStore][client-nuget-link] NuGet package in the client-consuming project, that is, the project for the application that uses the EventStore client.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package CommunityToolkit.Aspire.EventStore
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="CommunityToolkit.Aspire.EventStore"
                  Version="*" />
```

---

### Add EventStore client

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the `Microsoft.Extensions.Hosting.AspireEventStoreExtensions.AddEventStoreClient` extension method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register an `EventStoreClient` for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddEventStoreClient(connectionName: "eventstore");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the EventStore resource in the AppHost project. For more information, see [Add EventStore resource](#add-eventstore-resource).

You can then retrieve the `EventStoreClient` instance using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(EventStoreClient client)
{
    // Use client...
}
```

### Add keyed EventStore client

There might be situations where you want to register multiple `EventStoreClient` instances with different connection names. To register keyed EventStore clients, call the `Microsoft.Extensions.Hosting.AspireEventStoreExtensions.AddKeyedEventStoreClient`

```csharp
builder.AddKeyedEventStoreClient(name: "accounts");
builder.AddKeyedEventStoreClient(name: "orders");
```

Then you can retrieve the `EventStoreClient` instances using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(
    [FromKeyedServices("accounts")] EventStoreClient accountsClient,
    [FromKeyedServices("orders")] EventStoreClient ordersClient)
{
    // Use clients...
}
```

For more information on keyed services, see [.NET dependency injection: Keyed services](/dotnet/core/extensions/dependency-injection#keyed-services).

### Configuration

The .NET Aspire EventStore client integration provides multiple options to configure the server connection based on the requirements and conventions of your project.

#### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddEventStoreClient`:

```csharp
builder.AddEventStoreClient("eventstore");
```

Then the connection string will be retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "eventstore": "esdb://localhost:22113?tls=false"
  }
}
```

#### Use configuration providers

The .NET Aspire EventStore Client integration supports <xref:Microsoft.Extensions.Configuration>. It loads the `CommunityToolkit.Aspire.EventStore.EventStoreSettings` from configuration by using the `Aspire:EventStore:Client` key. Consider the following example _appsettings.json_ that configures some of the options:

```json
{
  "Aspire": {
    "EventStore": {
      "Client": {
        "ConnectionString": "esdb://localhost:22113?tls=false",
        "DisableHealthChecks": true
      }
    }
  }
}
```

#### Use inline delegates

Also you can pass the `Action<EventStoreSettings> configureSettings` delegate to set up some or all the options inline, for example to set the API key from code:

```csharp
builder.AddEventStoreClient(
    "eventstore",
    static settings => settings.DisableHealthChecks = true);
```

#### Client integration health checks

The .NET Aspire EventStore integration uses the configured client to perform a `IsHealthyAsync`. If the result is `true`, the health check is considered healthy, otherwise it's unhealthy. Likewise, if there's an exception, the health check is considered unhealthy with the error propagating through the health check failure.

## See also

- [EventStore](https://eventstore.com)
- [EventStore Client](https://github.com/EventStore/EventStore-Client-Dotnet)
- [.NET Aspire Community Toolkit GitHub repo](https://github.com/CommunityToolkit/Aspire)

[hosting-nuget-link]: https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.EventStore
[client-nuget-link]: https://nuget.org/packages/CommunityToolkit.Aspire.EventStore
