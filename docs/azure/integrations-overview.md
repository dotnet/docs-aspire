---
title: Azure integrations overview
description: Overview of the Azure integrations available in the .NET Aspire.
ms.date: 07/22/2025
uid: dotnet/aspire/integrations/azure-overview
---

# .NET Aspire Azure integrations overview

[Azure](/azure) is the most popular cloud platform for building and deploying [.NET applications](/dotnet/azure). The [Azure SDK for .NET](/dotnet/azure/sdk/azure-sdk-for-dotnet) allows for easy management and use of Azure services. .NET Aspire provides a set of integrations with Azure services, where you're free to add new resources or connect to existing ones. This article details some common aspects of all Azure integrations in .NET Aspire and aims to help you understand how to use them.

## Add Azure resources

All .NET Aspire Azure hosting integrations expose Azure resources and by convention are added using `AddAzure*` APIs. When you add these resources to your .NET Aspire AppHost, they represent an Azure service. The `AddAzure*` API returns an <xref:Aspire.Hosting.ApplicationModel.IResourceBuilder`1> where `T` is the type of Azure resource. These `IResourceBuilder<T>` (builder) interfaces provide a fluent API that allows you to configure the underlying Azure resource within the [app model](xref:dotnet/aspire/app-host#terminology). There are APIs for adding new Azure resources, marking resources as existing, and configuring how the resources behave in various execution contexts.

### Typical developer experience

When your .NET Aspire AppHost contains Azure resources, and you run it locally (typical developer <kbd>F5</kbd> or `dotnet run` experience), the [Azure resources are provisioned](local-provisioning.md) in your Azure subscription. This allows you as the developer to debug against them locally in the context of your AppHost.

.NET Aspire aims to minimize costs by defaulting to _Basic_ or _Standard_ [Stock Keeping Unit (SKU)](/partner-center/developer/product-resources#sku) for its Azure integrations. While these sensible defaults are provided, you can [customize the Azure resources](customize-azure-resources.md#azureprovisioning-customization) to suit your needs. Additionally, some integrations support [emulators](#local-emulators) or [containers](#local-containers), which are useful for local development, testing, and debugging. By default, when you run your app locally, the Azure resources use the actual Azure service. However, you can configure them to use local emulators or containers, avoiding costs associated with the actual Azure service during local development.

### Local emulators

Some Azure services can be emulated to run locally. Currently, .NET Aspire supports the following Azure emulators:

| Hosting integration | Description |
|--|--|
| [Azure AI Foundry](../azureai/azureai-foundry-integration.md) | Call `RunAsFoundryLocal` on the `IResourceBuilder<AzureAIFoundryResource>` to configure the resource to be [emulated with Foundry Local](/azure/ai-foundry/foundry-local/get-started). |
| [Azure Cosmos DB](../database/azure-cosmos-db-integration.md) | Call <xref:Aspire.Hosting.AzureCosmosExtensions.RunAsEmulator*?displayProperty=nameWithType> on the `IResourceBuilder<AzureCosmosDBResource>` to configure the Cosmos DB resource to be [emulated with the NoSQL API](/azure/cosmos-db/how-to-develop-emulator). |
| [Azure Event Hubs](../messaging/azure-event-hubs-integration.md) | Call <xref:Aspire.Hosting.AzureEventHubsExtensions.RunAsEmulator*?displayProperty=nameWithType> on the `IResourceBuilder<AzureEventHubsResource>` to configure the Event Hubs resource to be [emulated](/azure/event-hubs/overview-emulator). |
| [Azure Service Bus](../messaging/azure-service-bus-integration.md) | Call <xref:Aspire.Hosting.AzureServiceBusExtensions.RunAsEmulator*?displayProperty=nameWithType> on the `IResourceBuilder<AzureServiceBusResource>` to configure the Service Bus resource to be [emulated with Service Bus emulator](/azure/service-bus-messaging/overview-emulator). |
| [Azure SignalR Service](../real-time/azure-signalr-scenario.md) | Call <xref:Aspire.Hosting.AzureSignalRExtensions.RunAsEmulator*?displayProperty=nameWithType> on the `IResourceBuilder<AzureSignalRResource>` to configure the SignalR resource to be [emulated with Azure SignalR emulator](/azure/azure-signalr/signalr-howto-emulator). |
| [Azure Storage](../storage/azure-storage-blobs-integration.md) | Call <xref:Aspire.Hosting.AzureStorageExtensions.RunAsEmulator*?displayProperty=nameWithType> on the `IResourceBuilder<AzureStorageResource>` to configure the Storage resource to be [emulated with Azurite](/azure/storage/common/storage-use-azurite). |

To have your Azure resources use the local emulators, chain a call the `RunAsEmulator` method on the Azure resource builder. This method configures the Azure resource to use the local emulator instead of the actual Azure service.

> [!IMPORTANT]
> Calling any of the available `RunAsEmulator` APIs on an Azure resource builder doesn't effect the [publishing manifest](../deployment/manifest-format.md). When you publish your app, [the generated Bicep file](customize-azure-resources.md) reflects the actual Azure service, not the local emulator.

### Local containers

Some Azure resources can be substituted locally using open-source or on-premises containers. To substitute an Azure resource locally in a container, chain a call to the `RunAsContainer` method on the Azure resource builder. This method configures the Azure resource to use a containerized version of the service for local development and testing, rather than the actual Azure service.

Currently, .NET Aspire supports the following Azure services as containers:

| Hosting integration | Details |
|--|--|
| [Azure Cache for Redis](../caching/azure-cache-for-redis-integration.md) | Call <xref:Aspire.Hosting.AzureRedisExtensions.RunAsContainer*?displayProperty=nameWithType> on the `IResourceBuilder<AzureRedisCacheResource>` to configure it to run locally in a container, based on the `docker.io/library/redis` image. |
| [Azure PostgreSQL Flexible Server](../database/azure-postgresql-integration.md) | Call <xref:Aspire.Hosting.AzurePostgresExtensions.RunAsContainer*?displayProperty=nameWithType> on the `IResourceBuilder<AzurePostgresFlexibleServerResource>` to configure it to run locally in a container, based on the `docker.io/library/postgres` image. |
| [Azure SQL Server](../database/azure-sql-integration.md) | Call <xref:Aspire.Hosting.AzureSqlExtensions.RunAsContainer*?displayProperty=nameWithType> on the `IResourceBuilder<AzureSqlServerResource>` to configure it to run locally in a container, based on the `mcr.microsoft.com/mssql/server` image. |

> [!NOTE]
> Like emulators, calling `RunAsContainer` on an Azure resource builder doesn't effect the [publishing manifest](../deployment/manifest-format.md). When you publish your app, the [generated Bicep file](customize-azure-resources.md) reflects the actual Azure service, not the local container.

### Understand Azure integration APIs

.NET Aspire's strength lies in its ability to provide an amazing developer inner-loop. The Azure integrations are no different. They provide a set of common APIs and patterns that are shared across all Azure resources. These APIs and patterns are designed to make it easy to work with Azure resources in a consistent manner.

In the preceding containers section, you saw how to run Azure services locally in containers. If you're familiar with .NET Aspire, you might wonder how calling `AddAzureRedis("redis").RunAsContainer()` to get a local `docker.io/library/redis` container differs from `AddRedis("redis")`—as they both result in the same local container.

The answer is that there's no difference when running locally. However, when they're published you get different resources:

| API | Run mode | Publish mode |
|--|--|--|
| [AddAzureRedis("redis").RunAsContainer()](xref:Aspire.Hosting.AzureRedisExtensions.RunAsContainer*) | Local Redis container | Azure Cache for Redis |
| [AddRedis("redis")](xref:Aspire.Hosting.RedisBuilderExtensions.AddRedis*) | Local Redis container | Azure Container App with Redis image |

The same is true for SQL and PostgreSQL services:

| API | Run mode | Publish mode |
|--|--|--|
| [AddAzurePostgresFlexibleServer("postgres").RunAsContainer()](xref:Aspire.Hosting.AzurePostgresExtensions.RunAsContainer*) | Local PostgreSQL container | Azure PostgreSQL Flexible Server |
| [AddPostgres("postgres")](xref:Aspire.Hosting.PostgresBuilderExtensions.AddPostgres*) | Local PostgreSQL container | Azure Container App with PostgreSQL image |
| [AddAzureSqlServer("sql").RunAsContainer()](xref:Aspire.Hosting.AzureSqlExtensions.RunAsContainer*) | Local SQL Server container | Azure SQL Server |
| [AddSqlServer("sql")](xref:Aspire.Hosting.SqlServerBuilderExtensions.AddSqlServer*) | Local SQL Server container | Azure Container App with SQL Server image |

For more information on the difference between run and publish modes, see [.NET Aspire AppHost: Execution context](xref:dotnet/aspire/app-host#execution-context).

### APIs for expressing Azure resources in different modes

The distributed application builder, part of the [AppHost](../fundamentals/app-host-overview.md), uses the builder pattern to `AddAzure*` resources to the [_app model_](../fundamentals/app-host-overview.md#terminology). Developers can configure these resources and define their behavior in different execution contexts. Azure hosting integrations provide APIs to specify how these resources should be "published" and "run."

When the AppHost executes, the [_execution context_](../fundamentals/app-host-overview.md#execution-context) is used to determine whether the AppHost is in <xref:Aspire.Hosting.DistributedApplicationOperation.Run> or <xref:Aspire.Hosting.DistributedApplicationOperation.Publish> mode. The naming conventions for these APIs indicate the intended action for the resource.

The following table summarizes the naming conventions used to express Azure resources:

| Operation | API | Description |
|--|--|--|
| Publish | <xref:Aspire.Hosting.AzureResourceExtensions.PublishAsConnectionString``1(Aspire.Hosting.ApplicationModel.IResourceBuilder{``0})> | Changes the resource to be published as a connection string reference in the manifest. |
| Publish | <xref:Aspire.Hosting.ExistingAzureResourceExtensions.PublishAsExisting*> | Uses an existing Azure resource when the application is deployed instead of creating a new one. |
| Run | <xref:Aspire.Hosting.AzureSqlExtensions.RunAsContainer(Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.Azure.AzureSqlServerResource},System.Action{Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.ApplicationModel.SqlServerServerResource}})?displayProperty=nameWithType> <br/> <xref:Aspire.Hosting.AzureRedisExtensions.RunAsContainer(Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.Azure.AzureRedisCacheResource},System.Action{Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.ApplicationModel.RedisResource}})?displayProperty=nameWithType> <br/> <xref:Aspire.Hosting.AzurePostgresExtensions.RunAsContainer(Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.Azure.AzurePostgresFlexibleServerResource},System.Action{Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.ApplicationModel.PostgresServerResource}})> | Configures an equivalent container to run locally. For more information, see [Local containers](#local-containers). |
| Run | <xref:Aspire.Hosting.AzureCosmosExtensions.RunAsEmulator(Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.AzureCosmosDBResource},System.Action{Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.Azure.AzureCosmosDBEmulatorResource}})?displayProperty=nameWithType> <br/> <xref:Aspire.Hosting.AzureSignalRExtensions.RunAsEmulator(Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.ApplicationModel.AzureSignalRResource},System.Action{Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.Azure.AzureSignalREmulatorResource}})?displayProperty=nameWithType> <br/> <xref:Aspire.Hosting.AzureStorageExtensions.RunAsEmulator(Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.Azure.AzureStorageResource},System.Action{Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.Azure.AzureStorageEmulatorResource}})?displayProperty=nameWithType> <br/> <xref:Aspire.Hosting.AzureEventHubsExtensions.RunAsEmulator(Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.Azure.AzureEventHubsResource},System.Action{Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.Azure.AzureEventHubsEmulatorResource}})?displayProperty=nameWithType> <br/> <xref:Aspire.Hosting.AzureServiceBusExtensions.RunAsEmulator(Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.Azure.AzureServiceBusResource},System.Action{Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.Azure.AzureServiceBusEmulatorResource}})?displayProperty=nameWithType> | Configures the Azure resource to be emulated. For more information, see [Local emulators](#local-emulators). |
| Run | <xref:Aspire.Hosting.ExistingAzureResourceExtensions.RunAsExisting*> | Uses an existing resource when the application is running instead of creating a new one. |
| Publish and Run | <xref:Aspire.Hosting.ExistingAzureResourceExtensions.AsExisting``1(Aspire.Hosting.ApplicationModel.IResourceBuilder{``0},Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.ApplicationModel.ParameterResource},Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.ApplicationModel.ParameterResource})> | Uses an existing resource regardless of the operation. |

> [!NOTE]
> Not all APIs are available on all Azure resources. For example, some Azure resources can be containerized or emulated, while others can't.

For more information on execution modes, see [Execution context](../fundamentals/app-host-overview.md#execution-context).

#### General run mode API use cases

Use <xref:Aspire.Hosting.ExistingAzureResourceExtensions.RunAsExisting*> when you need to dynamically interact with an existing resource during runtime without needing to deploy or update it. Use <xref:Aspire.Hosting.ExistingAzureResourceExtensions.PublishAsExisting*> when declaring existing resources as part of a deployment configuration, ensuring the correct scopes and permissions are applied. Finally, use <xref:Aspire.Hosting.ExistingAzureResourceExtensions.AsExisting``1(Aspire.Hosting.ApplicationModel.IResourceBuilder{``0},Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.ApplicationModel.ParameterResource},Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.ApplicationModel.ParameterResource})> when declaring existing resources in both configurations, with a requirement to parameterize the references.

You can query whether a resource is marked as an existing resource, by calling the <xref:Aspire.Hosting.ExistingAzureResourceExtensions.IsExisting(Aspire.Hosting.ApplicationModel.IResource)> extension method on the <xref:Aspire.Hosting.ApplicationModel.IResource>. For more information, see [Use existing Azure resources](#use-existing-azure-resources).

## Use existing Azure resources

.NET Aspire provides support for referencing existing Azure resources. You mark an existing resource through the `PublishAsExisting`, `RunAsExisting`, and `AsExisting` APIs. These APIs allow developers to reference already-deployed Azure resources, configure them, and generate appropriate deployment manifests using Bicep templates.

[!INCLUDE [azure-configuration](includes/azure-configuration.md)]

Existing resources referenced with these APIs can be enhanced with [role assignments](role-assignments.md) and other customizations that are available with .NET Aspire's [infrastructure as code capabilities](customize-azure-resources.md). These APIs are limited to Azure resources that can be deployed with Bicep templates.

### Configure existing Azure resources for run mode

The <xref:Aspire.Hosting.ExistingAzureResourceExtensions.RunAsExisting*> method is used when a distributed application is executing in "run" mode. In this mode, it assumes that the referenced Azure resource already exists and integrates with it during execution without provisioning the resource. To mark an Azure resource as existing, call the `RunAsExisting` method on the resource builder. Consider the following example:

```csharp
var builder = DistributedApplication.CreateBuilder();

