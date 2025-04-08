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
