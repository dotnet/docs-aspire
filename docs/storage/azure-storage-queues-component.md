---
title: .NET Aspire Azure Queue Storage component
description: This article describes the .NET Aspire Azure Queue Storage component features and capabilities
ms.topic: how-to
ms.date: 04/24/2024
---

# .NET Aspire Azure Queue Storage component

In this article, you learn how to use the .NET Aspire Azure Queue Storage component. The `Aspire.Azure.Storage.Queues` library is used to register a <xref:Azure.Storage.Queues.QueueServiceClient> in the DI container for connecting to Azure Queue Storage. It also enables corresponding health checks, logging and telemetry.

## Get started

To get started with the .NET Aspire Azure Queue Storage component, install the [Aspire.Azure.Storage.Queues](https://www.nuget.org/packages/Aspire.Azure.Storage.Queues) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Azure.Storage.Queues --prerelease
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Azure.Storage.Queues"
                  Version="[SelectVersion]" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

In the _Program.cs_ file of your component-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireQueueStorageExtensions.AddAzureQueueClient%2A> extension to register a `QueueServiceClient` for use via the dependency injection container.

```csharp
builder.AddAzureQueueClient("queue");
```

You can then retrieve the `QueueServiceClient` instance using dependency injection. For example, to retrieve the client from an example service:

```csharp
public class ExampleService(QueueServiceClient client)
{
    // Use client...
}
```

## App host usage

To add Azure Storage hosting support to your <xref:Aspire.Hosting.IDistributedApplicationBuilder>, install the [Aspire.Hosting.Azure.Storage](https://www.nuget.org/packages/Aspire.Hosting.Azure.Storage) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure.Storage --prerelease
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure.Storage"
                  Version="[SelectVersion]" />
```

---

In your app host project, add a Storage Queue connection and consume the connection using the following methods, such as <xref:Aspire.Hosting.AzureStorageExtensions.AddAzureStorage%2A>:

```csharp
var queues = builder.AddAzureStorage("storage")
                    .AddQueues("queues");

var exampleProject = builder.AddProject<Projects.ExampleProject>()
                            .WithReference(queues);
```

The <xref:Aspire.Hosting.AzureStorageExtensions.AddQueues%2A> method will read connection information from the AppHost's configuration (for example, from "user secrets") under the `ConnectionStrings:queue` config key. The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method passes that connection information into a connection string named queue in the `ExampleProject` project. In the _Program.cs_ file of `ExampleProject`, the connection can be consumed using:

```csharp
builder.AddAzureQueueClient("queue");
```

### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddAzureQueueClient`:

```csharp
builder.AddAzureQueueClient("queueConnectionName");
```

And then the connection string will be retrieved from the `ConnectionStrings` configuration section. Two connection formats are supported:

#### Service URI

The recommended approach is to use a `ServiceUri`, which works with the <xref:Aspire.Azure.Storage.Queues.AzureStorageQueuesSettings.Credential?displayProperty=nameWithType> property to establish a connection. If no credential is configured, the <xref:Azure.Identity.DefaultAzureCredential?displayProperty=fullName> is used.

```json
{
  "ConnectionStrings": {
    "queueConnectionName": "https://{account_name}.queue.core.windows.net/"
  }
}
```

#### Connection string

Alternatively, an [Azure Storage connection string](/azure/storage/common/storage-configure-connection-string) can be used.

```json
{
  "ConnectionStrings": {
    "queueConnectionName": "AccountName=myaccount;AccountKey=myaccountkey"
  }
}
```

## Configuration

The .NET Aspire Azure Queue Storage component provides multiple options to configure the `QueueServiceClient` based on the requirements and conventions of your project.

### Use configuration providers

The .NET Aspire Azure Queue Storage component supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the <xref:Aspire.Azure.Storage.Queues.AzureStorageQueuesSettings> and <xref:Azure.Storage.Queues.QueueClientOptions> from configuration by using the `Aspire:Azure:Storage:Queues` key. Example _appsettings.json_ that configures some of the options:

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

### Use inline delegates

You can also pass the `Action<AzureStorageQueuesSettings> configureSettings` delegate to set up some or all the options inline, for example to disable the health check:

```csharp
builder.AddAzureQueueClient(
    "queue",
    static settings => settings.DisableHealthChecks  = true);
```

You can also set up the `QueueClientOptions` using `Action<IAzureClientBuilder<QueueServiceClient, QueueClientOptions>> configureClientBuilder` delegate, the second parameter of the <xref:Microsoft.Extensions.Hosting.AspireQueueStorageExtensions.AddAzureQueueClient%2A> method. For example, to set the first part of user-agent headers for all requests issues by this client:

```csharp
builder.AddAzureQueueClient(
    "queue",
    configureClientBuilder:
        static clientBuilder => clientBuilder.ConfigureOptions(
            static options =>
                options.Diagnostics.ApplicationId = "myapp"));
```

[!INCLUDE [component-health-checks](../includes/component-health-checks.md)]

The .NET Aspire Azure Queue Storage component handles the following:

- Adds the `AzureQueueStorageHealthCheck` health check, which attempts to connect to and query the storage queue
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic

[!INCLUDE [component-observability-and-telemetry](../includes/component-observability-and-telemetry.md)]

### Logging

The .NET Aspire Azure Queue Storage component uses the following log categories:

- `Azure.Core`
- `Azure.Identity`

### Tracing

The .NET Aspire Azure Queue Storage component will emit the following tracing activities using OpenTelemetry:

- "Azure.Storage.Queues.QueueClient"

### Metrics

The .NET Aspire Azure Queue Storage component currently does not support metrics by default due to limitations with the Azure SDK.

## See also

- [Azure Queues Storage docs](/azure/storage/queues/)
- [.NET Aspire components](../fundamentals/components-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
