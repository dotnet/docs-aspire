---
title: Azure integrations overview
description: Overview of the Azure integrations available in the .NET Aspire.
ms.date: 02/20/2025
uid: dotnet/aspire/integrations/azure-overview
---

# .NET Aspire Azure integrations overview

[Azure](/azure) is the most popular cloud platform for building and deploying [.NET applications](/dotnet/azure). The [Azure SDK for .NET](/dotnet/azure/sdk/azure-sdk-for-dotnet) allows for easy management and use of Azure services. .NET Aspire provides a set of integrations with Azure services, where you're free to add new resources or connect to existing ones. This article details some common aspects of all Azure integrations in .NET Aspire and aims to help you understand how to use them.

## Naming conventions and APIs for expressing Azure resources

The distributed application builder, part of the [app host](../fundamentals/app-host-overview.md), employs the builder pattern, allowing consumers to `AddAzure*` resources to the [_app model_](../fundamentals/app-host-overview.md#terminology). In addition to simply adding resources, developers can also express configuration and how said resource behave in various execution contexts. Azure hosting integrations provide APIs to specify how these resources should be "published" and "run".

### App host run modes

When the app host executes, the [_execution context_](../fundamentals/app-host-overview.md#execution-context) determines whether the app host is in <xref:Aspire.Hosting.DistributedApplicationOperation.Run> or <xref:Aspire.Hosting.DistributedApplicationOperation.Publish> mode. The naming conventions for these APIs indicate the intended action for the resource. For more information on execution modes, see [Execution context](../fundamentals/app-host-overview.md#execution-context).

The following table summarizes the naming conventions used to express Azure resources:

| API | Description |
|--|--|
| `PublishAsConnectionString` | Changes the resource to be published as a connection string reference in the manifest. |
| `PublishAsExisting` | Uses an existing Azure resource when the application is deployed instead of creating a new one. |
| `RunAsContainer` | Configures an equivalent container to run locally. For more information, see [Local containers](#local-containers). |
| `RunAsEmulator` | Configures the Azure resource to be emulated. For more information, see [Local emulators](#local-emulators). |
| `RunAsExisting` | Marks the resource as an existing resource when the application is running. |

> [!NOTE]
> Not all APIs are available on all Azure resources. For example, some Azure resources can be containerized or emulated, while others cannot.

When it comes to specifying whether an Azure resource in the app model is an existing resource, you can use the `AsExisting` API. The `AsExisting` method marks the resource as an existing resource in both run and publish modes. You can query whether a resource has been marked as an existing resource, by calling the `IsExisting` extension method on the <xref:Aspire.Hosting.ApplicationModel.IResource>. For more information, see [Mark Azure resources as existing](#mark-azure-resources-as-existing).

## Add existing Azure resources with connection strings

.NET Aspire provides the ability to [connect to existing resources](../fundamentals/app-host-overview.md#reference-existing-resources), including Azure resources. Expressing connection strings is useful when you have existing Azure resources that you want to use in your .NET Aspire app. The <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> API is used with the app host's [execution context](../fundamentals/app-host-overview.md#execution-context) to conditionally add a connection string to the app model.

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
- Adds a Azure Storage resource named `storage` in "publish" mode.
- Adds a connection string to an existing Azure Storage named `storage` in "run" mode.
- Adds a project named `api` to the builder.
- The `api` project references the `storage` resource regardless of the mode.

The consuming API project uses the connection string information with no knowledge of how the app host configured it. In "publish" mode, the code adds a new Azure Storage resource—which would be reflected in the [deployment manifest](../deployment/manifest-format.md) accordingly. When in "run" mode, the connection string corresponds to a configuration value visible to the app host. It's assumed that any and all role assignments for the target resource have been configured. This means, you'd likely configure an environment variable or a user secret to store the connection string. The configuration is resolved from the `ConnectionStrings__storage` (or `ConnectionStrings:storage`) configuration key. These configuration values can be viewed when the app runs. For more information, see [Resource details](../fundamentals/dashboard/explore.md#resource-details).

## Mark Azure resources as existing

In addition to the pre-existing <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> API, .NET Aspire provides expanded support for referencing existing Azure resources. This is achieved through the `PublishAsExisting`, `RunAsExisting`, and `AsExisting` APIs. These APIs allow developers to reference already-deployed Azure resources, configure them, and generate appropriate deployment manifests using Bicep templates.

### Configure existing Azure resources for run mode

The `RunAsExisting` method is used when a distributed application is executing in "run" mode. In this mode, it assumes that the referenced Azure resource already exists and integrates with it during execution without provisioning the resource. To mark an Azure resource as existing, call the `RunAsExisting` method on the resource builder. Consider the following example:

```csharp
var builder = DistributedApplication.CreateBuilder();

var existingResourceName = builder.AddParameter("existingResourceName");

var serviceBus = builder.AddAzureServiceBus("messaging")
    .RunAsExisting(existingResourceName)
    .WithQueue("queue");
```

The preceding code:

- Creates a new `builder` instance.
- Adds a parameter named `existingResourceName` to the builder.
- Adds an Azure Service Bus resource named `messaging` to the builder.
- Calls the `RunAsExisting` method on the `serviceBus` resource builder, passing the `existingResourceName` parameter—alternatively, you can use the `string` parameter overload.
- Adds a queue named `queue` to the `serviceBus` resource.

### Configure existing Azure resources for publish mode

The `PublishAsExisting` method is used in "publish" mode when the intent is to declare and reference an already-existing Azure resource during publish mode. This API facilitates the creation of manifests and templates that include resource definitions that map to existing resources in Bicep.

To mark an Azure resource as existing in for the "publish" mode, call the `PublishAsExisting` method on the resource builder. Consider the following example:

```csharp
var builder = DistributedApplication.CreateBuilder();

var existingResourceName = builder.AddParameter("existingResourceName");

var serviceBus = builder.AddAzureServiceBus("messaging")
    .PublishAsExisting(existingResourceName)
    .WithQueue("queue");
```

The preceding code:

- Creates a new `builder` instance.
- Adds a parameter named `existingResourceName` to the builder.
- Adds an Azure Service Bus resource named `messaging` to the builder.
- Calls the `PublishAsExisting` method on the `serviceBus` resource builder, passing the `existingResourceName` parameter—alternatively, you can use the `string` parameter overload.
- Adds a queue named `queue` to the `serviceBus` resource.

After the app host is executed in publish mode, the generated manifest file will include the `existingResourceName` parameter, which can be used to reference the existing Azure resource. Consider the following generated manifest file:

```json
{
  "type": "azure.bicep.v0",
  "connectionString": "{messaging.outputs.serviceBusEndpoint}",
  "path": "messaging.module.bicep",
  "params": {
    "existingResourceName": "{existingResourceName.value}",
    "principalType": "",
    "principalId": ""
  }
}
```

For more information on the manifest file, see [.NET Aspire manifest format for deployment tool builders](../deployment/manifest-format.md).

Additionally, the generated Bicep template will include the `existingResourceName` parameter, which can be used to reference the existing Azure resource. Consider the following generated Bicep template:

```bicep
@description('The location for the resource(s) to be deployed.')
param location string = resourceGroup().location

param existingResourceName string
param principalType string
param principalId string

resource messaging 'Microsoft.ServiceBus/namespaces@2024-01-01' existing = {
  name: existingResourceName
}

output serviceBusEndpoint string = messaging.properties.serviceBusEndpoint
```

For more information on the generated Bicep template, see [Infrastructure as code](#infrastructure-as-code).

> [!WARNING]
> When interacting with existing resources that require authentication, ensure the authentication strategy that you're configuring in the .NET Aspire application model aligns with the authentication strategy allowed by the existing resource. For example, it's not possible to use managed identity against an existing Azure PostgreSQL resource that isn't configured to allow managed identity. Similarly, if an existing Azure Redis resource disabled access keys, it's not possible to use access key authentication.

### Configure existing Azure resources

The `AsExisting` method is used when the distributed application is running in "run" or "publish" mode. Because the `AsExisting` method can operate in both scenarios, it can only support a parameterized reference to the resource name or resource group name.

To mark an Azure resource as existing, call the `AsExisting` method on the resource builder. Consider the following example:

```csharp
var builder = DistributedApplication.CreateBuilder();

var existingResourceName = builder.AddParameter("existingResourceName");

var serviceBus = builder.AddAzureServiceBus("messaging")
    .AsExisting(existingResourceName)
    .WithQueue("queue");
```

The preceding code:

- Creates a new `builder` instance.
- Adds a parameter named `existingResourceName` to the builder.
- Adds an Azure Service Bus resource named `messaging` to the builder.
- Calls the `AsExisting` method on the `serviceBus` resource builder, passing the `existingResourceName` parameter.
- Adds a queue named `queue` to the `serviceBus` resource.

## Add Azure resources

All .NET Aspire Azure hosting integrations expose Azure resources and by convention are added using `AddAzure*` APIs. When you add these resources to your .NET Aspire app host, they represent an Azure service. The `AddAzure*` API returns an <xref:Aspire.Hosting.ApplicationModel.IResourceBuilder`1> where `T` is the type of Azure resource. These `IResourceBuilder<T>` (builder) interfaces provide a fluent API that allows you to configure the underlying Azure resource within the [app model](xref:dotnet/aspire/app-host#terminology).

### Typical developer experience

When your .NET Aspire app host contains Azure resources, and you run it locally (typical developer <kbd>F5</kbd> or `dotnet run` experience), the [Azure resources are provisioned](local-provisioning.md) in your Azure subscription. This allows you as the developer to debug against them locally in the context of your app host.

.NET Aspire aims to minimize costs by defaulting to _Basic_ or _Standard_ [Stock Keeping Unit (SKU)](/partner-center/developer/product-resources#sku) for its Azure integrations. While these sensible defaults are provided, you can [customize the Azure resources](#azureprovisioning-customization) to suit your needs. Additionally, some integrations support [emulators](#local-emulators) or [containers](#local-containers), which are useful for local development, testing, and debugging. By default, when you run your app locally, the Azure resources use the actual Azure service. However, you can configure them to use local emulators or containers, avoiding costs associated with the actual Azure service during local development.

### Local emulators

Some Azure services can be run locally in emulators. Currently, .NET Aspire supports the following Azure emulators:

| Hosting integration | Description |
|--|--|
| Azure Cosmos DB | Call <xref:Aspire.Hosting.AzureCosmosExtensions.RunAsEmulator*?displayProperty=nameWithType> on the `IResourceBuilder<AzureCosmosDBResource>` to configure the Cosmos DB resource to be [emulated with the NoSQL API](/azure/cosmos-db/how-to-develop-emulator). |
| Azure Event Hubs | Call <xref:Aspire.Hosting.AzureEventHubsExtensions.RunAsEmulator*?displayProperty=nameWithType> on the `IResourceBuilder<AzureEventHubsResource>` to configure the Event Hubs resource to be [emulated](/azure/event-hubs/overview-emulator). |
| Azure Service Bus | Call <xref:Aspire.Hosting.AzureServiceBusExtensions.RunAsEmulator*?displayProperty=nameWithType> on the `IResourceBuilder<AzureServiceBusResource>` to configure the Service Bus resource to be [emulated with Service Bus emulator](/azure/service-bus-messaging/overview-emulator). |
| Azure SignalR Service | Call <xref:Aspire.Hosting.AzureSignalRExtensions.RunAsEmulator*?displayProperty=nameWithType> on the `IResourceBuilder<AzureSignalRResource>` to configure the SignalR resource to be [emulated with Azure SignalR emulator](/azure/azure-signalr/signalr-howto-emulator). |
| Azure Storage | Call <xref:Aspire.Hosting.AzureStorageExtensions.RunAsEmulator*?displayProperty=nameWithType> on the `IResourceBuilder<AzureStorageResource>` to configure the Storage resource to be [emulated with Azurite](/azure/storage/common/storage-use-azurite). |

To have your Azure resources use the local emulators, chain a call the `RunAsEmulator` method on the Azure resource builder. This method configures the Azure resource to use the local emulator instead of the actual Azure service.

> [!IMPORTANT]
> Calling any of the available `RunAsEmulator` APIs on an Azure resource builder doesn't impact the [publishing manifest](../deployment/manifest-format.md). When you publish your app, [generated Bicep file](#infrastructure-as-code) reflects the actual Azure service, not the local emulator.

### Local containers

Some Azure services can be run locally in containers. To run an Azure service locally in a container, chain a call to the `RunAsContainer` method on the Azure resource builder. This method configures the Azure resource to run locally in a container instead of the actual Azure service.

Currently, .NET Aspire supports the following Azure services as containers:

| Hosting integration | Details |
|--|--|
| Azure Cache for Redis | Call <xref:Aspire.Hosting.AzureRedisExtensions.RunAsContainer*?displayProperty=nameWithType> on the `IResourceBuilder<AzureRedisCacheResource>` to configure it to run locally in a container, based on the `docker.io/library/redis` image. |
| Azure PostgreSQL Flexible Server | Call <xref:Aspire.Hosting.AzurePostgresExtensions.RunAsContainer*?displayProperty=nameWithType> on the `IResourceBuilder<AzurePostgresFlexibleServerResource>` to configure it to run locally in a container, based on the `docker.io/library/postgres` image. |
| Azure SQL Server | Call <xref:Aspire.Hosting.AzureSqlExtensions.RunAsContainer*?displayProperty=nameWithType> on the `IResourceBuilder<AzureSqlServerResource>` to configure it to run locally in a container, based on the `mcr.microsoft.com/mssql/server` image. |

> [!NOTE]
> Like emulators, calling `RunAsContainer` on an Azure resource builder doesn't impact the [publishing manifest](../deployment/manifest-format.md). When you publish your app, the [generated Bicep file](#infrastructure-as-code) reflects the actual Azure service, not the local container.

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

For more information on the difference between run and publish modes, see [.NET Aspire app host: Execution context](xref:dotnet/aspire/app-host#execution-context).

## Infrastructure as code

The Azure SDK for .NET provides the [📦 Azure.Provisioning](https://www.nuget.org/packages/Azure.Provisioning) NuGet package and a suite of service-specific [Azure provisioning packages](https://www.nuget.org/packages?q=owner%3A+azure-sdk+description%3A+declarative+resource+provisioning&sortby=relevance). These Azure provisioning libraries make it easy to declaratively specify Azure infrastructure natively in .NET. Their APIs enable you to write object-oriented infrastructure in C#, resulting in Bicep. [Bicep is a domain-specific language (DSL)](/azure/azure-resource-manager/bicep/overview) for deploying Azure resources declaratively.

<!-- TODO: Add link from here to the Azure docs when they're written. -->

While it's possible to provision Azure resources manually, .NET Aspire simplifies the process by providing a set of APIs to express Azure resources. These APIs are available as extension methods in .NET Aspire Azure hosting libraries, extending the <xref:Aspire.Hosting.IDistributedApplicationBuilder> interface. When you add Azure resources to your app host, they add the appropriate provisioning functionality implicitly. In other words, you don't need to call any provisioning APIs directly.

Since .NET Aspire models Azure resources within Azure hosting integrations, the Azure SDK is used to provision these resources. Bicep files are generated that define the Azure resources you need. The generated Bicep files are output alongside the manifest file when you publish your app.

There are several ways to influence the generated Bicep files:

- [Azure.Provisioning customization](#azureprovisioning-customization):
  - [Configure infrastructure](#configure-infrastructure): Customize Azure resource infrastructure.
  - [Add Azure infrastructure](#add-azure-infrastructure): Manually add Azure infrastructure to your app host.
- [Use custom Bicep templates](#use-custom-bicep-templates):
  - [Reference Bicep files](#reference-bicep-files): Add a reference to a Bicep file on disk.
  - [Reference Bicep inline](#reference-bicep-inline): Add an inline Bicep template.

### Local provisioning and `Azure.Provisioning`

To avoid conflating terms and to help disambiguate "provisioning," it's important to understand the distinction between _local provisioning_ and _Azure provisioning_:

- **_Local provisioning:_**

  By default, when you call any of the Azure hosting integration APIs to add Azure resources, the <xref:Aspire.Hosting.AzureProvisionerExtensions.AddAzureProvisioning(Aspire.Hosting.IDistributedApplicationBuilder)> API is called implicitly. This API registers services in the dependency injection (DI) container that are used to provision Azure resources when the app host starts. This concept is known as _local provisioning_.  For more information, see [Local Azure provisioning](local-provisioning.md).

- **_`Azure.Provisioning`:_**

  `Azure.Provisioning` refers to the NuGet package, and is a set of libraries that lets you use C# to generate Bicep. The Azure hosting integrations in .NET Aspire use these libraries under the covers to generate Bicep files that define the Azure resources you need. For more information, see [`Azure.Provisioning` customization](#azureprovisioning-customization).

### `Azure.Provisioning` customization

All .NET Aspire Azure hosting integrations expose various Azure resources, and they're all subclasses of the <xref:Aspire.Hosting.Azure.AzureProvisioningResource> type—which itself inherits the <xref:Aspire.Hosting.Azure.AzureBicepResource>. This enables extensions that are generically type-constrained to this type, allowing for a fluent API to customize the infrastructure to your liking. While .NET Aspire provides defaults, you're free to influence the generated Bicep using these APIs.

#### Configure infrastructure

Regardless of the Azure resource you're working with, to configure its underlying infrastructure, you chain a call to the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure*> extension method. This method allows you to customize the infrastructure of the Azure resource by passing a `configure` delegate of type `Action<AzureResourceInfrastructure>`. The <xref:Aspire.Hosting.Azure.AzureResourceInfrastructure> type is a subclass of the <xref:Azure.Provisioning.Infrastructure?displayProperty=fullName>. This type exposes a massive API surface area for configuring the underlying infrastructure of the Azure resource.

Consider the following example:

:::code language="csharp" source="../snippets/azure/AppHost/Program.ConfigureInfrastructure.cs" id="infra":::

The preceding code:

- Adds a parameter named `storage-sku`.
- Adds Azure Storage with the <xref:Aspire.Hosting.AzureStorageExtensions.AddAzureStorage*> API named `storage`.
- Chains a call to `ConfigureInfrastructure` to customize the Azure Storage infrastructure:
  - Gets the provisionable resources.
  - Filters to a single <xref:Azure.Provisioning.Storage.StorageAccount>.
  - Assigns the `storage-sku` parameter to the <xref:Azure.Provisioning.Storage.StorageAccount.Sku?displayProperty=nameWithType> property:
    - A new instance of the <xref:Azure.Provisioning.Storage.StorageSku> has its `Name` property assigned from the result of the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.AsProvisioningParameter*> API.

This exemplifies flowing an [external parameter](../fundamentals/external-parameters.md) into the Azure Storage infrastructure, resulting in the generated Bicep file reflecting the desired configuration.

#### Add Azure infrastructure

Not all Azure services are exposed as .NET Aspire integrations. While they might be at a later time, you can still provision services that are available in `Azure.Provisioning.*` libraries. Imagine a scenario where you have worker service that's responsible for managing an Azure Container Registry. Now imagine that an app host project takes a dependency on the [📦 Azure.Provisioning.ContainerRegistry](https://www.nuget.org/packages/Azure.Provisioning.ContainerRegistry) NuGet package.

You can use the `AddAzureInfrastructure` API to add the Azure Container Registry infrastructure to your app host:

:::code language="csharp" source="../snippets/azure/AppHost/Program.AddAzureInfra.cs" id="add":::

The preceding code:

- Calls <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.AddAzureInfrastructure*> with a name of `acr`.
- Provides a `configureInfrastructure` delegate to customize the Azure Container Registry infrastructure:
  - Instantiates a <xref:Azure.Provisioning.ContainerRegistry.ContainerRegistryService> with the name `acr` and a standard SKU.
  - Adds the Azure Container Registry service to the `infra` variable.
  - Instantiates a <xref:Azure.Provisioning.ProvisioningOutput> with the name `registryName`, a type of `string`, and a value that corresponds to the name of the Azure Container Registry.
  - Adds the output to the `infra` variable.
- Adds a project named `worker` to the builder.
- Chains a call to <xref:Aspire.Hosting.ResourceBuilderExtensions.WithEnvironment*> to set the `ACR_REGISTRY_NAME` environment variable in the project to the value of the `registryName` output.

The functionality demonstrates how to add Azure infrastructure to your app host project, even if the Azure service isn't directly exposed as a .NET Aspire integration. It further shows how to flow the output of the Azure Container Registry into the environment of a dependent project.

Consider the resulting Bicep file:

:::code language="bicep" source="../snippets/azure/AppHost/acr.module.bicep":::

The Bicep file reflects the desired configuration of the Azure Container Registry, as defined by the `AddAzureInfrastructure` API.

### Use custom Bicep templates

When you're targeting Azure as your desired cloud provider, you can use Bicep to define your infrastructure as code. It aims to drastically simplify the authoring experience with a cleaner syntax and better support for modularity and code reuse.

While .NET Aspire provides a set of prebuilt Bicep templates, there might be times when you either want to customize the templates or create your own. This section explains the concepts and corresponding APIs that you can use to customize the Bicep templates.

> [!IMPORTANT]
> This section isn't intended to teach you Bicep, but rather to provide guidance on how to create custom Bicep templates for use with .NET Aspire.

As part of the [Azure deployment story for .NET Aspire](../deployment/overview.md), the Azure Developer CLI (`azd`) provides an understanding of your .NET Aspire project and the ability to deploy it to Azure. The `azd` CLI uses the Bicep templates to deploy the application to Azure.

#### Install `Aspire.Hosting.Azure` package

When you want to reference Bicep files, it's possible that you're not using any of the Azure hosting integrations. In this case, you can still reference Bicep files by installing the `Aspire.Hosting.Azure` package. This package provides the necessary APIs to reference Bicep files and customize the Azure resources.

> [!TIP]
> If you're using any of the Azure hosting integrations, you don't need to install the `Aspire.Hosting.Azure` package, as it's a transitive dependency.

To use any of this functionality, the [📦 Aspire.Hosting.Azure](https://www.nuget.org/packages/Aspire.Hosting.Azure) NuGet package must be installed:

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

#### What to expect from the examples

All the examples in this section assume that you're using the <xref:Aspire.Hosting.Azure> namespace. Additionally, the examples assume you have an <xref:Aspire.Hosting.IDistributedApplicationBuilder> instance:

```csharp
using Aspire.Hosting.Azure;

var builder = DistributedApplication.CreateBuilder(args);

// Examples go here...

builder.Build().Run();
```

By default, when you call any of the Bicep-related APIs, a call is also made to <xref:Aspire.Hosting.AzureProvisionerExtensions.AddAzureProvisioning%2A> that adds support for generating Azure resources dynamically during application startup. For more information, see [Local provisioning and `Azure.Provisioning`](#local-provisioning-and-azureprovisioning).

#### Reference Bicep files

Imagine that you have a Bicep template in a file named `storage.bicep` that provisions an Azure Storage Account:

:::code language="bicep" source="snippets/AppHost.Bicep/storage.bicep":::

To add a reference to the Bicep file on disk, call the <xref:Aspire.Hosting.AzureBicepResourceExtensions.AddBicepTemplate%2A> method. Consider the following example:

:::code language="csharp" source="snippets/AppHost.Bicep/Program.ReferenceBicep.cs" id="addfile":::

The preceding code adds a reference to a Bicep file located at `../infra/storage.bicep`. The file paths should be relative to the _app host_ project. This reference results in an <xref:Aspire.Hosting.Azure.AzureBicepResource> being added to the application's resources collection with the `"storage"` name, and the API returns an `IResourceBuilder<AzureBicepResource>` instance that can be used to further customize the resource.

#### Reference Bicep inline

While having a Bicep file on disk is the most common scenario, you can also add Bicep templates inline. Inline templates can be useful when you want to define a template in code or when you want to generate the template dynamically. To add an inline Bicep template, call the <xref:Aspire.Hosting.AzureBicepResourceExtensions.AddBicepTemplateString%2A> method with the Bicep template as a `string`. Consider the following example:

:::code language="csharp" source="snippets/AppHost.Bicep/Program.InlineBicep.cs" id="addinline":::

In this example, the Bicep template is defined as an inline `string` and added to the application's resources collection with the name `"ai"`. This example provisions an Azure AI resource.

#### Pass parameters to Bicep templates

[Bicep supports accepting parameters](/azure/azure-resource-manager/bicep/parameters), which can be used to customize the behavior of the template. To pass parameters to a Bicep template from .NET Aspire, chain calls to the <xref:Aspire.Hosting.AzureBicepResourceExtensions.WithParameter%2A> method as shown in the following example:

:::code language="csharp" source="snippets/AppHost.Bicep/Program.PassParameter.cs" id="addparameter":::

The preceding code:

- Adds a parameter named `"region"` to the `builder` instance.
- Adds a reference to a Bicep file located at `../infra/storage.bicep`.
- Passes the `"region"` parameter to the Bicep template, which is resolved using the standard parameter resolution.
- Passes the `"storageName"` parameter to the Bicep template with a _hardcoded_ value.
- Passes the `"tags"` parameter to the Bicep template with an array of strings.

For more information, see [External parameters](../fundamentals/external-parameters.md).

##### Well-known parameters

.NET Aspire provides a set of well-known parameters that can be passed to Bicep templates. These parameters are used to provide information about the application and the environment to the Bicep templates. The following well-known parameters are available:

| Field | Description | Value |
|--|--|--|
| <xref:Aspire.Hosting.Azure.AzureBicepResource.KnownParameters.KeyVaultName?displayProperty=nameWithType> | The name of the key vault resource used to store secret outputs. | `"keyVaultName"` |
| <xref:Aspire.Hosting.Azure.AzureBicepResource.KnownParameters.Location?displayProperty=nameWithType> | The location of the resource. This is required for all resources. | `"location"` |
| <xref:Aspire.Hosting.Azure.AzureBicepResource.KnownParameters.LogAnalyticsWorkspaceId?displayProperty=nameWithType> | The resource ID of the log analytics workspace. | `"logAnalyticsWorkspaceId"` |
| <xref:Aspire.Hosting.Azure.AzureBicepResource.KnownParameters.PrincipalId?displayProperty=nameWithType> | The principal ID of the current user or managed identity. | `"principalId"` |
| <xref:Aspire.Hosting.Azure.AzureBicepResource.KnownParameters.PrincipalName?displayProperty=nameWithType> | The principal name of the current user or managed identity. | `"principalName"` |
| <xref:Aspire.Hosting.Azure.AzureBicepResource.KnownParameters.PrincipalType?displayProperty=nameWithType> | The principal type of the current user or managed identity. Either `User` or `ServicePrincipal`. | `"principalType"` |

To use a well-known parameter, pass the parameter name to the <xref:Aspire.Hosting.AzureBicepResourceExtensions.WithParameter%2A> method, such as `WithParameter(AzureBicepResource.KnownParameters.KeyVaultName)`. You don't pass values for well-known parameters, as .NET Aspire resolves them on your behalf.

Consider an example where you want to set up an Azure Event Grid webhook. You might define the Bicep template as follows:

 :::code language="bicep" source="snippets/AppHost.Bicep/event-grid-webhook.bicep" highlight="3-4,27-35":::

This Bicep template defines several parameters, including the `topicName`, `webHookEndpoint`, `principalId`, `principalType`, and the optional `location`. To pass these parameters to the Bicep template, you can use the following code snippet:

:::code language="csharp" source="snippets/AppHost.Bicep/Program.PassParameter.cs" id="addwellknownparams":::

- The `webHookApi` project is added as a reference to the `builder`.
- The `topicName` parameter is passed a hardcoded name value.
- The `webHookEndpoint` parameter is passed as an expression that resolves to the URL from the `api` project references' "https" endpoint with the `/hook` route.
- The `principalId` and `principalType` parameters are passed as well-known parameters.

The well-known parameters are convention-based and shouldn't be accompanied with a corresponding value when passed using the `WithParameter` API. Well-known parameters simplify some common functionality, such as _role assignments_, when added to the Bicep templates, as shown in the preceding example. Role assignments are required for the Event Grid webhook to send events to the specified endpoint. For more information, see [Event Grid Data Sender role assignment](/azure/role-based-access-control/built-in-roles/integration#eventgrid-data-sender).

### Get outputs from Bicep references

In addition to passing parameters to Bicep templates, you can also get outputs from the Bicep templates. Consider the following Bicep template, as it defines an `output` named `endpoint`:

:::code language="bicep" source="snippets/AppHost.Bicep/storage-out.bicep":::

The Bicep defines an output named `endpoint`. To get the output from the Bicep template, call the <xref:Aspire.Hosting.AzureBicepResourceExtensions.GetOutput%2A> method on an `IResourceBuilder<AzureBicepResource>` instance as demonstrated in following C# code snippet:

:::code language="csharp" source="snippets/AppHost.Bicep/Program.GetOutputReference.cs" id="getoutput":::

In this example, the output from the Bicep template is retrieved and stored in an `endpoint` variable. Typically, you would pass this output as an environment variable to another resource that relies on it. For instance, if you had an ASP.NET Core Minimal API project that depended on this endpoint, you could pass the output as an environment variable to the project using the following code snippet:

```csharp
var storage = builder.AddBicepTemplate(
                name: "storage",
                bicepFile: "../infra/storage.bicep"
            );

var endpoint = storage.GetOutput("endpoint");

var apiService = builder.AddProject<Projects.AspireSample_ApiService>(
        name: "apiservice"
    )
    .WithEnvironment("STORAGE_ENDPOINT", endpoint);
```

For more information, see [Bicep outputs](/azure/azure-resource-manager/bicep/outputs).

### Get secret outputs from Bicep references

It's important to [avoid outputs for secrets](/azure/azure-resource-manager/bicep/scenarios-secrets#avoid-outputs-for-secrets) when working with Bicep. If an output is considered a _secret_, meaning it shouldn't be exposed in logs or other places, you can treat it as such. This can be achieved by storing the secret in Azure Key Vault and referencing it in the Bicep template. .NET Aspire's Azure integration provides a pattern for securely storing outputs from the Bicep template by allows resources to use the `keyVaultName` parameter to store secrets in Azure Key Vault.

Consider the following Bicep template as an example the helps to demonstrate this concept of securing secret outputs:

:::code language="bicep" source="snippets/AppHost.Bicep/cosmosdb.bicep" highlight="2,41":::

The preceding Bicep template expects a `keyVaultName` parameter, among several other parameters. It then defines an Azure Cosmos DB resource and stashes a secret into Azure Key Vault, named `connectionString` which represents the fully qualified connection string to the Cosmos DB instance. To access this secret connection string value, you can use the following code snippet:

:::code language="csharp" source="snippets/AppHost.Bicep/Program.cs" id="secrets":::

In the preceding code snippet, the `cosmos` Bicep template is added as a reference to the `builder`. The `connectionString` secret output is retrieved from the Bicep template and stored in a variable. The secret output is then passed as an environment variable (`ConnectionStrings__cosmos`) to the `api` project. This environment variable is used to connect to the Cosmos DB instance.

When this resource is deployed, the underlying deployment mechanism will automatically [Reference secrets from Azure Key Vault](/azure/container-apps/manage-secrets?tabs=azure-portal#reference-secret-from-key-vault). To guarantee secret isolation, .NET Aspire creates a Key Vault per source.

> [!NOTE]
> In _local provisioning_ mode, the secret is extracted from Key Vault and set it in an environment variable. For more information, see [Local Azure provisioning](local-provisioning.md).

## Publishing

When you publish your app, the Azure provisioning generated Bicep is used by the Azure Developer CLI to create the Azure resources in your Azure subscription. .NET Aspire outputs a [publishing manifest](../deployment/manifest-format.md), that's also a vital part of the publishing process. The Azure Developer CLI is a command-line tool that provides a set of commands to manage Azure resources.

For more information on publishing and deployment, see [Deploy a .NET Aspire project to Azure Container Apps using the Azure Developer CLI (in-depth guide)](../deployment/azure/aca-deployment-azd-in-depth.md).
