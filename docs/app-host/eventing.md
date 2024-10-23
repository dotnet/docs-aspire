---
title: Eventing in .NET Aspire
description: Learn how to use the .NET eventing features with .NET Aspire.
ms.date: 10/23/2024
---

# Eventing in .NET Aspire

In .NET Aspire, you use eventing to enable scenarios where you need to publish and subscribe to events during various [app host](xref:dotnet/aspire/app-host#app-host-life-cycles) life cycles. Generally speaking, eventing is more flexible than life cycle events, as you can run arbitrary code and you have more finite control. The eventing mechanisms in .NET Aspire are core to the [📦 Aspire.Hosting](https://www.nuget.org/packages/Aspire.Hosting) NuGet package. This package provides a set of interfaces and classes in the <xref:Aspire.Hosting.Eventing> namespace that you use to publish and subscribe to events in your .NET Aspire app host project. Eventing is scoped to the app host itself and the resources within.

In this article, you learn how to use the eventing features in .NET Aspire.

## App host eventing

The following events are available in the app host and are raised in the following order:

1. <xref:Aspire.Hosting.ApplicationModel.BeforeStartEvent>: This event is raised before the app host starts.
1. <xref:Aspire.Hosting.ApplicationModel.AfterEndpointsAllocatedEvent>: This event is raised after the app host has allocated endpoints.
1. <xref:Aspire.Hosting.ApplicationModel.AfterResourcesCreatedEvent>: This event is raised after the app host has created resources.

All of the preceding events are analogous to the [app host life cycles](xref:dotnet/aspire/app-host#app-host-life-cycles). That is, they could also be handled by an implementation of the <xref:Aspire.Hosting.Lifecycle.IDistributedApplicationLifecycleHook>. With the eventing API, however, you can run arbitrary code when these events are raised and event define custom events—any event that implements the <xref:Aspire.Hosting.Eventing.IDistributedApplicationEvent> interface.

### Subscribe to app host events

To subscribe to the built-in app host events, use the eventing API. After you have a distributed application builder instance, walk up to the <xref:Aspire.Hosting.IDistributedApplicationBuilder.Eventing?displayProperty=nameWithType> property and call the <xref:Aspire.Hosting.Eventing.IDistributedApplicationEventing.Subscribe*?displayProperty=nameWithType> API. Consider the following sample app host _Program.cs_ file:

:::code source="snippets/AspireApp/AspireApp.AppHost/Program.cs":::

The preceding code is based on the starter template with the addition of the subscribe calls. The `Subscribe<T>` API returns a <xref:Aspire.Hosting.Eventing.DistributedApplicationEventSubscription> instance that you can use to unsubscribe from the event. It's common to discard these, as you don't usually need to unsubscribe from events as the entire app is torn down when the app host is shut down.

When the app host is run, by the time the .NET Aspire dashboard is displayed, you should see the following log output in the console:

```Output
info: Program[0]
      1. BeforeStartEvent
info: Aspire.Hosting.DistributedApplication[0]
      Aspire version: 9.0.0
info: Aspire.Hosting.DistributedApplication[0]
      Distributed application starting.
info: Aspire.Hosting.DistributedApplication[0]
      Application host directory is: ..\AspireApp\AspireApp.AppHost
info: Program[0]
      2. AfterEndpointsAllocatedEvent
info: Aspire.Hosting.DistributedApplication[0]
      Now listening on: https://localhost:17178
info: Aspire.Hosting.DistributedApplication[0]
      Login to the dashboard at https://localhost:17178/login?t=<YOUR_TOKEN>
info: Program[0]
      3. AfterResourcesCreatedEvent
info: Aspire.Hosting.DistributedApplication[0]
      Distributed application started. Press Ctrl+C to shut down.
```

## Resource eventing

In addition to the app host events, you can also subscribe to resource events. Resource events are raised when a resource is created, updated, or deleted. The following events are available:

<xref:Aspire.Hosting.Eventing.IDistributedApplicationResourceEvent>

## Event dispatch behavior

When events are dispatched, you can control how the events are dispatched to subscribers. The event dispatch behavior is controlled by the `EventDispatchBehavior` enum. The following behaviors are available:

<!--
- <xref:Aspire.Hosting.Eventing.EventDispatchBehavior.BlockingSequential>: Fires events sequentially and blocks until they're all processed.
- <xref:Aspire.Hosting.Eventing.EventDispatchBehavior.BlockingConcurrent>: Fires events concurrently and blocks until they are all processed.
- <xref:Aspire.Hosting.Eventing.EventDispatchBehavior.NonBlockingSequential>: Fires events sequentially but doesn't block.
- <xref:Aspire.Hosting.Eventing.EventDispatchBehavior.NonBlockingConcurrent>: Fires events concurrently but doesn't block.
-->

- `EventDispatchBehavior.BlockingSequential`: Fires events sequentially and blocks until they're all processed.
- `EventDispatchBehavior.BlockingConcurrent`: Fires events concurrently and blocks until they are all processed.
- `EventDispatchBehavior.NonBlockingSequential`: Fires events sequentially but doesn't block.
- `EventDispatchBehavior.NonBlockingConcurrent`: Fires events concurrently but doesn't block.

The default behavior is `EventDispatchBehavior.BlockingSequential`. To override this behavior, when calling a publishing API such as <xref:Aspire.Hosting.Eventing.IDistributedApplicationEventing.PublishAsync*>, provide the desired behavior as an argument.
