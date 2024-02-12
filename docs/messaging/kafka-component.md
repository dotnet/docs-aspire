---
title: .NET Aspire Confluent Kafka component
description: Learn how to use the .NET Aspire Confluent Kafka client message-broker component.
ms.topic: how-to
ms.date: 02/12/2024
---

# .NET Aspire Confluent Kafka component

In this article, you learn how to use the .NET Aspire Confluent Kafka client message-broker. The `Aspire.Confluent.Kafka` library registers an [`IProducer<TKey, TValue>`](https://docs.confluent.io/platform/current/clients/confluent-kafka-dotnet/_site/api/Confluent.Kafka.IProducer-2.html) and an [`IConsumer<TKey, TValue>`](https://docs.confluent.io/platform/current/clients/confluent-kafka-dotnet/_site/api/Confluent.Kafka.IConsumer-2.html) in the dependency injection (DI) container for connecting to a Confluent Kafka server. It enables corresponding health check, logging and telemetry.

## Get started

To get started with the .NET Aspire Confluent Kafka component, install the [Aspire.Confluent.Kafka](https://www.nuget.org/packages/Aspire.Confluent.Kafka) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Confluent.Kafka --prerelease
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Confluent.Kafka"
                  Version="[SelectVersion]" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).


## TODO: WIP RESUMER HERE - Example usage

In the _Program.cs_ file of your component-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireConfluent KafkaExtensions.AddConfluent Kafka%2A> extension method to register an `IConnection` for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddConfluent Kafka("messaging");
```

You can then retrieve the `IConnection` instance using dependency injection. For example, to retrieve the connection from a Web API controller:

```csharp
public class ExampleService(IConnection connection)
{
    // Use connection...
}
```

## App host usage

In your app host project, register a Confluent Kafka server and consume the connection using the following methods, such as <xref:Aspire.Hosting.Confluent KafkaBuilderExtensions.AddConfluent Kafka%2A>:

```csharp
// Service registration
var messaging = builder.AddConfluent Kafka("messaging");

// Service consumption
builder.AddProject<Projects.ExampleProject>()
       .WithReference(messaging);
```

The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method configures a connection in the `ExampleProject` project named `messaging`.

## Configuration

The .NET Aspire Confluent Kafka component provides multiple options to configure the connection based on the requirements and conventions of your project.

### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddConfluent Kafka`:

```csharp
builder.AddConfluent Kafka("Confluent KafkaConnection");
```

And then the connection string will be retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "Confluent KafkaConnection": "amqp://username:password@localhost:5672"
  }
}
```

For more information on how to format this connection string, see the [Confluent Kafka URI specification docs](https://www.Confluent Kafka.com/uri-spec.html).

### Use configuration providers

The .NET Aspire Confluent Kafka component supports <xref:Microsoft.Extensions.Configuration>. It loads the `Confluent KafkaClientSettings` from configuration by using the `Aspire:Confluent Kafka:Client` key. Example `appsettings.json` that configures some of the options:

```json
{
  "Aspire": {
    "Confluent Kafka": {
      "Client": {
        "HealthChecks": false
      }
    }
  }
}
```

### Use inline delegates

Also you can pass the `Action<Confluent KafkaClientSettings> configureSettings` delegate to set up some or all the options inline, for example to disable health checks from code:

```csharp
builder.AddConfluent Kafka(
    "messaging",
    static settings => settings.HealthChecks = false);
```

You can also set up the [IConnectionFactory](https://Confluent Kafka.github.io/Confluent Kafka-dotnet-client/api/Confluent Kafka.Client.IConnectionFactory.html) using the `Action<IConnectionFactory> configureConnectionFactory` delegate parameter of the `AddConfluent Kafka` method. For example to set the client provided name for connections:

```csharp
builder.AddConfluent Kafka(
    "messaging",
    static configureConnectionFactory:
        factory => factory.ClientProvidedName = "MyApp");
```

[!INCLUDE [component-health-checks](../includes/component-health-checks.md)]

The .NET Aspire Confluent Kafka component handles the following:

- Adds the health check when <xref:Aspire.Confluent Kafka.Client.Confluent KafkaClientSettings.HealthChecks?displayProperty=nameWithType> is `true`, which attempts to connect to and create a channel on the Confluent Kafka server.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic.

[!INCLUDE [component-observability-and-telemetry](../includes/component-observability-and-telemetry.md)]

### Logging

The .NET Aspire Confluent Kafka component uses the following log categories:

- `Confluent Kafka.Client`

### Tracing

The .NET Aspire Confluent Kafka component will emit the following tracing activities using OpenTelemetry:

- "Aspire.Confluent Kafka.Client"

### Metrics

The .NET Aspire Confluent Kafka component currently doesn't support metrics by default. If that changes in the future, this section will be updated to reflect those changes.

## See also

- [Confluent Kafka .NET Client docs](https://Confluent Kafka.github.io/Confluent Kafka-dotnet-client)
- [.NET Aspire components](../fundamentals/components-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
