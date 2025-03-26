---
title: Add Azure role assignments
description: Learn how to add Azure role assignments to .NET Aspire resources.
ms.date: 03/24/2025
---

# Add Azure role assignments

All .NET Aspire Azure hosting integrations define Azure resources. [These resources](integrations-overview.md#add-azure-resources) come with default role assignments. You can replace these default role assignments with built-in role or custom role assignments. In this article, you learn how to add Azure role assignments to .NET Aspire resources.

## Add built-in role assignments

For every Azure resource you add to the [app model](xref:dotnet/aspire/app-host#terminology), you can add built-in role assignments. When a resource is referenced by another resource, a dependency is created. The dependent resource has default role access that's defined by each Azure resource When an API project resource references an Azure resource, default role assignments are created. You can override these defaults and assign built-in roles that corresponding to the resource type. Consider the following example:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var search = builder.AddAzureSearch("search");

var api = builder.AddProject<Projects.Api>("api")
                 .WithReference(search);
```

In the preceding code, the `search` resource is referenced by the `api` resource. The `search` resource has the following default role assignments:

- <xref:Azure.Provisioning.Search.SearchBuiltInRole.SearchIndexDataContributor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.Search.SearchBuiltInRole.SearchServiceContributor?displayProperty=nameWithType>

To override these defaults and assign built-in roles that correspond to the resource type. For example:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var search = builder.AddAzureSearch("search");

var api = builder.AddProject<Projects.Api>("api")
                 .WithRoleAssignments(search, SearchBuiltInRole.SearchIndexDataReader)
                 .WithReference(search);
```

The preceding code replaces the default role assignments with the following:

- <xref:Azure.Provisioning.Search.SearchBuiltInRole.SearchIndexDataReader?displayProperty=nameWithType>

This limits the API project to only read data from the Azure Search resource. The API project can no longer write data to the Azure Search resource.

When you chain a call to the `WithRoleAssignments` method, it overrides the default role assignments, replacing them with the given role assignments. The `WithRoleAssignments` method takes the resource to which the role assignment applies and the built-in role to assign. In this case, the `search` resource is assigned the `SearchIndexDataReader` role.

For more information, see [Azure documentation](https://learn.microsoft.com/en-us/azure/role-based-access-control/built-in-roles).

## Built-in role assignments

All built-in roles are defined in <xref:Azure.Provisioning> namespaces.

### Azure App Configuration

The provisioning resource type is <xref:Azure.Provisioning.AppConfiguration.AppConfigurationStore>, and the built-in roles are:

- <xref:Azure.Provisioning.AppConfiguration.AppConfigurationBuiltInRole.AppConfigurationDataOwner?displayProperty=nameWithType>
- <xref:Azure.Provisioning.AppConfiguration.AppConfigurationBuiltInRole.AppConfigurationDataReader?displayProperty=nameWithType>

### Azure App Container

The provisioning resource type is <xref:Azure.Provisioning.AppContainer.ContainerApp>, and the built-in roles are:

- <xref:Azure.Provisioning.AppContainers.AppContainersBuiltInRole.Contributor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.AppContainers.AppContainersBuiltInRole.Owner?displayProperty=nameWithType>
- <xref:Azure.Provisioning.AppContainers.AppContainersBuiltInRole.Reader?displayProperty=nameWithType>

### Azure Application Insights

The provisioning resource type is <xref:Azure.Provisioning.ApplicationInsights.ApplicationInsightsComponent>, and the built-in roles are:

- <xref:Azure.Provisioning.ApplicationInsights.ApplicationInsightsBuiltInRole.ApplicationInsightsComponentContributor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.ApplicationInsights.ApplicationInsightsBuiltInRole.ApplicationInsightsSnapshotDebugger?displayProperty=nameWithType>
- <xref:Azure.Provisioning.ApplicationInsights.ApplicationInsightsBuiltInRole.MonitoringContributor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.ApplicationInsights.ApplicationInsightsBuiltInRole.MonitoringMetricsPublisher?displayProperty=nameWithType>
- <xref:Azure.Provisioning.ApplicationInsights.ApplicationInsightsBuiltInRole.MonitoringReader?displayProperty=nameWithType>
- <xref:Azure.Provisioning.ApplicationInsights.ApplicationInsightsBuiltInRole.WorkbookContributor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.ApplicationInsights.ApplicationInsightsBuiltInRole.WorkbookReader?displayProperty=nameWithType>

### Azure AI (Formerly Cognitive Services)

The provisioning resource type is <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesAccount>, and the built-in roles are:

- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.AzureAIDeveloper?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.AzureAIEnterpriseNetworkConnectionApprover?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.AzureAIInferenceDeploymentOperator?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.CognitiveServicesContributor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.CognitiveServicesCustomVisionContributor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.CognitiveServicesCustomVisionDeployment?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.CognitiveServicesCustomVisionLabeler?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.CognitiveServicesCustomVisionReader?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.CognitiveServicesCustomVisionTrainer?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.CognitiveServicesDataReader?displayProperty=nameWithType><xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.CognitiveServicesOpenAIUser?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.CognitiveServicesFaceRecognizer?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.CognitiveServicesMetricsAdvisorAdministrator?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.CognitiveServicesOpenAIContributor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.CognitiveServicesOpenAIUser?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.CognitiveServicesQnAMakerEditor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.CognitiveServicesQnAMakerReader?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.CognitiveServicesUsagesReader?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CognitiveServices.CognitiveServicesBuiltInRole.CognitiveServicesUser?displayProperty=nameWithType>

### Azure Cosmos DB

The provisioning resource type is <xref:Azure.Provisioning.CosmosDB.CosmosDBAccount>, and the built-in roles are:

- <xref:Azure.Provisioning.CosmosDB.CosmosDBBuiltInRole.CosmosDBOperator?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CosmosDB.CosmosDBBuiltInRole.CosmosBackupOperator?displayProperty=nameWithType>
- <xref:Azure.Provisioning.CosmosDB.CosmosDBBuiltInRole.CosmosRestoreOperator?displayProperty=nameWithType>

### Azure Event Hubs

The provisioning resource type is <xref:Azure.Provisioning.EventHubs.EventHubNamespace>, and the built-in roles are:

- <xref:Azure.Provisioning.EventHubs.EventHubsBuiltInRole.AzureEventHubsDataOwner?displayProperty=nameWithType>
- <xref:Azure.Provisioning.EventHubs.EventHubsBuiltInRole.AzureEventHubsDataReceiver?displayProperty=nameWithType>
- <xref:Azure.Provisioning.EventHubs.EventHubsBuiltInRole.AzureEventHubsDataSender?displayProperty=nameWithType>
- <xref:Azure.Provisioning.EventHubs.EventHubsBuiltInRole.SchemaRegistryContributor?displayProperty=nameWithType>
- <xref:Azure.Provisioning.EventHubs.EventHubsBuiltInRole.SchemaRegistryReader?displayProperty=nameWithType>

### Azure Key Vault

The provisioning resource type is <xref:Azure.Provisioning.KeyVault.KeyVaultService>, and the built-in roles are:

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

<!--

- Functions
- KeyVault
- OperationalInsights
- PostgreSQL
- Redis
- Search
- ServiceBus
- SignalR
- Sql
- Storage
- WebPubSub

 -->