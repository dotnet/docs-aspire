---
title: Local Azure provisioning
description: Learn how to use Azure resources in your local development environment.
ms.date: 04/05/2024
---

# Local Azure provisioning

.NET Aspire simplifies local cloud-native app development with its compelling app host model. This model allows you to run your app locally with the same configuration and services as in Azure.

In this article you learn how to provision Azure resources from your local development environment through the [.NET Aspire app host](../../fundamentals/app-host-overview.md). All of this is possible with the help of the [Azure.Provisioning.* libraries](#azure-provisioning-libraries), which provide a set of APIs to provision Azure resources. These packages are transitive dependencies of the .NET Aspire Azure hosting libraries you use in your app host, so you don't need to install them separately.

## Assumptions

This article assumes that you have an Azure account and subscription. If you don't have an Azure account, you can create a free one at [Azure Free Account](https://azure.microsoft.com/free/). For provisioning functionality to work correctly, you'll need to be authenticated with Azure. Additionally, you'll need to provide some configuration values so that the provisioning logic can create resources on your behalf.

## Configuration

When utilizing Azure resources in your local development environment, you need to provide the necessary configuration values. Configuration values are specified under the `Azure` section:

- `SubscriptionId`: The Azure subscription ID.
- `AllowResourceGroupCreation`: A boolean value that indicates whether to create a new resource group.
- `ResourceGroup`: The name of the resource group to use.
- `Location`: The Azure region to use.

Consider the following example _appsettings.json_ configuration:

```json
{
  "Azure": {
    "SubscriptionId": "<Your subscription id>",
    "AllowResourceGroupCreation": true,
    "ResourceGroup": "<Valid resource group name>",
    "Location": "<Valid Azure location>",
  }
}
```

> [!IMPORTANT]
> It's recommended to store these values as app secrets. For more information, see [Manage app secrets](/aspnet/core/security/app-secrets).

After you've configured the necessary values, you can start provisioning Azure resources in your local development environment.

### Tooling support

In Visual Studio, you can use Connected Services to configure the default Azure provisioning settings. Select the app host project, right-click on the **Connected Services** node, and select **Azure Resource Provisioning Settings**:

:::image type="content" source="media/azure-resource-provisioning-settings.png" lightbox="media/azure-resource-provisioning-settings.png" alt-text="Visual Studio 2022: .NET Aspire App Host project, Connected Services context menu.":::

This will open a dialog where you can configure the Azure provisioning settings, as shown in the following screenshot:

:::image type="content" source="media/azure-provisioning-settings-dialog.png" lightbox="media/azure-provisioning-settings-dialog.png" alt-text="Visual Studio 2022: Azure Resource Provisioning Settings dialog.":::

### Missing configuration value hints

When the `Azure` configuration section is missing, has missing values, or is invalid, the [.NET Aspire dashboard](../../fundamentals/dashboard.md) provides useful hints. For example, consider an app host that's missing the `SubscriptionId` configuration value that's attempting to use an Azure Key Vault resource, the **Resources** page indicates the **State** as **Missing subscription configuration**:

:::image type="content" source="media/resources-kv-missing-subscription.png" alt-text=".NET Aspire dashboard: Missing subscription configuration.":::

Additionally, the **Console logs** display this information as well, consider the following screenshot:

:::image type="content" source="media/console-logs-kv-missing-subscription.png" lightbox="media/console-logs-kv-missing-subscription.png" alt-text=".NET Aspire dashboard: Console logs, missing subscription configuration.":::

## App host provisioning APIs

The app host provides a set of APIs to express Azure resources. These APIs are available as extension methods in .NET Aspire Azure hosting libraries, extending the `IDistributedApplicationBuilder` interface. When you add Azure resources to your app host, they'll add the appropriate provisioning functionality implicitly. In other words, you don't need to call any provisioning APIs directly.

When the app host starts, the following provisioning logic is executed:

1. The `Azure` configuration section is validated.
1. When invalid the dashboard and app host output provides hints as to what's missing. For more information, see [Missing configuration value hints](#missing-configuration-value-hints).
1. When valid Azure resources are conditionally provisioned:
    1. If an Azure deployment for a given resource doesn't exist, it's created and configured as a deployment.
    1. The configuration of said deployment is stamped with a checksum as a means to support only provisioning resources as necessary.

### Use existing Azure resources

The app host automatically manages provisioning of Azure resources. The first time the app host runs, it provisions the resources specified in the app host. Subsequent runs don't provision the resources again unless the app host configuration changes.

If you've already provisioned Azure resources outside of the app host and want to use them, you can provide the connection string with the `AddConnectionString` API as shown in the following Azure Key Vault example:

```csharp
// Service registration
var secrets = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureKeyVault("secrets")
    : builder.AddConnectionString("secrets");

// Service consumption
builder.AddProject<Projects.ExampleProject>()
       .WithReference(secrets)
```

The preceding code snippet shows how to add an Azure Key Vault to the app host. The `AddAzureKeyVault` API is used to add the Azure Key Vault to the app host. The `AddConnectionString` API is used to provide the connection string to the app host.

Alternatively, for some Azure resources, you can opt-in to running them as an emulator with the `RunAsEmulator` API. This API is available for Azure Cosmos DB and Azure Storage. For example, to run Azure Cosmos DB as an emulator, you can use the following code snippet:

```csharp
var cosmos = builder.AddAzureCosmosDB("cosmos")
                    .RunAsEmulator();
```

The `RunAsEmulator` API configures an Azure Cosmos DB resource to be emulated using the Azure Cosmos DB emulator with the NoSQL API.

### .NET Aspire Azure hosting libraries

If you're using Azure resources in your app host, you're using one or more of the [.NET Aspire Azure hosting libraries](../../fundamentals/app-host-overview.md#azure-hosting-libraries). These hosting libraries provide extension methods to the `IDistributedApplicationBuilder` interface to add Azure resources to your app host.

### Azure provisioning libraries

The following Azure provisioning libraries are available:

- [📦 Azure.Provisioning.AppConfiguration](https://www.nuget.org/packages/Azure.Provisioning.AppConfiguration)
- [📦 Azure.Provisioning.ApplicationInsights](https://www.nuget.org/packages/Azure.Provisioning.ApplicationInsights)
- [📦 Azure.Provisioning.CognitiveServices](https://www.nuget.org/packages/Azure.Provisioning.CognitiveServices)
- [📦 Azure.Provisioning.CosmosDB](https://www.nuget.org/packages/Azure.Provisioning.CosmosDB)
- [📦 Azure.Provisioning.EventHubs](https://www.nuget.org/packages/Azure.Provisioning.EventHubs)
- [📦 Azure.Provisioning.KeyVault](https://www.nuget.org/packages/Azure.Provisioning.KeyVault)
- [📦 Azure.Provisioning.OperationalInsights](https://www.nuget.org/packages/Azure.Provisioning.OperationalInsights)
- [📦 Azure.Provisioning.PostgreSql](https://www.nuget.org/packages/Azure.Provisioning.PostgreSql)
- [📦 Azure.Provisioning.Redis](https://www.nuget.org/packages/Azure.Provisioning.Redis)
- [📦 Azure.Provisioning.Resources](https://www.nuget.org/packages/Azure.Provisioning.Resources)
- [📦 Azure.Provisioning.Search](https://www.nuget.org/packages/Azure.Provisioning.Search)
- [📦 Azure.Provisioning.ServiceBus](https://www.nuget.org/packages/Azure.Provisioning.ServiceBus)
- [📦 Azure.Provisioning.SignalR](https://www.nuget.org/packages/Azure.Provisioning.SignalR)
- [📦 Azure.Provisioning.Sql](https://www.nuget.org/packages/Azure.Provisioning.Sql)
- [📦 Azure.Provisioning.Storage](https://www.nuget.org/packages/Azure.Provisioning.Storage)
- [📦 Azure.Provisioning](https://www.nuget.org/packages/Azure.Provisioning)

> [!TIP]
> You shouldn't need to install these packages manually in your projects, as they're transitive dependencies of the corresponding .NET Aspire Azure hosting libraries.

## Known limitations

After provisioning Azure resources in this way, you must manually clean up the resources in the Azure portal as .NET Aspire doesn't provide a built-in mechanism to delete Azure resources. The easiest way to achieve this is by deleting the configured resource group. This can be done in the [Azure portal](/azure/azure-resource-manager/management/delete-resource-group?tabs=azure-portal#delete-resource-group) or by using the Azure CLI:

```azurecli
az group delete --name <ResourceGroupName>
```

Replace `<ResourceGroupName>` with the name of the resource group you want to delete. For more information, see [az group delete](/cli/azure/group#az-group-delete).
