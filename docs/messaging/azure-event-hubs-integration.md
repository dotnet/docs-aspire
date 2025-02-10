---
title: .NET Aspire Azure Event Hubs integration
description: This article describes the .NET Aspire Azure Event Hubs integration features and capabilities.
ms.date: 02/10/2025
---

# .NET Aspire Azure Event Hubs integration

[Azure Event Hubs](/azure/event-hubs/event-hubs-abous) is a native data-streaming service in the cloud that can stream millions of events per second, with low latency, from any source to any destination. The .NET Aspire Azure Event Hubs integration enables you to connect to Azure Event Hubs instances from your .NET applications.

## Hosting integration

The .NET Aspire [Azure Event Hubs](https://azure.microsoft.com/products/event-hubs) hosting integration models the various Event Hub resources as the following types:

- <xref:Aspire.Hosting.Azure.AzureEventHubsResource>: Represents a top-level Azure Event Hubs resource, used for representing collections of hubs and the connection information to the underlying Azure resource.
- <xref:Aspire.Hosting.Azure.AzureEventHubResource>: Represents a single Event Hub resource.
- <xref:Aspire.Hosting.Azure.AzureEventHubsEmulatorResource>: Represents an Azure Event Hubs emulator as a container resource.
- <xref:Aspire.Hosting.Azure.AzureEventHubConsumerGroupResource>: Represents a consumer group within an Event Hub resource.

To access these types and APIs for expressing them within your [app host](xref:dotnet/aspire/app-host) project, install the [📦 Aspire.Hosting.Azure.EventHubs](https://www.nuget.org/packages/Aspire.Hosting.Azure.EventHubs) NuGet package:

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

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add an Azure Event Hubs resource

To add an <xref:Aspire.Hosting.Azure.AzureEventHubsResource> to your app host project, call the <xref:Aspire.Hosting.AzureEventHubsExtensions.AddAzureEventHubs*> method providing a name, and then chain a call to <xref:Aspire.Hosting.AzureEventHubsExtensions.AddHub*>:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var eventHubs = builder.AddAzureEventHubs("event-hubs")
                       .AddHub("messages");

builder.AddProject<Projects.ExampleService>()
       .WithReference(eventHubs);

// After adding all resources, run the app...
```

When you add an Azure Event Hubs resource to the app host, it exposes other useful APIs to add Event Hub resources, consumer groups, express explicit provisioning configuration, and enables the use of the Azure Event Hubs emulator. The preceding code adds an Azure Event Hubs resource named `event-hubs` and an Event Hub named `messages` to the app host project. The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference*> method passes the connection information to the `ExampleService` project.

> [!IMPORTANT]
> When you call <xref:Aspire.Hosting.AzureEventHubsExtensions.AddAzureEventHubs*>, it implicitly calls <xref:Aspire.Hosting.AzureProvisionerExtensions.AddAzureProvisioning(Aspire.Hosting.IDistributedApplicationBuilder)>—which adds support for generating Azure resources dynamically during app startup. The app must configure the appropriate subscription and location. For more information, see [Local provisioning: Configuration](../azure/local-provisioning.md#configuration)

#### Generated provisioning Bicep

If you're new to [Bicep](/azure/azure-resource-manager/bicep/overview), it's a domain-specific language for defining Azure resources. With .NET Aspire, you don't need to write Bicep by-hand, instead the provisioning APIs generate Bicep for you. When you publish your app, the generated Bicep is output alongside the manifest file. When you add an Azure Event Hubs resource, the following Bicep is generated:

<!-- markdownlint-disable MD033 -->
<br/>
<details>
<summary id="azure-event-hubs"><strong>Toggle Azure Event Hubs Bicep.</strong></summary>
<p aria-labelledby="azure-event-hubs">

:::code language="bicep" source="../snippets/azure/AppHost/event-hubs.module.bicep":::

</p>
</details>
<!-- markdownlint-enable MD033 -->

The preceding Bicep is a module that provisions an Azure Event Hubs resource with the following defaults:

- `location`: The location of the resource group.
- `sku`: The SKU of the Event Hubs resource, defaults to `Standard`.
- `principalId`: The principal ID of the Event Hubs resource.
- `principalType`: The principal type of the Event Hubs resource.
- `event_hubs`: The Event Hubs resource.
- `event_hubs_AzureEventHubsDataOwner`: The Event Hubs resource owner, based on the build-in `Azure Event Hubs Data Owner` role. For more information, see [Azure Event Hubs Data Owner](/azure/role-based-access-control/built-in-roles/analytics#azure-event-hubs-data-owner).
- `eventHubsEndpoint`: The endpoint of the Event Hubs resource.

The generated Bicep is a starting point and can be customized to meet your specific requirements.

#### Customize provisioning infrastructure

All .NET Aspire Azure resources are subclasses of the <xref:Aspire.Hosting.Azure.AzureProvisioningResource> type. This type enables the customization of the generated Bicep by providing a fluent API to configure the Azure resources—using the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure``1(Aspire.Hosting.ApplicationModel.IResourceBuilder{``0},System.Action{Aspire.Hosting.Azure.AzureResourceInfrastructure})> API. For example, you can configure the `kind`, `consistencyPolicy`, `locations`, and more. The following example demonstrates how to customize the Azure Cosmos DB resource:

:::code language="csharp" source="../snippets/azure/AppHost/Program.ConfigureEventHubsInfra.cs" id="configure":::

The preceding code:

- Chains a call to the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure*> API:
  - The `infra` parameter is an instance of the <xref:Aspire.Hosting.Azure.AzureResourceInfrastructure> type.
  - The provisionable resources are retrieved by calling the <xref:Azure.Provisioning.Infrastructure.GetProvisionableResources> method.
  - The single <xref:Azure.Provisioning.EventHubs.EventHubsNamespace> resource is retrieved.
  - The <xref:Azure.Provisioning.EventHubs.EventHubsNamespace.Sku?displayProperty=nameWithType> property is assigned to a new instance of <xref:Azure.Provisioning.EventHubs.EventHubsSku> with a `Premium` name and tier, and a `Capacity` of `7`.
  - The <xref:Azure.Provisioning.EventHubs.EventHubsNamespace.PublicNetworkAccess> property is assigned to `SecuredByPerimeter`.
  - A tag is added to the Event Hubs resource with a key of `ExampleKey` and a value of `Example value`.

There are many more configuration options available to customize the Event Hubs resource resource. For more information, see <xref:Azure.Provisioning.PostgreSql>. For more information, see [`Azure.Provisioning` customization](../azure/integrations-overview.md#azureprovisioning-customization).

### Connect to an existing Azure Event Hubs namespace

You might have an existing Azure Event Hubs namespace that you want to connect to. Instead of representing a new Azure Event Hubs resource, you can add a connection string to the app host. To add a connection to an existing Azure Event Hubs namespace, call the <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var eventHubs = builder.AddConnectionString("event-hubs");

builder.AddProject<Projects.WebApplication>("web")
       .WithReference(eventHubs);

// After adding all resources, run the app...
```

[!INCLUDE [connection-strings-alert](../includes/connection-strings-alert.md)]

The connection string is configured in the app host's configuration, typically under [User Secrets](/aspnet/core/security/app-secrets), under the `ConnectionStrings` section. The app host injects this connection string as an environment variable into all dependent resources, for example:

```json
{
  "ConnectionStrings": {
    "event-hubs": "{your_namespace}.servicebus.windows.net"
  }
}
```

The dependent resource can access the injected connection string by calling the <xref:Microsoft.Extensions.Configuration.ConfigurationExtensions.GetConnectionString*> method, and passing the connection name as the parameter, in this case `"event-hubs"`. The `GetConnectionString` API is shorthand for `IConfiguration.GetSection("ConnectionStrings")[name]`.

### Add Event Hub consumer group

To add a consumer group, chain a call on an `IResourceBuilder<AzureEventHubsResource>` to the <xref:Aspire.Hosting.AzureEventHubsExtensions.AddConsumerGroup*> API:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var eventHubs = builder.AddAzureEventHubs("event-hubs")
                       .AddConsumerGroup("messages");

builder.AddProject<Projects.ExampleService>()
       .WithReference(eventHubs);

// After adding all resources, run the app...
```

When you call `AddConsumerGroup`, it configures your Event Hubs resource to have a consumer group named `messages`. The consumer group is created in the Azure Event Hubs namespace that's represented by the `AzureEventHubsResource` that you added earlier. For more information, see [Azure Event Hubs: Consumer groups](/azure/event-hubs/event-hubs-features#consumer-groups).

### Add Azure Event Hubs emulator resource

The .NET Aspire Azure Event Hubs hosting integration supports running the Event Hubs resource as an emulator locally, based on the `mcr.microsoft.com/azure-messaging/eventhubs-emulator/latest` container image. This is beneficial for situations where you want to run the Event Hubs resource locally for development and testing purposes, avoiding the need to provision an Azure resource or connect to an existing Azure Event Hubs server.

To run the Event Hubs resource as an emulator, call the <xref:Aspire.Hosting.AzureEventHubsExtensions.RunAsEmulator*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var eventHubs = builder.AddAzureEventHubs("event-hubs")
                       .AddHub("messages")
                       .RunAsEmulator();

var exampleProject = builder.AddProject<Projects.ExampleProject>()
                            .WithReference(eventHubs);

// After adding all resources, run the app...
```

The preceding code configures an Azure Event Hubs resource to run locally in a container. For more information, see [Azure Event Hubs Emulator](/azure/event-hubs/overview-emulator).

#### Configure Event Hubs emulator container

There are various configurations available for container resources, for example, you can configure the container's ports, data bind mounts, data volumes, or providing a wholistic JSON configuration which overrides everything.

##### Configure Event Hubs emulator container host port

By default, the Event Hubs emulator container when configured by .NET Aspire, exposes the following endpoints:

| Endpoint | Image | Container port | Host port |
|--|--|--|--|
| `emulator` | `mcr.microsoft.com/azure-messaging/eventhubs-emulator/latest` | 5672 | dynamic |

The port that it's listening on is dynamic by default. When the container starts, the port is mapped to a random port on the host machine. To configure the endpoint port, chain calls on the container resource builder provided by the `RunAsEmulator` method as shown in the following example:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var eventHubs = builder.AddAzureEventHubs("event-hubs")
                       .AddHub("messages")
                       .RunAsEmulator(emulator =>
                       {
                           emulator.WithHostPort(7777);
                       });

builder.AddProject<Projects.ExampleService>()
       .WithReference(eventHubs);

// After adding all resources, run the app...
```

The preceding code configures the Azure Event emulator container's existing `emulator` endpoint to listen on port `7777`. The Azure Event emulator container's port is mapped to the host port as shown in the following table:

| Endpoint name | Port mapping (`container:host`) |
|---------------|---------------------------------|
| `emulator`    | `5672:7777`                     |

##### Add Event Hubs emulator with data volume

To add a data volume to the Event Hubs emulator resource, call the <xref:Aspire.Hosting.AzureEventHubsExtensions.WithDataVolume*> method on the Event Hubs emulator resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var eventHubs = builder.AddAzureEventHubs("event-hubs")
                       .AddHub("messages")
                       .RunAsEmulator(emulator =>
                       {
                           emulator.WithDataVolume();
                       });

builder.AddProject<Projects.ExampleService>()
       .WithReference(eventHubs);

// After adding all resources, run the app...
```

The data volume is used to persist the Event Hubs emulator data outside the lifecycle of its container. The data volume is mounted at the `/data` path in the container. A name is generated at random unless you provide a set the `name` parameter. For more information on data volumes and details on why they're preferred over [bind mounts](#add-event-hubs-emulator-with-data-bind-mount), see [Docker docs: Volumes](https://docs.docker.com/engine/storage/volumes).

##### Add Event Hubs emulator with data bind mount

The add a bind mount to the Event Hubs emulator container, chain a call to the <xref:Aspire.Hosting.AzureEventHubsExtensions.WithDataBindMount*> API, as shown in the following example:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var eventHubs = builder.AddAzureEventHubs("event-hubs")
                       .AddHub("messages")
                       .RunAsEmulator(emulator =>
                       {
                           emulator.WithDataBindMount("/path/to/data");
                       });

builder.AddProject<Projects.ExampleService>()
       .WithReference(eventHubs);

// After adding all resources, run the app...
```

[!INCLUDE [data-bind-mount-vs-volumes](../includes/data-bind-mount-vs-volumes.md)]

Data bind mounts rely on the host machine's filesystem to persist the Azure Event Hubs emulator resource data across container restarts. The data bind mount is mounted at the `/path/to/data` path on the host machine in the container. For more information on data bind mounts, see [Docker docs: Bind mounts](https://docs.docker.com/engine/storage/bind-mounts).

##### Configure Event Hubs emulator container JSON configuration

The Event Hubs emulator container runs with a default [_config.json_](https://github.com/Azure/azure-event-hubs-emulator-installer/blob/main/EventHub-Emulator/Config/Config.json) file. You can override this file entirely, or update the JSON configuration with a <xref:System.Text.Json.Nodes.JsonNode> representation of the configuration.

To provide a custom JSON configuration file, call the <xref:Aspire.Hosting.AzureEventHubsExtensions.WithConfigurationFile*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var eventHubs = builder.AddAzureEventHubs("event-hubs")
                       .AddHub("messages")
                       .RunAsEmulator(emulator =>
                       {
                           emulator.WithConfigurationFile("./messaging/custom-config.json");
                       });

builder.AddProject<Projects.ExampleService>()
       .WithReference(eventHubs);

// After adding all resources, run the app...
```

The preceding code configures the Event Hubs emulator container to use a custom JSON configuration file located at `./messaging/custom-config.json`. This will be mounted at the `/Eventhubs_Emulator/ConfigFiles/Config.json` path on the container, as a read-only file. To instead override specific properties in the default configuration, call the <xref:Aspire.Hosting.AzureEventHubsExtensions.WithConfiguration*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var eventHubs = builder.AddAzureEventHubs("event-hubs")
                       .AddHub("messages")
                       .RunAsEmulator(emulator =>
                       {
                           emulator.WithConfiguration(
                               (JsonNode configuration) =>
                               {
                                   var userConfig = configuration["UserConfig"];
                                   var ns = userConfig["NamespaceConfig"][0];
                                   var firstEntity = ns["Entities"][0];
                                   
                                   firstEntity["PartitionCount"] = 5;
                               });
                       });

builder.AddProject<Projects.ExampleService>()
       .WithReference(eventHubs);

// After adding all resources, run the app...
```

The preceding code retrieves the `UserConfig` node from the default configuration. It then updates the first entity's `PartitionCount` to a `5`.

### Hosting integration health checks

The Azure Event Hubs hosting integration automatically adds a health check for the Event Hubs resource. The health check verifies that the Event Hubs is running and that a connection can be established to it.

The hosting integration relies on the [📦 AspNetCore.HealthChecks.Azure.Messaging.EventHubs](https://www.nuget.org/packages/AspNetCore.HealthChecks.Azure.Messaging.EventHubs) NuGet package.

## Client integration

To get started with the .NET Aspire Azure Event Hubs client integration, install the [📦 Aspire.Azure.Messaging.EventHubs](https://www.nuget.org/packages/Aspire.Azure.Messaging.EventHubs) NuGet package in the client-consuming project, that is, the project for the application that uses the Event Hubs client.

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

### Supported Event Hubs client types

The following Event Hub clients are supported by the library, along with their corresponding options and settings classes:

| Azure client type | Azure options class | .NET Aspire settings class |
|--|--|--|
| <xref:Azure.Messaging.EventHubs.Producer.EventHubProducerClient> | <xref:Azure.Messaging.EventHubs.Producer.EventHubProducerClientOptions> | <xref:Aspire.Azure.Messaging.EventHubs.AzureMessagingEventHubsProducerSettings> |
| <xref:Azure.Messaging.EventHubs.Producer.EventHubBufferedProducerClient> | <xref:Azure.Messaging.EventHubs.Producer.EventHubBufferedProducerClientOptions> | <xref:Aspire.Azure.Messaging.EventHubs.AzureMessagingEventHubsBufferedProducerSettings> |
| <xref:Azure.Messaging.EventHubs.Consumer.EventHubConsumerClient> | <xref:Azure.Messaging.EventHubs.Consumer.EventHubConsumerClientOptions> | <xref:Aspire.Azure.Messaging.EventHubs.AzureMessagingEventHubsConsumerSettings> |
| <xref:Azure.Messaging.EventHubs.EventProcessorClient> | <xref:Azure.Messaging.EventHubs.EventProcessorClientOptions> | <xref:Aspire.Azure.Messaging.EventHubs.AzureMessagingEventHubsProcessorSettings> |
| <xref:Microsoft.Azure.EventHubs.PartitionReceiver> | <xref:Azure.Messaging.EventHubs.Primitives.PartitionReceiverOptions> | <xref:Aspire.Azure.Messaging.EventHubs.AzureMessagingEventHubsPartitionReceiverSettings> |

The client types are from the Azure SDK for .NET, as are the corresponding options classes. The settings classes are provided by the .NET Aspire. The settings classes are used to configure the client instances.

### Add an Event Hubs producer client

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireEventHubsExtensions.AddAzureEventHubProducerClient*> extension method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register an <xref:Azure.Messaging.EventHubs.Producer.EventHubProducerClient> for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddAzureEventHubProducerClient(connectionName: "event-hubs");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the Event Hubs resource in the app host project. For more information, see [Add an Azure Event Hubs resource](#add-an-azure-event-hubs-resource).

After adding the `EventHubProducerClient`, you can retrieve the client instance using dependency injection. For example, to retrieve your data source object from an example service define it as a constructor parameter and ensure the `ExampleService` class is registered with the dependency injection container:

```csharp
public class ExampleService(EventHubProducerClient client)
{
    // Use client...
}
```

For more information, see:

- [Azure.Messaging.EventHubs documentation](https://github.com/Azure/azure-sdk-for-net/blob/main/sdk/eventhub/Azure.Messaging.EventHubs/README.md) for examples on using the `EventHubProducerClient`.
- [Dependency injection in .NET](/dotnet/core/extensions/dependency-injection) for details on dependency injection.

#### Additional APIs to consider

The client integration provides additional APIs to configure client instances. When you need to register an Event Hubs client, consider the following APIs:

| Azure client type | Registration API |
|--|--|
| <xref:Azure.Messaging.EventHubs.Producer.EventHubProducerClient> | <xref:Microsoft.Extensions.Hosting.AspireEventHubsExtensions.AddAzureEventHubProducerClient*> |
| <xref:Azure.Messaging.EventHubs.Producer.EventHubBufferedProducerClient> | <xref:Microsoft.Extensions.Hosting.AspireEventHubsExtensions.AddAzureEventHubBufferedProducerClient*> |
| <xref:Azure.Messaging.EventHubs.Consumer.EventHubConsumerClient> | <xref:Microsoft.Extensions.Hosting.AspireEventHubsExtensions.AddAzureEventHubConsumerClient*> |
| <xref:Azure.Messaging.EventHubs.EventProcessorClient> | <xref:Microsoft.Extensions.Hosting.AspireEventHubsExtensions.AddAzureEventProcessorClient*> |
| <xref:Microsoft.Azure.EventHubs.PartitionReceiver> | <xref:Microsoft.Extensions.Hosting.AspireEventHubsExtensions.AddAzurePartitionReceiverClient*> |

All of the aforementioned APIs include optional parameters to configure the client instances.

### Add keyed Event Hubs producer client

There might be situations where you want to register multiple `EventHubProducerClient` instances with different connection names. To register keyed Event Hubs clients, call the <xref:Microsoft.Extensions.Hosting.AspireServiceBusExtensions.AddKeyedAzureServiceBusClient*> method:

```csharp
builder.AddKeyedAzureEventHubProducerClient(name: "messages");
builder.AddKeyedAzureEventHubProducerClient(name: "commands");
```

> [!IMPORTANT]
> When using keyed services, it's expected that your Event Hubs resource configured two named hubs, one for the `messages` and one for the `commands`.

Then you can retrieve the client instances using dependency injection. For example, to retrieve the clients from a service:

```csharp
public class ExampleService(
    [KeyedService("messages")] EventHubProducerClient messagesClient,
    [KeyedService("commands")] EventHubProducerClient commandsClient)
{
    // Use clients...
}
```

For more information, see [Keyed services in .NET](/dotnet/core/extensions/dependency-injection#keyed-services).

#### Additional keyed APIs to consider

The client integration provides additional APIs to configure keyed client instances. When you need to register a keyed Event Hubs client, consider the following APIs:

| Azure client type | Registration API |
|--|--|
| <xref:Azure.Messaging.EventHubs.Producer.EventHubProducerClient> | <xref:Microsoft.Extensions.Hosting.AspireEventHubsExtensions.AddKeyedAzureEventHubProducerClient*> |
| <xref:Azure.Messaging.EventHubs.Producer.EventHubBufferedProducerClient> | <xref:Microsoft.Extensions.Hosting.AspireEventHubsExtensions.AddKeyedAzureEventHubBufferedProducerClient*> |
| <xref:Azure.Messaging.EventHubs.Consumer.EventHubConsumerClient> | <xref:Microsoft.Extensions.Hosting.AspireEventHubsExtensions.AddKeyedAzureEventHubConsumerClient*> |
| <xref:Azure.Messaging.EventHubs.EventProcessorClient> | <xref:Microsoft.Extensions.Hosting.AspireEventHubsExtensions.AddKeyedAzureEventProcessorClient*> |
| <xref:Microsoft.Azure.EventHubs.PartitionReceiver> | <xref:Microsoft.Extensions.Hosting.AspireEventHubsExtensions.AddKeyedAzurePartitionReceiverClient*> |

All of the aforementioned APIs include optional parameters to configure the client instances.

### Configuration

The .NET Aspire Azure Event Hubs library provides multiple options to configure the Azure Event Hubs connection based on the requirements and conventions of your project. Either a `FullyQualifiedNamespace` or a `ConnectionString` is a required to be supplied.

#### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, provide the name of the connection string when calling `builder.AddAzureEventHubProducerClient()` and other supported Event Hubs clients. In this example, the connection string does not include the `EntityPath` property, so the `EventHubName` property must be set in the settings callback:

```csharp
builder.AddAzureEventHubProducerClient(
    "event-hubs",
    static settings =>
    {
        settings.EventHubName = "MyHub";
    });
```

And then the connection information will be retrieved from the `ConnectionStrings` configuration section. Two connection formats are supported:

##### Fully Qualified Namespace (FQN)

The recommended approach is to use a fully qualified namespace, which works with the <xref:Aspire.Azure.Messaging.EventHubs.AzureMessagingEventHubsSettings.Credential?displayProperty=nameWithType> property to establish a connection. If no credential is configured, the <xref:Azure.Identity.DefaultAzureCredential> is used.

```json
{
  "ConnectionStrings": {
    "event-hubs": "{your_namespace}.servicebus.windows.net"
  }
}
```

#### Connection string

Alternatively, use a connection string:

```json
{
  "ConnectionStrings": {
    "event-hubs": "Endpoint=sb://mynamespace.servicebus.windows.net/;SharedAccessKeyName=accesskeyname;SharedAccessKey=accesskey;EntityPath=MyHub"
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
    "event-hubs",
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

- `Azure.Messaging.EventHubs.*`

### Metrics

The .NET Aspire Azure Event Hubs integration currently doesn't support metrics by default due to limitations with the Azure SDK for .NET. If that changes in the future, this section will be updated to reflect those changes.

## See also

- [Azure Event Hubs](/azure/event-hubs/)
- [.NET Aspire Azure integrations overview](../azure/integrations-overview.md)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
