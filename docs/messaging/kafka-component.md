---
title: .NET Aspire Apache Kafka component
description: Learn how to use the .NET Aspire Apache Kafka client message-broker component.
ms.topic: how-to
ms.date: 02/12/2024
---

# .NET Aspire Apache Kafka component

In this article, you learn how to use the .NET Aspire Apache Kafka client message-broker. The `Aspire.Confluent.Kafka` library registers an [`IProducer<TKey, TValue>`](https://docs.confluent.io/platform/current/clients/confluent-kafka-dotnet/_site/api/Confluent.Kafka.IProducer-2.html) and an [`IConsumer<TKey, TValue>`](https://docs.confluent.io/platform/current/clients/confluent-kafka-dotnet/_site/api/Confluent.Kafka.IConsumer-2.html) in the dependency injection (DI) container for connecting to a Apache Kafka server. It enables corresponding health check, logging and telemetry.

## Get started

To get started with the .NET Aspire Apache Kafka component, install the [Aspire.Confluent.Kafka](https://www.nuget.org/packages/Aspire.Confluent.Kafka) NuGet package.

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

## Example usage

In the _Program.cs_ file of your component-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireKafkaProducerExtensions.AddKafkaProducer%2A> extension method to register an `IProducer<TKey, TValue>` for use via the dependency injection container. The method takes two generic parameters corresponding to the type of the key and the type of the message to send to the broker. These generic parameters will be used to new an instance of `ProducerBuilder<TKey, TValue>`. This method also take connection name parameter.

```csharp
builder.AddKafkaProducer<string, string>("messaging");
```

You can then retrieve the `IProducer<TKey, TValue>` instance using dependency injection. For example, to retrieve the producer from an `IHostedService`:

```csharp
internal sealed class MyWorker(IProducer<string, string> producer) : BackgroundService
{
    // Use producer...
}
```

## App host usage

In your app host project, register a Kafka container and consume the connection using the following methods:

```csharp
var messaging = builder.AddKafka("messaging");

var myService = builder.AddProject<Projects.MyService>()
                       .WithReference(messaging);
```

The `WithReference` method configures a connection in the `MyService` project named `messaging`. In the _Program.cs_ file of `MyService`, the Apache Kafka broker connection can be consumed using:

```csharp
builder.AddKafkaProducer<string, string>("messaging");
```

or

```csharp
builder.AddKafkaConsumer<string, string>("messaging");
```

## Configuration

The .NET Aspire Apache Kafka component provides multiple options to configure the connection based on the requirements and conventions of your project.

### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddKafkaProducer()` or `builder.AddKafkaProducer()`:

```csharp
builder.AddKafkaProducer<string, string>("myConnection");
```

And then the connection string will be retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "myConnection": "broker:9092"
  }
}
```

The value provided as connection string will be set to the `BootstrapServers`  property of the produced `IProducer<TKey, TValue>` or `IConsumer<TKey, TValue>` instance. Refer to [BootstrapServers](https://docs.confluent.io/platform/current/clients/confluent-kafka-dotnet/_site/api/Confluent.Kafka.ClientConfig.html#Confluent_Kafka_ClientConfig_BootstrapServers) for more information.

### Use configuration providers

The .NET Aspire Apache Kafka component supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the `KafkaProducerSettings` or `KafkaConsumerSettings` from configuration by respectively using the `Aspire:Confluent:Kafka:Producer` and `Aspire.Confluent:Kafka:Consumer` keys. This example `appsettings.json` configures some of the options:

```json
{
  "Aspire": {
    "Confluent": {
      "Kafka": {
        "Producer": {
          "DisableHealthChecks": false,
          "Config": {
            "Acks": "All"
          }
        }
      }
    }
  }
}
```

The `Config` properties of both  `Aspire:Confluent:Kafka:Producer` and `Aspire.Confluent:Kafka:Consumer` configuration sections respectively bind to instances of [`ProducerConfig`](https://docs.confluent.io/platform/current/clients/confluent-kafka-dotnet/_site/api/Confluent.Kafka.ProducerConfig.html) and [`ConsumerConfig`](https://docs.confluent.io/platform/current/clients/confluent-kafka-dotnet/_site/api/Confluent.Kafka.ConsumerConfig.html).

`Confluent.Kafka.Consumer<TKey, TValue>` requires the `ClientId` property to be set to let the broker track consumed message offsets.

### Use inline delegates

#### Configuring `KafkaProducerSettings` and `KafkaConsumerSettings`

You can pass the `Action<KafkaProducerSettings> configureSettings` delegate to set up some or all the options inline, for example to disable health checks from code:

```csharp
builder.AddKafkaProducer<string, string>("messaging", settings => settings.DisableHealthChecks  = true);
```

You can configure inline a consumer from code:

```csharp
builder.AddKafkaConsumer<string, string>("messaging", settings => settings.DisableHealthChecks  = true);
```

#### Configuring `ProducerBuilder<TKey, TValue>` and `ConsumerBuilder<TKey, TValue>`

To configure `Confluent.Kafka` builders, pass an `Action<ProducerBuilder<TKey, TValue>>` (or `Action<ConsumerBuilder<TKey, TValue>>`):

```csharp
builder.AddKafkaProducer<string, MyMessage>("messaging", producerBuilder => {
  producerBuilder.SetValueSerializer(new MyMessageSerializer());
})
```

Refer to [`ProducerBuilder<TKey, TValue>`](https://docs.confluent.io/platform/current/clients/confluent-kafka-dotnet/_site/api/Confluent.Kafka.ProducerBuilder-2.html) and [`ConsumerBuilder<TKey, TValue>`](https://docs.confluent.io/platform/current/clients/confluent-kafka-dotnet/_site/api/Confluent.Kafka.ConsumerBuilder-2.html) API documentation for more information.

[!INCLUDE [component-health-checks](../includes/component-health-checks.md)]

The .NET Aspire Apache Kafka component handles the following:

- Adds the `Aspire.Confluent.Kafka.Producer` health check when <xref:Aspire.Confluent.Kafka.KafkaProducerSettings.HealthChecks?displayProperty=nameWithType> is `true`.
- Adds the `Aspire.Confluent.Kafka.Consumer` health check when <xref:Aspire.Confluent.Kafka.KafkaConsumerSettings.HealthChecks?displayProperty=nameWithType> is `true`.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic.

[!INCLUDE [component-observability-and-telemetry](../includes/component-observability-and-telemetry.md)]

### Logging

The .NET Aspire Apache Kafka component uses the following log categories:

- `Aspire.Confluent.Kafka`

### Tracing

The .NET Aspire Apache Kafka component will emit the following tracing activities using OpenTelemetry:

- `Aspire.Confluent.Kafka`

### Metrics

The .NET Aspire Apache Kafka component will emit the following metrics using OpenTelemetry:

- `messaging.kafka.network.tx`
- `messaging.kafka.network.transmitted`
- `messaging.kafka.network.rx`
- `messaging.kafka.network.received`
- `messaging.publish.messages`
- `messaging.kafka.message.transmitted`
- `messaging.receive.messages`
- `messaging.kafka.message.received`

## See also

- [Apache Kafka](https://kafka.apache.org/)
- [Confluent](https://www.confluent.io/)
- [Confluent Kafka .NET client docs](https://docs.confluent.io/platform/current/clients/confluent-kafka-dotnet/_site/api/Confluent.Kafka.html)
- [.NET Aspire components](../fundamentals/components-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
