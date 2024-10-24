---
title: .NET Aspire Azure Service Bus integration
description: This article describes the .NET Aspire Azure Service Bus integration features and capabilities
ms.topic: how-to
ms.date: 08/12/2024
---

# .NET Aspire Azure Service Bus integration

Cloud-native apps often require communication with messaging services such as [Azure Service Bus](/azure/service-bus-messaging/service-bus-messaging-overview). Messaging services help decouple applications and enable scenarios that rely on features such as queues, topics and subscriptions, atomic transactions, load balancing, and more. The .NET Aspire Service Bus integration handles the following concerns to connect your app to Azure Service Bus:

- A <xref:Azure.Messaging.ServiceBus.ServiceBusClient> is registered in the DI container for connecting to Azure Service Bus.
- Applies `ServiceBusClient` configurations either inline through code or through configuration file settings.

## Prerequisites

- Azure subscription - [create one for free](https://azure.microsoft.com/free/)
- Azure Service Bus namespace, learn more about how to [add a Service Bus namespace](/azure/service-bus-messaging/service-bus-dotnet-get-started-with-queues?#create-a-namespace-in-the-azure-portal). Alternatively, you can use a connection string, which is not recommended in production environments.

## Get started

To get started with the .NET Aspire Azure Service Bus integration, install the [Aspire.Azure.Messaging.ServiceBus](https://www.nuget.org/packages/Aspire.Azure.Messaging.ServiceBus) NuGet package in the client-consuming project, i.e., the project for the application that uses the Azure Service Bus client.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Azure.Messaging.ServiceBus
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Azure.Messaging.ServiceBus"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireServiceBusExtensions.AddAzureServiceBusClient%2A> extension to register a `ServiceBusClient` for use via the dependency injection container.

```csharp
builder.AddAzureServiceBusClient("messaging");
```

To retrieve the configured <xref:Azure.Messaging.ServiceBus.ServiceBusClient> instance using dependency injection, require it as a constructor parameter. For example, to retrieve the client from an example service:

```csharp
public class ExampleService(ServiceBusClient client)
{
    // ...
}
```

## App host usage

To add Azure Service Bus hosting support to your <xref:Aspire.Hosting.IDistributedApplicationBuilder>, install the [Aspire.Hosting.Azure.ServiceBus](https://www.nuget.org/packages/Aspire.Hosting.Azure.ServiceBus) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure.ServiceBus
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure.ServiceBus"
                  Version="*" />
```

---

In your app host project, register the Service Bus integration and consume the service using the following methods:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var serviceBus = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureServiceBus("messaging")
    : builder.AddConnectionString("messaging");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(serviceBus)
```

## Configuration

The .NET Aspire Service Bus integration provides multiple options to configure the `ServiceBusClient` based on the requirements and conventions of your project.

### Use configuration providers

The Service Bus integration supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the `AzureMessagingServiceBusSettings` from _:::no-loc text="appsettings.json":::_ or other configuration files using `Aspire:Azure:Messaging:ServiceBus` key.

```json
{
  "Aspire": {
    "Azure": {
      "Messaging": {
        "ServiceBus": {
          "DisableHealthChecks": true,
          "DisableTracing": false,
          "ClientOptions": {
            "Identifier": "CLIENT_ID"
          }
        }
      }
    }
  }
}
```

If you have set up your configurations in the `Aspire:Azure:Messaging:ServiceBus` section of your _:::no-loc text="appsettings.json":::_ file you can just call the method `AddAzureServiceBusClient` without passing any parameters.

### Use inline delegates

You can also pass the `Action<AzureMessagingServiceBusSettings>` delegate to set up some or all the options inline, for example to set the `FullyQualifiedNamespace`:

```csharp
builder.AddAzureServiceBusClient(
    "messaging",
    static settings => settings.FullyQualifiedNamespace = "YOUR_SERVICE_BUS_NAMESPACE");
```

You can also set up the [ServiceBusClientOptions](/dotnet/api/azure.messaging.servicebus.servicebusclientoptions) using `Action<IAzureClientBuilder<ServiceBusClient, ServiceBusClientOptions>>` delegate, the second parameter of the `AddAzureServiceBus` method. For example to set the `ServiceBusClient` ID to identify the client:

```csharp
builder.AddAzureServiceBusClient(
    "messaging",
    static clientBuilder =>
        clientBuilder.ConfigureOptions(
            static options => options.Identifier = "CLIENT_ID"));
```

### Configuration options

The following configurable options are exposed through the <xref:Aspire.Azure.Messaging.ServiceBus.AzureMessagingServiceBusSettings> class:

| Name | Description |
|--|--|
| `ConnectionString` | The connection string used to connect to the Service Bus namespace. |
| `Credential` | The credential used to authenticate to the Service Bus namespace. |
| `FullyQualifiedNamespace` | The fully qualified Service Bus namespace. |
| `DisableTracing` | Disables tracing for the Service Bus client. |
| **<sup>†</sup>**`HealthCheckQueueName` | The name of the queue used for health checks. |
| **<sup>†</sup>**`HealthCheckTopicName` | The name of the topic used for health checks. |

_**<sup>†</sup>** At least one of the name options are mandatory when enabling health checks._

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

### Logging

The .NET Aspire Azure Service Bus integration uses the following log categories:

- `Azure.Core`
- `Azure.Identity`
- `Azure-Messaging-ServiceBus`

### Tracing

> [!NOTE]
> Service Bus `ActivitySource` support in the Azure SDK for .NET is experimental, and the shape of activities may change in the future without notice.

You can enable tracing in several ways:

- Setting the `Azure.Experimental.EnableActivitySource` [runtime configuration setting](/dotnet/core/runtime-config/) to `true`. Which can be done with either:  
  - Call `AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);`.
  - Add the `RuntimeHostConfigurationOption` setting to your project file:

      ```xml
      <ItemGroup>
          <RuntimeHostConfigurationOption
               Include="Azure.Experimental.EnableActivitySource"
               Value="true" />
      </ItemGroup>
      ```

- Set the `AZURE_EXPERIMENTAL_ENABLE_ACTIVITY_SOURCE` environment variable to "true".
  - Can be achieved by chaining a call to `WithEnvironment("AZURE_EXPERIMENTAL_ENABLE_ACTIVITY_SOURCE", "true")`

When enabled, the .NET Aspire Azure Service Bus integration will emit the following tracing activities using OpenTelemetry:

- `Message`
- `ServiceBusSender.Send`
- `ServiceBusSender.Schedule`
- `ServiceBusSender.Cancel`
- `ServiceBusReceiver.Receive`
- `ServiceBusReceiver.ReceiveDeferred`
- `ServiceBusReceiver.Peek`
- `ServiceBusReceiver.Abandon`
- `ServiceBusReceiver.Complete`
- `ServiceBusReceiver.DeadLetter`
- `ServiceBusReceiver.Defer`
- `ServiceBusReceiver.RenewMessageLock`
- `ServiceBusSessionReceiver.RenewSessionLock`
- `ServiceBusSessionReceiver.GetSessionState`
- `ServiceBusSessionReceiver.SetSessionState`
- `ServiceBusProcessor.ProcessMessage`
- `ServiceBusSessionProcessor.ProcessSessionMessage`
- `ServiceBusRuleManager.CreateRule`
- `ServiceBusRuleManager.DeleteRule`
- `ServiceBusRuleManager.GetRules`

For more information, see:

- [Azure SDK for .NET: Distributed tracing and the Service Bus client](https://github.com/Azure/azure-sdk-for-net/blob/Azure.Messaging.ServiceBus_7.17.5/sdk/servicebus/Azure.Messaging.ServiceBus/TROUBLESHOOTING.md#distributed-tracing).
- [Azure SDK for .NET: OpenTelemetry configuration](https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/core/Azure.Core/samples/Diagnostics.md#opentelemetry-configuration).
- [Azure SDK for .NET: Enabling experimental tracing features](https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/core/Azure.Core/samples/Diagnostics.md#enabling-experimental-tracing-features).

### Metrics

The .NET Aspire Azure Service Bus integration currently doesn't support metrics by default due to limitations with the Azure SDK for .NET. If that changes in the future, this section will be updated to reflect those changes.

## See also

- [Azure Service Bus](/azure/service-bus-messaging/)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
