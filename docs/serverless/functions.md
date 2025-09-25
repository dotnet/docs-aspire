---
title: .NET Aspire Azure Functions integration (Preview)
description: Learn how to integrate Azure Functions with .NET Aspire.
ms.date: 11/13/2024
zone_pivot_groups: dev-environment
---

# .NET Aspire Azure Functions integration (Preview)

[!INCLUDE [includes-hosting](../includes/includes-hosting.md)]

> [!IMPORTANT]
> The .NET Aspire Azure Functions integration is currently in preview and is subject to change.

[Azure Functions](/azure/azure-functions/functions-overview) is a serverless solution that allows you to write less code, maintain less infrastructure, and save on costs. The .NET Aspire Azure Functions integration enables you to develop, debug, and orchestrate an Azure Functions .NET project as part of the AppHost.

It's expected that you've installed the required Azure tooling:

:::zone pivot="visual-studio"

- [Configure Visual Studio for Azure development with .NET](/dotnet/azure/configure-visual-studio)

:::zone-end
:::zone pivot="vscode"

- [Configure Visual Studio Code for Azure development with .NET](/dotnet/azure/configure-vs-code)

:::zone-end
:::zone pivot="dotnet-cli"

- [Install the Azure Functions Core Tools](/azure/azure-functions/functions-run-local?tabs=isolated-process&pivots=programming-language-csharp#install-the-azure-functions-core-tools)

:::zone-end

## Supported scenarios

The .NET Aspire Azure Functions integration has several key supported scenarios. This section outlines the scenarios and provides details related to the implementation of each approach.

### Supported triggers

The following table lists the supported triggers for Azure Functions in the .NET Aspire integration:

| Trigger | Attribute | Details |
|--|--|--|
| Azure Event Hubs trigger | `EventHubTrigger` | [📦 Aspire.Hosting.Azure.EventHubs](https://www.nuget.org/packages/Aspire.Hosting.Azure.EventHubs) |
| Azure Service Bus trigger | `ServiceBusTrigger` | [📦 Aspire.Hosting.Azure.ServiceBus](https://www.nuget.org/packages/Aspire.Hosting.Azure.ServiceBus) |
| Azure Storage Blobs trigger | `BlobTrigger` | [📦 Aspire.Hosting.Azure.Storage](https://www.nuget.org/packages/Aspire.Hosting.Azure.Storage) |
| Azure Storage Queues trigger | `QueueTrigger` | [📦 Aspire.Hosting.Azure.Storage](https://www.nuget.org/packages/Aspire.Hosting.Azure.Storage) |
| Azure CosmosDB trigger | `CosmosDbTrigger` | [📦 Aspire.Hosting.Azure.CosmosDB](https://www.nuget.org/packages/Aspire.Hosting.Azure.CosmosDB) |
| HTTP trigger | `HttpTrigger` | Supported without any additional resource dependencies. |
| Timer trigger | `TimerTrigger` | Supported without any additional resource dependencies—relies on implicit host storage. |

> [!IMPORTANT]
> Other Azure Functions triggers and bindings aren't currently supported in the .NET Aspire Azure Functions integration.

### Deployment

Currently, deployment is supported only to containers on Azure Container Apps (ACA) using the SDK container publish function in `Microsoft.Azure.Functions.Worker.Sdk`. This deployment methodology doesn't currently support KEDA-based autoscaling.

#### Configure external HTTP endpoints

To make HTTP triggers publicly accessible, call the <xref:Aspire.Hosting.ResourceBuilderExtensions.WithExternalHttpEndpoints*> API on the <xref:Aspire.Hosting.Azure.AzureFunctionsProjectResource>. For more information, see [Add Azure Functions resource](#add-azure-functions-resource).

## Azure Function project constraints

The .NET Aspire Azure Functions integration has the following project constraints:

- You must target .NET 8.0 or later.
- You must use a .NET 9 SDK.
- It currently only supports .NET workers with the [isolated worker model](/azure/azure-functions/dotnet-isolated-process-guide).
- Requires the following NuGet packages:
  - [📦 Microsoft.Azure.Functions.Worker](https://www.nuget.org/packages/Microsoft.Azure.Functions.Worker): Use the `FunctionsApplicationBuilder`.
  - [📦 Microsoft.Azure.Functions.Worker.Sdk](https://www.nuget.org/packages/Microsoft.Azure.Functions.Worker.Sdk): Adds support for `dotnet run` and `azd publish`.
  - [📦 Microsoft.Azure.Functions.Http.AspNetCore](https://www.nuget.org/packages/Microsoft.Azure.Functions.Worker.Extensions.Http.AspNetCore): Adds HTTP trigger-supporting APIs.

:::zone pivot="visual-studio"

If you encounter issues with the Azure Functions project, such as:

> There is no Functions runtime available that matches the version specified in the project

In Visual Studio, try checking for an update on the Azure Functions tooling. Open the **Options** dialog, navigate to **Projects and Solutions**, and then select **Azure Functions**. Select the **Check for updates** button to ensure you have the latest version of the Azure Functions tooling:

:::image type="content" source="media/visual-studio-auzre-functions-options.png" alt-text="Visual Studio: Options / Projects and Solutions / Azure Functions.":::

:::zone-end

## Hosting integration

The Azure Functions hosting integration models an Azure Functions resource as the <xref:Aspire.Hosting.Azure.AzureFunctionsProjectResource> (subtype of <xref:Aspire.Hosting.ApplicationModel.ProjectResource>) type. To access this type and APIs that allow you to add it to your [AppHost](xref:dotnet/aspire/app-host) project install the [📦 Aspire.Hosting.Azure.Functions](https://www.nuget.org/packages/Aspire.Hosting.Azure.Functions) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure.Functions --prerelease
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure.Functions"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add Azure Functions resource

In your AppHost project, call <xref:Aspire.Hosting.AzureFunctionsProjectResourceExtensions.AddAzureFunctionsProject*> on the `builder` instance to add an Azure Functions resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var functions = builder.AddAzureFunctionsProject<Projects.ExampleFunctions>("functions")
                       .WithExternalHttpEndpoints();

builder.AddProject<Projects.ExampleProject>()
       .WithReference(functions)
       .WaitFor(functions);

// After adding all resources, run the app...
```

When .NET Aspire adds an Azure Functions project resource the AppHost, as shown in the preceding example, the `functions` resource can be referenced by other project resources. The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method configures a connection in the `ExampleProject` named `"functions"`. If the Azure Resource was deployed and it exposed an HTTP trigger, its endpoint would be external due to the call to <xref:Aspire.Hosting.ResourceBuilderExtensions.WithExternalHttpEndpoints*>. For more information, see [Reference resources](../fundamentals/app-host-overview.md#reference-resources).

### Add Azure Functions resource with host storage

If you want to modify the default host storage account that the Azure Functions host uses, call the <xref:Aspire.Hosting.AzureFunctionsProjectResourceExtensions.WithHostStorage*> method on the Azure Functions project resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage")
                     .RunAsEmulator();

var functions = builder.AddAzureFunctionsProject<Projects.ExampleFunctions>("functions")
                       .WithHostStorage(storage);

builder.AddProject<Projects.ExampleProject>()
       .WithReference(functions)
       .WaitFor(functions);

// After adding all resources, run the app...
```

The preceding code relies on the [📦 Aspire.Hosting.Azure.Storage](https://www.nuget.org/packages/Aspire.Hosting.Azure.Storage) NuGet package to add an Azure Storage resource that runs as an emulator. The `storage` resource is then passed to the `WithHostStorage` API, explicitly setting the host storage to the emulated resource.

> [!NOTE]
> If you're not using the implicit host storage, you must manually assign the `StorageAccountContributor` role to your resource for deployed instances. The implicit host storage is automatically configured with the following roles to support maximum interoperability with all scenarios:
>
> - <xref:Azure.Provisioning.Storage.StorageBuiltInRole.StorageBlobDataContributor?displayProperty=nameWithType>
> - <xref:Azure.Provisioning.Storage.StorageBuiltInRole.StorageTableDataContributor?displayProperty=nameWithType>
> - <xref:Azure.Provisioning.Storage.StorageBuiltInRole.StorageQueueDataContributor?displayProperty=nameWithType>
> - <xref:Azure.Provisioning.Storage.StorageBuiltInRole.StorageAccountContributor?displayProperty=nameWithType>
>
> For production scenarios, it is recommended to register the storage account explicitly with the `WithHostStorage` and `WithRoleAssignment` APIs and register a more tailored set of roles.
>
> ```csharp
> var builder = DistributedApplication.CreateBuilder(args);
> 
> var storage = builder.AddAzureStorage("storage");
> 
> builder.AddAzureFunctionsProject<Projects.ExampleFunctions>("functions")
>        .WithHostStorage(storage)
>        .WithRoleAssignments(storage, StorageBuiltInRole.StorageBlobDataReader,
>                                     StorageBuiltInRole.StorageQueueDataReader);
> ```

### Reference resources in Azure Functions

To reference other Azure resources in an Azure Functions project, chain a call to `WithReference` on the Azure Functions project resource and provide the resource to reference:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var storage = builder.AddAzureStorage("storage").RunAsEmulator();
var blobs = storage.AddBlobs("blobs");

builder.AddAzureFunctionsProject<Projects.ExampleFunctions>("functions")
       .WithHostStorage(storage)
       .WithReference(blobs);

builder.Build().Run();
```

The preceding code adds an Azure Storage resource to the AppHost and references it in the Azure Functions project. The `blobs` resource is added to the `storage` resource and then referenced by the `functions` resource. The connection information required to connect to the `blobs` resource is automatically injected into the Azure Functions project and enables the project to define a `BlobTrigger` that relies on `blobs` resource.

## See also

- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
- [Azure Functions documentation](/azure/azure-functions/functions-overview)
- [.NET Aspire and Functions image gallery sample](/samples/dotnet/aspire-samples/aspire-azure-functions-with-blob-triggers)
