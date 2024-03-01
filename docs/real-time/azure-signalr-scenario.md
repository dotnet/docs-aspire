---
title: .NET Aspire support for Azure SignalR Service
description: Learn how to use the Azure SignalR Service with .NET Aspire.
ms.topic: how-to
ms.date: 03/01/2024
---

# .NET Aspire support for Azure SignalR Service

In this article, you learn how to use .NET Aspire to express an Azure SignalR Service resource. Demonstrating how to write a SignalR app is beyond the scope of this article, instead you explore an app that's already been written and how it's wired up with .NET Aspire. Like other Azure resources within the .NET Aspire [app model](../fundamentals/app-host-overview.md#define-the-app-model), you benefit from simple provisioning and deployment with the Azure Developer CLI (AZD). For more information, see [Deploy a .NET Aspire app to Azure Container Apps using the AZD (in-depth guide)](../deployment/azure/aca-deployment-azd-in-depth.md).

## Hub host

Whichever project you're using to host your <xref:Microsoft.AspNetCore.SignalR.Hub> is where you'll wire up your Azure SignalR Service resource. The following example demonstrates how to use the `AddNamedAzureSignalR` extension method is chained on the <xref:Microsoft.Extensions.DependencyInjection.SignalRDependencyInjectionExtensions.AddSignalR> method:

:::code language="csharp" source="snippets/signalr/SignalR.ApiService/Program.cs":::

Calling `AddNamedAzureSignalR` adds Azure SignalR with the specified name, the connection string will be read from `ConnectionStrings_{name}`, the settings are loaded from `Azure:SignalR:{name}` section.

## App host

In the app host project, you express an `AzureSignalRResource` with the `AddAzureSignalR` method. The following example demonstrates how the resource is referenced by the consuming project, in this case the `Hub` host project:

:::code language="csharp" source="snippets/signalr/SignalR.AppHost/Program.cs":::

In the preceding code:

- The `AddAzureSignalR` method is called to express the `AzureSignalRResource`.
- The `signalr` resource is referenced by the `Hub` host project, in this case known as `apiService`.
- The `apiService` project resource is referenced by the `SignalR_Web` project.

## See also

- [Azure SignalR Service](/azure/azure-signalr/signalr-overview)
