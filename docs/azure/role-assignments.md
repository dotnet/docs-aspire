---
title: Manage Azure role assignments
description: Learn how to override Azure role assignments on .NET Aspire resources.
ms.date: 03/31/2025
---

# Manage Azure role assignments

All .NET Aspire Azure hosting integrations define Azure resources. [These resources](integrations-overview.md#add-azure-resources) come with default role assignments. You can replace these default role assignments with built-in role [or custom role assignments](integrations-overview.md#infrastructure-as-code). In this article, you learn how to manage Azure role assignments on .NET Aspire resources.

## Default built-in role assignments

When you add an Azure resource to the [app model](xref:dotnet/aspire/app-host#terminology), it's assigned default roles. If a resource depends on another resource, it inherits the same role assignments as the referenced resource unless explicitly overridden.

Consider a scenario where an API project resource references an [Azure Search](../azureai/azureai-search-document-integration.md) resource. The API project is given the default role assignments, as shown in the following example:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var search = builder.AddAzureSearch("search");

var api = builder.AddProject<Projects.Api>("api")
                 .WithReference(search);
```

In the example code, the `api` project resource depends on the Azure `search` resource, meaning it references the `search` resource. By default, the `search` resource is assigned the following built-in roles:

- <xref:Azure.Provisioning.Search.SearchBuiltInRole.SearchIndexDataContributor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.Search.SearchBuiltInRole.SearchServiceContributor?displayProperty=nameWithType>

These role assignments allow the API project to read and write data to the Azure Search resource, and manage it. However, this behavior might not always be desirable. For instance, you might want to restrict the API project to only read data from the Azure Search resource.

## Override default role assignments

<!-- TODO: Add xref links when available for the WithRoleAssignments API. -->

To override the default role assignment, use the `WithRoleAssignments` API and assign built-in roles as shown in the following example:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var search = builder.AddAzureSearch("search");

var api = builder.AddProject<Projects.Api>("api")
                 .WithRoleAssignments(search, SearchBuiltInRole.SearchIndexDataReader)
                 .WithReference(search);
```

When you use the `WithRoleAssignments` method, it replaces the default role assignments with the specified ones. This method requires two parameters: the resource to which the role assignment applies and the built-in role to assign. In the preceding example, the `search` resource is assigned the <xref:Azure.Provisioning.Search.SearchBuiltInRole.SearchIndexDataReader?displayProperty=nameWithType> role.

When you replace the default role assignments with the `SearchIndexDataReader` role, the API project is restricted to only reading data from the Azure Search resource. This ensures the API project can't write data to the Azure Search resource.

For more information, see [Azure built-in roles](/azure/role-based-access-control/built-in-roles).

## Built-in role assignment reference

All built-in roles are defined within the <xref:Azure.Provisioning> namespaces and are included in the corresponding [📦 Azure.Provisioning.*](https://www.nuget.org/packages?q=Azure.Provisioning) NuGet packages. Each .NET Aspire Azure hosting integration automatically depends on the appropriate provisioning package. For more information, see [Infrastructure as code](integrations-overview.md#infrastructure-as-code).

The following sections list the built-in roles for each Azure provisioning type that can be used as a parameter to the `WithRoleAssignments` API.

### Azure App Configuration

The provisioning resource type is <xref:Azure.Provisioning.AppConfiguration.AppConfigurationStore>, and the built-in roles are defined in the <xref:Azure.Provisioning.AppConfiguration.AppConfigurationBuiltInRole> struct. The built-in roles are:

- <xref:Azure.Provisioning.AppConfiguration.AppConfigurationBuiltInRole.AppConfigurationDataOwner?displayProperty=nameWithType>
- <xref:Azure.Provisioning.AppConfiguration.AppConfigurationBuiltInRole.AppConfigurationDataReader?displayProperty=nameWithType>

<!--
For more information, see [.NET Aspire Azure App Configuration integration](../configuration/azure-app-configuration-integration.md).
-->

### Azure App Container

The provisioning resource type is <xref:Azure.Provisioning.AppContainers.ContainerApp>, and the built-in roles are defined in the <xref:Azure.Provisioning.AppContainers.AppContainersBuiltInRole> struct. The built-in roles are:

- <xref:Azure.Provisioning.AppContainers.AppContainersBuiltInRole.Contributor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.AppContainers.AppContainersBuiltInRole.Owner?displayProperty=nameWithType>
- <xref:Azure.Provisioning.AppContainers.AppContainersBuiltInRole.Reader?displayProperty=nameWithType>

<!--
For more information, see [.NET Aspire Azure App Container integration](../hosting/azure-app-container-integration.md).
-->

### Azure Application Insights

The provisioning resource type is <xref:Azure.Provisioning.ApplicationInsights.ApplicationInsightsComponent>, and the built-in roles are defined in the <xref:Azure.Provisioning.ApplicationInsights.ApplicationInsightsBuiltInRole> struct. The built-in roles are:

- <xref:Azure.Provisioning.ApplicationInsights.ApplicationInsightsBuiltInRole.ApplicationInsightsComponentContributor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.ApplicationInsights.ApplicationInsightsBuiltInRole.ApplicationInsightsSnapshotDebugger?displayProperty=nameWithType>
- <xref:Azure.Provisioning.ApplicationInsights.ApplicationInsightsBuiltInRole.MonitoringContributor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.ApplicationInsights.ApplicationInsightsBuiltInRole.MonitoringMetricsPublisher?displayProperty=nameWithType>
- <xref:Azure.Provisioning.ApplicationInsights.ApplicationInsightsBuiltInRole.MonitoringReader?displayProperty=nameWithType>
- <xref:Azure.Provisioning.ApplicationInsights.ApplicationInsightsBuiltInRole.WorkbookContributor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.ApplicationInsights.ApplicationInsightsBuiltInRole.WorkbookReader?displayProperty=nameWithType>

For more information, see [Use Application Insights for .NET Aspire telemetry](../deployment/azure/application-insights.md).

### Azure AI (formerly Cognitive Services)

The provisioning resource type is <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesAccount>, and the built-in roles are defined in the <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole> struct. The built-in roles are:

- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.AzureAIDeveloper?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.AzureAIEnterpriseNetworkConnectionApprover?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.AzureAIInferenceDeploymentOperator?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.CognitiveServicesContributor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.CognitiveServicesCustomVisionContributor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.CognitiveServicesCustomVisionDeployment?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.CognitiveServicesCustomVisionLabeler?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.CognitiveServicesCustomVisionReader?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.CognitiveServicesCustomVisionTrainer?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.CognitiveServicesDataReader?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.CognitiveServicesFaceRecognizer?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.CognitiveServicesMetricsAdvisorAdministrator?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.CognitiveServicesOpenAIContributor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.CognitiveServicesOpenAIUser?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.CognitiveServicesOpenAIUser?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.CognitiveServicesQnAMakerEditor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.CognitiveServicesQnAMakerReader?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.CognitiveServicesUsagesReader?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.CognitiveServicesUser?displayProperty=nameWithType>

For more information, see [.NET Aspire Azure OpenAI integration (Preview)](../azureai/azureai-openai-integration.md).

### Azure Cosmos DB

The provisioning resource type is <xref:Azure.Provisioning.CosmosDB.CosmosDBAccount>, and the built-in roles are defined in the <xref:Azure.Provisioning.CosmosDB.CosmosDBBuiltInRole> struct. The built-in roles are:

- <xref:Azure.Provisioning.CosmosDB.CosmosDBBuiltInRole.CosmosDBOperator?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CosmosDB.CosmosDBBuiltInRole.CosmosBackupOperator?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CosmosDB.CosmosDBBuiltInRole.CosmosRestoreOperator?displayProperty=nameWithType>

For more information, see:

- [.NET Aspire Azure Cosmos DB integration](../database/azure-cosmos-db-integration.md).
- [.NET Aspire Cosmos DB Entity Framework Core integration](../database/azure-cosmos-db-entity-framework-integration.md).

### Azure Event Hubs

The provisioning resource type is <xref:Azure.Provisioning.EventHubs.EventHubsNamespace>, and the built-in roles are defined in the <xref:Azure.Provisioning.EventHubs.EventHubsBuiltInRole> struct. The built-in roles are:

- <xref:Azure.Provisioning.EventHubs.EventHubsBuiltInRole.AzureEventHubsDataOwner?displayProperty=nameWithType>
- <xref:Azure.Provisioning.EventHubs.EventHubsBuiltInRole.AzureEventHubsDataReceiver?displayProperty=nameWithType>
- <xref:Azure.Provisioning.EventHubs.EventHubsBuiltInRole.AzureEventHubsDataSender?displayProperty=nameWithType>
- <xref:Azure.Provisioning.EventHubs.EventHubsBuiltInRole.SchemaRegistryContributor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.EventHubs.EventHubsBuiltInRole.SchemaRegistryReader?displayProperty=nameWithType>

For more information, see [.NET Aspire Azure Event Hubs integration](../messaging/azure-event-hubs-integration.md).

### Azure Key Vault

The provisioning resource type is <xref:Azure.Provisioning.KeyVault.KeyVaultService>, and the built-in roles are defined in the <xref:Azure.Provisioning.KeyVault.KeyVaultBuiltInRole> struct. The built-in roles are:

- <xref:Azure.Provisioning.KeyVault.KeyVaultBuiltInRole.KeyVaultAdministrator?displayProperty=nameWithType>
- <xref:Azure.Provisioning.KeyVault.KeyVaultBuiltInRole.KeyVaultCertificatesOfficer?displayProperty=nameWithType>
- <xref:Azure.Provisioning.KeyVault.KeyVaultBuiltInRole.KeyVaultCertificateUser?displayProperty=nameWithType>
- <xref:Azure.Provisioning.KeyVault.KeyVaultBuiltInRole.KeyVaultContributor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.KeyVault.KeyVaultBuiltInRole.KeyVaultCryptoOfficer?displayProperty=nameWithType>
- <xref:Azure.Provisioning.KeyVault.KeyVaultBuiltInRole.KeyVaultCryptoServiceEncryptionUser?displayProperty=nameWithType>
- <xref:Azure.Provisioning.KeyVault.KeyVaultBuiltInRole.KeyVaultCryptoServiceReleaseUser?displayProperty=nameWithType>
- <xref:Azure.Provisioning.KeyVault.KeyVaultBuiltInRole.KeyVaultCryptoUser?displayProperty=nameWithType>
- <xref:Azure.Provisioning.KeyVault.KeyVaultBuiltInRole.KeyVaultDataAccessAdministrator?displayProperty=nameWithType>
- <xref:Azure.Provisioning.KeyVault.KeyVaultBuiltInRole.KeyVaultDataAccessAdministrator?displayProperty=nameWithType>
- <xref:Azure.Provisioning.KeyVault.KeyVaultBuiltInRole.KeyVaultReader?displayProperty=nameWithType>
- <xref:Azure.Provisioning.KeyVault.KeyVaultBuiltInRole.KeyVaultSecretsOfficer?displayProperty=nameWithType>
- <xref:Azure.Provisioning.KeyVault.KeyVaultBuiltInRole.KeyVaultSecretsUser?displayProperty=nameWithType>
- <xref:Azure.Provisioning.KeyVault.KeyVaultBuiltInRole.ManagedHsmContributor?displayProperty=nameWithType>

For more information, see [.NET Aspire Azure Key Vault integration](../security/azure-security-key-vault-integration.md).

### Azure AI Search

The provisioning resource type is <xref:Azure.Provisioning.Search.SearchService>, and the built-in roles are defined in the <xref:Azure.Provisioning.Search.SearchBuiltInRole> struct. The built-in roles are:

- <xref:Azure.Provisioning.Search.SearchBuiltInRole.SearchIndexDataContributor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.Search.SearchBuiltInRole.SearchIndexDataReader?displayProperty=nameWithType>
- <xref:Azure.Provisioning.Search.SearchBuiltInRole.SearchServiceContributor?displayProperty=nameWithType>

For more information, see [.NET Aspire Azure AI Search integration](../azureai/azureai-search-document-integration.md).

### Azure Service Bus

The provisioning resource type is <xref:Azure.Provisioning.ServiceBus.ServiceBusNamespace>, and the built-in roles are defined in the <xref:Azure.Provisioning.ServiceBus.ServiceBusBuiltInRole> struct. The built-in roles are:

- <xref:Azure.Provisioning.ServiceBus.ServiceBusBuiltInRole.AzureServiceBusDataOwner?displayProperty=nameWithType>
- <xref:Azure.Provisioning.ServiceBus.ServiceBusBuiltInRole.AzureServiceBusDataReceiver?displayProperty=nameWithType>
- <xref:Azure.Provisioning.ServiceBus.ServiceBusBuiltInRole.AzureServiceBusDataSender?displayProperty=nameWithType>

For more information, see [.NET Aspire Azure Service Bus integration](../messaging/azure-service-bus-integration.md).

### Azure SignalR Service

The provisioning resource type is <xref:Azure.Provisioning.SignalR.SignalRService>, and the built-in roles are defined in the <xref:Azure.Provisioning.SignalR.SignalRBuiltInRole> struct. The built-in roles are:

- <xref:Azure.Provisioning.SignalR.SignalRBuiltInRole.SignalRAccessKeyReader?displayProperty=nameWithType>
- <xref:Azure.Provisioning.SignalR.SignalRBuiltInRole.SignalRAppServer?displayProperty=nameWithType>
- <xref:Azure.Provisioning.SignalR.SignalRBuiltInRole.SignalRContributor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.SignalR.SignalRBuiltInRole.SignalRRestApiOwner?displayProperty=nameWithType>
- <xref:Azure.Provisioning.SignalR.SignalRBuiltInRole.SignalRRestApiReader?displayProperty=nameWithType>
- <xref:Azure.Provisioning.SignalR.SignalRBuiltInRole.SignalRServiceOwner?displayProperty=nameWithType>

For more information, see [.NET Aspire support for Azure SignalR Service](../real-time/azure-signalr-scenario.md).

### Azure SQL

The provisioning resource type is <xref:Azure.Provisioning.Sql.SqlServer>, and the built-in roles are defined in the <xref:Azure.Provisioning.Sql.SqlBuiltInRole> struct. The built-in roles are:

- <xref:Azure.Provisioning.Sql.SqlBuiltInRole.AzureConnectedSqlServerOnboarding?displayProperty=nameWithType>
- <xref:Azure.Provisioning.Sql.SqlBuiltInRole.SqlDBContributor?displayProperty=nameWithType?displayProperty=nameWithType>
- <xref:Azure.Provisioning.Sql.SqlBuiltInRole.SqlManagedInstanceContributor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.Sql.SqlBuiltInRole.SqlSecurityManager?displayProperty=nameWithType>
- <xref:Azure.Provisioning.Sql.SqlBuiltInRole.SqlServerContributor?displayProperty=nameWithType>

<!--
For more information, see [.NET Aspire Azure SQL integration](../data/azure-sql-integration.md).
-->

### Azure Storage

The provisioning resource type is <xref:Azure.Provisioning.Storage.StorageAccount>, and the built-in roles are defined in the <xref:Azure.Provisioning.Storage.StorageBuiltInRole> struct. The built-in roles are:

- <xref:Azure.Provisioning.Storage.StorageBuiltInRole.ClassicStorageAccountContributor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.Storage.StorageBuiltInRole.ClassicStorageAccountKeyOperatorServiceRole?displayProperty=nameWithType>
- <xref:Azure.Provisioning.Storage.StorageBuiltInRole.StorageAccountBackupContributor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.Storage.StorageBuiltInRole.StorageAccountContributor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.Storage.StorageBuiltInRole.StorageAccountKeyOperatorServiceRole?displayProperty=nameWithType>
- <xref:Azure.Provisioning.Storage.StorageBuiltInRole.StorageBlobDataContributor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.Storage.StorageBuiltInRole.StorageBlobDataOwner?displayProperty=nameWithType>
- <xref:Azure.Provisioning.Storage.StorageBuiltInRole.StorageBlobDataReader?displayProperty=nameWithType>
- <xref:Azure.Provisioning.Storage.StorageBuiltInRole.StorageBlobDelegator?displayProperty=nameWithType>
- <xref:Azure.Provisioning.Storage.StorageBuiltInRole.StorageFileDataPrivilegedContributor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.Storage.StorageBuiltInRole.StorageFileDataPrivilegedReader?displayProperty=nameWithType>
- <xref:Azure.Provisioning.Storage.StorageBuiltInRole.StorageFileDataSmbShareContributor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.Storage.StorageBuiltInRole.StorageFileDataSmbShareElevatedContributor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.Storage.StorageBuiltInRole.StorageFileDataSmbShareReader?displayProperty=nameWithType>
- <xref:Azure.Provisioning.Storage.StorageBuiltInRole.StorageQueueDataContributor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.Storage.StorageBuiltInRole.StorageQueueDataMessageProcessor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.Storage.StorageBuiltInRole.StorageQueueDataMessageSender?displayProperty=nameWithType>
- <xref:Azure.Provisioning.Storage.StorageBuiltInRole.StorageQueueDataReader?displayProperty=nameWithType>
- <xref:Azure.Provisioning.Storage.StorageBuiltInRole.StorageTableDataContributor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.Storage.StorageBuiltInRole.StorageTableDataReader?displayProperty=nameWithType>

For more information, see:

- [.NET Aspire Azure Blob Storage integration](../storage/azure-storage-blobs-integration.md)
- [.NET Aspire Azure Data Tables integration](../storage/azure-storage-tables-integration.md)
- [.NET Aspire Azure Queue Storage integration](../storage/azure-storage-queues-integration.md)

### Azure Web PubSub

The provisioning resource type is <xref:Azure.Provisioning.WebPubSub.WebPubSubService>, and the built-in roles are defined in the <xref:Azure.Provisioning.WebPubSub.WebPubSubBuiltInRole> struct. The built-in roles are:

- <xref:Azure.Provisioning.WebPubSub.WebPubSubBuiltInRole.WebPubSubContributor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.WebPubSub.WebPubSubBuiltInRole.WebPubSubServiceOwner?displayProperty=nameWithType>
- <xref:Azure.Provisioning.WebPubSub.WebPubSubBuiltInRole.WebPubSubServiceReader?displayProperty=nameWithType>

For more information, see [.NET Aspire Azure Web PubSub integration](../messaging/azure-web-pubsub-integration.md).

## See also

- [.NET Aspire Azure integrations overview](integrations-overview.md)
- [Azure role-based access control (RBAC)](/azure/role-based-access-control/overview)
