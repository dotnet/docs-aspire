---
title: .NET Aspire Azure integrations overview
description: Overview of the Azure integrations available in the .NET Aspire.
ms.date: 12/12/2024
uid: integrations/azure-overview
---

<!--

Somebody reading the overview is supposed to understand:

- What happens when you add an azure hosing integration and run locally 
  - What about emulators?
  - What about redis? I see AddRedis and AddAzureRedis, why do we have 2? (same for SQL and Postgres)
- How do I customize azure resources?
- What are these azure resources doing under the covers (leave the specifics for each integration doc, not the overview)

Understanding of the AzureBicepResource and AzureProvisioning resource:
 
- Bicep is foundational to understanding how aspire integrates with Azure. You can write bicep in C# with the Azure.Provisioning libraries. Aspire azure integrations use those libraries under the covers.
- How do users pass Aspire parameter resource to AspireProvisioning resources and how do generally set parameters on resources.
- How do you get outputs from resources (GetOutput)
- How do are secrets handled in resources.
- What are the special parameters that the system provides for me.

Ties back to deployment:

- What happens when these resources are deployed?
- How does these show up in the manifest?
- What does azd do here?

-->

# .NET Aspire Azure integrations overview

[Azure](/azure) is the most popular cloud platform for building and deploying [.NET applications](/dotnet/azure). The [Azure SDK for .NET](/dotnet/azure/sdk/azure-sdk-for-dotnet) allows for easy management and use of Azure services. .NET Aspire provides a set of integrations with Azure services. This article details some common aspects of all Azure integrations in .NET Aspire and aims to help you understand how to use them.

## Add Azure resources

