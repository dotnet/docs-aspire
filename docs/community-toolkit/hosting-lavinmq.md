---
title: LavinMQ hosting extensions
description: Learn about the Aspire Community Toolkit LavinMQ hosting extensions package which provides extra functionality to the Aspire LavinMQ hosting package.
ms.date: 08/22/2025
ai-usage: ai-generated
---

# Aspire LavinMQ integration

[!INCLUDE [includes-hosting](../includes/includes-hosting.md)]

[!INCLUDE [banner](includes/banner.md)]

[LavinMQ](https://www.lavinmq.com/) is a reliable messaging and streaming broker, which is easy to deploy on cloud environments, on-premises, and on your local machine. The Aspire LavinMQ integration enables you to create new container instances from .NET with the [`docker.io/cloudamqp/lavinmq` container image](https://hub.docker.com/r/cloudamqp/lavinmq).

LavinMQ is a message broker built on Crystal. LavinMQ implements the AMQP protocol. Being a message queue software, also called a message broker, messages are published by a sending service called a producer, via the broker, to then be consumed by the receiving service called a consumer. Essentially, like a postal service, LavinMQ gives an organized, safe place for messages to wait until another application or part of the system can come along and consume them for processing. It is known to be a wire compatible messaging server with RabbitMQ, which means that you can use the RabbitMQ client to interact with LavinMQ.

## Hosting integration

The LavinMQ hosting integration models a LavinMQ server as the `Aspire.Hosting.ApplicationModel.LavinMQContainerResource` type. To access this type and its APIs add the [📦 CommunityToolkit.Aspire.Hosting.LavinMQ](https://www.nuget.org/packages/CommunityToolkit.Aspire.Hosting.LavinMQ) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package CommunityToolkit.Aspire.Hosting.LavinMQ
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="CommunityToolkit.Aspire.Hosting.LavinMQ"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add LavinMQ server resource

In your app host project, call `AddLavinMQ` on the `builder` instance to add a LavinMQ container resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var lavinMq = builder.AddLavinMQ("messaging");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(lavinMq);

// After adding all resources, run the app...
```

When Aspire adds a container image to the app host, as shown in the preceding example with the `docker.io/cloudamqp/lavinmq` image, it creates a new LavinMQ server instance on your local machine. A reference to your LavinMQ server (the `lavinMq` variable) is added to the `ExampleProject`. The LavinMQ server resource includes default credentials with a `username` of `"guest"` and a `password` of `"guest"`.

The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method configures a connection in the `ExampleProject` named `"messaging"`. For more information, see [Built-in resource types](../fundamentals/app-host-overview.md#built-in-resource-types).

> [!TIP]
> If you'd rather connect to an existing LavinMQ server, call <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> instead. For more information, see [Reference existing resources](../fundamentals/app-host-overview.md#reference-existing-resources).

### LavinMQ container resource management plugin

LavinMQ includes a management interface by default that provides a web-based interface for monitoring and managing the message broker.

### Add LavinMQ server resource with data volume

To add a data volume to the LavinMQ container resource, call the `Aspire.Hosting.LavinMQBuilderExtensions.WithDataVolume` method on the LavinMQ container resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var lavinMq = builder.AddLavinMQ("messaging")
                     .WithDataVolume(isReadOnly: false);

builder.AddProject<Projects.ExampleProject>()
        .WithReference(lavinMq);

// After adding all resources, run the app...
```

The data volume is used to persist the LavinMQ server data outside the lifecycle of its container. The data volume is mounted at the `/var/lib/lavinmq` path in the LavinMQ server container and when a `name` parameter isn't provided, the name is generated at random. For more information on data volumes and details on why they're preferred over [bind mounts](#add-lavinmq-server-resource-with-data-bind-mount), see [Docker docs: Volumes](https://docs.docker.com/engine/storage/volumes).

### Add LavinMQ server resource with data bind mount

To add a data bind mount to the LavinMQ container resource, call the `Aspire.Hosting.LavinMQBuilderExtensions.WithDataBindMount` method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var lavinMq = builder.AddLavinMQ("messaging")
                     .WithDataBindMount(
                         source: @"C:\LavinMQ\Data",
                         isReadOnly: false);

builder.AddProject<Projects.ExampleProject>()
        .WithReference(lavinMq);

// After adding all resources, run the app...
```

[!INCLUDE [data-bind-mount-vs-volumes](../includes/data-bind-mount-vs-volumes.md)]

Data bind mounts rely on the host machine's filesystem to persist the LavinMQ server data across container restarts. The data bind mount is mounted at the `C:\LavinMQ\Data` on Windows (or `/LavinMQ/Data` on Unix) path on the host machine in the LavinMQ server container. For more information on data bind mounts, see [Docker docs: Bind mounts](https://docs.docker.com/engine/storage/bind-mounts).

### Hosting integration health checks

The LavinMQ hosting integration automatically adds a health check for the LavinMQ server resource. The health check verifies that the LavinMQ server is running and that a connection can be established to it.

Because LavinMQ is wire compatible with AMQP 0.9.1, we can leverage the existing RabbitMQ health check integration.

The hosting integration relies on the [📦 AspNetCore.HealthChecks.RabbitMQ](https://www.nuget.org/packages/AspNetCore.HealthChecks.RabbitMQ) NuGet package.

## Client integration

LavinMQ is wire compatible with RabbitMQ. This may introduce a bit of confusion at first, but think of LavinMQ as a RabbitMQ server with a different name in terms of the .NET client integration. You can see an example of using the explicit RabbitMQ.Client in the [LavinMQ .NET sample code documentation](https://lavinmq.com/documentation/dot-net-sample-code). To get started with the Aspire RabbitMQ client integration, install the [📦 Aspire.RabbitMQ.Client](https://www.nuget.org/packages/Aspire.RabbitMQ.Client) NuGet package in the client-consuming project, that is, the project for the application that uses the LavinMQ client. The LavinMQ client integration registers an [IConnection](https://www.rabbitmq.com/dotnet-api-guide.html) instance that you can use to interact with LavinMQ.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.RabbitMQ.Client
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.RabbitMQ.Client"
                  Version="*" />
```

---

### Add LavinMQ/RabbitMQ client

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireRabbitMQExtensions.AddRabbitMQClient%2A> extension method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register an `IConnection` for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddRabbitMQClient(connectionName: "messaging");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the LavinMQ container resource in the app host project. For more information, see [Add LavinMQ server resource](#add-lavinmq-server-resource).

You can then retrieve the `IConnection` instance using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(IConnection connection)
{
    // Use connection...
}
```

For more information on dependency injection, see [.NET dependency injection](/dotnet/core/extensions/dependency-injection).

### Add keyed RabbitMQ client

There might be situations where you want to register multiple `IConnection` instances with different connection names. To register keyed LavinMQ clients, call the <xref:Microsoft.Extensions.Hosting.AspireRabbitMQExtensions.AddKeyedRabbitMQClient*> method:

```csharp
builder.AddKeyedRabbitMQClient(name: "chat");
builder.AddKeyedRabbitMQClient(name: "queue");
```

Then you can retrieve the `IConnection` instances using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(
    [FromKeyedServices("chat")] IConnection chatConnection,
    [FromKeyedServices("queue")] IConnection queueConnection)
{
    // Use connections...
}
```

For more information on keyed services, see [.NET dependency injection: Keyed services](/dotnet/core/extensions/dependency-injection#keyed-services).

### Configuration

The Aspire RabbitMQ integration provides multiple options to configure the connection based on the requirements and conventions of your project.

#### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling the <xref:Microsoft.Extensions.Hosting.AspireRabbitMQExtensions.AddRabbitMQClient*> method:

```csharp
builder.AddRabbitMQClient(connectionName: "messaging");
```

Then the connection string is retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "messaging": "amqp://username:password@localhost:5672"
  }
}
```

For more information on how to format this connection string, see the [RabbitMQ URI specification docs](https://www.rabbitmq.com/uri-spec.html).

#### Use configuration providers

The Aspire LavinMQ integration supports <xref:Microsoft.Extensions.Configuration>. It loads the <xref:Aspire.RabbitMQ.Client.RabbitMQClientSettings> from configuration by using the `Aspire:RabbitMQ:Client` key. The following snippet is an example of a _:::no-loc text="appsettings.json":::_ file that configures some of the options:

```json
{
  "Aspire": {
    "RabbitMQ": {
      "Client": {
        "ConnectionString": "amqp://username:password@localhost:5672",
        "DisableHealthChecks": true,
        "DisableTracing": true,
        "MaxConnectRetryCount": 2
      }
    }
  }
}
```

For the complete RabbitMQ client integration JSON schema, see [Aspire.RabbitMQ.Client/ConfigurationSchema.json](https://github.com/dotnet/aspire/blob/v9.0.0/src/Components/Aspire.RabbitMQ.Client/ConfigurationSchema.json).

#### Use inline delegates

Also you can pass the `Action<RabbitMQClientSettings> configureSettings` delegate to set up some or all the options inline, for example to disable health checks from code:

```csharp
builder.AddRabbitMQClient(
    "messaging",
    static settings => settings.DisableHealthChecks = true);
```

You can also set up the [IConnectionFactory](https://rabbitmq.github.io/rabbitmq-dotnet-client/api/RabbitMQ.Client.IConnectionFactory.html) using the `Action<IConnectionFactory> configureConnectionFactory` delegate parameter of the `AddRabbitMQClient` method. For example to set the client provided name for connections:

```csharp
builder.AddRabbitMQClient(
    "messaging",
    configureConnectionFactory:
        static factory => factory.ClientProvidedName = "MyApp");
```

### Client integration health checks

By default, Aspire integrations enable [health checks](../fundamentals/health-checks.md) for all services. For more information, see [Aspire integrations overview](../fundamentals/integrations-overview.md).

The Aspire RabbitMQ integration:

- Adds the health check when <xref:Aspire.RabbitMQ.Client.RabbitMQClientSettings.DisableHealthChecks?displayProperty=nameWithType> is `false`, which attempts to connect to and create a channel on the LavinMQ server.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic.

### Observability and telemetry

Aspire integrations automatically set up Logging, Tracing, and Metrics configurations, which are sometimes known as *the pillars of observability*. For more information about integration observability and telemetry, see [Aspire integrations overview](../fundamentals/integrations-overview.md). Depending on the backing service, some integrations might only support some of these features. For example, some integrations support logging and tracing, but not metrics. Telemetry features can also be disabled using the techniques presented in the [Configuration](#configuration) section.

#### Logging

The Aspire RabbitMQ integration uses the following log categories:

- `RabbitMQ.Client`

#### Tracing

The Aspire RabbitMQ integration emits the following tracing activities using OpenTelemetry:

- `Aspire.RabbitMQ.Client`

#### Metrics

The Aspire RabbitMQ integration currently doesn't support metrics by default.

## See also

- [Send messages with RabbitMQ in Aspire](/training/modules/send-messages-rabbitmq-dotnet-aspire-app)
- [RabbitMQ .NET Client docs](https://rabbitmq.github.io/rabbitmq-dotnet-client)
- [Aspire integrations](../fundamentals/integrations-overview.md)
- [Aspire GitHub repo](https://github.com/dotnet/aspire)
