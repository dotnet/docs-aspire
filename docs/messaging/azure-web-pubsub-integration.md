---
title: .NET Aspire Azure Web PubSub integration
description: This article describes the .NET Aspire Azure Web PubSub integration features and capabilities.
ms.topic: how-to
ms.date: 04/01/2025
---

# .NET Aspire Azure Web PubSub integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[Azure Web PubSub](/azure/azure-web-pubsub/) is a fully managed real-time messaging service that enables you to build real-time web applications using WebSockets and publish-subscribe patterns. The .NET Aspire Azure Web PubSub integration enables you to connect to Azure Web PubSub instances from your .NET applications.

## Hosting integration

The .NET Aspire [Azure Web PubSub](https://azure.microsoft.com/products/web-pubsub) hosting integration models the Web PubSub resources as the following types:

- <xref:Aspire.Hosting.ApplicationModel.AzureWebPubSubResource>: Represents an Azure Web PubSub resource, including connection information to the underlying Azure resource.
- <xref:Aspire.Hosting.ApplicationModel.AzureWebPubSubHubResource>: Represents a Web PubSub hub settings resource, which contains the settings for a hub. For example, you can specify if the hub allows anonymous connections or add event handlers to the hub.

To access these types and APIs for expressing them within your [app host](xref:dotnet/aspire/app-host) project, install the [📦 Aspire.Hosting.Azure.WebPubSub](https://www.nuget.org/packages/Aspire.Hosting.Azure.WebPubSub) NuGet package:

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure.WebPubSub
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure.WebPubSub"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add an Azure Web PubSub resource

To add an Azure Web PubSub resource to your app host project, call the <xref:Aspire.Hosting.AzureWebPubSubExtensions.AddAzureWebPubSub*> method providing a name:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var webPubSub = builder.AddAzureWebPubSub("web-pubsub");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(webPubSub);

// After adding all resources, run the app...
```

The preceding code adds an Azure Web PubSub resource named `web-pubsub` to the app host project. The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference*> method passes the connection information to the `ExampleProject` project.

> [!IMPORTANT]
> When you call `AddAzureWebPubSub`, it implicitly calls <xref:Aspire.Hosting.AzureProvisionerExtensions.AddAzureProvisioning(Aspire.Hosting.IDistributedApplicationBuilder)>—which adds support for generating Azure resources dynamically during app startup. The app must configure the appropriate subscription and location. For more information, see [Local provisioning: Configuration](../azure/local-provisioning.md#configuration).

### Add an Azure Web PubSub hub resource

When you add an Azure Web PubSub resource, you can also add a child hub resource. The hub resource is a logical grouping of connections and event handlers. To add an Azure Web PubSub hub resource to your app host project, chain a call to the <xref:Aspire.Hosting.AzureWebPubSubExtensions.AddHub*> method providing a resource and hub name:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var worker = builder.AddProject<Projects.WorkerService>("worker")
                    .WithExternalHttpEndpoints();

var webPubSub = builder.AddAzureWebPubSub("web-pubsub");
var messagesHub = webPubSub.AddHub(name: "messages", hubName: "messageHub");

// After adding all resources, run the app...
```

The preceding code adds an Azure Web PubSub hub resource named `messages` and a hub name of `messageHub` , which enables the addition of event handlers. To add an event handler, call the <xref:Aspire.Hosting.AzureWebPubSubExtensions.AddEventHandler*>:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var worker = builder.AddProject<Projects.WorkerService>("worker")
                    .WithExternalHttpEndpoints();

var webPubSub = builder.AddAzureWebPubSub("web-pubsub");
var messagesHub = webPubSub.AddHub(name: "messages", hubName: "messageHub");

messagesHub.AddEventHandler(
    $"{worker.GetEndpoint("https")}/eventhandler/",
    systemEvents: ["connected"]);

// After adding all resources, run the app...
```

The preceding code adds a worker service project named `worker` with an external HTTP endpoint. The hub named `messages` resource is added to the `web-pubsub` resource, and an event handler is added to the `messagesHub` resource. The event handler URL is set to the worker service's external HTTP endpoint. For more information, see [Azure Web PubSub event handlers](/azure/azure-web-pubsub/howto-develop-eventhandler).

#### Generated provisioning Bicep

When you publish your app, .NET Aspire provisioning APIs generate Bicep alongside the manifest file. Bicep is a domain-specific language for defining Azure resources. For more information, see [Bicep Overview](/azure/azure-resource-manager/bicep/overview).

When you add an Azure Web PubSub resource, the following Bicep is generated:

:::code language="bicep" source="../snippets/azure/AppHost/web-pubsub.module.bicep":::

The preceding Bicep is a module that provisions an Azure Web PubSub resource with the following defaults:

- `location`: The location of the resource group.
- `sku`: The SKU of the Web PubSub resource, defaults to `Free_F1`.
- `principalId`: The principal ID of the Web PubSub resource.
- `principalType`: The principal type of the Web PubSub resource.
- `messages_url_0`: The URL of the event handler for the `messages` hub.
- `messages`: The name of the hub resource.
- `web_pubsub`: The name of the Web PubSub resource.
- `web_pubsub_WebPubSubServiceOwner`: The role assignment for the Web PubSub resource owner. For more information, see [Azure Web PubSub Service Owner](/azure/role-based-access-control/built-in-roles/web-and-mobile#web-pubsub-service-owner).
- `endpoint`: The endpoint of the Web PubSub resource.

The generated Bicep is a starting point and is influenced by changes to the provisioning infrastructure in C#. Customizations to the Bicep file directly will be overwritten, so make changes through the C# provisioning APIs to ensure they are reflected in the generated files.

#### Customize provisioning infrastructure

All .NET Aspire Azure resources are subclasses of the <xref:Aspire.Hosting.Azure.AzureProvisioningResource> type. This type enables the customization of the generated Bicep by providing a fluent API to configure the Azure resources—using the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure``1(Aspire.Hosting.ApplicationModel.IResourceBuilder{``0},System.Action{Aspire.Hosting.Azure.AzureResourceInfrastructure})> API. For example, you can configure the `kind`, `consistencyPolicy`, `locations`, and more. The following example demonstrates how to customize the Azure Cosmos DB resource:

:::code language="csharp" source="../snippets/azure/AppHost/Program.ConfigureWebPubSubInfra.cs" id="configure":::

The preceding code:

- Chains a call to the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure*> API:
  - The `infra` parameter is an instance of the <xref:Aspire.Hosting.Azure.AzureResourceInfrastructure> type.
  - The provisionable resources are retrieved by calling the <xref:Azure.Provisioning.Infrastructure.GetProvisionableResources> method.
  - The single <xref:Azure.Provisioning.WebPubSub.WebPubSubService> resource is retrieved.
  - The <xref:Azure.Provisioning.WebPubSub.WebPubSubService.Sku?displayProperty=nameWithType> object has its name and capacity properties set to `Standard_S1` and `5`, respectively.
  - A tag is added to the Web PubSub resource with a key of `ExampleKey` and a value of `Example value`.

There are many more configuration options available to customize the Web PubSub resource. For more information, see <xref:Azure.Provisioning.WebPubSub>. For more information, see [`Azure.Provisioning` customization](../azure/integrations-overview.md#azureprovisioning-customization).

### Connect to an existing Azure Web PubSub instance

You might have an existing Azure Web PubSub service that you want to connect to. You can chain a call to annotate that your <xref:Aspire.Hosting.ApplicationModel.AzureWebPubSubResource> is an existing resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var existingPubSubName = builder.AddParameter("existingPubSubName");
var existingPubSubResourceGroup = builder.AddParameter("existingPubSubResourceGroup");

var webPubSub = builder.AddAzureWebPubSub("web-pubsub")
                       .AsExisting(existingPubSubName, existingPubSubResourceGroup);

builder.AddProject<Projects.ExampleProject>()
       .WithReference(webPubSub);

// After adding all resources, run the app...
```

For more information on treating Azure Web PubSub resources as existing resources, see [Use existing Azure resources](../azure/integrations-overview.md#use-existing-azure-resources).

Alternatively, instead of representing an Azure Web PubSub resource, you can add a connection string to the app host. Which is a weakly-typed approach that's based solely on a `string` value. To add a connection to an existing Azure Web PubSub service, call the <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString%2A> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var webPubSub = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureWebPubSub("web-pubsub")
    : builder.AddConnectionString("web-pubsub");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(webPubSub);

// After adding all resources, run the app...
```

[!INCLUDE [connection-strings-alert](../includes/connection-strings-alert.md)]

The connection string is configured in the app host's configuration, typically under User Secrets, under the `ConnectionStrings` section:

```json
{
  "ConnectionStrings": {
    "web-pubsub": "https://{account_name}.webpubsub.azure.com"
  }
}
```

For more information, see [Add existing Azure resources with connection strings](../azure/integrations-overview.md#add-existing-azure-resources-with-connection-strings).

## Client integration

The .NET Aspire Azure Web PubSub client integration is used to connect to an Azure Web PubSub service using the <xref:Azure.Messaging.WebPubSub.WebPubSubServiceClient>. To get started with the .NET Aspire Azure Web PubSub service client integration, install the [📦 Aspire.Azure.Messaging.WebPubSub](https://www.nuget.org/packages/Aspire.Azure.Messaging.WebPubSub) NuGet package in the application.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Azure.Messaging.WebPubSub
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Azure.Messaging.WebPubSub"
                  Version="*" />
```

---

### Supported Web PubSub client types

The following Web PubSub client types are supported by the library:

| Azure client type | Azure options class | .NET Aspire settings class |
|--|--|--|
| <xref:Azure.Messaging.WebPubSub.WebPubSubServiceClient> | <xref:Azure.Messaging.WebPubSub.WebPubSubServiceClientOptions> | <xref:Aspire.Azure.Messaging.WebPubSub.AzureMessagingWebPubSubSettings> |

### Add Web PubSub client

In the _Program.cs_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireWebPubSubExtensions.AddAzureWebPubSubServiceClient*> extension method to register a `WebPubSubServiceClient` for use via the dependency injection container. The method takes a connection name parameter:

```csharp
builder.AddAzureWebPubSubServiceClient(
    connectionName: "web-pubsub");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the Web PubSub resource in the app host project. For more information, see [Add an Azure Web PubSub resource](#add-an-azure-web-pubsub-resource).

After adding the `WebPubSubServiceClient`, you can retrieve the client instance using dependency injection. For example, to retrieve your data source object from an example service define it as a constructor parameter and ensure the `ExampleService` class is registered with the dependency injection container:

```csharp
public class ExampleService(WebPubSubServiceClient client)
{
    // Use client...
}
```

For more information, see:

- [Azure.Messaging.WebPubSub documentation](/azure/azure-web-pubsub/howto-create-serviceclient-with-net-and-azure-identity) for examples on using the `WebPubSubServiceClient`.
- [Dependency injection in .NET](/dotnet/core/extensions/dependency-injection) for details on dependency injection.

### Add keyed Web PubSub client

There might be situations where you want to register multiple `WebPubSubServiceClient` instances with different connection names. To register keyed Web PubSub clients, call the <xref:Microsoft.Extensions.Hosting.AspireWebPubSubExtensions.AddKeyedAzureWebPubSubServiceClient*> method:

```csharp
builder.AddKeyedAzureWebPubSubServiceClient(name: "messages");
builder.AddKeyedAzureWebPubSubServiceClient(name: "commands");
```

> [!IMPORTANT]
> When using keyed services, it's expected that your Web PubSub resource configured two named hubs, one for the `messages` and one for the `commands`.

Then you can retrieve the client instances using dependency injection. For example, to retrieve the clients from a service:

```csharp
public class ExampleService(
    [KeyedService("messages")] WebPubSubServiceClient messagesClient,
    [KeyedService("commands")] WebPubSubServiceClient commandsClient)
{
    // Use clients...
}
```

If you want to register a single `WebPubSubServiceClient` instance with a specific connection name, there's an overload that uses the connection name as the service key. Call the `AddAzureWebPubSubServiceClient` method. This method registers the client as a singleton service in the dependency injection container.

```csharp
builder.AddKeyedAzureWebPubSubServiceClient(connectionName: "web-pubsub");
```

For more information, see [Keyed services in .NET](/dotnet/core/extensions/dependency-injection#keyed-services).

### Configuration

The .NET Aspire Azure Web PubSub library provides multiple options to configure the Azure Web PubSub connection based on the requirements and conventions of your project. Either an `Endpoint` or a `ConnectionString` must be supplied.

#### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `AddAzureWebPubSubServiceClient`:

```csharp
builder.AddAzureWebPubSubServiceClient(
    "web-pubsub",
    settings => settings.HubName = "your_hub_name");
```

The connection information is retrieved from the `ConnectionStrings` configuration section. Two connection formats are supported:

- **Service endpoint (recommended)**: Uses the service endpoint with `DefaultAzureCredential`.

    ```json
    {
      "ConnectionStrings": {
        "web-pubsub": "https://{account_name}.webpubsub.azure.com"
      }
    }
    ```

- **Connection string**: Includes an access key.

    ```json
    {
      "ConnectionStrings": {
        "web-pubsub": "Endpoint=https://{account_name}.webpubsub.azure.com;AccessKey={account_key}"
      }
    }
    ```

#### Use configuration providers

The library supports <xref:Microsoft.Extensions.Configuration>. It loads settings from configuration using the `Aspire:Azure:Messaging:WebPubSub` key:

```json
{
  "Aspire": {
    "Azure": {
      "Messaging": {
        "WebPubSub": {
          "DisableHealthChecks": true,
          "HubName": "your_hub_name"
        }
      }
    }
  }
}
```

For the complete Azure OpenAI client integration JSON schema, see [Aspire.Azure.Messaging.WebPubSub/ConfigurationSchema.json](https://github.com/dotnet/aspire/blob/main/src/Components/Aspire.Azure.Messaging.WebPubSub/ConfigurationSchema.json).

#### Use inline delegates

You can configure settings inline:

```csharp
builder.AddAzureWebPubSubServiceClient(
    "web-pubsub",
    settings => settings.DisableHealthChecks = true);
```

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

#### Logging

The .NET Aspire Azure Web PubSub integration uses the following log categories:

- `Azure`
- `Azure.Core`
- `Azure.Identity`
- `Azure.Messaging.WebPubSub`

#### Tracing

The .NET Aspire Azure Web PubSub integration will emit the following tracing activities using OpenTelemetry:

- `Azure.Messaging.WebPubSub.*`

#### Metrics

The .NET Aspire Azure Web PubSub integration currently doesn't support metrics by default due to limitations with the Azure SDK for .NET. If that changes in the future, this section will be updated to reflect those changes.

## See also

- [Azure Web PubSub](/azure/azure-web-pubsub/)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