var existingServiceBusName = builder.AddParameter("existingServiceBusName");
var existingServiceBusResourceGroup = builder.AddParameter("existingServiceBusResourceGroup");

var serviceBus = builder.AddAzureServiceBus("messaging")
                        .RunAsExisting(existingServiceBusName, existingServiceBusResourceGroup);

serviceBus.AddServiceBusQueue("queue");
```

The preceding code:

- Creates a new `builder` instance.
- Adds a parameter named `existingServiceBusName` to the builder.
- Adds an Azure Service Bus resource named `messaging` to the builder.
- Calls the `RunAsExisting` method on the `serviceBus` resource builder, passing the `existingServiceBusName` parameter—alternatively, you can use the `string` parameter overload.
- Adds a queue named `queue` to the `serviceBus` resource.

By default, the Service Bus parameter reference is assumed to be in the same Azure resource group. However, if it's in a different resource group, you can pass the resource group explicitly as a parameter to correctly specify the appropriate resource grouping.

### Configure existing Azure resources for publish mode

The <xref:Aspire.Hosting.ExistingAzureResourceExtensions.PublishAsExisting*> method is used in "publish" mode when the intent is to declare and reference an already-existing Azure resource during publish mode. This API facilitates the creation of manifests and templates that include resource definitions that map to existing resources in Bicep.

To mark an Azure resource as existing in for the "publish" mode, call the `PublishAsExisting` method on the resource builder. Consider the following example:

```csharp
var builder = DistributedApplication.CreateBuilder();