All .NET Aspire Azure hosting integrations expose Azure resources and by convention are added using `AddAzure*` APIs. When you add these resources to your .NET Aspire app host, they represent an Azure service. The `AddAzure*` API returns an <xref:Aspire.Hosting.ApplicationModel.IResourceBuilder`1> where `T` is the type of Azure resource. These `IResourceBuilder<T>` (builder) interfaces provide a fluent API that allows you to configure the underlying Azure resource within the [app model](xref:dotnet/aspire/app-host#terminology).

### Typical developer experience

When your .NET Aspire app host contains Azure resources, and you run it locally (typical developer <kbd>F5</kbd> experience), the Azure resources are provisioned in your Azure subscription. However, they're not yet deployed. Instead, they're running locally in the context of your app host.

.NET Aspire optimizes for "as free as possible" with its Azure integrations—defaulting to Basic and Standard SKUs in most cases. Additionally, some Azure integrations support [emulators](#local-emulators) or [containers](#local-containers), which enables compelling scenarios for local development, testing, and debugging. When you run your app locally, the Azure resources are configured to use the actual Azure service by default. However, you can configure them to use local emulators or containers instead. This means that you avoid any costs associated with running the actual Azure service for local development.

### Local emulators

Some Azure services can be run locally in emulators. Currently, .NET Aspire supports the following Azure emulators:

| Integration | Description |
|----------|-------------|
| Azure Cosmos DB | Call <xref:Aspire.Hosting.AzureCosmosExtensions.RunAsEmulator*?displayProperty=nameWithType> on the `IResourceBuilder<AzureCosmosDBResource>` to configure the Cosmos DB resource to be [emulated with the NoSQL API](/azure/cosmos-db/how-to-develop-emulator). |
| Azure Event Hubs | Call <xref:Aspire.Hosting.AzureEventHubsExtensions.RunAsEmulator*?displayProperty=nameWithType> on the `IResourceBuilder<AzureEventHubsResource>` to configure the Event Hubs resource to be [emulated](/azure/event-hubs/overview-emulator). |
| Azure Storage | Call <xref:Aspire.Hosting.AzureStorageExtensions.RunAsEmulator*?displayProperty=nameWithType> on the `IResourceBuilder<AzureStorageResource>` to configure the Storage resource to be [emulated with Azurite](/azure/storage/common/storage-use-azurite). |

To have your Azure resources use the local emulators, chain a call the `RunAsEmulator` method on the Azure resource builder. This method configures the Azure resource to use the local emulator instead of the actual Azure service.

> [!IMPORTANT]
> Calling `RunAsEmulator` on an Azure resource builder doesn't impact the publishing manifest. When you publish your app, the generated Bicep file will reflect the actual Azure service, not the local emulator.

### Local containers

Some Azure services can be run locally in containers. To run an Azure service locally in a container, chain a call to the `RunAsContainer` method on the Azure resource builder. This method configures the Azure resource to run locally in a container instead of the actual Azure service.

Currently, .NET Aspire supports the following Azure services as containers:

| Integration | Details |
|---------------|---------|
| Azure Cache for Redis | Call <xref:Aspire.Hosting.AzureRedisExtensions.RunAsContainer*?displayProperty=nameWithType> on the `IResourceBuilder<AzureRedisCacheResource>` to configure it to run locally in a container, based on the `docker.io/library/redis` image. |
| Azure PostgreSQL Flexible Server  | Call <xref:Aspire.Hosting.AzurePostgresExtensions.RunAsContainer*?displayProperty=nameWithType> on the `IResourceBuilder<AzurePostgresFlexibleServerResource>` to configure it to run locally in a container, based on the `docker.io/library/postgres` image. |
| Azure SQL Server | Call <xref:Aspire.Hosting.AzureSqlExtensions.RunAsContainer*?displayProperty=nameWithType> on the `IResourceBuilder<AzureSqlServerResource>` to configure it to run locally in a container, based on the `mcr.microsoft.com/mssql/server` image. |

> [!NOTE]
> Like emulators, calling `RunAsContainer` on an Azure resource builder doesn't impact the publishing manifest. When you publish your app, the generated Bicep file will reflect the actual Azure service, not the local container.

## Infrastructure as code

The Azure SDK for .NET provides the [📦 Azure.Provisioning](https://www.nuget.org/packages/Azure.Provisioning) NuGet package and a suite of service-specific [Azure provisioning packages](#azure-provisioning-packages). These Azure provisioning libraries make it easy to declaratively specify Azure infrastructure natively in .NET.

While it's possible to provision Azure resources manually, .NET Aspire simplifies the process by providing a set of APIs to express Azure resources. These APIs are available as extension methods in .NET Aspire Azure hosting libraries, extending the <xref:Aspire.Hosting.IDistributedApplicationBuilder> interface. When you add Azure resources to your app host, they add the appropriate provisioning functionality implicitly. In other words, you don't need to call any provisioning APIs directly.

Since .NET Aspire models Azure resources within Azure hosting integrations, the Azure SDK is used to provision these resources. The Azure SDK generates Bicep files that define the Azure resources you need. If you're unfamiliar with Bicep, it's a domain-specific language used to describe and provision Azure resources. The generated Bicep files are output alongside the manifest file when you publish your app. For more information, see [Azure provisioning Bicep](#azure-provisioning-bicep).

### Azure provisioning customization

All .NET Aspire Azure hosting integrations expose various Azure resources, and they're all subclasses of the <xref:Aspire.Hosting.Azure.AzureProvisioningResource> type—which itself inherits the <xref:Aspire.Hosting.Azure.AzureBicepResource>. This enables extensions that are generically type-constrained to this type, allowing for a fluent API to customize the infrastructure to your liking. While the Azure SDK relies on defaults, you're free to influence the generated Bicep using these APIs.

#### Configure infrastructure

Regardless of the Azure resource you're working with, to configure its underlying infrastructure, you chain a call to the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure*> extension method. This method allows you to customize the infrastructure of the Azure resource by passing a `configure` delegate of type `Action<AzureResourceInfrastructure>`. The <xref:Aspire.Hosting.Azure.AzureResourceInfrastructure> type is a subclass of the <xref:Azure.Provisioning.Infrastructure?displayProperty=fullName>. This type exposes a massive API surface area for configuring the underlying infrastructure of the Azure resource.

Each `configure` action:

- Calls the <xref:Azure.Provisioning.Infrastructure.GetProvisionableResources> API to get the provisionable resources.
- Filters the provisionable resources based on target resource type to customize.
- Applies the desired configuration to the resource.

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
  - Instantiates an <xref:Azure.Provisioning.ContainerRegistry.ContainerRegistryService> with the name `acr` and a standard SKU.
  - Adds the Azure Container Registry service to the `infra` variable.
  - Instantiates an <xref:Azure.Provisioning.ProvisioningOutput> with the name `registryName`, a type of `string`, and a value that corresponds to the name of the Azure Container Registry.
  - Adds the output to the `infra` variable.
- Adds a project named `worker` to the builder.
- Chains a call to <xref:Aspire.Hosting.ResourceBuilderExtensions.WithEnvironment*> to set the `ACR_REGISTRY_NAME` environment variable in the project to the value of the `registryName` output.

This example demonstrates how to add Azure infrastructure to your app host project, even if the Azure service isn't directly exposed as a .NET Aspire integration. It further shows how to flow the output of the Azure Container Registry into the environment of a project.

## Publishing

When you publish your app, the Azure provisioning generated Bicep is used by the Azure Developer CLI to create the Azure resources in your Azure subscription. The Azure Developer CLI is a command-line tool that provides a set of commands to manage Azure resources. For more information, see [Azure Developer CLI](/azure/developer/azure-developer-cli).
