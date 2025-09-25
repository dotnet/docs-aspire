---
title: Local Azure provisioning
description: Learn how to use Azure resources in your local development environment.
ms.date: 09/24/2025
uid: dotnet/aspire/local-azure-provisioning
---

# Local Azure provisioning

.NET Aspire simplifies local cloud-native app development with its compelling AppHost model. This model allows you to run your app locally with the same configuration and services as in Azure. In this article, you learn how to provision Azure resources from your local development environment through the [.NET Aspire AppHost](xref:dotnet/aspire/app-host).

The .NET Aspire dashboard provides an [interactive experience](../extensibility/interaction-service.md) for configuring Azure resources, automatically prompting you for required settings when they're missing. This streamlined approach eliminates the need for manual configuration file editing in most scenarios.

> [!NOTE]
> To be clear, resources are provisioned in Azure, but the provisioning process is initiated from your local development environment. To optimize your local development experience, consider using emulator or containers when available. For more information, see [Typical developer experience](integrations-overview.md#typical-developer-experience).

## Requirements

This article assumes that you have an Azure account and subscription. If you don't have an Azure account, you can create a free one at [Azure Free Account](https://azure.microsoft.com/free/). For provisioning functionality to work correctly, you need to be authenticated with Azure. Ensure that you have the [Azure CLI](/cli/azure/install-azure-cli) installed.

When you run your AppHost with Azure resources, the .NET Aspire dashboard automatically prompts you for any missing configuration values, making the setup process straightforward and interactive.

## AppHost provisioning APIs

The AppHost provides a set of APIs to express Azure resources. These APIs are available as extension methods in .NET Aspire Azure hosting libraries, extending the <xref:Aspire.Hosting.IDistributedApplicationBuilder> interface. When you add Azure resources to your AppHost, they add the appropriate provisioning functionality implicitly. In other words, you don't need to call any provisioning APIs directly.

When the AppHost starts, the following provisioning logic is executed:

1. The `Azure` configuration section is validated.
1. When invalid, the dashboard prompts you to enter the required configuration values or provides detailed error information. For more information, see [Configuration prompts and error handling](#configuration-prompts-and-error-handling).
1. When valid, Azure resources are conditionally provisioned:
    1. If an Azure deployment for a given resource doesn't exist, the AppHost creates and configures it as a deployment.
    1. The AppHost stamps the deployment configuration with a checksum to provision resources only when necessary.

### Use existing Azure resources

The AppHost automatically manages provisioning of Azure resources. The first time the AppHost runs, it provisions the resources specified in the AppHost. Subsequent runs don't provision the resources again unless the AppHost configuration changes.

To use Azure resources that you provision outside of the AppHost, provide the connection string with the <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> API, as shown in the following Azure Key Vault example:

```csharp
// Service registration
var secrets = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureKeyVault("secrets")
    : builder.AddConnectionString("secrets");

// Service consumption
builder.AddProject<Projects.ExampleProject>()
       .WithReference(secrets)
```

The preceding code snippet shows how to add an Azure Key Vault to the AppHost. The <xref:Aspire.Hosting.AzureKeyVaultResourceExtensions.AddAzureKeyVault*> API is used to add the Azure Key Vault to the AppHost. The `AddConnectionString` API is used to provide the connection string to the AppHost.

Alternatively, for some Azure resources, you can opt in to running them as an emulator with the `RunAsEmulator` API. This API is available for [Azure Cosmos DB](../database/azure-cosmos-db-integration.md) and [Azure Storage](../storage/azure-storage-integrations.md) integrations. For example, to run Azure Cosmos DB as an emulator, you can use the following code snippet:

```csharp
var cosmos = builder.AddAzureCosmosDB("cosmos")
                    .RunAsEmulator();
```

The <xref:Aspire.Hosting.AzureCosmosExtensions.RunAsEmulator*> API configures an Azure Cosmos DB resource to be emulated using the Azure Cosmos DB emulator with the NoSQL API.

### .NET Aspire Azure hosting integrations

If you're using Azure resources in your AppHost, you're using one or more of the [.NET Aspire Azure hosting integrations](integrations-overview.md). These hosting libraries provide extension methods to the <xref:Aspire.Hosting.IDistributedApplicationBuilder> interface to add Azure resources to your AppHost.

## Configuration

When utilizing Azure resources in your local development environment, you need to provide the necessary configuration values. You can provide these values in two ways:

- **Interactive prompting** (recommended): Let the .NET Aspire dashboard prompt you for the required values.
- **Manual configuration**: Specify the values directly in your configuration files.

### Interactive configuration prompting

When you run your AppHost and it requires Azure resources, the .NET Aspire dashboard automatically prompts you for any missing Azure configuration values. This interactive approach simplifies the setup process and eliminates the need to manually configure settings files.

When the AppHost detects missing Azure configuration, the dashboard displays a message bar with an **Enter values** button:

:::image type="content" source="media/azure-missing-config-prompt.png" lightbox="media/azure-missing-config-prompt.png" alt-text=".NET Aspire dashboard: Azure configuration prompt message bar.":::

Clicking **Enter values** opens a dialog where you can provide the required Azure settings:

:::image type="content" source="media/azure-config-dialog.png" lightbox="media/azure-config-dialog.png" alt-text=".NET Aspire dashboard: Azure configuration dialog.":::

The configuration values you need to provide are:

- **Subscription ID**: Your Azure subscription ID.
- **Resource Group**: The name of the resource group to use (a default name is suggested).
- **Location**: The Azure region to use.

The dashboard automatically saves these values to your user secrets for future use, so you only need to enter them once per project. For more information on user secrets, see [Manage app secrets](/aspnet/core/security/app-secrets).

### Manual configuration

Alternatively, you can manually specify configuration values under the `Azure` section in your _:::no-loc text="appsettings.json":::_ file or user secrets:

- `SubscriptionId`: The Azure subscription ID.
- `AllowResourceGroupCreation`: A boolean value that indicates whether to create a new resource group.
- `ResourceGroup`: The name of the resource group to use.
- `Location`: The Azure region to use.

Consider the following example _:::no-loc text="appsettings.json":::_ configuration:

```json
{
  "Azure": {
    "SubscriptionId": "<Your subscription id>",
    "AllowResourceGroupCreation": true,
    "ResourceGroup": "<Valid resource group name>",
    "Location": "<Valid Azure location>"
  }
}
```

> [!IMPORTANT]
> Store these values as app secrets. For more information, see [Manage app secrets](/aspnet/core/security/app-secrets).

Once you configure the necessary values—either through interactive prompting or manual configuration—you can provision Azure resources in your local development environment.

### Azure provisioning credential store

The .NET Aspire AppHost uses a credential store for Azure resource authentication and authorization. Depending on your subscription, the correct credential store might be needed for multitenant provisioning scenarios.

With the [📦 Aspire.Hosting.Azure](https://nuget.org/packages/Aspire.Hosting.Azure) NuGet package installed, and if your AppHost depends on Azure resources, the default Azure credential store relies on the <xref:Azure.Identity.DefaultAzureCredential>. To change this behavior, you can set the credential store value in the _:::no-loc text="appsettings.json":::_ file, as shown in the following example:

```json
{
  "Azure": {
    "CredentialSource": "AzureCli"
  }
}
```

As with all [configuration-based settings](/dotnet/core/extensions/configuration), you can configure these with alternative providers, such as [user secrets](/aspnet/core/security/app-secrets) or [environment variables](/dotnet/core/extensions/configuration-providers#environment-variable-configuration-provider). The `Azure:CredentialSource` value can be set to one of the following values:

- `AzureCli`: Delegates to the <xref:Azure.Identity.AzureCliCredential>.
- `AzurePowerShell`: Delegates to the <xref:Azure.Identity.AzurePowerShellCredential>.
- `VisualStudio`: Delegates to the <xref:Azure.Identity.VisualStudioCredential>.
- `AzureDeveloperCli`: Delegates to the <xref:Azure.Identity.AzureDeveloperCliCredential>.
- `InteractiveBrowser`: Delegates to the <xref:Azure.Identity.InteractiveBrowserCredential>.

> [!TIP]
> For more information about the Azure SDK authentication and authorization, see [Credential chains in the Azure Identity library for .NET](/dotnet/azure/sdk/authentication/credential-chains?tabs=dac#defaultazurecredential-overview).

### Tooling support

In Visual Studio, you can use Connected Services to configure the default Azure provisioning settings. Select the AppHost project, right-click on the **Connected Services** node, and select **Azure Resource Provisioning Settings**:

:::image type="content" loc-scope="visual-studio" source="media/azure-resource-provisioning-settings.png" lightbox="media/azure-resource-provisioning-settings.png" alt-text="Visual Studio 2022: .NET Aspire AppHost project, Connected Services context menu.":::

This opens a dialog where you can configure the Azure provisioning settings, as shown in the following screenshot:

:::image type="content" loc-scope="visual-studio" source="media/azure-provisioning-settings-dialog.png" lightbox="media/azure-provisioning-settings-dialog.png" alt-text="Visual Studio 2022: Azure Resource Provisioning Settings dialog.":::

### Configuration prompts and error handling

When the `Azure` configuration section is missing, has missing values, or is invalid, the [.NET Aspire dashboard](../fundamentals/dashboard/overview.md) provides interactive prompts to help you configure the required values. The dashboard displays a message bar prompting you to **Enter values** for the missing configuration.

If you dismiss the prompt or there are validation errors, the dashboard provides detailed error information. For example, consider an AppHost that's missing the `SubscriptionId` configuration value that's attempting to use an Azure Key Vault resource. The **Resources** page indicates the **State** as **Missing subscription configuration**:

:::image type="content" source="media/resources-kv-missing-subscription.png" alt-text=".NET Aspire dashboard: Missing subscription configuration.":::

Additionally, the **Console logs** display this information as well, consider the following screenshot:

:::image type="content" source="media/console-logs-kv-missing-subscription.png" lightbox="media/console-logs-kv-missing-subscription.png" alt-text=".NET Aspire dashboard: Console logs, missing subscription configuration.":::

## Known limitations

After provisioning Azure resources in this way, you must manually clean up the resources in the Azure portal as .NET Aspire doesn't provide a built-in mechanism to delete Azure resources. The easiest way to achieve this is by deleting the configured resource group. This can be done in the [Azure portal](/azure/azure-resource-manager/management/delete-resource-group?tabs=azure-portal#delete-resource-group) or by using the Azure CLI:

```azurecli
az group delete --name <ResourceGroupName>
```

Replace `<ResourceGroupName>` with the name of the resource group you want to delete. For more information, see [az group delete](/cli/azure/group#az-group-delete).