var existingServiceBusName = builder.AddParameter("existingServiceBusName");
var existingServiceBusResourceGroup = builder.AddParameter("existingServiceBusResourceGroup");

var serviceBus = builder.AddAzureServiceBus("messaging")
                        .PublishAsExisting(existingServiceBusName, existingServiceBusResourceGroup);

serviceBus.AddServiceBusQueue("queue");
```

The preceding code:

- Creates a new `builder` instance.
- Adds a parameter named `existingServiceBusName` to the builder.
- Adds an Azure Service Bus resource named `messaging` to the builder.
- Calls the `PublishAsExisting` method on the `serviceBus` resource builder, passing the `existingServiceBusName` parameter—alternatively, you can use the `string` parameter overload.
- Adds a queue named `queue` to the `serviceBus` resource.

After the AppHost is executed in publish mode, the generated manifest file will include the `existingResourceName` parameter, which can be used to reference the existing Azure resource. Consider the following generated partial snippet of the manifest file:

```json
"messaging": {
  "type": "azure.bicep.v0",
  "connectionString": "{messaging.outputs.serviceBusEndpoint}",
  "path": "messaging.module.bicep",
  "params": {
    "existingServiceBusName": "{existingServiceBusName.value}",
    "principalType": "",
    "principalId": ""
  }
},
"queue": {
  "type": "value.v0",
  "connectionString": "{messaging.outputs.serviceBusEndpoint}"
}
```

For more information on the manifest file, see [.NET Aspire manifest format for deployment tool builders](../deployment/manifest-format.md).

Additionally, the generated Bicep template includes the `existingResourceName` parameter, which can be used to reference the existing Azure resource. Consider the following generated Bicep template:

```bicep
@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param existingServiceBusName string

