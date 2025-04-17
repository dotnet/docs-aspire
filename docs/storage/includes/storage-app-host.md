---
ms.topic: include
---

The .NET Aspire [Azure Storage](/azure/storage/) hosting integration models the various storage resources as the following types:

- <xref:Aspire.Hosting.Azure.AzureStorageResource>: Represents an Azure Storage resource.
- <xref:Aspire.Hosting.Azure.AzureStorageEmulatorResource>: Represents an Azure Storage emulator resource (Azurite).
- <xref:Aspire.Hosting.Azure.AzureBlobStorageResource>: Represents an Azure Blob storage resource.
- <xref:Aspire.Hosting.Azure.AzureQueueStorageResource>: Represents an Azure Queue storage resource.
- <xref:Aspire.Hosting.Azure.AzureTableStorageResource>: Represents an Azure Table storage resource.

To access these types and APIs for expressing them, add the [📦 Aspire.Hosting.Azure.Storage](https://www.nuget.org/packages/Aspire.Hosting.Azure.Storage) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure.Storage
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure.Storage"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add Azure Storage resource

In your app host project, call <xref:Aspire.Hosting.AzureStorageExtensions.AddAzureStorage*> to add and return an Azure Storage resource builder.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage");

// An Azure Storage resource is required to add any of the following:
//
// - Azure Blob storage resource.
// - Azure Queue storage resource.
// - Azure Table storage resource.

// After adding all resources, run the app...
```

When you add an `AzureStorageResource` to the app host, it exposes other useful APIs to add Azure Blob, Queue, and Table storage resources. In other words, you must add an `AzureStorageResource` before adding any of the other storage resources.

> [!IMPORTANT]
> When you call <xref:Aspire.Hosting.AzureStorageExtensions.AddAzureStorage*>, it implicitly calls <xref:Aspire.Hosting.AzureProvisionerExtensions.AddAzureProvisioning*>—which adds support for generating Azure resources dynamically during app startup. The app must configure the appropriate subscription and location. For more information, see [Local provisioning: Configuration](../../azure/local-provisioning.md#configuration).

#### Provisioning-generated Bicep

If you're new to [Bicep](/azure/azure-resource-manager/bicep/overview), it's a domain-specific language for defining Azure resources. With .NET Aspire, you don't need to write Bicep by-hand, instead the provisioning APIs generate Bicep for you. When you publish your app, the generated Bicep is output alongside the manifest file. When you add an Azure Storage resource, the following Bicep is generated:

:::code language="bicep" source="../../snippets/azure/AppHost/storage.module.bicep":::

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

### Connect to an existing Azure Storage account

You might have an existing Azure Storage account that you want to connect to. Instead of representing a new Azure Storage resource, you can add a connection string to the app host. To add a connection to an existing Azure Storage account, call the <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var blobs = builder.AddConnectionString("blobs");

builder.AddProject<Projects.WebApplication>("web")
       .WithReference(blobs);

// After adding all resources, run the app...
```

[!INCLUDE [connection-strings-alert](../../includes/connection-strings-alert.md)]

The connection string is configured in the app host's configuration, typically under [User Secrets](/aspnet/core/security/app-secrets), under the `ConnectionStrings` section. The app host injects this connection string as an environment variable into all dependent resources, for example:

```json
{
    "ConnectionStrings": {
        "blobs": "https://{account_name}.blob.core.windows.net/"
    }
}
```

The dependent resource can access the injected connection string by calling the <xref:Microsoft.Extensions.Configuration.ConfigurationExtensions.GetConnectionString*> method, and passing the connection name as the parameter, in this case `"blobs"`. The `GetConnectionString` API is shorthand for `IConfiguration.GetSection("ConnectionStrings")[name]`.

### Add Azure Storage emulator resource

To add an Azure Storage emulator resource, chain a call on an `IResourceBuilder<AzureStorageResource>` to the <xref:Aspire.Hosting.AzureStorageExtensions.RunAsEmulator*> API:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage")
                     .RunAsEmulator();

// After adding all resources, run the app...
```

When you call `RunAsEmulator`, it configures your storage resources to run locally using an emulator. The emulator in this case is [Azurite](/azure/storage/common/storage-use-azurite). The Azurite open-source emulator provides a free local environment for testing your Azure Blob, Queue Storage, and Table Storage apps and it's a perfect companion to the .NET Aspire Azure hosting integration. Azurite isn't installed, instead, it's accessible to .NET Aspire as a container. When you add a container to the app host, as shown in the preceding example with the `mcr.microsoft.com/azure-storage/azurite` image, it creates and starts the container when the app host starts. For more information, see [Container resource lifecycle](../../fundamentals/orchestrate-resources.md#container-resource-lifecycle).

#### Configure Azurite container

There are various configurations available to container resources, for example, you can configure the container's ports, environment variables, it's [lifetime](../../fundamentals/orchestrate-resources.md#container-resource-lifetime), and more.

##### Configure Azurite container ports

By default, the Azurite container when configured by .NET Aspire, exposes the following endpoints:

| Endpoint | Container port | Host port |
|----------|----------------|-----------|
| `blob`   | 10000          | dynamic   |
| `queue`  | 10001          | dynamic   |
| `table`  | 10002          | dynamic   |

The port that they're listening on is dynamic by default. When the container starts, the ports are mapped to a random port on the host machine. To configure the endpoint ports, chain calls on the container resource builder provided by the `RunAsEmulator` method as shown in the following example:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage").RunAsEmulator(
                     azurite =>
                     {
                         azurite.WithBlobPort(27000)
                                .WithQueuePort(27001)
                                .WithTablePort(27002);
                     });

// After adding all resources, run the app...
```

The preceding code configures the Azurite container's existing `blob`, `queue`, and `table` endpoints to listen on ports `27000`, `27001`, and `27002`, respectively. The Azurite container's ports are mapped to the host ports as shown in the following table:

| Endpoint name | Port mapping (`container:host`) |
|--------------:|---------------------------------|
| `blob`        | `10000:27000`                   |
| `queue`       | `10001:27001`                   |
| `table`       | `10002:27002`                   |

##### Configure Azurite container with persistent lifetime

To configure the Azurite container with a persistent lifetime, call the <xref:Aspire.Hosting.ContainerResourceBuilderExtensions.WithLifetime*> method on the Azurite container resource and pass <xref:Aspire.Hosting.ApplicationModel.ContainerLifetime.Persistent?displayProperty=nameWithType>:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage").RunAsEmulator(
                     azurite =>
                     {
                         azurite.WithLifetime(ContainerLifetime.Persistent);
                     });

