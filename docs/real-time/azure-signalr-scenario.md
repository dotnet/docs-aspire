---
title: .NET Aspire support for Azure SignalR Service
description: Learn how to use the Azure SignalR Service with .NET Aspire.
ms.topic: how-to
ms.date: 04/18/2024
---

# .NET Aspire support for Azure SignalR Service

In this article, you learn how to use .NET Aspire to express an Azure SignalR Service resource. Demonstrating how to write a SignalR app is beyond the scope of this article. Instead, you explore an app that's already been written and how it's wired up with .NET Aspire. Like other Azure resources within the .NET Aspire [app model](../fundamentals/app-host-overview.md#define-the-app-model), you benefit from simple provisioning and deployment with the Azure Developer CLI (`azd`). For more information, see [Deploy a .NET Aspire app to Azure Container Apps using the `azd` (in-depth guide)](../deployment/azure/aca-deployment-azd-in-depth.md).

## Hub host

The hub host project is where you host your SignalR hub, the project that calls <xref:Microsoft.Extensions.DependencyInjection.SignalRDependencyInjectionExtensions.AddSignalR> and <xref:Microsoft.AspNetCore.SignalR.HubRouteBuilder.MapHub%2A> for example.

### Install the NuGet package

You need to install the [Microsoft.Azure.SignalR](https://www.nuget.org/packages/Microsoft.Azure.SignalR) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Microsoft.Azure.SignalR
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Microsoft.Azure.SignalR"
                  Version="[SelectVersion]" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Express the resource

Whichever project you're using to host your <xref:Microsoft.AspNetCore.SignalR.Hub> is where you'll wire up your Azure SignalR Service resource. The following example demonstrates how to use the `AddNamedAzureSignalR` extension method which is chained on the `AddSignalR` method:

:::code language="csharp" source="snippets/signalr/SignalR.ApiService/Program.cs" highlight="7-8,14":::

Calling `AddNamedAzureSignalR` adds Azure SignalR with the specified name, the connection string will be read from `ConnectionStrings_{name}`, the settings are loaded from `Azure:SignalR:{name}` section.

## App host

In the [app host project](../fundamentals/app-host-overview.md#app-host-project), you express an `AzureSignalRResource` with the `AddAzureSignalR` method. The following example demonstrates how the resource is referenced by the consuming project, in this case the `Hub` host project:

:::code language="csharp" source="snippets/signalr/SignalR.AppHost/Program.cs":::

In the preceding code:

- The `builder` has its execution context checked to see if it's in publish mode.
- When publishing the `AddAzureSignalR` method is called to express the `AzureSignalRResource`.
- When not publishing, the `AddConnectionString` method is called to express an `IResourceWithConnectionString` to an existing resource.
- The `signalr` resource is referenced by the `Hub` host project, in this case known as `apiService`.
- The `apiService` project resource is referenced by the `SignalR_Web` project.

## See also

- [Azure SignalR Service](/azure/azure-signalr/signalr-overview)