param principalType string

param principalId string

resource messaging 'Microsoft.ServiceBus/namespaces@2024-01-01' existing = {
  name: existingServiceBusName
}

resource messaging_AzureServiceBusDataOwner 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(messaging.id, principalId, subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '090c5cfd-751d-490a-894a-3ce6f1109419'))
  properties: {
    principalId: principalId
    roleDefinitionId: subscriptionResourceId('Microsoft.Authorization/roleDefinitions', '090c5cfd-751d-490a-894a-3ce6f1109419')
    principalType: principalType
  }
  scope: messaging
}

resource queue 'Microsoft.ServiceBus/namespaces/queues@2024-01-01' = {
  name: 'queue'
  parent: messaging
}

output serviceBusEndpoint string = messaging.properties.serviceBusEndpoint
```

For more information on the generated Bicep templates, see [Customize Azure resources](customize-azure-resources.md) and [consider other publishing APIs](#publish-as-azure-container-app).

> [!WARNING]
> When interacting with existing resources that require authentication, ensure the authentication strategy that you're configuring in the .NET Aspire application model aligns with the authentication strategy allowed by the existing resource. For example, it's not possible to use managed identity against an existing Azure PostgreSQL resource that isn't configured to allow managed identity. Similarly, if an existing Azure Redis resource disabled access keys, it's not possible to use access key authentication.

### Configure existing Azure resources in all modes

The <xref:Aspire.Hosting.ExistingAzureResourceExtensions.AsExisting``1(Aspire.Hosting.ApplicationModel.IResourceBuilder{``0},Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.ApplicationModel.ParameterResource},Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.ApplicationModel.ParameterResource})> method is used when the distributed application is running in "run" or "publish" mode. Because the `AsExisting` method operates in both scenarios, it only supports a parameterized reference to the resource name or resource group name. This approach helps prevent the use of the same resource in both testing and production environments.

To mark an Azure resource as existing, call the `AsExisting` method on the resource builder. Consider the following example:

```csharp
var builder = DistributedApplication.CreateBuilder();

