---
title: .NET Aspire Azure Queue Storage integration
description: This article describes the .NET Aspire Azure Queue Storage integration features and capabilities.
ms.date: 05/09/2025
uid: storage/azure-queue-storage-integration
---

# .NET Aspire Azure Queue Storage integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[Azure Queue Storage](https://azure.microsoft.com/services/storage/queues/) is a service for storing large numbers of messages that can be accessed from anywhere in the world via authenticated calls. The .NET Aspire Azure Queue Storage integration enables you to connect to existing Azure Queue Storage instances or create new instances from .NET applications.

## Hosting integration

[!INCLUDE [storage-app-host](includes/storage-app-host.md)]

### Add Azure Queue Storage resource

In your app host project, register the Azure Queue Storage integration by chaining a call to <xref:Aspire.Hosting.AzureStorageExtensions.AddQueues*> on the `IResourceBuilder<IAzureStorageResource>` instance returned by <xref:Aspire.Hosting.AzureStorageExtensions.AddAzureStorage*>. The following example demonstrates how to add an Azure Queue Storage resource named `storage` and a queue resource named `queues`:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var queues = builder.AddAzureStorage("storage")
                    .AddQueues("queues");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(queues);

// After adding all resources, run the app...
```

The preceding code:

- Adds an Azure Storage resource named `storage`.
- Adds a queue named `queues` to the storage resource.
- Adds the `storage` resource to the `ExampleProject` and waits for it to be ready before starting the project.

[!INCLUDE [storage-bicep](includes/storage-bicep.md)]

[!INCLUDE [storage-emulator](includes/storage-emulator.md)]

[!INCLUDE [storage-connections](includes/storage-connections.md)]

[!INCLUDE [storage-hosting-health-checks](includes/storage-hosting-health-checks.md)]

## Client integration

To get started with the .NET Aspire Azure Queue Storage client integration, install the [📦 Aspire.Azure.Storage.Queues](https://www.nuget.org/packages/Aspire.Azure.Storage.Queues) NuGet package in the client-consuming project, that is, the project for the application that uses the Azure Queue Storage client. The Azure Queue Storage client integration registers a <xref:Azure.Storage.Queues.QueueServiceClient> instance that you can use to interact with Azure Queue Storage.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Azure.Storage.Queues
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Azure.Storage.Queues"
                  Version="*" />
```

---

### Add Azure Queue Storage client

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireQueueStorageExtensions.AddAzureQueueClient%2A> extension method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register a `QueueServiceClient` for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddAzureQueueClient("queue");
```

You can then retrieve the `QueueServiceClient` instance using dependency injection. For example, to retrieve the client from a service:

```csharp
public class ExampleService(QueueServiceClient client)
{
    // Use client...
}
```

### Configuration

The .NET Aspire Azure Queue Storage integration provides multiple options to configure the `QueueServiceClient` based on the requirements and conventions of your project.

#### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling <xref:Microsoft.Extensions.Hosting.AspireQueueStorageExtensions.AddAzureQueueClient*>:

```csharp
builder.AddAzureQueueClient("queue");
```

Then the connection string is retrieved from the `ConnectionStrings` configuration section, and two connection formats are supported:

##### Service URI

The recommended approach is to use a `ServiceUri`, which works with the <xref:Aspire.Azure.Storage.Queues.AzureStorageQueuesSettings.Credential?displayProperty=nameWithType> property to establish a connection. If no credential is configured, the <xref:Azure.Identity.DefaultAzureCredential?displayProperty=fullName> is used.

```json
{
  "ConnectionStrings": {
    "queue": "https://{account_name}.queue.core.windows.net/"
  }
}
```

##### Connection string

Alternatively, an [Azure Storage connection string](/azure/storage/common/storage-configure-connection-string) can be used.

```json
{
  "ConnectionStrings": {
    "queue": "AccountName=myaccount;AccountKey=myaccountkey"
  }
}
```

For more information, see [Configure Azure Storage connection strings](/azure/storage/common/storage-configure-connection-string).

#### Use configuration providers

The .NET Aspire Azure Queue Storage integration supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the <xref:Aspire.Azure.Storage.Queues.AzureStorageQueuesSettings> and <xref:Azure.Storage.Queues.QueueClientOptions> from configuration by using the `Aspire:Azure:Storage:Queues` key. The following snippet is an example of a _:::no-loc text="appsettings.json":::_ file that configures some of the options:

```json
{
  "Aspire": {
    "Azure": {
      "Storage": {
        "Queues": {
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

For the complete Azure Storage Queues client integration JSON schema, see [Aspire.Azure.Data.Queues/ConfigurationSchema.json](https://github.com/dotnet/aspire/blob/e3d170c14198caf53e62818e1f71a0526449c585/src/Components/Aspire.Azure.Storage.Queues/ConfigurationSchema.json).

#### Use inline delegates

You can also pass the `Action<AzureStorageQueuesSettings> configureSettings` delegate to set up some or all the options inline, for example to configure health checks:

```csharp
builder.AddAzureQueueClient(
    "queue",
    settings => settings.DisableHealthChecks = true);
```

You can also set up the <xref:Azure.Storage.Queues.QueueClientOptions> using `Action<IAzureClientBuilder<QueueServiceClient, QueueClientOptions>> configureClientBuilder` delegate, the second parameter of the `AddAzureQueueClient` method. For example, to set the first part of user-agent headers for all requests issues by this client:

```csharp
builder.AddAzureQueueClient(
    "queue",
    configureClientBuilder: clientBuilder =>
        clientBuilder.ConfigureOptions(
            options => options.Diagnostics.ApplicationId = "myapp"));
```

### Client integration health checks

By default, .NET Aspire integrations enable [health checks](../fundamentals/health-checks.md) for all services. For more information, see [.NET Aspire integrations overview](../fundamentals/integrations-overview.md).

The .NET Aspire Azure Queue Storage integration:

- Adds the health check when <xref:Aspire.Azure.Storage.Queues.AzureStorageQueuesSettings.DisableHealthChecks?displayProperty=nameWithType> is `false`, which attempts to connect to the Azure Queue Storage.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic.

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

### Logging

The .NET Aspire Azure Queue Storage integration uses the following log categories:

- `Azure.Core`
- `Azure.Identity`

### Tracing

The .NET Aspire Azure Queue Storage integration emits the following tracing activities using OpenTelemetry:

- `Azure.Storage.Queues.QueueClient`

### Metrics

The .NET Aspire Azure Queue Storage integration currently doesn't support metrics by default due to limitations with the Azure SDK.

## See also

- [Azure Queue Storage docs](/azure/storage/queues/)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
