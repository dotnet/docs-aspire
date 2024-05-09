---
title: .NET Aspire Azure Event Hubs component
description: This article describes the .NET Aspire Azure Event Hubs component features and capabilities.
ms.topic: how-to
ms.date: 04/18/2024
---

# .NET Aspire Azure Event Hubs component

In this article, you learn how to use the .NET Aspire Azure Event Hubs component. The `Aspire.Azure.Messaging.EventHubs` library offers options for registering an <xref:Azure.Messaging.EventHubs.Producer.EventHubProducerClient>, an <xref:Azure.Messaging.EventHubs.Consumer.EventHubConsumerClient>, an <xref:Azure.Messaging.EventHubs.EventProcessorClient> or a <xref:Azure.Messaging.EventHubs.Primitives.PartitionReceiver> in the DI container for connecting to [Azure Event Hubs](/azure/event-hubs).

## Prerequisites

- Azure subscription: [create one for free](https://azure.microsoft.com/free/).
- Azure Event Hubs namespace: for more information, see [add an Event Hubs namespace](/azure/event-hubs/event-hubs-create). Alternatively, you can use a connection string, which isn't recommended in production environments.

## Get started

To get started with the .NET Aspire Azure Event Hubs component, install the [Aspire.Azure.Messaging.EventHubs](https://www.nuget.org/packages/Aspire.Azure.Messaging.EventHubs) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Azure.Messaging.EventHubs --prerelease
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Azure.Messaging.EventHubs"
                  Version="[SelectVersion]" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Supported clients with options classes

The following clients are supported by the library, along with their corresponding options and settings classes:

| Client type              | Options class                   | Settings class                                     |
|--------------------------|---------------------------------|----------------------------------------------------|
| `EventHubProducerClient` | `EventHubProducerClientOptions` | `AzureMessagingEventHubsProducerSettings`          |
| `EventHubConsumerClient` | `EventHubConsumerClientOptions` | `AzureMessagingEventHubsConsumerSettings`          |
| `EventProcessorClient`   | `EventProcessorClientOptions`   | `AzureMessagingEventHubsProcessorSettings`         |
| `PartitionReceiver`      | `PartitionReceiverOptions`      | `AzureMessagingEventHubsPartitionReceiverSettings` |

## Example usage

The following example assumes that you have an Azure Event Hubs namespace and an Event Hub created and wish to configure an `EventHubProducerClient` to send events to the Event Hub. The `EventHubConsumerClient`, `EventProcessorClient`, and `PartitionReceiver`are configured in a similar manner.

In the _Program.cs_ file of your component-consuming project, call the `AddAzureEventHubProducerClient` extension to register a `EventHubProducerClient` for use via the dependency injection container.

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

To add Azure Event Hub hosting support to your <xref:Aspire.Hosting.IDistributedApplicationBuilder>, install the [Aspire.Hosting.Azure.EventHubs](https://www.nuget.org/packages/Aspire.Hosting.Azure.EventHubs) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure.EventHubs --prerelease
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure.EventHubs"
                  Version="[SelectVersion]" />
```

---

In your app host project, add an Event Hubs connection and an Event Hub resource and consume the connection using the following methods:

```csharp
var eventHubs = builder.AddAzureEventHubs("eventHubsConnectionName")
                       .AddEventHub("MyHub");;

var ExampleService = builder.AddProject<Projects.ExampleService>()
                            .WithReference(eventHubs);
```

The `AddAzureEventHubs` method will read connection information from the AppHost's configuration (for example, from "user secrets") under the `ConnectionStrings:eventHubsConnectionName` config key. The `WithReference` method passes that connection information into a connection string named `eventHubsConnectionName` in the `ExampleService` project.

> [!IMPORTANT]
> Even though we are creating an Event Hub using the `AddEventHub` at the same time as the namespace, as of .NET Aspire version `preview-5`, the connection string will not include the `EntityPath` property, so the `EventHubName` property must be set in the settings callback for the preferred client. Future versions of Aspire will include the `EntityPath` property in the connection string and will not require the `EventHubName` property to be set in this scenario.

In the _Program.cs_ file of `ExampleService`, the connection can be consumed using by calling of the supported Event Hubs client extension methods:

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

The .NET Aspire Azure Event Hubs library supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the `AzureMessagingEventHubsSettings` and the associated Options, e.g. `EventProcessorClientOptions`, from configuration by using the `Aspire:Azure:Messaging:EventHubs:` key prefix, followed by the name of the specific client in use. For example, consider the `appsettings.json` that configures some of the options for an `EventProcessorClient`:

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

[!INCLUDE [component-observability-and-telemetry](../includes/component-observability-and-telemetry.md)]

### Logging

The .NET Aspire Azure Event Hubs component uses the following log categories:

- `Azure.Core`
- `Azure.Identity`

### Tracing

The .NET Aspire Azure Event Hubs component will emit the following tracing activities using OpenTelemetry:

- "Azure.Messaging.EventHubs.*"

### Metrics

The .NET Aspire Azure Event Hubs component currently doesn't support metrics by default due to limitations with the Azure SDK for .NET. If that changes in the future, this section will be updated to reflect those changes.

## See also

- [Azure Event Hubs](/azure/event-hubs/)
- [.NET Aspire components](../fundamentals/components-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