// After adding all resources, run the app...
```

For more information, see [Container resource lifetime](../../fundamentals/orchestrate-resources.md#container-resource-lifetime).

##### Configure Azurite container with data volume

To add a data volume to the Azure Storage emulator resource, call the <xref:Aspire.Hosting.AzureStorageExtensions.WithDataVolume*> method on the Azure Storage emulator resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage").RunAsEmulator(
                     azurite =>
                     {
                         azurite.WithDataVolume();
                     });

// After adding all resources, run the app...
```

The data volume is used to persist the Azurite data outside the lifecycle of its container. The data volume is mounted at the `/data` path in the Azurite container and when a `name` parameter isn't provided, the name is formatted as `.azurite/{resource name}`. For more information on data volumes and details on why they're preferred over [bind mounts](#configure-azurite-container-with-data-bind-mount), see [Docker docs: Volumes](https://docs.docker.com/engine/storage/volumes).

##### Configure Azurite container with data bind mount

To add a data bind mount to the Azure Storage emulator resource, call the <xref:Aspire.Hosting.AzureStorageExtensions.WithDataBindMount*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage").RunAsEmulator(
                     azurite =>
                     {
                         azurite.WithDataBindMount("../Azurite/Data");
                     });

// After adding all resources, run the app...
```

[!INCLUDE [data-bind-mount-vs-volumes](../../includes/data-bind-mount-vs-volumes.md)]

Data bind mounts rely on the host machine's filesystem to persist the Azurite data across container restarts. The data bind mount is mounted at the `../Azurite/Data` path on the host machine relative to the app host directory (<xref:Aspire.Hosting.IDistributedApplicationBuilder.AppHostDirectory?displayProperty=nameWithType>) in the Azurite container. For more information on data bind mounts, see [Docker docs: Bind mounts](https://docs.docker.com/engine/storage/bind-mounts).

### Connect to storage resources

When the .NET Aspire app host runs, the storage resources can be accessed by external tools, such as the [Azure Storage Explorer](https://azure.microsoft.com/features/storage-explorer/). If your storage resource is running locally using Azurite, it will automatically be picked up by the Azure Storage Explorer.

> [!NOTE]
> The Azure Storage Explorer discovers Azurite storage resources assuming the default ports are used. If you've [configured the Azurite container to use different ports](#configure-azurite-container-ports), you'll need to configure the Azure Storage Explorer to connect to the correct ports.

To connect to the storage resource from Azure Storage Explorer, follow these steps:

1. Run the .NET Aspire app host.
1. Open the Azure Storage Explorer.
1. View the **Explorer** pane.
1. Select the **Refresh all** link to refresh the list of storage accounts.
1. Expand the **Emulator & Attached** node.
1. Expand the **Storage Accounts** node.
1. You should see a storage account with your resource's name as a prefix:

    :::image type="content" source="../media/azure-storage-explorer.png" lightbox="../media/azure-storage-explorer.png" alt-text="Azure Storage Explorer: Azurite storage resource discovered.":::

You're free to explore the storage account and its contents using the Azure Storage Explorer. For more information on using the Azure Storage Explorer, see [Get started with Storage Explorer](/azure/storage/storage-explorer/vs-azure-tools-storage-manage-with-storage-explorer).
