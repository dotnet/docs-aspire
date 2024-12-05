---
title: .NET Aspire Azure Blob Storage integration
description: This article describes the .NET Aspire Azure Blob Storage integration features and capabilities.
ms.topic: how-to
ms.date: 12/05/2024
uid: storage/azure-blob-storage-integration
---

# .NET Aspire Azure Blob Storage integration

[Azure Blob Storage](https://azure.microsoft.com/services/storage/blobs/) is a service for storing large amounts of unstructured data. The .NET Aspire Azure Blob Storage integration enables you to connect to existing Azure Blob Storage instances or create new instances from .NET applications.

## Hosting integration

[!INCLUDE [storage-app-host](includes/storage-app-host.md)]

### Add Azure Blob Storage resource

In your app host project, register the Azure Blob Storage integration and consume the service using the following methods, such as <xref:Aspire.Hosting.AzureStorageExtensions.AddAzureStorage%2A>:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var blobs = builder.AddAzureStorage("storage")
                   .RunAsEmulator();
                   .AddBlobs("blobs");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(blobs)
       .WaitFor(blobs);

// After adding all resources, run the app...
```

The preceding code:

- Adds an Azure Storage resource named `storage`.
- Chains a call to <xref:Aspire.Hosting.AzureStorageExtensions.RunAsEmulator*> to configure the storage resource to run locally using an emulator. The emulator in this case is [Azurite](/azure/storage/common/storage-use-azurite).
- Adds a blob container named `blobs` to the storage resource.
- Adds the `storage` resource to the `ExampleProject` and waits for it to be ready before starting the project.

[!INCLUDE [storage-hosting-health-checks](includes/storage-hosting-health-checks.md)]

## Client integration

To get started with the .NET Aspire Azure Blob Storage client integration, install the [📦 Aspire.Azure.Storage.Blobs](https://www.nuget.org/packages/Aspire.Azure.Storage.Blobs) NuGet package in the client-consuming project, that is, the project for the application that uses the Azure Blob Storage client. The Azure Blob Storage client integration registers a <xref:Azure.Storage.Blobs.BlobServiceClient> instance that you can use to interact with Azure Blob Storage.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Azure.Storage.Blobs
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Azure.Storage.Blobs"
                  Version="*" />
```

---

### Add Azure Blob Storage client

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireBlobStorageExtensions.AddAzureBlobClient%2A> extension method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register a `BlobServiceClient` for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddAzureBlobClient("blobs");
```

You can then retrieve the `BlobServiceClient` instance using dependency injection. For example, to retrieve the client from a service:

```csharp
public class ExampleService(BlobServiceClient client)
{
    // Use client...
}
```

### Configuration

The .NET Aspire Azure Blob Storage integration provides multiple options to configure the `BlobServiceClient` based on the requirements and conventions of your project.

#### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddAzureBlobClient`:

```csharp
builder.AddAzureBlobClient("blobs");
```

And then the connection string will be retrieved from the `ConnectionStrings` configuration section. Two connection formats are supported:

##### Service URI

The recommended approach is to use a `ServiceUri`, which works with the <xref:Aspire.Azure.Storage.Blobs.AzureStorageBlobsSettings.Credential?displayProperty=nameWithType> property to establish a connection. If no credential is configured, the <xref:Azure.Identity.DefaultAzureCredential?displayProperty=fullName> is used.

```json
{
  "ConnectionStrings": {
    "blobsConnectionName": "https://{account_name}.blob.core.windows.net/"
  }
}
```

##### Connection string

Alternatively, an [Azure Storage connection string](/azure/storage/common/storage-configure-connection-string) can be used.

```json
{
  "ConnectionStrings": {
    "blobsConnectionName": "AccountName=myaccount;AccountKey=myaccountkey"
  }
}
```

#### Use configuration providers

The .NET Aspire Azure Blob Storage integration supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the <xref:Aspire.Azure.Storage.Blobs.AzureStorageBlobsSettings> and <xref:Azure.Storage.Blobs.BlobClientOptions> from configuration by using the `Aspire:Azure:Storage:Blobs` key. Example _:::no-loc text="appsettings.json":::_ that configures some of the options:

```json
{
  "Aspire": {
    "Azure": {
      "Storage": {
        "Blobs": {
          "DisableHealthChecks": true,
          "DisableTracing": false,
          "ClientOptions": {
            "Diagnostics": {
              "ApplicationId": "myapp"
            }
          }
        }
      }
    }
  }
}
```

#### Use inline delegates

You can also pass the `Action<AzureStorageBlobsSettings> configureSettings` delegate to set up some or all the options inline, for example to configure health checks:

```csharp
builder.AddAzureBlobClient(
    "blobs",
    static settings => settings.DisableHealthChecks  = true);
```

You can also set up the `BlobClientOptions` using `Action<IAzureClientBuilder<BlobServiceClient, BlobClientOptions>> configureClientBuilder` delegate, the second parameter of the `AddAzureBlobClient` method. For example, to set the first part of user-agent headers for all requests issues by this client:

```csharp
builder.AddAzureBlobClient(
    "blobs",
    static configureClientBuilder: clientBuilder =>
        clientBuilder.ConfigureOptions(
            static options => options.Diagnostics.ApplicationId = "myapp"));
```

### Client integration health checks

By default, .NET Aspire integrations enable [health checks](../fundamentals/health-checks.md) for all services. For more information, see [.NET Aspire integrations overview](../fundamentals/integrations-overview.md).

The .NET Aspire Azure Blob Storage integration:

- Adds the health check when <xref:Aspire.Azure.Storage.Blobs.AzureStorageBlobsSettings.DisableHealthChecks?displayProperty=nameWithType> is `false`, which attempts to connect to the Azure Blob Storage.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic.

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

### Logging

The .NET Aspire Azure Blob Storage integration uses the following log categories:

- `Azure.Core`
- `Azure.Identity`

### Tracing

The .NET Aspire Azure Blob Storage integration will emit the following tracing activities using OpenTelemetry:

- `Azure.Storage.Blobs.BlobContainerClient`

### Metrics

The .NET Aspire Azure Blob Storage integration currently does not support metrics by default due to limitations with the Azure SDK.

## See also

- [Azure Blob Storage docs](/azure/storage/blobs/)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)