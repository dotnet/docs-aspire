---
title: .NET Aspire Azure Service Bus integration
description: Learn how to install and configure the .NET Aspire Azure Service Bus integration to connect to Azure Service Bus instances from .NET applications.
ms.date: 07/22/2025
ms.custom: sfi-ropc-nochange
---

# .NET Aspire Azure Service Bus integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[Azure Service Bus](https://azure.microsoft.com/services/service-bus/) is a fully managed enterprise message broker with message queues and publish-subscribe topics. The .NET Aspire Azure Service Bus integration enables you to connect to Azure Service Bus instances from .NET applications.

## Hosting integration

The .NET Aspire [Azure Service Bus](https://azure.microsoft.com/services/service-bus/) hosting integration models the various Service Bus resources as the following types:

- <xref:Aspire.Hosting.Azure.AzureServiceBusResource>: Represents an Azure Service Bus resource.
- <xref:Aspire.Hosting.Azure.AzureServiceBusQueueResource>: Represents an Azure Service Bus queue resource.
- <xref:Aspire.Hosting.Azure.AzureServiceBusSubscriptionResource>: Represents an Azure Service Bus subscription resource.
- <xref:Aspire.Hosting.Azure.AzureServiceBusTopicResource>: Represents an Azure Service Bus topic resource.
- <xref:Aspire.Hosting.Azure.AzureServiceBusEmulatorResource>: Represents an Azure Service Bus emulator resource.

To access these types and APIs for expressing them, add the [📦 Aspire.Hosting.Azure.ServiceBus](https://www.nuget.org/packages/Aspire.Hosting.Azure.ServiceBus) NuGet package in the [AppHost](xref:dotnet/aspire/app-host) project.

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

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add Azure Service Bus resource

In your AppHost project, call <xref:Aspire.Hosting.AzureServiceBusExtensions.AddAzureServiceBus*> to add and return an Azure Service Bus resource builder.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var serviceBus = builder.AddAzureServiceBus("messaging");

// After adding all resources, run the app...
```

When you add an <xref:Aspire.Hosting.Azure.AzureServiceBusResource> to the AppHost, it exposes other useful APIs to add queues and topics. In other words, you must add an `AzureServiceBusResource` before adding any of the other Service Bus resources.

> [!IMPORTANT]
> When you call <xref:Aspire.Hosting.AzureServiceBusExtensions.AddAzureServiceBus*>, it implicitly calls <xref:Aspire.Hosting.AzureProvisionerExtensions.AddAzureProvisioning*>—which adds support for generating Azure resources dynamically during app startup. The app must configure the appropriate subscription and location. For more information, see [Configuration](../azure/local-provisioning.md#configuration).

### Connect to an existing Azure Service Bus namespace

You might have an existing Azure Service Bus namespace that you want to connect to. Chain a call to annotate that your <xref:Aspire.Hosting.Azure.AzureServiceBusResource> is an existing resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var existingServiceBusName = builder.AddParameter("existingServiceBusName");
var existingServiceBusResourceGroup = builder.AddParameter("existingServiceBusResourceGroup");

var serviceBus = builder.AddAzureServiceBus("messaging")
                        .AsExisting(existingServiceBusName, existingServiceBusResourceGroup);

builder.AddProject<Projects.WebApplication>("web")
       .WithReference(serviceBus);

// After adding all resources, run the app...
```

[!INCLUDE [azure-configuration](../azure/includes/azure-configuration.md)]

For more information on treating Azure Service Bus resources as existing resources, see [Use existing Azure resources](../azure/integrations-overview.md#use-existing-azure-resources).

> [!NOTE]
> Alternatively, instead of representing an Azure Service Bus resource, you can add a connection string to the AppHost. This approach is weakly-typed, and doesn't work with role assignments or infrastructure customizations. For more information, see [Add existing Azure resources with connection strings](../azure/integrations-overview.md#add-existing-azure-resources-with-connection-strings).

### Add Azure Service Bus queue

To add an Azure Service Bus queue, call the <xref:Aspire.Hosting.AzureServiceBusExtensions.AddServiceBusQueue*> method on the `IResourceBuilder<AzureServiceBusResource>`:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var serviceBus = builder.AddAzureServiceBus("messaging");
var queue = serviceBus.AddServiceBusQueue("queue");

// After adding all resources, run the app...
```

When you call <xref:Aspire.Hosting.AzureServiceBusExtensions.AddServiceBusQueue(Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.Azure.AzureServiceBusResource},System.String,System.String)>, it configures your Service Bus resources to have a queue named `queue`. The expresses an explicit parent-child relationship, between the `messaging` Service Bus resource and its child `queue`. The queue is created in the Service Bus namespace that's represented by the `AzureServiceBusResource` that you added earlier. For more information, see [Queues, topics, and subscriptions in Azure Service Bus](/azure/service-bus-messaging/service-bus-queues-topics-subscriptions).

### Add Azure Service Bus topic and subscription

To add an Azure Service Bus topic, call the <xref:Aspire.Hosting.AzureServiceBusExtensions.AddServiceBusTopic*> method on the `IResourceBuilder<AzureServiceBusResource>`:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var serviceBus = builder.AddAzureServiceBus("messaging");
var topic = serviceBus.AddServiceBusTopic("topic");

// After adding all resources, run the app...
```

When you call <xref:Aspire.Hosting.AzureServiceBusExtensions.AddServiceBusTopic(Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.Azure.AzureServiceBusResource},System.String,System.String)>, it configures your Service Bus resources to have a topic named `topic`. The topic is created in the Service Bus namespace that's represented by the `AzureServiceBusResource` that you added earlier.

To add a subscription for the topic, call the <xref:Aspire.Hosting.AzureServiceBusExtensions.AddServiceBusSubscription*> method on the `IResourceBuilder<AzureServiceBusTopicResource>` and configure it using the <xref:Aspire.Hosting.AzureServiceBusExtensions.WithProperties*> method:

```csharp
using Aspire.Hosting.Azure;

var builder = DistributedApplication.CreateBuilder(args);

var serviceBus = builder.AddAzureServiceBus("messaging");
var topic = serviceBus.AddServiceBusTopic("topic");
topic.AddServiceBusSubscription("sub1")
     .WithProperties(subscription =>
     {
         subscription.MaxDeliveryCount = 10;
         subscription.Rules.Add(
             new AzureServiceBusRule("app-prop-filter-1")
             {
                 CorrelationFilter = new()
                 {
                     ContentType = "application/text",
                     CorrelationId = "id1",
                     Subject = "subject1",
                     MessageId = "msgid1",
                     ReplyTo = "someQueue",
                     ReplyToSessionId = "sessionId",
                     SessionId = "session1",
                     SendTo = "xyz"
                 }
             });
     });

// After adding all resources, run the app...
```

The preceding code not only adds a topic and creates and configures a subscription named `sub1` for the topic. The subscription has a maximum delivery count of `10` and a rule named `app-prop-filter-1`. The rule is a correlation filter that filters messages based on the `ContentType`, `CorrelationId`, `Subject`, `MessageId`, `ReplyTo`, `ReplyToSessionId`, `SessionId`, and `SendTo` properties.

For more information, see [Queues, topics, and subscriptions in Azure Service Bus](/azure/service-bus-messaging/service-bus-queues-topics-subscriptions).

### Add Azure Service Bus emulator resource

To add an Azure Service Bus emulator resource, chain a call on an `<IResourceBuilder<AzureServiceBusResource>>` to the <xref:Aspire.Hosting.AzureServiceBusExtensions.RunAsEmulator*> API:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var serviceBus = builder.AddAzureServiceBus("messaging")
                        .RunAsEmulator();

// After adding all resources, run the app...
```

When you call `RunAsEmulator`, it configures your Service Bus resources to run locally using an emulator. The emulator in this case is the [Azure Service Bus Emulator](/azure/service-bus-messaging/overview-emulator). The Azure Service Bus Emulator provides a free local environment for testing your Azure Service Bus apps and it's a perfect companion to the .NET Aspire Azure hosting integration. The emulator isn't installed, instead, it's accessible to .NET Aspire as a container. When you add a container to the AppHost, as shown in the preceding example with the `mcr.microsoft.com/azure-messaging/servicebus-emulator` image (and the companion `mcr.microsoft.com/azure-sql-edge` image), it creates and starts the container when the AppHost starts. For more information, see [Container resource lifecycle](../fundamentals/orchestrate-resources.md#container-resource-lifecycle).

#### Configure Service Bus emulator container

There are various configurations available for container resources, for example, you can configure the container's ports or providing a wholistic JSON configuration which overrides everything.

##### Configure Service Bus emulator container host port

By default, the Service Bus emulator container when configured by .NET Aspire, exposes the following endpoints:

| Endpoint | Image | Container port | Host port |
|--|--|--|--|
| `emulator` | `mcr.microsoft.com/azure-messaging/servicebus-emulator` | 5672 | dynamic |
| `tcp` | `mcr.microsoft.com/azure-sql-edge` | 1433 | dynamic |

The port that it's listening on is dynamic by default. When the container starts, the port is mapped to a random port on the host machine. To configure the endpoint port, chain calls on the container resource builder provided by the `RunAsEmulator` method and then the <xref:Aspire.Hosting.AzureServiceBusExtensions.WithHostPort(Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.Azure.AzureServiceBusEmulatorResource},System.Nullable{System.Int32})> as shown in the following example:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var serviceBus = builder.AddAzureServiceBus("messaging").RunAsEmulator(
                         emulator =>
                         {
                             emulator.WithHostPort(7777);
                         });

// After adding all resources, run the app...
```

The preceding code configures the Service Bus emulator container's existing `emulator` endpoint to listen on port `7777`. The Service Bus emulator container's port is mapped to the host port as shown in the following table:

| Endpoint name | Port mapping (`container:host`) |
|---------------|---------------------------------|
| `emulator`    | `5672:7777`                     |

##### Configure Service Bus emulator container JSON configuration

The Service Bus emulator automatically generates a configration similar to this [_config.json_](https://github.com/Azure/azure-service-bus-emulator-installer/blob/main/ServiceBus-Emulator/Config/Config.json) file from the configured resources. You can override this generated file entirely, or update the JSON configuration with a <xref:System.Text.Json.Nodes.JsonNode> representation of the configuration.

To provide a custom JSON configuration file, call the <xref:Aspire.Hosting.AzureServiceBusExtensions.WithConfigurationFile(Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.Azure.AzureServiceBusEmulatorResource},System.String)> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var serviceBus = builder.AddAzureServiceBus("messaging").RunAsEmulator(
                         emulator =>
                         {
                             emulator.WithConfigurationFile(
                                 path: "./messaging/custom-config.json");
                         });
```

The preceding code configures the Service Bus emulator container to use a custom JSON configuration file located at `./messaging/custom-config.json`. To instead override specific properties in the default configuration, call the <xref:Aspire.Hosting.AzureServiceBusExtensions.WithConfiguration(Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.Azure.AzureServiceBusEmulatorResource},System.Action{System.Text.Json.Nodes.JsonNode})> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var serviceBus = builder.AddAzureServiceBus("messaging").RunAsEmulator(
                         emulator =>
                         {
                             emulator.WithConfiguration(
                                 (JsonNode configuration) =>
                                 {
                                     var userConfig = configuration["UserConfig"];
                                     var ns = userConfig["Namespaces"][0];
                                     var firstQueue = ns["Queues"][0];
                                     var properties = firstQueue["Properties"];
                                     
                                     properties["MaxDeliveryCount"] = 5;
                                     properties["RequiresDuplicateDetection"] = true;
                                     properties["DefaultMessageTimeToLive"] = "PT2H";
                                 });
                         });

// After adding all resources, run the app...
```

The preceding code retrieves the `UserConfig` node from the default configuration. It then updates the first queue's properties to set the `MaxDeliveryCount` to `5`, `RequiresDuplicateDetection` to `true`, and `DefaultMessageTimeToLive` to `2 hours`.

### Provisioning-generated Bicep

If you're new to Bicep, it's a domain-specific language for defining Azure resources. With .NET Aspire, you don't need to write Bicep by-hand, instead the provisioning APIs generate Bicep for you. When you publish your app, the generated Bicep is output alongside the manifest file. When you add an Azure Service Bus resource, the following Bicep is generated:

:::code language="bicep" source="../snippets/azure/AppHost/service-bus/service-bus.bicep":::

The preceding Bicep is a module that provisions an Azure Service Bus namespace resource. Additionally, role assignments are created for the Azure resource in a separate module:

:::code language="bicep" source="../snippets/azure/AppHost/service-bus-roles/service-bus-roles.bicep":::

In addition to the Service Bus namespace, it also provisions an Azure role-based access control (Azure RBAC) built-in role of Azure Service Bus Data Owner. The role is assigned to the Service Bus namespace's resource group. For more information, see [Azure Service Bus Data Owner](/azure/role-based-access-control/built-in-roles/integration#azure-service-bus-data-owner).

#### Customize provisioning infrastructure

All .NET Aspire Azure resources are subclasses of the <xref:Aspire.Hosting.Azure.AzureProvisioningResource> type. This type enables the customization of the generated Bicep by providing a fluent API to configure the Azure resources—using the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure``1(Aspire.Hosting.ApplicationModel.IResourceBuilder{``0},System.Action{Aspire.Hosting.Azure.AzureResourceInfrastructure})> API. For example, you can configure the sku, location, and more. The following example demonstrates how to customize the Azure Service Bus resource:

:::code language="csharp" source="../snippets/azure/AppHost/Program.ConfigureServiceBusInfra.cs" id="configure":::

The preceding code:

- Chains a call to the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure*> API:
  - The infra parameter is an instance of the <xref:Aspire.Hosting.Azure.AzureResourceInfrastructure> type.
  - The provisionable resources are retrieved by calling the <xref:Azure.Provisioning.Infrastructure.GetProvisionableResources> method.
  - The single <xref:Azure.Provisioning.ServiceBus.ServiceBusNamespace> is retrieved.
  - The <xref:Azure.Provisioning.ServiceBus.ServiceBusNamespace.Sku?displayProperty=nameWithType> created with a <xref:Azure.Provisioning.ServiceBus.ServiceBusSkuTier.Premium?displayProperty=nameWithType>
  - A tag is added to the Service Bus namespace with a key of `ExampleKey` and a value of `Example value`.

There are many more configuration options available to customize the Azure Service Bus resource. For more information, see <xref:Azure.Provisioning.ServiceBus>. For more information, see [Azure.Provisioning customization](../azure/customize-azure-resources.md#azureprovisioning-customization).

### Hosting integration health checks

The Azure Service Bus hosting integration automatically adds a health check for the Service Bus resource. The health check verifies that the Service Bus is running and that a connection can be established to it.

The hosting integration relies on the [📦 AspNetCore.HealthChecks.AzureServiceBus](https://www.nuget.org/packages/AspNetCore.HealthChecks.AzureServiceBus) NuGet package.

## Client integration

To get started with the .NET Aspire Azure Service Bus client integration, install the [📦 Aspire.Azure.Messaging.ServiceBus](https://www.nuget.org/packages/Aspire.Azure.Messaging.ServiceBus) NuGet package in the client-consuming project, that is, the project for the application that uses the Service Bus client. The Service Bus client integration registers a <xref:Azure.Messaging.ServiceBus.ServiceBusClient> instance that you can use to interact with Service Bus.

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

### Add Service Bus client

In the :::no-loc text="Program.cs"::: file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireServiceBusExtensions.AddAzureServiceBusClient*> extension method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register a <xref:Azure.Messaging.ServiceBus.ServiceBusClient> for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddAzureServiceBusClient(connectionName: "messaging");
```

> [!TIP]
> The correct `connectionName` parameter depends on your AppHost code:
>
> - If you subscribed to a topic in the AppHost, then the `connectionString` you pass to `AddAzureServiceBusClient()` must match the name you passed to `AddServiceBusSubscription()`.
> - Otherwise, the `connectionString` you pass to `AddAzureServiceBusClient()` must match the name you passed to `AddAzureServiceBus()` when you created the Service Bus resource.
>
> For more information, see [Add Azure Service Bus resource](#add-azure-service-bus-resource) and [Add Azure Service Bus topic and subscription](#add-azure-service-bus-topic-and-subscription).

You can then retrieve the <xref:Azure.Messaging.ServiceBus.ServiceBusClient> instance using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(ServiceBusClient client)
{
    // Use client...
}
```

For more information on dependency injection, see [.NET dependency injection](/dotnet/core/extensions/dependency-injection).

### Add keyed Service Bus client

There might be situations where you want to register multiple `ServiceBusClient` instances with different connection names. To register keyed Service Bus clients, call the <xref:Microsoft.Extensions.Hosting.AspireServiceBusExtensions.AddKeyedAzureServiceBusClient*> method:

```csharp
builder.AddKeyedAzureServiceBusClient(name: "mainBus");
builder.AddKeyedAzureServiceBusClient(name: "loggingBus");
```

> [!IMPORTANT]
> When using keyed services, it's expected that your Service Bus resource configured two named buses, one for the `mainBus` and one for the `loggingBus`.

Then you can retrieve the `ServiceBusClient` instances using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(
    [FromKeyedServices("mainBus")] ServiceBusClient mainBusClient,
    [FromKeyedServices("loggingBus")] ServiceBusClient loggingBusClient)
{
    // Use clients...
}
```

For more information on keyed services, see [.NET dependency injection: Keyed services](/dotnet/core/extensions/dependency-injection#keyed-services).

### Configuration

The .NET Aspire Azure Service Bus integration provides multiple options to configure the connection based on the requirements and conventions of your project.

#### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling the <xref:Microsoft.Extensions.Hosting.AspireServiceBusExtensions.AddAzureServiceBusClient*> method:

```csharp
builder.AddAzureServiceBusClient("messaging");
```

Then the connection string is retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "messaging": "Endpoint=sb://{namespace}.servicebus.windows.net/;SharedAccessKeyName={keyName};SharedAccessKey={key};"
  }
}
```

For more information on how to format this connection string, see the ConnectionString documentation.

#### Use configuration providers

The .NET Aspire Azure Service Bus integration supports <xref:Microsoft.Extensions.Configuration>. It loads the <xref:Aspire.Azure.Messaging.ServiceBus.AzureMessagingServiceBusSettings> from configuration by using the `Aspire:Azure:Messaging:ServiceBus` key. The following snippet is an example of a :::no-loc text="appsettings.json"::: file that configures some of the options:

```json
{
  "Aspire": {
    "Azure": {
      "Messaging": {
        "ServiceBus": {
          "ConnectionString": "Endpoint=sb://{namespace}.servicebus.windows.net/;SharedAccessKeyName={keyName};SharedAccessKey={key};",
          "DisableTracing": false
        }
      }
    }
  }
}
```

For the complete Service Bus client integration JSON schema, see [Aspire.Azure.Messaging.ServiceBus/ConfigurationSchema.json](https://github.com/dotnet/aspire/blob/v9.1.0/src/Components/Aspire.Azure.Messaging.ServiceBus/ConfigurationSchema.json).

#### Use named configuration

The .NET Aspire Azure Service Bus integration supports named configuration, which allows you to configure multiple instances of the same resource type with different settings. The named configuration uses the connection name as a key under the main configuration section.

```json
{
  "Aspire": {
    "Azure": {
      "Messaging": {
        "ServiceBus": {
          "bus1": {
            "ConnectionString": "Endpoint=sb://namespace1.servicebus.windows.net/;SharedAccessKeyName=keyName;SharedAccessKey=key;",
            "DisableTracing": false
          },
          "bus2": {
            "ConnectionString": "Endpoint=sb://namespace2.servicebus.windows.net/;SharedAccessKeyName=keyName;SharedAccessKey=key;",
            "DisableTracing": true
          }
        }
      }
    }
  }
}
```

In this example, the `bus1` and `bus2` connection names can be used when calling `AddAzureServiceBusClient`:

```csharp
builder.AddAzureServiceBusClient("bus1");
builder.AddAzureServiceBusClient("bus2");
```

Named configuration takes precedence over the top-level configuration. If both are provided, the settings from the named configuration override the top-level settings.

#### Use inline delegates

Also you can pass the `Action<AzureMessagingServiceBusSettings> configureSettings` delegate to set up some or all the options inline, for example to disable tracing from code:

```csharp
builder.AddAzureServiceBusClient(
    "messaging",
    static settings => settings.DisableTracing = true);
```

You can also set up the <xref:Azure.Messaging.ServiceBus.ServiceBusClientOptions?displayProperty=fullName> using the optional `Action<ServiceBusClientOptions> configureClientOptions` parameter of the `AddAzureServiceBusClient` method. For example to set the <xref:Azure.Messaging.ServiceBus.ServiceBusClientOptions.Identifier?displayProperty=nameWithType> user-agent header suffix for all requests issues by this client:

```csharp
builder.AddAzureServiceBusClient(
    "messaging",
    configureClientOptions:
        clientOptions => clientOptions.Identifier = "myapp");
```

### Client integration health checks

By default, .NET Aspire integrations enable health checks for all services. For more information, see [.NET Aspire integrations overview](../fundamentals/integrations-overview.md).

The .NET Aspire Azure Service Bus integration:

- Adds the health check when <xref:Aspire.Azure.Messaging.ServiceBus.AzureMessagingServiceBusSettings.DisableTracing?displayProperty=nameWithType> is `false`, which attempts to connect to the Service Bus.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic.

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

#### Logging

The .NET Aspire Azure Service Bus integration uses the following log categories:

- `Azure.Core`
- `Azure.Identity`
- `Azure-Messaging-ServiceBus`

In addition to getting Azure Service Bus request diagnostics for failed requests, you can configure latency thresholds to determine which successful Azure Service Bus request diagnostics will be logged. The default values are 100 ms for point operations and 500 ms for non point operations.

```csharp
builder.AddAzureServiceBusClient(
    "messaging",
    configureClientOptions:
        clientOptions => {
            clientOptions.ServiceBusClientTelemetryOptions = new()
            {
                ServiceBusThresholdOptions = new()
                {
                    PointOperationLatencyThreshold = TimeSpan.FromMilliseconds(50),
                    NonPointOperationLatencyThreshold = TimeSpan.FromMilliseconds(300)
                }
            };
        });
```

#### Tracing

The .NET Aspire Azure Service Bus integration will emit the following tracing activities using OpenTelemetry:

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

Azure Service Bus tracing is currently in preview, so you must set the experimental switch to ensure traces are emitted.

```csharp
AppContext.SetSwitch("Azure.Experimental.EnableActivitySource", true);
```

For more information, see [Azure Service Bus: Distributed tracing and correlation through Service Bus messaging](/azure/service-bus-messaging/service-bus-end-to-end-tracing).

#### Metrics

The .NET Aspire Azure Service Bus integration currently doesn't support metrics by default due to limitations with the Azure SDK.

## See also

- [Azure Service Bus](https://azure.microsoft.com/services/service-bus)
- [.NET Aspire integrations overview](../fundamentals/integrations-overview.md)
- [.NET Aspire Azure integrations overview](../azure/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
