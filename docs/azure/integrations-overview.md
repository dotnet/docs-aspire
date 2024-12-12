---
title: .NET Aspire Azure integrations overview
description: Overview of the Azure integrations available in the .NET Aspire.
ms.date: 12/11/2024
uid: azure-integrations-overview
---

# .NET Aspire Azure integrations overview

[Azure](/azure) is the most popular cloud platform for building and deploying [.NET applications](/dotnet/azure).  The Azure SDK for .NET allows for easy management and use of Azure services. .NET Aspire provides a set of integrations with Azure services. This article details some common aspects of all Azure integrations in .NET Aspire and aims to help you understand how to use them.

## Infrastructure as code

The **Azure SDK for .NET**, provides the [📦 Azure.Provisioning](https://www.nuget.org/packages/Azure.Provisioning) NuGet package and a suite of [Azure provisioning packages](#azure-provisioning-packages). These Azure provisioning libraries make it easy to declaratively specify Azure infrastructure natively in .NET.

While it's possible to provision Azure resources manually, .NET Aspire simplifies the process by providing a set of APIs to express Azure resources. These APIs are available as extension methods in .NET Aspire Azure hosting libraries, extending the <xref:Aspire.Hosting.IDistributedApplicationBuilder> interface. When you add Azure resources to your app host, they'll add the appropriate provisioning functionality implicitly. In other words, you don't need to call any provisioning APIs directly.

Since .NET Aspire models Azure resources within Azure hosting integrations, the Azure SDK is used to provision these resources. The Azure SDK generates Bicep files that define the Azure resources you need. If you're unfamiliar with Bicep, it's a domain-specific language used to describe and provision Azure resources. The generated Bicep files are output alongside the manifest file when you publish your app. For more information, see [Azure provisioning Bicep](#azure-provisioning-bicep).

### Azure provisioning defaults

The Azure provisioning libraries provide default values for Azure resources. These defaults are only a starting point and aren't intended for all use cases. You can customize these defaults in multiple ways to meet your specific requirements. For more information, see [Azure provisioning customization](#azure-provisioning-customization) and [Use custom Bicep templates](../deployment/azure/custom-bicep-templates.md).

#### Azure role assignment defaults

By default, the Azure provisioning libraries assign the following roles to the corresponding Azure resources as part of the app host provisioning process. These roles protect your resources and ensure that only authorized users can access them. When an Azure hosting integration provides role assignments, the client integration is automatically injected the appropriate connection details to consume these resources. The following table lists the default role assignments for each Azure resource:

| Integration | Role / ID | Description |
|-------------|-----------|-------------|
| Azure App Configuration | [App Configuration Data Owner](/azure/role-based-access-control/built-in-roles/integration#app-configuration-data-owner)<br/>`5ae67dd6-50cb-40e7-96ff-dc2bfa4b606b` | Read, write, and delete Azure App Configuration key-values. |
| Azure CosmosDB | N/A | No role assignment, instead it uses secrets in Azure Key Vault. |
| Azure Event Hubs | [Event Hubs Data Owner](/azure/role-based-access-control/built-in-roles/analytics#azure-event-hubs-data-owner)<br/>`f526a384-b230-433a-b45c-95f59c4a2dec` | Allows for full access to Azure Event Hubs resources. |
| Azure Application Insights | N/A | No role assignment and doesn't provide a client integration. |
| Azure Key Vault | [Key Vault Administrator](/azure/role-based-access-control/built-in-roles/security#key-vault-administrator)<br/>`00482a5a-887f-4fb3-b363-3b7fe8e74483` | Perform all data plane operations on a key vault and all objects in it, including certificates, keys, and secrets. |
| Azure Log Analytics | N/A | No role assignment and doesn't provide a client integration. |
| Azure OpenAI | [Cognitive Services OpenAI Contributor](/azure/role-based-access-control/built-in-roles/ai-machine-learning#cognitive-services-openai-contributor)<br/>`a001fd3d-188f-4b5d-821b-7da978bf7442` | Full access including the ability to fine-tune, deploy and generate text. |
| Azure Postgres Flexible Server | N/A | No role assignment, but creates an administrator based on the random (or user-provided) principal. |
| Azure Redis | N/A | No role assignment, but creates an access policy assignment based on the random (or user-provided) principal. |
| Azure Search | [Search Index Data Contributor](/azure/role-based-access-control/built-in-roles/ai-machine-learning#search-index-data-contributor)<br/>`8ebe5a00-799e-43f5-93ac-243d3dce84a7` | Grants full access to Azure Cognitive Search index data. |
| Azure Service Bus | [Service Bus Data Owner](/azure/role-based-access-control/built-in-roles/integration#azure-service-bus-data-owner)<br/>`090c5cfd-751d-490a-894a-3ce6f1109419` | Allows for full access to Azure Service Bus resources. |
| Azure SignalR | [SignalR App Server](/azure/role-based-access-control/built-in-roles/web-and-mobile#signalr-app-server)<br/>`420fcaa2-552c-430f-98ca-3264be4806c7` | Lets your app server access SignalR Service with Entra ID auth options. |
| Azure SQL | N/A | No role assignment, but adds the random (or user-provided) principal as an administrator. |
| Azure Storage | - [Storage Blob Data Contributor](/azure/role-based-access-control/built-in-roles/storage#storage-blob-data-contributor)<br/>`ba92f5b4-2d11-453d-a403-e96b0029c9fe`<br/> - [Storage Queue Data Contributor](/azure/role-based-access-control/built-in-roles/storage#storage-table-data-contributor)<br/>`0a9a7e1f-b9d0-4cc4-a60d-0319b160aaa3`<br/> - [Storage Table Data Contributor](/azure/role-based-access-control/built-in-roles/storage#storage-queue-data-contributor)<br/>`974c5e8b-45b9-4653-ba55-5f855dd0fb88`<br/> | - Read, write, and delete Azure Storage containers and blobs. <br/> - Read, write, and delete Azure Storage queues and queue messages. <br/> - Allows for read, write and delete access to Azure Storage tables and entities. |
| Azure Web PubSub | [Web PubSub Service Owner](/azure/role-based-access-control/built-in-roles/web-and-mobile#web-pubsub-service-owner)<br/>`12cf5a90-567b-43ae-8102-96cf46c7d9b4` | Full access to Azure Web PubSub Service REST APIs. |

### Azure provisioning Bicep

The Azure provisioning libraries generate Bicep files that define the Azure resources in your app host. The generated Bicep files are output alongside the manifest file when you publish your app. The generated Bicep is a starting point and can be customized to meet your specific requirements. For more information, see [Azure provisioning customization](#azure-provisioning-customization).

#### Azure App Configuration defaults

The Azure App Configuration integration provides the following default Bicep when the resource is named `config`:

<!-- markdownlint-disable MD033 -->
<br/>
<details>
<summary id='app-config'><strong>Toggle Azure App Configuration Bicep.</strong></summary>
<p aria-labelledby='app-config'>

:::code language="bicep" source="../snippets/azure/AppHost/config.module.bicep":::

</p>
</details>
<!-- markdownlint-enable MD033 -->

#### Azure Application Insights defaults

The Azure Application Insights integration provides the following default Bicep when the resource is named `app-insights`:

<!-- markdownlint-disable MD033 -->
<br/>
<details>
<summary id='app-insights'><strong>Toggle Azure Application Insights Bicep.</strong></summary>
<p aria-labelledby='app-insights'>

:::code language="bicep" source="../snippets/azure/AppHost/app-insights.module.bicep":::

</p>
</details>
<!-- markdownlint-enable MD033 -->

#### Azure CosmosDB defaults

The [Azure CosmosDB integration](../database/azure-cosmos-db-integration.md) provides the following default Bicep when the resource is named `cosmos`:

<!-- markdownlint-disable MD033 -->
<br/>
<details>
<summary id='cosmos'><strong>Toggle Azure CosmosDB Bicep.</strong></summary>
<p aria-labelledby='cosmos'>

:::code language="bicep" source="../snippets/azure/AppHost/cosmos.module.bicep":::

</p>
</details>
<!-- markdownlint-enable MD033 -->

#### Azure Event Hubs defaults

The [Azure Event Hubs integration](../messaging/azure-event-hubs-integration.md) provides the following default Bicep when the resource is named `event-hubs`:

<!-- markdownlint-disable MD033 -->
<br/>
<details>
<summary id='event-hubs'><strong>Toggle Azure Event Hubs Bicep.</strong></summary>
<p aria-labelledby='event-hubs'>

:::code language="bicep" source="../snippets/azure/AppHost/event-hubs.module.bicep":::

</p>
</details>
<!-- markdownlint-enable MD033 -->

#### Azure Key Vault defaults

The [Azure Key Vault integration](../security/azure-security-key-vault-integration.md) provides the following default Bicep when the resource is named `key-vault`:

<!-- markdownlint-disable MD033 -->
<br/>
<details>
<summary id='key-vault'><strong>Toggle Azure Key Vault Bicep.</strong></summary>
<p aria-labelledby='key-vault'>

:::code language="bicep" source="../snippets/azure/AppHost/key-vault.module.bicep":::

</p>
</details>
<!-- markdownlint-enable MD033 -->

#### Azure Log Analytics Workspace defaults

The Azure Log Analytics Workspace integration provides the following default Bicep when the resource is named `log-analytics-workspace`:

<!-- markdownlint-disable MD033 -->
<br/>
<details>
<summary id='log-analytics-workspace'><strong>Toggle Azure Log Analytics Workspace Bicep.</strong></summary>
<p aria-labelledby='log-analytics-workspace'>

:::code language="bicep" source="../snippets/azure/AppHost/log-analytics-workspace.module.bicep":::

</p>
</details>
<!-- markdownlint-enable MD033 -->

#### Azure OpenAI defaults

The [Azure OpenAI integration](../azureai/azureai-openai-integration.md) provides the following default Bicep when the resource is named `openai`:

<!-- markdownlint-disable MD033 -->
<br/>
<details>
<summary id='openai'><strong>Toggle Azure OpenAI Bicep.</strong></summary>
<p aria-labelledby='openai'>

:::code language="bicep" source="../snippets/azure/AppHost/openai.module.bicep":::

</p>
</details>
<!-- markdownlint-enable MD033 -->

#### Azure Postgres Flexible Server defaults

The Azure Postgres Flexible Server integration provides the following default Bicep when the resource is named `postgres-flexible`:

<!-- markdownlint-disable MD033 -->
<br/>
<details>
<summary id='postgres-flexible'><strong>Toggle Azure Postgres Flexible Server Bicep.</strong></summary>
<p aria-labelledby='postgres-flexible'>

:::code language="bicep" source="../snippets/azure/AppHost/postgres-flexible.module.bicep":::

</p>
</details>
<!-- markdownlint-enable MD033 -->

#### Azure Redis defaults

The Azure Redis integration provides the following default Bicep when the resource is named `redis`:

<!-- markdownlint-disable MD033 -->
<br/>
<details>
<summary id='redis'><strong>Toggle Azure Redis Bicep.</strong></summary>
<p aria-labelledby='redis'>

:::code language="bicep" source="../snippets/azure/AppHost/redis.module.bicep":::

</p>
</details>
<!-- markdownlint-enable MD033 -->

#### Azure Search defaults

The [Azure Search integration](../azureai/azureai-search-document-integration.md) provides the following default Bicep when the resource is named `search`:

<!-- markdownlint-disable MD033 -->
<br/>
<details>
<summary id='search'><strong>Toggle Azure Search Bicep.</strong></summary>
<p aria-labelledby='search'>

:::code language="bicep" source="../snippets/azure/AppHost/search.module.bicep":::

</p>
</details>
<!-- markdownlint-enable MD033 -->

#### Azure Service Bus defaults

The s[Azure Service Bus integration](../messaging/azure-service-bus-integration.md) provides the following default Bicep when the resource is named `service-bus`:

<!-- markdownlint-disable MD033 -->
<br/>
<details>
<summary id='service-bus'><strong>Toggle Azure Service Bus Bicep.</strong></summary>
<p aria-labelledby='service-bus'>

:::code language="bicep" source="../snippets/azure/AppHost/service-bus.module.bicep":::

</p>
</details>
<!-- markdownlint-enable MD033 -->

#### Azure SignalR defaults

The [Azure SignalR integration](../real-time/azure-signalr-scenario.md) provides the following default Bicep when the resource is named `signalr`:

<!-- markdownlint-disable MD033 -->
<br/>
<details>
<summary id='signalr'><strong>Toggle Azure SignalR Bicep.</strong></summary>
<p aria-labelledby='signalr'>

:::code language="bicep" source="../snippets/azure/AppHost/signalr.module.bicep":::

</p>
</details>
<!-- markdownlint-enable MD033-->

#### Azure Storage defaults

The Azure Storage integrations, [Blobs](../storage/azure-storage-blobs-integration.md), [Queues](../storage/azure-storage-queues-integration.md), and [Tables](../storage/azure-storage-tables-integration.md), all share the following default Bicep when the resource is named `storage`:

<!-- markdownlint-disable MD033 -->
<br/>
<details>
<summary id='storage'><strong>Toggle Azure Storage Bicep.</strong></summary>
<p aria-labelledby='storage'>

:::code language="bicep" source="../snippets/azure/AppHost/storage.module.bicep":::

</p>
</details>
<!-- markdownlint-enable MD033 -->

#### Azure Web PubSub defaults

The [Azure Web PubSub integration](../messaging/azure-web-pubsub-integration.md) provides the following default Bicep when the resource is named `web-pub-sub`:

<!-- markdownlint-disable MD033 -->
<br/>
<details>
<summary id='web-pub-sub'><strong>Toggle Azure Web PubSub Bicep.</strong></summary>
<p aria-labelledby='web-pub-sub'>

:::code language="bicep" source="../snippets/azure/AppHost/web-pub-sub.module.bicep":::

</p>
</details>
<!-- markdownlint-enable MD033 -->

### Azure provisioning customization

All .NET Aspire Azure hosting integrations expose various Azure resources, and they're all subclasses of the <xref:Aspire.Hosting.Azure.AzureProvisioningResource> type—which itself is a subclass of the <xref:Aspire.Hosting.Azure.AzureBicepResource>. This enables extensions that are generically type-constrained to this type, allowing for a fluent API to customize the infrastructure to your liking. While the Azure SDK relies on defaults, you're free to influence the generated Bicep using these APIs.

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

### Azure provisioning packages

The following Azure provisioning libraries are available:

- [📦 Azure.Provisioning.AppConfiguration](https://www.nuget.org/packages/Azure.Provisioning.AppConfiguration)
- [📦 Azure.Provisioning.AppContainers](https://www.nuget.org/packages/Azure.Provisioning.AppContainers)
- [📦 Azure.Provisioning.AppService](https://www.nuget.org/packages/Azure.Provisioning.AppService)
- [📦 Azure.Provisioning.ApplicationInsights](https://www.nuget.org/packages/Azure.Provisioning.ApplicationInsights)
- [📦 Azure.Provisioning.CognitiveServices](https://www.nuget.org/packages/Azure.Provisioning.CognitiveServices)
- [📦 Azure.Provisioning.Communication](https://www.nuget.org/packages/Azure.Provisioning.Communication)
- [📦 Azure.Provisioning.ContainerRegistry](https://www.nuget.org/packages/Azure.Provisioning.ContainerRegistry)
- [📦 Azure.Provisioning.ContainerService](https://www.nuget.org/packages/Azure.Provisioning.ContainerService)
- [📦 Azure.Provisioning.CosmosDB](https://www.nuget.org/packages/Azure.Provisioning.CosmosDB)
- [📦 Azure.Provisioning.Deployment](https://www.nuget.org/packages/Azure.Provisioning.Deployment)
- [📦 Azure.Provisioning.EventGrid](https://www.nuget.org/packages/Azure.Provisioning.EventGrid)
- [📦 Azure.Provisioning.EventHubs](https://www.nuget.org/packages/Azure.Provisioning.EventHubs)
- [📦 Azure.Provisioning.KeyVault](https://www.nuget.org/packages/Azure.Provisioning.KeyVault)
- [📦 Azure.Provisioning.Kubernetes](https://www.nuget.org/packages/Azure.Provisioning.Kubernetes)
- [📦 Azure.Provisioning.KubernetesConfiguration](https://www.nuget.org/packages/Azure.Provisioning.KubernetesConfiguration)
- [📦 Azure.Provisioning.OperationalInsights](https://www.nuget.org/packages/Azure.Provisioning.OperationalInsights)
- [📦 Azure.Provisioning.PostgreSql](https://www.nuget.org/packages/Azure.Provisioning.PostgreSql)
- [📦 Azure.Provisioning.Redis](https://www.nuget.org/packages/Azure.Provisioning.Redis)
- [📦 Azure.Provisioning.Search](https://www.nuget.org/packages/Azure.Provisioning.Search)
- [📦 Azure.Provisioning.ServiceBus](https://www.nuget.org/packages/Azure.Provisioning.ServiceBus)
- [📦 Azure.Provisioning.SignalR](https://www.nuget.org/packages/Azure.Provisioning.SignalR)
- [📦 Azure.Provisioning.Sql](https://www.nuget.org/packages/Azure.Provisioning.Sql)
- [📦 Azure.Provisioning.Storage](https://www.nuget.org/packages/Azure.Provisioning.Storage)
- [📦 Azure.Provisioning.WebPubSub](https://www.nuget.org/packages/Azure.Provisioning.WebPubSub)
- [📦 Azure.Provisioning](https://www.nuget.org/packages/Azure.Provisioning)

> [!TIP]
> You don't need to install these packages manually in your app host projects, as they're transitive dependencies of the corresponding .NET Aspire Azure hosting integrations your app host references.

## Publishing

When you publish your app, the Azure provisioning generated Bicep is used by the Azure Developer CLI to create the Azure resources in your Azure subscription. The Azure Developer CLI is a command-line tool that provides a set of commands to manage Azure resources. For more information, see [Azure Developer CLI](/azure/developer/azure-developer-cli).