var existingServiceBusName = builder.AddParameter("existingServiceBusName");
var existingServiceBusResourceGroup = builder.AddParameter("existingServiceBusResourceGroup");

var serviceBus = builder.AddAzureServiceBus("messaging")
                        .AsExisting(existingServiceBusName, existingServiceBusResourceGroup);

serviceBus.AddServiceBusQueue("queue");
```

The preceding code:

- Creates a new `builder` instance.
- Adds a parameter named `existingServiceBusName` to the builder.
- Adds an Azure Service Bus resource named `messaging` to the builder.
- Calls the `AsExisting` method on the `serviceBus` resource builder, passing the `existingServiceBusName` parameter.
- Adds a queue named `queue` to the `serviceBus` resource.

## Add existing Azure resources with connection strings

.NET Aspire provides the ability to [connect to existing resources](../fundamentals/app-host-overview.md#reference-existing-resources), including Azure resources. Expressing connection strings is useful when you have existing Azure resources that you want to use in your .NET Aspire app. The <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> API is used with the AppHost's [execution context](../fundamentals/app-host-overview.md#execution-context) to conditionally add a connection string to the app model.

[!INCLUDE [connection-strings-alert](../includes/connection-strings-alert.md)]

Consider the following example, where in _publish_ mode you add an Azure Storage resource while in _run_ mode you add a connection string to an existing Azure Storage:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureStorage("storage")
    : builder.AddConnectionString("storage");

builder.AddProject<Projects.Api>("api")
       .WithReference(storage);

// After adding all resources, run the app...
```

