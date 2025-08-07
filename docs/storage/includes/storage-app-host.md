---
ms.topic: include
ms.custom: sfi-ropc-nochange
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

In your AppHost project, call <xref:Aspire.Hosting.AzureStorageExtensions.AddAzureStorage*> to add and return an Azure Storage resource builder.

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

[!INCLUDE [storage-bicep](storage-bicep.md)]

### Connect to an existing Azure Storage account

You might have an existing Azure Storage account that you want to connect to. You can chain a call to annotate that your <xref:Aspire.Hosting.Azure.AzureStorageResource> is an existing resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var existingStorageName = builder.AddParameter("existingStorageName");
var existingStorageResourceGroup = builder.AddParameter("existingStorageResourceGroup");

var storageaccount = builder.AddAzureStorage("storage")
                    .AsExisting(existingStorageName, existingStorageResourceGroup)
                    .AddBlobs("blobs");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(storageaccount);

// After adding all resources, run the app...
```

[!INCLUDE [azure-configuration](../../azure/includes/azure-configuration.md)]

For more information on treating Azure Storage resources as existing resources, see [Use existing Azure resources](../../azure/integrations-overview.md#use-existing-azure-resources).

> [!NOTE]
> Alternatively, instead of representing an Azure Storage account resource, you can add a connection string to the app host. This approach is weakly-typed, and doesn't work with role assignments or infrastructure customizations. For more information, see [Add existing Azure resources with connection strings](../../azure/integrations-overview.md#add-existing-azure-resources-with-connection-strings).

### Add Azure Storage emulator resource

To add an Azure Storage emulator resource, chain a call on an `IResourceBuilder<AzureStorageResource>` to the <xref:Aspire.Hosting.AzureStorageExtensions.RunAsEmulator*> API:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage")
                     .RunAsEmulator();

// After adding all resources, run the app...
```

When you call `RunAsEmulator`, it configures your storage resources to run locally using an emulator. The emulator in this case is [Azurite](/azure/storage/common/storage-use-azurite). The Azurite open-source emulator provides a free local environment for testing your Azure Blob, Queue Storage, and Table Storage apps and it's a perfect companion to the .NET Aspire Azure hosting integration. Azurite isn't installed, instead, it's accessible to .NET Aspire as a container. When you add a container to the app host, as shown in the preceding example with the `mcr.microsoft.com/azure-storage/azurite` image, it creates and starts the container when the AppHost starts. For more information, see [Container resource lifecycle](../../fundamentals/orchestrate-resources.md#container-resource-lifecycle).

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

Data bind mounts rely on the host machine's filesystem to persist the Azurite data across container restarts. The data bind mount is mounted at the `../Azurite/Data` path on the host machine relative to the AppHost directory (<xref:Aspire.Hosting.IDistributedApplicationBuilder.AppHostDirectory?displayProperty=nameWithType>) in the Azurite container. For more information on data bind mounts, see [Docker docs: Bind mounts](https://docs.docker.com/engine/storage/bind-mounts).

### Connect to storage resources

When the .NET Aspire AppHost runs, the storage resources can be accessed by external tools, such as the [Azure Storage Explorer](https://azure.microsoft.com/features/storage-explorer/). If your storage resource is running locally using Azurite, it will automatically be picked up by the Azure Storage Explorer.

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
