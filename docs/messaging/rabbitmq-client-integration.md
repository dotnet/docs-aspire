---
title: .NET Aspire RabbitMQ integration
description: Learn how to use the .NET Aspire RabbitMQ message-broker integration, which includes both hosting and client integrations.
ms.topic: how-to
ms.date: 10/04/2024
---

# .NET Aspire RabbitMQ integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[RabbitMQ](https://www.rabbitmq.com/) is a reliable and mature messaging and streaming broker, which is easy to deploy on cloud environments, on-premises, and on your local machine. The .NET Aspire RabbitMQ integration enables you to connect to existing RabbitMQ instances, or create new instances from .NET with the `docker.io/library/rabbitmq` container image.

<!-- 
Overview of the thing we're integrating with, why you'd want to use it, and what you can do with it.
Image: diagram showing the thing we're integrating with and the components of the integration.
-->



## Hosting integration

The RabbitMQ hosting integration models a RabbitMQ server as the <xref:Aspire.Hosting.ApplicationModel.RabbitMQServerResource> type. To access this type and APIs that allow you to add it to your app host, install the [📦 Aspire.Hosting.RabbitMQ](https://www.nuget.org/packages/Aspire.Hosting.RabbitMQ) NuGet package in the [app host](xref:aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.RabbitMQ
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.RabbitMQ"
                  Version="[SelectVersion]" />
```

---

### Add RabbitMQ server resource

In your app host project, call <xref:Aspire.Hosting.RabbitMQBuilderExtensions.AddRabbitMQ*> on the `builder` instance to add a RabbitMQ server resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var rabbitmq = builder.AddRabbitMQ("messaging");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(rabbitmq);

// After adding all resources, run the app...
```

When .NET Aspire adds a container image to the app host, as shown in the preceding example with the `docker.io/library/rabbitmq` image, it creates a new RabbitMQ server instance on your local machine. A reference to your RabbitMQ server (the `rabbitmq` variable) is added to the `ExampleProject`. The RabbitMQ server resource defaults include a `username` of `"guest"` and a `password` that's randomly generated using the <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.CreateDefaultPasswordParameter(Aspire.Hosting.IDistributedApplicationBuilder,System.String,System.Boolean,System.Boolean,System.Boolean,System.Boolean,System.Int32,System.Int32,System.Int32,System.Int32)> API.

The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method configures a connection in the `ExampleProject` named `"messaging"`.

> [!TIP]
> If you'd rather connect to an existing RabbitMQ server, call <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString(Aspire.Hosting.IDistributedApplicationBuilder,System.String,System.String)> instead. For more information, see [Reference existing resources](../fundamentals/app-host-overview.md#reference-existing-resources).

### Add RabbitMQ server resource with parameters

When you want to explicitly provide the username and password used by the container image, you can provide those as parameters. Consider the following alternative example:

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

The RabbitMQ management plugin provides an HTTP-based API for management and monitoring of your RabbitMQ server. .NET Aspire will add another container image `docker.io/library/rabbitmq-management` to the app host that runs the management plugin.

### Add RabbitMQ server resource with data volume

To add a data volume to the RabbitMQ server resource, call the <xref:Aspire.Hosting.RabbitMQBuilderExtensions.WithDataVolume*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var rabbitmq = builder.AddRabbitMQ("messaging")
                      .WithDataVolume("/var/lib/rabbitmq");

builder.AddProject<Projects.ExampleProject>()
        .WithReference(rabbitmq);

// After adding all resources, run the app...
```

The data volume is used to persist the RabbitMQ server data across container restarts. The data volume is mounted at the `/var/lib/rabbitmq` path in the RabbitMQ server container. For more information on data volumes, see [Docker docs: Volumes](https://docs.docker.com/engine/storage/volumes).

### Add RabbitMQ server resource with data bind mount

To add a data bind mount to the RabbitMQ server resource, call the <xref:Aspire.Hosting.RabbitMQBuilderExtensions.WithDataBindMount*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var rabbitmq = builder.AddRabbitMQ("messaging")
                      .WithDataBindMount("/var/lib/rabbitmq", "/mnt/rabbitmq");

builder.AddProject<Projects.ExampleProject>()
        .WithReference(rabbitmq);

// After adding all resources, run the app...
```

The data bind mount is used to persist the RabbitMQ server data across container restarts. The data bind mount is mounted at the `/mnt/rabbitmq` path in the RabbitMQ server container. For more information on data bind mounts, see [Docker docs: Bind mounts](https://docs.docker.com/engine/storage/bind-mounts).

<!--
TODO: Were are still missing the following bits:

- Container lifetimes, link to related docs.
- Health checks... in hosting, different than client.
-->

## Client integration

<!--
We need to also call out any related extension methods per/type of integration.
-->

To get started with the .NET Aspire RabbitMQ client integration, install the [Aspire.RabbitMQ.Client](https://www.nuget.org/packages/Aspire.RabbitMQ.Client) NuGet package in the client-consuming project, i.e., the project for the application that uses the RabbitMQ client. The RabbitMQ client integration registers an [IConnection](https://rabbitmq.github.io/rabbitmq-dotnet-client/api/RabbitMQ.Client.IConnection.html) instance that you can use to interact with RabbitMQ.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.RabbitMQ.Client
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.RabbitMQ.Client"
                  Version="[SelectVersion]" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireRabbitMQExtensions.AddRabbitMQClient%2A> extension method to register an `IConnection` for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddRabbitMQClient("messaging");
```

You can then retrieve the `IConnection` instance using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(IConnection connection)
{
    // Use connection...
}
```

<!-- Keyed services - DI, examples... -->
<!-- Consider Advanced section. -->

### Configuration

The .NET Aspire RabbitMQ integration provides multiple options to configure the connection based on the requirements and conventions of your project.

#### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddRabbitMQClient`:

```csharp
builder.AddRabbitMQClient("RabbitMQConnection");
```

And then the connection string will be retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "RabbitMQConnection": "amqp://username:password@localhost:5672"
  }
}
```

For more information on how to format this connection string, see the [RabbitMQ URI specification docs](https://www.rabbitmq.com/uri-spec.html).

#### Use configuration providers

The .NET Aspire RabbitMQ integration supports <xref:Microsoft.Extensions.Configuration>. It loads the `RabbitMQClientSettings` from configuration by using the `Aspire:RabbitMQ:Client` key. Example _:::no-loc text="appsettings.json":::_ that configures some of the options:

```json
{
  "Aspire": {
    "RabbitMQ": {
      "Client": {
        "DisableHealthChecks": true
      }
    }
  }
}
```

#### Use inline delegates

Also you can pass the `Action<RabbitMQClientSettings> configureSettings` delegate to set up some or all the options inline, for example to disable health checks from code:

```csharp
builder.AddRabbitMQClient(
    "messaging",
    static settings => settings.DisableHealthChecks  = true);
```

You can also set up the [IConnectionFactory](https://rabbitmq.github.io/rabbitmq-dotnet-client/api/RabbitMQ.Client.IConnectionFactory.html) using the `Action<IConnectionFactory> configureConnectionFactory` delegate parameter of the `AddRabbitMQClient` method. For example to set the client provided name for connections:

```csharp
builder.AddRabbitMQClient(
    "messaging",
    static configureConnectionFactory:
        factory => factory.ClientProvidedName = "MyApp");
```

[!INCLUDE [integration-health-checks](../includes/integration-health-checks.md)]

The .NET Aspire RabbitMQ integration handles the following:

- Adds the health check when <xref:Aspire.RabbitMQ.Client.RabbitMQClientSettings.DisableHealthChecks?displayProperty=nameWithType> is `true`, which attempts to connect to and create a channel on the RabbitMQ server.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic.

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

#### Logging

The .NET Aspire RabbitMQ integration uses the following log categories:

- `RabbitMQ.Client`

#### Tracing

The .NET Aspire RabbitMQ integration will emit the following tracing activities using OpenTelemetry:

- "Aspire.RabbitMQ.Client"

#### Metrics

The .NET Aspire RabbitMQ integration currently doesn't support metrics by default. If that changes in the future, this section will be updated to reflect those changes.

## See also

- [RabbitMQ .NET Client docs](https://rabbitmq.github.io/rabbitmq-dotnet-client)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
