---
title: .NET Aspire RabbitMQ integration
description: Learn how to use the .NET Aspire RabbitMQ message-broker integration, which includes both hosting and client integrations.
ms.date: 10/11/2024
uid: messaging/rabbitmq-integration
---

# .NET Aspire RabbitMQ integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[RabbitMQ](https://www.rabbitmq.com/) is a reliable messaging and streaming broker, which is easy to deploy on cloud environments, on-premises, and on your local machine. The .NET Aspire RabbitMQ integration enables you to connect to existing RabbitMQ instances, or create new instances from .NET with the [`docker.io/library/rabbitmq` container image](https://hub.docker.com/_/rabbitmq).

<!-- 
TODO: Diagram showing the thing we're integrating with and the components of the integration.
-->

## Hosting integration

The RabbitMQ hosting integration models a RabbitMQ server as the <xref:Aspire.Hosting.ApplicationModel.RabbitMQServerResource> type. To access this type and APIs that allow you to add it to your [app model](xref:dotnet/aspire/app-host#define-the-app-model), install the [📦 Aspire.Hosting.RabbitMQ](https://www.nuget.org/packages/Aspire.Hosting.RabbitMQ) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.RabbitMQ
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.RabbitMQ"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add RabbitMQ server resource

In your app host project, call <xref:Aspire.Hosting.RabbitMQBuilderExtensions.AddRabbitMQ*> on the `builder` instance to add a RabbitMQ server resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var rabbitmq = builder.AddRabbitMQ("messaging");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(rabbitmq);

// After adding all resources, run the app...
```

When .NET Aspire adds a container image to the app host, as shown in the preceding example with the `docker.io/library/rabbitmq` image, it creates a new RabbitMQ server instance on your local machine. A reference to your RabbitMQ server (the `rabbitmq` variable) is added to the `ExampleProject`. The RabbitMQ server resource includes default credentials with a `username` of `"guest"` and randomly generated `password` using the <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.CreateDefaultPasswordParameter*> method.

The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method configures a connection in the `ExampleProject` named `"messaging"`. For more information, see [Container resource lifecycle](../fundamentals/app-host-overview.md#container-resource-lifecycle).

> [!TIP]
> If you'd rather connect to an existing RabbitMQ server, call <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> instead. For more information, see [Reference existing resources](../fundamentals/app-host-overview.md#reference-existing-resources).

### Add RabbitMQ server resource with management plugin

To add the [RabbitMQ management plugin](https://www.rabbitmq.com/docs/management) to the RabbitMQ server resource, call the <xref:Aspire.Hosting.RabbitMQBuilderExtensions.WithManagementPlugin*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var rabbitmq = builder.AddRabbitMQ("messaging")
                      .WithManagementPlugin();

builder.AddProject<Projects.ExampleProject>()
        .WithReference(rabbitmq);

// After adding all resources, run the app...
```

The RabbitMQ management plugin provides an HTTP-based API for management and monitoring of your RabbitMQ server. .NET Aspire adds another container image [`docker.io/library/rabbitmq-management`](https://hub.docker.com/_/rabbitmq) to the app host that runs the management plugin.

### Add RabbitMQ server resource with data volume

To add a data volume to the RabbitMQ server resource, call the <xref:Aspire.Hosting.RabbitMQBuilderExtensions.WithDataVolume*> method on the RabbitMQ server resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var rabbitmq = builder.AddRabbitMQ("messaging")
                      .WithDataVolume(isReadOnly: false);

builder.AddProject<Projects.ExampleProject>()
        .WithReference(rabbitmq);

// After adding all resources, run the app...
```

The data volume is used to persist the RabbitMQ server data outside the lifecycle of its container. The data volume is mounted at the `/var/lib/rabbitmq` path in the RabbitMQ server container and when a `name` parameter isn't provided, the name is generated at random. For more information on data volumes and details on why they're preferred over [bind mounts](#add-rabbitmq-server-resource-with-data-bind-mount), see [Docker docs: Volumes](https://docs.docker.com/engine/storage/volumes).

### Add RabbitMQ server resource with data bind mount

To add a data bind mount to the RabbitMQ server resource, call the <xref:Aspire.Hosting.RabbitMQBuilderExtensions.WithDataBindMount*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var rabbitmq = builder.AddRabbitMQ("messaging")
                      .WithDataBindMount(
                          source: @"C:\RabbitMQ\Data",
                          isReadOnly: false);

builder.AddProject<Projects.ExampleProject>()
        .WithReference(rabbitmq);

// After adding all resources, run the app...
```

[!INCLUDE [data-bind-mount-vs-volumes](../includes/data-bind-mount-vs-volumes.md)]

Data bind mounts rely on the host machine's filesystem to persist the RabbitMQ server data across container restarts. The data bind mount is mounted at the `C:\RabbitMQ\Data` on Windows (or `/RabbitMQ/Data` on Unix) path on the host machine in the RabbitMQ server container. For more information on data bind mounts, see [Docker docs: Bind mounts](https://docs.docker.com/engine/storage/bind-mounts).

### Add RabbitMQ server resource with parameters

When you want to explicitly provide the username and password used by the container image, you can provide these credentials as parameters. Consider the following alternative example:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var username = builder.AddParameter("username", secret: true);
var password = builder.AddParameter("password", secret: true);

var rabbitmq = builder.AddRabbitMQ("messaging", username, password);

builder.AddProject<Projects.ExampleProject>()
       .WithReference(rabbitmq);

// After adding all resources, run the app...
```

For more information on providing parameters, see [External parameters](../fundamentals/external-parameters.md).

<!--
TODO: Link to Container lifetimes content that doesn't exist yet.
-->

### Hosting integration health checks

The RabbitMQ hosting integration automatically adds a health check for the RabbitMQ server resource. The health check verifies that the RabbitMQ server is running and that a connection can be established to it.

The hosting integration relies on the [📦 AspNetCore.HealthChecks.Rabbitmq](https://www.nuget.org/packages/AspNetCore.HealthChecks.Rabbitmq) NuGet package.

## Client integration

To get started with the .NET Aspire RabbitMQ client integration, install the [📦 Aspire.RabbitMQ.Client](https://www.nuget.org/packages/Aspire.RabbitMQ.Client) NuGet package in the client-consuming project, that is, the project for the application that uses the RabbitMQ client. The RabbitMQ client integration registers an [IConnection](https://rabbitmq.github.io/rabbitmq-dotnet-client/api/RabbitMQ.Client.IConnection.html) instance that you can use to interact with RabbitMQ.

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

### Add RabbitMQ client

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireRabbitMQExtensions.AddRabbitMQClient%2A> extension method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register an `IConnection` for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddRabbitMQClient(connectionName: "messaging");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the RabbitMQ server resource in the app host project. For more information, see [Add RabbitMQ server resource](#add-rabbitmq-server-resource).

You can then retrieve the `IConnection` instance using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(IConnection connection)
{
    // Use connection...
}
```

For more information on dependency injection, see [.NET dependency injection](/dotnet/core/extensions/dependency-injection).

### Add keyed RabbitMQ client

There might be situations where you want to register multiple `IConnection` instances with different connection names. To register keyed RabbitMQ clients, call the <xref:Microsoft.Extensions.Hosting.AspireRabbitMQExtensions.AddKeyedRabbitMQClient*> method:

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

The .NET Aspire RabbitMQ integration provides multiple options to configure the connection based on the requirements and conventions of your project.

#### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddRabbitMQClient`:

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

The .NET Aspire RabbitMQ integration supports <xref:Microsoft.Extensions.Configuration>. It loads the <xref:Aspire.RabbitMQ.Client.RabbitMQClientSettings> from configuration by using the `Aspire:RabbitMQ:Client` key. The following snippet is an example of a _:::no-loc text="appsettings.json":::_ file that configures some of the options:

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

For the complete RabbitMQ client integration JSON schema, see [Aspire.RabbitMQ.Client/ConfigurationSchema.json](https://github.com/dotnet/aspire/blob/v8.2.1/src/Components/Aspire.RabbitMQ.Client/ConfigurationSchema.json).

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

By default, .NET Aspire integrations enable [health checks](../fundamentals/health-checks.md) for all services. For more information, see [.NET Aspire integrations overview](../fundamentals/integrations-overview.md).

The .NET Aspire RabbitMQ integration:

- Adds the health check when <xref:Aspire.RabbitMQ.Client.RabbitMQClientSettings.DisableHealthChecks?displayProperty=nameWithType> is `false`, which attempts to connect to and create a channel on the RabbitMQ server.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic.

### Observability and telemetry

.NET Aspire integrations automatically set up Logging, Tracing, and Metrics configurations, which are sometimes known as *the pillars of observability*. For more information about integration observability and telemetry, see [.NET Aspire integrations overview](../fundamentals/integrations-overview.md). Depending on the backing service, some integrations might only support some of these features. For example, some integrations support logging and tracing, but not metrics. Telemetry features can also be disabled using the techniques presented in the [Configuration](#configuration) section.

#### Logging

The .NET Aspire RabbitMQ integration uses the following log categories:

- `RabbitMQ.Client`

#### Tracing

The .NET Aspire RabbitMQ integration emits the following tracing activities using OpenTelemetry:

- `Aspire.RabbitMQ.Client`

#### Metrics

The .NET Aspire RabbitMQ integration currently doesn't support metrics by default.

## See also

- [Send messages with RabbitMQ in .NET Aspire](/training/modules/send-messages-rabbitmq-dotnet-aspire-app)
- [RabbitMQ .NET Client docs](https://rabbitmq.github.io/rabbitmq-dotnet-client)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
