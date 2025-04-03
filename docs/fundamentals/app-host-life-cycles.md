---
title: App Host life cycles in .NET Aspire
description: Learn how to create hooks that respond to .NET Aspire life cycles and run custom code.
ms.date: 04/05/2025
ms.topic: article
uid: dotnet/aspire/app-host-life-cycles
---

# App Host life cycles in .NET Aspire

The .NET Aspire app host exposes several life cycles that you can hook into by implementing the <xref:Aspire.Hosting.Lifecycle.IDistributedApplicationLifecycleHook> interface. The following lifecycle methods are available:

| Order | Method | Description |
|--|--|--|
| **1** | <xref:Aspire.Hosting.Lifecycle.IDistributedApplicationLifecycleHook.BeforeStartAsync%2A> | Executes before the distributed application starts. |
| **2** | <xref:Aspire.Hosting.Lifecycle.IDistributedApplicationLifecycleHook.AfterEndpointsAllocatedAsync%2A> | Executes after the orchestrator allocates endpoints for resources in the application model. |
| **3** | <xref:Aspire.Hosting.Lifecycle.IDistributedApplicationLifecycleHook.AfterResourcesCreatedAsync%2A> | Executes after the resource was created by the orchestrator. |

While the app host provides life cycle hooks, you might want to register custom events. For more information, see [Eventing in .NET Aspire](../app-host/eventing.md).

## Register a life cycle hook

To register a life cycle hook, implement the <xref:Aspire.Hosting.Lifecycle.IDistributedApplicationLifecycleHook> interface and register the hook with the app host using the <xref:Aspire.Hosting.Lifecycle.LifecycleHookServiceCollectionExtensions.AddLifecycleHook*> API:

:::code source="snippets/lifecycles/AspireApp/AspireApp.AppHost/Program.cs":::

The preceding code:

- Implements the <xref:Aspire.Hosting.Lifecycle.IDistributedApplicationLifecycleHook> interface as a `LifecycleLogger`.
- Registers the life cycle hook with the app host using the <xref:Aspire.Hosting.Lifecycle.LifecycleHookServiceCollectionExtensions.AddLifecycleHook*> API.
- Logs a message for all the events.

When this app host is run, the life cycle hook is executed for each event. The following output is generated:

```Output
info: LifecycleLogger[0]
      BeforeStartAsync
info: Aspire.Hosting.DistributedApplication[0]
      Aspire version: 9.0.0
info: Aspire.Hosting.DistributedApplication[0]
      Distributed application starting.
info: Aspire.Hosting.DistributedApplication[0]
      Application host directory is: ..\AspireApp\AspireApp.AppHost
info: LifecycleLogger[0]
      AfterEndpointsAllocatedAsync
info: Aspire.Hosting.DistributedApplication[0]
      Now listening on: https://localhost:17043
info: Aspire.Hosting.DistributedApplication[0]
      Login to the dashboard at https://localhost:17043/login?t=d80f598bc8a64c7ee97328a1cbd55d72
info: LifecycleLogger[0]
      AfterResourcesCreatedAsync
info: Aspire.Hosting.DistributedApplication[0]
      Distributed application started. Press Ctrl+C to shut down.
```

The preferred way to hook into the app host life cycle is to use the eventing API. For more information, see [Eventing in .NET Aspire](../app-host/eventing.md).

## See also

- [.NET Aspire orchestration overview](app-host-overview.md)
- [Orchestrate resources in .NET Aspire](orchestrate-resources.md)
- [Eventing in .NET Aspire](../app-host/eventing.md)
