---
ms.topic: include
---

### Provisioning-generated Bicep

If you're new to [Bicep](/azure/azure-resource-manager/bicep/overview), it's a domain-specific language for defining Azure resources. With .NET Aspire, you don't need to write Bicep by-hand, instead the provisioning APIs generate Bicep for you. When you publish your app, the generated Bicep is output alongside the manifest file. When you add an Azure Storage resource, the following Bicep is generated:

:::code language="bicep" source="../../snippets/azure/AppHost/storage/storage.bicep":::

The preceding Bicep is a module that provisions an Azure Storage account with the following defaults:

- `kind`: The kind of storage account. The default is `StorageV2`.
- `sku`: The SKU of the storage account. The default is `Standard_GRS`.
- `properties`: The properties of the storage account:
  - `accessTier`: The access tier of the storage account. The default is `Hot`.
  - `allowSharedKeyAccess`: A boolean value that indicates whether the storage account permits requests to be authorized with the account access key. The default is `false`.
  - `minimumTlsVersion`: The minimum supported TLS version for the storage account. The default is `TLS1_2`.
  - `networkAcls`: The network ACLs for the storage account. The default is `{ defaultAction: 'Allow' }`.

In addition to the storage account, it also provisions a blob container.

The following role assignments are added to the storage account to grant your application access. See the [built-in Azure role-based access control (Azure RBAC) roles](/azure/role-based-access-control/built-in-roles#storage) for more information:

| Role / ID | Description |
|------|-------------|
| Storage Blob Data Contributor<br/>`ba92f5b4-2d11-453d-a403-e96b0029c9fe` | Read, write, and delete Azure Storage containers and blobs. |
| Storage Table Data Contributor<br/>`0a9a7e1f-b9d0-4cc4-a60d-0319b160aaa3` | Read, write, and delete Azure Storage tables and entities. |
| Storage Queue Data Contributor<br/>`974c5e8b-45b9-4653-ba55-5f855dd0fb88` | Read, write, and delete Azure Storage queues and queue messages. |

Additionally, role assignments are created for the Azure resource in a separate module:

:::code language="bicep" source="../../snippets/azure/AppHost/storage-roles/storage-roles.bicep":::

The generated Bicep is a starting point and is influenced by changes to the provisioning infrastructure in C#. Customizations to the Bicep file directly will be overwritten, so make changes through the C# provisioning APIs to ensure they are reflected in the generated files.

#### Customize provisioning infrastructure

All .NET Aspire Azure resources are subclasses of the <xref:Aspire.Hosting.Azure.AzureProvisioningResource> type. This type enables the customization of the generated Bicep by providing a fluent API to configure the Azure resources—using the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure``1(Aspire.Hosting.ApplicationModel.IResourceBuilder{``0},System.Action{Aspire.Hosting.Azure.AzureResourceInfrastructure})> API. For example, you can configure the `kind`, `sku`, `properties`, and more. The following example demonstrates how to customize the Azure Storage resource:

:::code language="csharp" source="../../snippets/azure/AppHost/Program.ConfigureStorageInfra.cs" id="configure":::

The preceding code:

- Chains a call to the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure*> API:
  - The `infra` parameter is an instance of the <xref:Aspire.Hosting.Azure.AzureResourceInfrastructure> type.
  - The provisionable resources are retrieved by calling the <xref:Azure.Provisioning.Infrastructure.GetProvisionableResources> method.
  - The single <xref:Azure.Provisioning.Storage.StorageAccount> is retrieved.
  - The <xref:Azure.Provisioning.Storage.StorageAccount.AccessTier?displayProperty=nameWithType> is assigned to <xref:Azure.Provisioning.Storage.StorageAccountAccessTier.Cool?displayProperty=nameWithType>.
  - The <xref:Azure.Provisioning.Storage.StorageAccount.Sku?displayProperty=nameWithType> is assigned to a new <xref:Azure.Provisioning.Storage.StorageSku> with a `Name` of <xref:Azure.Provisioning.Storage.StorageSkuName.PremiumZrs>.
  - A tag is added to the storage account with a key of `ExampleKey` and a value of `Example value`.

There are many more configuration options available to customize the Azure Storage resource. For more information, see <xref:Azure.Provisioning.Storage>.

<!-- TODO: Add link to generic doc covering configuring infra -->
