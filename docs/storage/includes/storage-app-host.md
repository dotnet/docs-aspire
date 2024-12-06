---
ms.topic: include
---

The .NET Aspire [Azure Storage](/azure/storage/) hosting integration models the various storage resources as the following types:

- <xref:Aspire.Hosting.Azure.AzureStorageResource>
- <xref:Aspire.Hosting.Azure.AzureStorageEmulatorResource>
- <xref:Aspire.Hosting.Azure.AzureBlobStorageResource>
- <xref:Aspire.Hosting.Azure.AzureQueueStorageResource>
- <xref:Aspire.Hosting.Azure.AzureTableStorageResource>

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

When you add an `AzureStorageResource` to the app host, it exposes other useful APIS to add Azure Blob, Queue, and Table storage resources. In other words, you must add an `AzureStorageResource` before adding any of the other storage resources.

> [!IMPORTANT]
> When you call `AddAzureStorage`, it implicitly calls <xref:Aspire.Hosting.AzureProvisionerExtensions.AddAzureProvisioning*>—which adds support for generating Azure resources dynamically during app startup. The app must configure the appropriate subscription and location.

#### Generated provisioning Bicep

If you're new to [Bicep](/azure/azure-resource-manager/bicep/overview), it's a domain-specific language for defining Azure resources. With .NET Aspire, you don't need to write Bicep by-hand, instead the provisioning APIs generate Bicep for you. When you publish your app, the generated Bicep is output alongside the manifest file.

When you add an Azure Storage resource, the following Bicep is generated:

<!-- markdownlint-disable MD033 -->
<br/>
<details>
<summary id="storage-bicep"><strong>Expand the Azure Storage bicep.</strong></summary>
<p aria-labelledby="storage-json">

:::code language="bicep" source="../../snippets/azure/AppHost/storage.module.bicep":::

</p>

</details>
<!-- markdownlint-enable MD033 -->

For more information, see [.NET Aspire manifest format for deployment tool builders](../../deployment/manifest-format.md).

### Add Azure Storage emulator resource

To add an Azure Storage emulator resource, chain a call on an `IResourceBuilder<AzureStorageResource>` to the <xref:Aspire.Hosting.AzureStorageExtensions.RunAsEmulator*> API:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage")
                     .RunAsEmulator();

// After adding all resources, run the app...
```

When you call `RunAsEmulator`, it configures your storage resources to run locally using an emulator. The emulator in this case is [Azurite](/azure/storage/common/storage-use-azurite). The Azurite open-source emulator provides a free local environment for testing your Azure Blob, Queue Storage, and Table Storage apps and it's a perfect companion to the .NET Aspire Azure hosting integration. Azurite is not installed, instead it's delivered as a container.

When .NET Aspire adds a container to the app host, as shown in the preceding example with the `mcr.microsoft.com/azure-storage/azurite` image, it creates a new Azurite instance on your local machine.

### Add Azure Storage resource with data volume

To add a data volume to the Azure Storage resource, call the <xref:Aspire.Hosting.AzureStorageExtensions.WithDataVolume*> method on the Azure Storage emulator resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage").RunAsEmulator(
                     static azurite =>
                     {
                         azurite.WithDataVolume();
                     });

// After adding all resources, run the app...
```

The data volume is used to persist the Azurite data outside the lifecycle of its container. The data volume is mounted at the `/data` path in the Azurite container and when a `name` parameter isn't provided, the name is formatted as `.azurite/{resource name}`. For more information on data volumes and details on why they're preferred over [bind mounts](#add-azure-storage-resource-with-data-bind-mount), see [Docker docs: Volumes](https://docs.docker.com/engine/storage/volumes).

### Add Azure Storage resource with data bind mount

To add a data bind mount to the Azure Storage resource, call the <xref:Aspire.Hosting.AzureStorageExtensions.WithDataBindMount*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage").RunAsEmulator(
                     static azurite =>
                     {
                         azurite.WithDataBindMount(@"C:\Azurite\Data");
                     });

// After adding all resources, run the app...
```

[!INCLUDE [data-bind-mount-vs-volumes](../../includes/data-bind-mount-vs-volumes.md)]

Data bind mounts rely on the host machine's filesystem to persist the Azurite data across container restarts. The data bind mount is mounted at the `C:\Azurite\Data` on Windows (or `/Azurite/Data` on Unix) path on the host machine in the Azurite container. For more information on data bind mounts, see [Docker docs: Bind mounts](https://docs.docker.com/engine/storage/bind-mounts).
