---
ms.topic: include
---

The [Azure Storage](/azure/storage/) hosting integration models the various storage resources as the following types:

- <xref:Aspire.Hosting.Azure.AzureStorageResource>
- <xref:Aspire.Hosting.Azure.AzureStorageEmulatorResource>
- <xref:Aspire.Hosting.Azure.AzureBlobStorageResource>
- <xref:Aspire.Hosting.Azure.AzureQueueStorageResource>
- <xref:Aspire.Hosting.Azure.AzureTableStorageResource>

To access these types and APIs, add the [📦 Aspire.Hosting.Azure.Storage](https://www.nuget.org/packages/Aspire.Hosting.Azure.Storage) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

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
// - Azure Blob storage resource.
// - Azure Queue storage resource.
// - Azure Table storage resource

// After adding all resources, run the app...
```

When you add an `AzureStorageResource` to the app host, it exposes other useful APIS to add Azure Blob, Queue, and Table storage resources. In other words, you must add an `AzureStorageResource` before adding any of the other storage resources.

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
