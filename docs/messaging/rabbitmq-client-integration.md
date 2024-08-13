---
title: .NET Aspire RabbitMQ integration
description: Learn how to use the .NET Aspire RabbitMQ client message-broker integration.
ms.topic: how-to
ms.date: 08/12/2024
---

# .NET Aspire RabbitMQ integration

In this article, you learn how to use the .NET Aspire RabbitMQ client message-broker. The `Aspire.RabbitMQ.Client` library is used to register an [IConnection](https://rabbitmq.github.io/rabbitmq-dotnet-client/api/RabbitMQ.Client.IConnection.html) in the dependency injection (DI) container for connecting to a RabbitMQ server. It enables corresponding health check, logging and telemetry.

## Get started

To get started with the .NET Aspire RabbitMQ integration, install the [Aspire.RabbitMQ.Client](https://www.nuget.org/packages/Aspire.RabbitMQ.Client) NuGet package in the client-consuming project, i.e., the project for the application that uses the RabbitMQ client.

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

## Example usage

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

## App host usage

To model the RabbitMQ resource in the app host, install the [Aspire.Hosting.RabbitMQ](https://www.nuget.org/packages/Aspire.Hosting.RabbitMQ) NuGet package in the [app host](xref:aspire/app-host) project.

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

In your app host project, register a RabbitMQ server and consume the connection using the following methods, such as <xref:Aspire.Hosting.RabbitMQBuilderExtensions.AddRabbitMQ%2A>:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var messaging = builder.AddRabbitMQ("messaging");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(messaging);
```

The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method configures a connection in the `ExampleProject` project named `messaging`.

When you want to explicitly provide the username and password, you can provide those as parameters. Consider the following alternative example:

```csharp
var username = builder.AddParameter("username", secret: true);
var password = builder.AddParameter("password", secret: true);

var messaging = builder.AddRabbitMQ("messaging", username, password);

// Service consumption
builder.AddProject<Projects.ExampleProject>()
       .WithReference(messaging);
```

For more information, see [External parameters](../fundamentals/external-parameters.md).

## Configuration

The .NET Aspire RabbitMQ integration provides multiple options to configure the connection based on the requirements and conventions of your project.

### Use a connection string

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

### Use configuration providers

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

### Use inline delegates

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

### Logging

The .NET Aspire RabbitMQ integration uses the following log categories:

- `RabbitMQ.Client`

### Tracing

The .NET Aspire RabbitMQ integration will emit the following tracing activities using OpenTelemetry:

- "Aspire.RabbitMQ.Client"

### Metrics

The .NET Aspire RabbitMQ integration currently doesn't support metrics by default. If that changes in the future, this section will be updated to reflect those changes.

## See also

- [RabbitMQ .NET Client docs](https://rabbitmq.github.io/rabbitmq-dotnet-client)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