The preceding code:

- Creates a new `builder` instance.
- Adds an Azure Storage resource named `storage` in "publish" mode.
- Adds a connection string to an existing Azure Storage named `storage` in "run" mode.
- Adds a project named `api` to the builder.
- The `api` project references the `storage` resource regardless of the mode.

The consuming API project uses the connection string information with no knowledge of how the AppHost configured it. In "publish" mode, the code adds a new Azure Storage resource—which would be reflected in the [deployment manifest](../deployment/manifest-format.md) accordingly. When in "run" mode the connection string corresponds to a configuration value visible to the AppHost. It's assumed that all role assignments for the target resource are configured. This means, you'd likely configure an environment variable or a user secret to store the connection string. The configuration is resolved from the `ConnectionStrings__storage` (or `ConnectionStrings:storage`) configuration key. These configuration values can be viewed when the app runs. For more information, see [Resource details](../fundamentals/dashboard/explore.md#resource-details).

Unlike existing resources modeled with [the first-class `AsExisting` API](#use-existing-azure-resources), existing resource modeled as connection strings can't be enhanced with additional role assignments or infrastructure customizations.

## Publish as Azure Container App

.NET Aspire allows you to publish primitive resources as [Azure Container Apps](/azure/container-apps/overview), a serverless platform that reduces infrastructure management. Supported resource types include:

- <xref:Aspire.Hosting.ApplicationModel.ContainerResource>: Represents a specified container.
- <xref:Aspire.Hosting.ApplicationModel.ExecutableResource>: Represents a specified executable process.
- <xref:Aspire.Hosting.ApplicationModel.ProjectResource>: Represents a specified .NET project.

To publish these resources, use the following APIs:

- <xref:Aspire.Hosting.AzureContainerAppContainerExtensions.PublishAsAzureContainerApp``1(Aspire.Hosting.ApplicationModel.IResourceBuilder{``0},System.Action{Aspire.Hosting.Azure.AzureResourceInfrastructure,Azure.Provisioning.AppContainers.ContainerApp})?displayProperty=nameWithType>
- <xref:Aspire.Hosting.AzureContainerAppExecutableExtensions.PublishAsAzureContainerApp``1(Aspire.Hosting.ApplicationModel.IResourceBuilder{``0},System.Action{Aspire.Hosting.Azure.AzureResourceInfrastructure,Azure.Provisioning.AppContainers.ContainerApp})?displayProperty=nameWithType>
- <xref:Aspire.Hosting.AzureContainerAppProjectExtensions.PublishAsAzureContainerApp``1(Aspire.Hosting.ApplicationModel.IResourceBuilder{``0},System.Action{Aspire.Hosting.Azure.AzureResourceInfrastructure,Azure.Provisioning.AppContainers.ContainerApp})?displayProperty=nameWithType>

These APIs configure the resource to be published as an Azure Container App and implicitly call <xref:Aspire.Hosting.AzureContainerAppExtensions.AddAzureContainerAppsInfrastructure(Aspire.Hosting.IDistributedApplicationBuilder)> to add the necessary infrastructure and Bicep files to your AppHost. As an example, consider the following code:

```csharp
var builder = DistributedApplication.CreateBuilder();

var env = builder.AddParameter("env");

var api = builder.AddProject<Projects.AspireApi>("api")
                 .PublishAsAzureContainerApp<Projects.AspireApi>((infra, app) =>
                 {
                     app.Template.Containers[0].Value!.Env.Add(new ContainerAppEnvironmentVariable
                     {
                         Name = "Hello",
                         Value = env.AsProvisioningParameter(infra)
                     });
                 });
```

The preceding code:

- Creates a new `builder` instance.
- Adds a parameter named `env` to the builder.
- Adds a project named `api` to the builder.
- Calls the `PublishAsAzureContainerApp` method on the `api` resource builder, passing a lambda expression that configures the Azure Container App infrastructure—where `infra` is the <xref:Aspire.Hosting.Azure.AzureResourceInfrastructure> and `app` is the <xref:Azure.Provisioning.AppContainers.ContainerApp>.
  - Adds an environment variable named `Hello` to the container app, using the `env` parameter.
  - The `AsProvisioningParameter` method is used to treat `env` as either a new <xref:Azure.Provisioning.ProvisioningParameter> in infrastructure, or reuses an existing bicep parameter if one with the same name already exists.

To configure the Azure Container App environment, see [Configure Azure Container Apps environments](configure-aca-environments.md). For more information, see <xref:Azure.Provisioning.AppContainers.ContainerApp> and <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.AsProvisioningParameter*>.

> [!TIP]
> If you're working with Azure Container Apps, you might also be interested in the [.NET Aspire Azure Container Registry integration](container-registry-integration.md).

## Publishing

When you publish your app, the Azure provisioning generated Bicep is used by the Azure Developer CLI to create the Azure resources in your Azure subscription. .NET Aspire outputs a [publishing manifest](../deployment/manifest-format.md), that's also a vital part of the publishing process. The Azure Developer CLI is a command-line tool that provides a set of commands to manage Azure resources.

For more information on publishing and deployment, see [Deploy a .NET Aspire project to Azure Container Apps using the Azure Developer CLI (in-depth guide)](../deployment/azure/aca-deployment-azd-in-depth.md).
