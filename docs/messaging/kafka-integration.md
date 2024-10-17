---
title: .NET Aspire Apache Kafka integration
description: Learn how to use the .NET Aspire Apache Kafka client message-broker integration.
ms.date: 10/11/2024
uid: messaging/kafka-integration
---

# .NET Aspire Apache Kafka integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[Apache Kafka](https://kafka.apache.org/) is an open-source distributed event streaming platform. It's useful for building real-time data pipelines and streaming applications. The .NET Aspire Apache Kafka integration enables you to connect to existing Kafka instances, or create new instances from .NET with the [`docker.io/confluentinc/confluent-local` container image](https://hub.docker.com/r/confluentinc/cp-kafka).

## Hosting integration

The Apache Kafka hosting integration models a Kafka server as the <xref:Aspire.Hosting.KafkaServerResource> type. To access this type, install the [📦 Aspire.Hosting.Kafka](https://www.nuget.org/packages/Aspire.Hosting.Kafka) NuGet package in the [app host](xref:dotnet/aspire/app-host) project, then add it with the builder.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Kafka
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Kafka"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add Kafka server resource

In your app host project, call <xref:Aspire.Hosting.KafkaBuilderExtensions.AddKafka*> on the `builder` instance to add a Kafka server resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var kafka = builder.AddKafka("kafka");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(kafka);

// After adding all resources, run the app...
```

When .NET Aspire adds a container image to the app host, as shown in the preceding example with the `docker.io/confluentinc/confluent-local` image, it creates a new Kafka server instance on your local machine. A reference to your Kafka server (the `kafka` variable) is added to the `ExampleProject`. The Kafka server resource includes default ports

The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method configures a connection in the `ExampleProject` named `"kafka"`. For more information, see [Container resource lifecycle](../fundamentals/app-host-overview.md#container-resource-lifecycle).

> [!TIP]
> If you'd rather connect to an existing Kafka server, call <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> instead. For more information, see [Reference existing resources](../fundamentals/app-host-overview.md#reference-existing-resources).

### Add Kafka UI

To add the [Kafka UI](https://hub.docker.com/r/provectuslabs/kafka-ui) to the Kafka server resource, call the <xref:Aspire.Hosting.KafkaBuilderExtensions.WithKafkaUI*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var kafka = builder.AddKafka("kafka")
                   .WithKafkaUI();

builder.AddProject<Projects.ExampleProject>()
       .WithReference(kafka);

// After adding all resources, run the app...
```

The Kafka UI is a free, open-source web UI to monitor and manage Apache Kafka clusters. .NET Aspire adds another container image [`docker.io/provectuslabs/kafka-ui`](https://hub.docker.com/r/provectuslabs/kafka-ui) to the app host that runs the Kafka UI.

### Change the Kafka UI host port

To change the Kafka UI host port, chain a call to the <xref:Aspire.Hosting.KafkaBuilderExtensions.WithHostPort*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var kafka = builder.AddKafka("kafka")
                   .WithKafkaUI()
                   .WithHostPort(9100);

builder.AddProject<Projects.ExampleProject>()
       .WithReference(kafka);

// After adding all resources, run the app...
```

The Kafka UI is accessible at `http://localhost:9100` in the preceding example.

### Add Kafka server resource with data volume

To add a data volume to the Kafka server resource, call the <xref:Aspire.Hosting.KafkaBuilderExtensions.WithDataVolume*> method on the Kafka server resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var kafka = builder.AddKafka("kafka")
                   .WithDataVolume(isReadOnly: false);

builder.AddProject<Projects.ExampleProject>()
       .WithReference(kafka);

// After adding all resources, run the app...
```

The data volume is used to persist the Kafka server data outside the lifecycle of its container. The data volume is mounted at the `/var/lib/kafka/data` path in the Kafka server container and when a `name` parameter isn't provided, the name is generated at random. For more information on data volumes and details on why they're preferred over [bind mounts](#add-kafka-server-resource-with-data-bind-mount), see [Docker docs: Volumes](https://docs.docker.com/engine/storage/volumes).

### Add Kafka server resource with data bind mount

To add a data bind mount to the Kafka server resource, call the <xref:Aspire.Hosting.KafkaBuilderExtensions.WithDataBindMount*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var kafka = builder.AddKafka("kafka")
                   .WithDataBindMount(
                       source: @"C:\Kafka\Data",
                       isReadOnly: false);

builder.AddProject<Projects.ExampleProject>()
       .WithReference(kafka);

// After adding all resources, run the app...
```

[!INCLUDE [data-bind-mount-vs-volumes](../includes/data-bind-mount-vs-volumes.md)]

Data bind mounts rely on the host machine's filesystem to persist the Kafka server data across container restarts. The data bind mount is mounted at the `C:\Kafka\Data` on Windows (or `/Kafka/Data` on Unix) path on the host machine in the Kafka server container. For more information on data bind mounts, see [Docker docs: Bind mounts](https://docs.docker.com/engine/storage/bind-mounts).

### Hosting integration health checks

The Kafka hosting integration automatically adds a health check for the Kafka server resource. The health check verifies that a Kafka producer with the specified connection name is able to connect and persist a topic to the Kafka server.

The hosting integration relies on the [📦 AspNetCore.HealthChecks.Kafka](https://www.nuget.org/packages/AspNetCore.HealthChecks.Kafka) NuGet package.

## Client integration

To get started with the .NET Aspire Apache Kafka integration, install the [📦 Aspire.Confluent.Kafka](https://www.nuget.org/packages/Aspire.Confluent.Kafka) NuGet package in the client-consuming project, that is, the project for the application that uses the Apache Kafka client.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Confluent.Kafka
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Confluent.Kafka"
                  Version="*" />
```

---

### Add Kafka producer

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireKafkaProducerExtensions.AddKafkaProducer%2A> extension method to register an `IProducer<TKey, TValue>` for use via the dependency injection container. The method takes two generic parameters corresponding to the type of the key and the type of the message to send to the broker. These generic parameters are used by `AddKafkaProducer` to create an instance of `ProducerBuilder<TKey, TValue>`. This method also takes connection name parameter.

```csharp
builder.AddKafkaProducer<string, string>("messaging");
```

You can then retrieve the `IProducer<TKey, TValue>` instance using dependency injection. For example, to retrieve the producer from an `IHostedService`:

```csharp
internal sealed class Worker(IProducer<string, string> producer) : BackgroundService
{
    // Use producer...
}
```

For more information on workers, see [Worker services in .NET](/dotnet/core/extensions/workers).

### Add Kafka consumer

To register an `IConsumer<TKey, TValue>` for use via the dependency injection container, call the <xref:Microsoft.Extensions.Hosting.AspireKafkaConsumerExtensions.AddKafkaConsumer%2A> extension method in the _:::no-loc text="Program.cs":::_ file of your client-consuming project. The method takes two generic parameters corresponding to the type of the key and the type of the message to receive from the broker. These generic parameters are used by `AddKafkaConsumer` to create an instance of `ConsumerBuilder<TKey, TValue>`. This method also takes connection name parameter.

```csharp
builder.AddKafkaConsumer<string, string>("messaging");
```

You can then retrieve the `IConsumer<TKey, TValue>` instance using dependency injection. For example, to retrieve the consumer from an `IHostedService`:

```csharp
internal sealed class Worker(IConsumer<string, string> consumer) : BackgroundService
{
    // Use consumer...
}
```

### Add keyed Kafka producers or consumers

There might be situations where you want to register multiple producer or consumer instances with different connection names. To register keyed Kafka producers or consumers, call the appropriate API:

- <xref:Microsoft.Extensions.Hosting.AspireKafkaProducerExtensions.AddKeyedKafkaProducer%2A>: Registers a keyed Kafka producer.
- <xref:Microsoft.Extensions.Hosting.AspireKafkaConsumerExtensions.AddKeyedKafkaConsumer%2A>: Registers a keyed Kafka consumer.

For more information on keyed services, see [.NET dependency injection: Keyed services](/dotnet/core/extensions/dependency-injection#keyed-services).

### Configuration

The .NET Aspire Apache Kafka integration provides multiple options to configure the connection based on the requirements and conventions of your project.

#### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddKafkaProducer()` or `builder.AddKafkaProducer()`:

```csharp
builder.AddKafkaProducer<string, string>("kafka-producer");
```

Then the connection string is retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "kafka-producer": "broker:9092"
  }
}
```

The connection string value is set to the `BootstrapServers`  property of the produced `IProducer<TKey, TValue>` or `IConsumer<TKey, TValue>` instance. For more information, see [BootstrapServers](https://docs.confluent.io/platform/current/clients/confluent-kafka-dotnet/_site/api/Confluent.Kafka.ClientConfig.html#Confluent_Kafka_ClientConfig_BootstrapServers).

#### Use configuration providers

The .NET Aspire Apache Kafka integration supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the <xref:Aspire.Confluent.Kafka.KafkaProducerSettings> or <xref:Aspire.Confluent.Kafka.KafkaConsumerSettings> from configuration by respectively using the `Aspire:Confluent:Kafka:Producer` and `Aspire.Confluent:Kafka:Consumer` keys. The following snippet is an example of a _:::no-loc text="appsettings.json":::_ file that configures some of the options:

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

For the complete Kafka client integration JSON schema, see [Aspire.Confluent.Kafka/ConfigurationSchema.json](https://github.com/dotnet/aspire/blob/v8.2.1/src/Components/Aspire.Confluent.Kafka/ConfigurationSchema.json).

#### Use inline delegates

There are several inline delegates available to configure various options.

##### Configure`KafkaProducerSettings` and `KafkaConsumerSettings`

You can pass the `Action<KafkaProducerSettings> configureSettings` delegate to set up some or all the options inline, for example to disable health checks from code:

```csharp
builder.AddKafkaProducer<string, string>(
    "messaging", 
    static settings => settings.DisableHealthChecks = true);
```

You can configure inline a consumer from code:

```csharp
builder.AddKafkaConsumer<string, string>(
    "messaging",
    static settings => settings.DisableHealthChecks = true);
```

##### Configure `ProducerBuilder<TKey, TValue>` and `ConsumerBuilder<TKey, TValue>`

To configure `Confluent.Kafka` builders, pass an `Action<ProducerBuilder<TKey, TValue>>` (or `Action<ConsumerBuilder<TKey, TValue>>`):

```csharp
builder.AddKafkaProducer<string, MyMessage>(
    "messaging",
    static producerBuilder => 
    {
        var messageSerializer = new MyMessageSerializer();
        producerBuilder.SetValueSerializer(messageSerializer);
    })
```

When registering producers and consumers, if you need to access a service registered in the DI container, you can pass an `Action<IServiceProvider, ProducerBuilder<TKey, TValue>>` or `Action<IServiceProvider, ConsumerBuilder<TKey, TValue>>` respectively:

- <xref:Microsoft.Extensions.Hosting.AspireKafkaProducerExtensions.AddKafkaProducer``2(Microsoft.Extensions.Hosting.IHostApplicationBuilder,System.String,System.Action{System.IServiceProvider,Confluent.Kafka.ProducerBuilder{``0,``1}})>
- <xref:Microsoft.Extensions.Hosting.AspireKafkaConsumerExtensions.AddKafkaConsumer``2(Microsoft.Extensions.Hosting.IHostApplicationBuilder,System.String,System.Action{System.IServiceProvider,Confluent.Kafka.ConsumerBuilder{``0,``1}})>

Consider the following producer registration example:

```csharp
builder.AddKafkaProducer<string, MyMessage>(
    "messaging",
    static (serviceProvider, producerBuilder) => 
    {
        var messageSerializer = serviceProvider.GetRequiredServices<MyMessageSerializer>();
        producerBuilder.SetValueSerializer(messageSerializer);
    })
```

For more information, see [`ProducerBuilder<TKey, TValue>`](https://docs.confluent.io/platform/current/clients/confluent-kafka-dotnet/_site/api/Confluent.Kafka.ProducerBuilder-2.html) and [`ConsumerBuilder<TKey, TValue>`](https://docs.confluent.io/platform/current/clients/confluent-kafka-dotnet/_site/api/Confluent.Kafka.ConsumerBuilder-2.html) API documentation.

### Client integration health checks

By default, .NET Aspire integrations enable [health checks](../fundamentals/health-checks.md) for all services. For more information, see [.NET Aspire integrations overview](../fundamentals/integrations-overview.md).

The .NET Aspire Apache Kafka integration handles the following health check scenarios:

- Adds the `Aspire.Confluent.Kafka.Producer` health check when <xref:Aspire.Confluent.Kafka.KafkaProducerSettings.DisableHealthChecks?displayProperty=nameWithType> is `false`.
- Adds the `Aspire.Confluent.Kafka.Consumer` health check when <xref:Aspire.Confluent.Kafka.KafkaConsumerSettings.DisableHealthChecks?displayProperty=nameWithType> is `false`.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic.

### Observability and telemetry

.NET Aspire integrations automatically set up Logging, Tracing, and Metrics configurations, which are sometimes known as *the pillars of observability*. For more information about integration observability and telemetry, see [.NET Aspire integrations overview](../fundamentals/integrations-overview.md). Depending on the backing service, some integrations may only support some of these features. For example, some integrations support logging and tracing, but not metrics. Telemetry features can also be disabled using the techniques presented in the [Configuration](#configuration) section.

#### Logging

The .NET Aspire Apache Kafka integration uses the following log categories:

- `Aspire.Confluent.Kafka`

#### Tracing

The .NET Aspire Apache Kafka integration dos not emit distributed traces.

#### Metrics

The .NET Aspire Apache Kafka integration emits the following metrics using OpenTelemetry:

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
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
