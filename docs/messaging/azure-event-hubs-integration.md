---
title: .NET Aspire Azure Event Hubs integration
description: This article describes the .NET Aspire Azure Event Hubs integration features and capabilities.
ms.topic: how-to
ms.date: 08/26/2024
---

# .NET Aspire Azure Event Hubs integration

In this article, you learn how to use the .NET Aspire Azure Event Hubs integration. The `Aspire.Azure.Messaging.EventHubs` library offers options for registering the following types:

- <xref:Azure.Messaging.EventHubs.Producer.EventHubProducerClient>
- <xref:Azure.Messaging.EventHubs.Producer.EventHubBufferedProducerClient>
- <xref:Azure.Messaging.EventHubs.Consumer.EventHubConsumerClient>
- <xref:Azure.Messaging.EventHubs.EventProcessorClient>
- <xref:Azure.Messaging.EventHubs.Primitives.PartitionReceiver>

These type are registered in the DI container for connecting to [Azure Event Hubs](/azure/event-hubs).

## Prerequisites

- Azure subscription: [create one for free](https://azure.microsoft.com/free/).
- Azure Event Hubs namespace: for more information, see [add an Event Hubs namespace](/azure/event-hubs/event-hubs-create). Alternatively, you can use a connection string, which isn't recommended in production environments.

## Get started

To get started with the .NET Aspire Azure Event Hubs integration, install the [Aspire.Azure.Messaging.EventHubs](https://www.nuget.org/packages/Aspire.Azure.Messaging.EventHubs) NuGet package in the client-consuming project, i.e., the project for the application that uses the Azure Event Hubs client.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Azure.Messaging.EventHubs
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Azure.Messaging.EventHubs"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Supported clients with options classes

The following clients are supported by the library, along with their corresponding options and settings classes:

| Azure Client type                | Azure Options class                     | .NET Aspire Settings class                         |
|----------------------------------|-----------------------------------------|----------------------------------------------------|
| `EventHubProducerClient`         | `EventHubProducerClientOptions`         | `AzureMessagingEventHubsProducerSettings`          |
| `EventHubBufferedProducerClient` | `EventHubBufferedProducerClientOptions` | `AzureMessagingEventHubsBufferedProducerSettings`  |
| `EventHubConsumerClient`         | `EventHubConsumerClientOptions`         | `AzureMessagingEventHubsConsumerSettings`          |
| `EventProcessorClient`           | `EventProcessorClientOptions`           | `AzureMessagingEventHubsProcessorSettings`         |
| `PartitionReceiver`              | `PartitionReceiverOptions`              | `AzureMessagingEventHubsPartitionReceiverSettings` |

The client type are from the Azure SDK for .NET, as are the corresponding options classes. The settings classes are provided by the .NET Aspire Azure Event Hubs integration library.

## Example usage

The following example assumes that you have an Azure Event Hubs namespace and an Event Hub created and wish to configure an `EventHubProducerClient` to send events to the Event Hub. The `EventHubBufferedProducerClient`, `EventHubConsumerClient`, `EventProcessorClient`, and `PartitionReceiver`are configured in a similar manner.

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the `AddAzureEventHubProducerClient` extension to register a `EventHubProducerClient` for use via the dependency injection container.

```csharp
builder.AddAzureEventHubProducerClient("eventHubsConnectionName");
```

You can then retrieve the `EventHubProducerClient` instance using dependency injection. For example, to retrieve the client from a service:

```csharp
public class ExampleService(EventHubProducerClient client)
{
    // Use client...
}
```

For more information, see the [Azure.Messaging.EventHubs documentation](https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/eventhub/Azure.Messaging.EventHubs/README.md) for examples on using the `EventHubProducerClient`.

## App host usage

To add Azure Event Hub hosting support to your <xref:Aspire.Hosting.IDistributedApplicationBuilder>, install the [Aspire.Hosting.Azure.EventHubs](https://www.nuget.org/packages/Aspire.Hosting.Azure.EventHubs) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure.EventHubs
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure.EventHubs"
                  Version="*" />
```

---

In your app host project, add an Event Hubs connection and an Event Hub resource and consume the connection using the following methods:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var eventHubs = builder.AddAzureEventHubs("eventHubsConnectionName")
                       .AddEventHub("MyHub");

var exampleService = builder.AddProject<Projects.ExampleService>()
                            .WithReference(eventHubs);
```

The `AddAzureEventHubs` method will read connection information from the AppHost's configuration (for example, from "user secrets") under the `ConnectionStrings:eventHubsConnectionName` config key. The `WithReference` method passes that connection information into a connection string named `eventHubsConnectionName` in the `ExampleService` project.

As of .NET Aspire 8.1, the Azure EventHubs extension for .NET Aspire supports launching a local emulator for EventHubs. You can use the emulator by applying the `RunAsEmulator()` extension method as follows:

```csharp
var eventHubs = builder.AddAzureEventHubs("eventHubsConnectionName")
                       .RunAsEmulator()
                       .AddEventHub("MyHub");
```

The emulator for Azure EventHubs results in two container resources being launched inside .NET Aspire derived from the name of the Event Hubs resource name.

> [!IMPORTANT]
> Even though we are creating an Event Hub using the `AddEventHub` at the same time as the namespace, as of .NET Aspire version `preview-5`, the connection string will not include the `EntityPath` property, so the `EventHubName` property must be set in the settings callback for the preferred client. Future versions of Aspire will include the `EntityPath` property in the connection string and will not require the `EventHubName` property to be set in this scenario.

In the _:::no-loc text="Program.cs":::_ file of `ExampleService`, the connection can be consumed using by calling of the supported Event Hubs client extension methods:

```csharp
builder.AddAzureEventProcessorClient(
    "eventHubsConnectionName",
    static settings =>
    {
        settings.EventHubName = "MyHub";
    });
```

## Configuration

The .NET Aspire Azure Event Hubs library provides multiple options to configure the Azure Event Hubs connection based on the requirements and conventions of your project. Either a `FullyQualifiedNamespace` or a `ConnectionString` is a required to be supplied.

### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, provide the name of the connection string when calling `builder.AddAzureEventHubProducerClient()` and other supported Event Hubs clients. In this example, the connection string does not include the `EntityPath` property, so the `EventHubName` property must be set in the settings callback:

```csharp
builder.AddAzureEventHubProducerClient(
    "eventHubsConnectionName",
    static settings =>
    {
        settings.EventHubName = "MyHub";
    });
```

And then the connection information will be retrieved from the `ConnectionStrings` configuration section. Two connection formats are supported:

#### Fully Qualified Namespace (FQN)

The recommended approach is to use a fully qualified namespace, which works with the `AzureMessagingEventHubsSettings.Credential` property to establish a connection. If no credential is configured, the <xref:Azure.Identity.DefaultAzureCredential> is used.

```json
{
  "ConnectionStrings": {
    "eventHubsConnectionName": "{your_namespace}.servicebus.windows.net"
  }
}
```

#### Connection string

Alternatively, use a connection string:

```json
{
  "ConnectionStrings": {
    "eventHubsConnectionName": "Endpoint=sb://mynamespace.servicebus.windows.net/;SharedAccessKeyName=accesskeyname;SharedAccessKey=accesskey;EntityPath=MyHub"
  }
}
```

### Use configuration providers

The .NET Aspire Azure Event Hubs library supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the `AzureMessagingEventHubsSettings` and the associated Options, e.g. `EventProcessorClientOptions`, from configuration by using the `Aspire:Azure:Messaging:EventHubs:` key prefix, followed by the name of the specific client in use. For example, consider the _:::no-loc text="appsettings.json":::_ that configures some of the options for an `EventProcessorClient`:

```json
{
  "Aspire": {
    "Azure": {
      "Messaging": {
        "EventHubs": {
          "EventProcessorClient": {
            "EventHubName": "MyHub",
            "ClientOptions": {
              "Identifier": "PROCESSOR_ID"
            }
          }
        }
      }
    }
  }
}
```

You can also setup the Options type using the optional `Action<IAzureClientBuilder<EventProcessorClient, EventProcessorClientOptions>> configureClientBuilder` parameter of the `AddAzureEventProcessorClient` method. For example, to set the processor's client ID for this client:

```csharp
builder.AddAzureEventProcessorClient(
    "eventHubsConnectionName",
    configureClientBuilder: clientBuilder => clientBuilder.ConfigureOptions(
        options => options.Identifier = "PROCESSOR_ID"));
```

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

### Logging

The .NET Aspire Azure Event Hubs integration uses the following log categories:

- `Azure.Core`
- `Azure.Identity`

### Tracing

The .NET Aspire Azure Event Hubs integration will emit the following tracing activities using OpenTelemetry:

- "Azure.Messaging.EventHubs.*"

### Metrics

The .NET Aspire Azure Event Hubs integration currently doesn't support metrics by default due to limitations with the Azure SDK for .NET. If that changes in the future, this section will be updated to reflect those changes.

## See also

- [Azure Event Hubs](/azure/event-hubs/)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
