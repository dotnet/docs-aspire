---
title: .NET Aspire Azure Service Bus component
description: This article describes the .NET Aspire Azure Service Bus component features and capabilities
ms.topic: how-to
ms.date: 04/24/2024
---

# .NET Aspire Azure Service Bus component

Cloud-native apps often require communication with messaging services such as [Azure Service Bus](/azure/service-bus-messaging/service-bus-messaging-overview). Messaging services help decouple applications and enable scenarios that rely on features such as queues, topics and subscriptions, atomic transactions, load balancing, and more. The .NET Aspire Service Bus component handles the following concerns to connect your app to Azure Service Bus:

- A <xref:Azure.Messaging.ServiceBus.ServiceBusClient> is registered in the DI container for connecting to Azure Service Bus.
- Applies `ServiceBusClient` configurations either inline through code or through configuration file settings.

## Prerequisites

- Azure subscription - [create one for free](https://azure.microsoft.com/free/)
- Azure Service Bus namespace, learn more about how to [add a Service Bus namespace](/azure/service-bus-messaging/service-bus-dotnet-get-started-with-queues?#create-a-namespace-in-the-azure-portal). Alternatively, you can use a connection string, which is not recommended in production environments.

## Get started

To get started with the .NET Aspire Azure Service Bus component, install the [Aspire.Azure.Messaging.ServiceBus](https://www.nuget.org/packages/Aspire.Azure.Messaging.ServiceBus) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Azure.Messaging.ServiceBus --prerelease
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Azure.Messaging.ServiceBus"
                  Version="[SelectVersion]" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

In the _Program.cs_ file of your component-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireServiceBusExtensions.AddAzureServiceBusClient%2A> extension to register a `ServiceBusClient` for use via the dependency injection container.

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

To add Azure Service Bus hosting support to your <xref:Aspire.Hosting.IDistributedApplicationBuilder>, install the [Aspire.Hosting.Azure.ServiceBus](https://www.nuget.org/packages/Aspire.Hosting.Azure.ServiceBus) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure.ServiceBus --prerelease
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure.ServiceBus"
                  Version="[SelectVersion]" />
```

---

In your app host project, register the Service Bus component and consume the service using the following methods:

```csharp
// Service registration
var serviceBus = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureServiceBus("messaging")
    : builder.AddConnectionString("messaging");

// Service consumption
builder.AddProject<Projects.ExampleProject>()
       .WithReference(serviceBus)
```

## Configuration

The .NET Aspire Service Bus component provides multiple options to configure the `ServiceBusClient` based on the requirements and conventions of your project.

### Use configuration providers

The Service Bus component supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the `AzureMessagingServiceBusSettings` from _appsettings.json_ or other configuration files using `Aspire:Azure:Messaging:ServiceBus` key.

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

If you have set up your configurations in the `Aspire:Azure:Messaging:ServiceBus` section of your _appsettings.json_ file you can just call the method `AddAzureServiceBus` without passing any parameters.

### Use inline delegates

You can also pass the `Action<AzureMessagingServiceBusSettings>` delegate to set up some or all the options inline, for example to set the `FullyQualifiedNamespace`:

```csharp
builder.AddAzureServiceBus(
    "messaging",
    static settings => settings.FullyQualifiedNamespace = "YOUR_SERVICE_BUS_NAMESPACE");
```

You can also set up the [ServiceBusClientOptions](/dotnet/api/azure.messaging.servicebus.servicebusclientoptions) using `Action<IAzureClientBuilder<ServiceBusClient, ServiceBusClientOptions>>` delegate, the second parameter of the `AddAzureServiceBus` method. For example to set the `ServiceBusClient` ID to identify the client:

```csharp
builder.AddAzureServiceBus(
    "messaging",
    static clientBuilder =>
        clientBuilder.ConfigureOptions(
            static options => options.Identifier = "CLIENT_ID"));
```

### Named instances

If you want to add more than one [ServiceBusClient](/dotnet/api/azure.messaging.servicebus.servicebusclient) you can use named instances. Load the named configuration section from the JSON config by calling the `AddAzureServiceBus` method and passing in the `INSTANCE_NAME`.

```csharp
builder.AddAzureServiceBus("INSTANCE_NAME");
```

The corresponding configuration JSON is defined as follows:

```json
{
  "Aspire": {
    "Azure": {
      "Messaging": {
        "INSTANCE_NAME": {
          "FullyQualifiedNamespace": "YOUR_SERVICE_BUS_NAMESPACE",
          "ClientOptions": {
            "Identifier": "CLIENT_ID"
          }
        }
      }
    }
  }
}
```

### Configuration options

The following configurable options are exposed through the <xref:Aspire.Azure.Messaging.ServiceBus.AzureMessagingServiceBusSettings> class:

| Name                      | Description                                                         |
|---------------------------|---------------------------------------------------------------------|
| `ConnectionString`        | The connection string used to connect to the Service Bus namespace. |
| `Credential`              | The credential used to authenticate to the Service Bus namespace.   |
| `FullyQualifiedNamespace` | The fully qualified Service Bus namespace.                          |

[!INCLUDE [component-observability-and-telemetry](../includes/component-observability-and-telemetry.md)]

### Logging

The .NET Aspire Azure Service Bus component uses the following log categories:

- `Azure.Core`
- `Azure.Identity`

### Tracing

The .NET Aspire Azure Service Bus component will emit the following tracing activities using OpenTelemetry:

- "Azure.Data.Tables.TableServiceClient"

### Metrics

The .NET Aspire Azure Service Bus component currently doesn't support metrics by default due to limitations with the Azure SDK for .NET. If that changes in the future, this section will be updated to reflect those changes.

## See also

- [Azure Service Bus](/azure/service-bus-messaging/)
- [.NET Aspire components](../fundamentals/components-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
