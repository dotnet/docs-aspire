---
title: Eventing in .NET Aspire
description: Learn how to use the .NET eventing features with .NET Aspire.
ms.date: 10/24/2024
---

# Eventing in .NET Aspire

In .NET Aspire, you use eventing to enable scenarios where you need to publish and subscribe to events during various [app host](xref:dotnet/aspire/app-host#app-host-life-cycles) life cycles. Generally speaking, eventing is more flexible than life cycle events, as you can run arbitrary code and you have more finite control. The eventing mechanisms in .NET Aspire are core to the [📦 Aspire.Hosting](https://www.nuget.org/packages/Aspire.Hosting) NuGet package. This package provides a set of interfaces and classes in the <xref:Aspire.Hosting.Eventing> namespace that you use to publish and subscribe to events in your .NET Aspire app host project. Eventing is scoped to the app host itself and the resources within.

In this article, you learn how to use the eventing features in .NET Aspire.

## App host eventing

The following events are available in the app host and are occur in the following order:

1. <xref:Aspire.Hosting.ApplicationModel.BeforeStartEvent>: This event is raised before the app host starts.
1. <xref:Aspire.Hosting.ApplicationModel.AfterEndpointsAllocatedEvent>: This event is raised after the app host has allocated endpoints.
1. <xref:Aspire.Hosting.ApplicationModel.AfterResourcesCreatedEvent>: This event is raised after the app host has created resources.

All of the preceding events are analogous to the [app host life cycles](xref:dotnet/aspire/app-host#app-host-life-cycles). That is, they could also be handled by an implementation of the <xref:Aspire.Hosting.Lifecycle.IDistributedApplicationLifecycleHook>. With the eventing API, however, you can run arbitrary code when these events are raised and event define custom events—any event that implements the <xref:Aspire.Hosting.Eventing.IDistributedApplicationEvent> interface.

### Subscribe to app host events

To subscribe to the built-in app host events, use the eventing API. After you have a distributed application builder instance, walk up to the <xref:Aspire.Hosting.IDistributedApplicationBuilder.Eventing?displayProperty=nameWithType> property and call the <xref:Aspire.Hosting.Eventing.IDistributedApplicationEventing.Subscribe``1(System.Func{``0,System.Threading.CancellationToken,System.Threading.Tasks.Task})> API. Consider the following sample app host _Program.cs_ file:

:::code source="snippets/AspireApp/AspireApp.AppHost/Program.cs":::

The preceding code is based on the starter template with the addition of the subscribe calls. The `Subscribe<T>` API returns a <xref:Aspire.Hosting.Eventing.DistributedApplicationEventSubscription> instance that you can use to unsubscribe from the event. It's common to discard these, as you don't usually need to unsubscribe from events as the entire app is torn down when the app host is shut down.

When the app host is run, by the time the .NET Aspire dashboard is displayed, you should see the following log output in the console:

:::code language="Output" source="snippets/AspireApp/AspireApp.AppHost/Console.txt" highlight="2,10,16":::

The log output confirms that event handlers are executed in the order of the app host life cycle events. The subscription order doesn't affect execution order. The `BeforeStartEvent` is triggered first, followed by `AfterEndpointsAllocatedEvent`, and finally `AfterResourcesCreatedEvent`.

## Resource eventing

In addition to the app host events, you can also subscribe to resource events. Resource events are raised specific to an individual resource. Resource events are defined as implementations of the <xref:Aspire.Hosting.Eventing.IDistributedApplicationResourceEvent> interface. The following resource events are available in the listed order:

1. `ConnectionStringAvailableEvent`: This event is raised when a connection string becomes available for a resource.
1. <xref:Aspire.Hosting.ApplicationModel.BeforeResourceStartedEvent>: This event is raised by orchestrators before they have started a new resource.

### Subscribe to resource events

To subscribe to resource events, use the eventing API. After you have a distributed application builder instance, walk up to the <xref:Aspire.Hosting.IDistributedApplicationBuilder.Eventing?displayProperty=nameWithType> property and call the <xref:Aspire.Hosting.Eventing.IDistributedApplicationEventing.Subscribe``1(Aspire.Hosting.ApplicationModel.IResource,System.Func{``0,System.Threading.CancellationToken,System.Threading.Tasks.Task})> API. Consider the following sample app host _Program.cs_ file:

:::code source="snippets/AspireApp/AspireApp.ResourceAppHost/Program.cs":::

The preceding code subscribes to the `ConnectionStringAvailableEvent` and `BeforeResourceStartedEvent` events on the `cache` resource. When <xref:Aspire.Hosting.RedisBuilderExtensions.AddRedis*> is called, it returns an <xref:Aspire.Hosting.ApplicationModel.IResourceBuilder`1> where `T` is a <xref:Aspire.Hosting.ApplicationModel.RedisResource>. The resource builder exposes the resource as the <xref:Aspire.Hosting.ApplicationModel.IResourceBuilder`1.Resource?displayProperty=nameWithType> property. This resource is then passed to the `Subscribe` API to subscribe to the events on the resource.

When the app host is run, by the time the .NET Aspire dashboard is displayed, you should see the following log output in the console:

:::code language="Output" source="snippets/AspireApp/AspireApp.ResourceAppHost/Console.txt" highlight="8,10":::

## Publish events

When subscribing to any of the built-in events, you don't need to publish the event yourself as the app host orchestrator does this for you. However, you can publish custom events with the eventing API. To publish an event, you have to first define an event as an implementation of either the <xref:Aspire.Hosting.Eventing.IDistributedApplicationEvent> or <xref:Aspire.Hosting.Eventing.IDistributedApplicationResourceEvent> interface. You need to determine which interface to implement based on whether the event is a global app host event or a resource-specific event.

Then, you can subscribe and publish the event by calling the either of the following APIs:

- <xref:Aspire.Hosting.Eventing.IDistributedApplicationEventing.PublishAsync``1(``0,System.Threading.CancellationToken)>: Publishes an event to all subscribes of the specific event type.
- `PublishAsync<T>(T @event, EventDispatchBehavior, CancellationToken)`: Publishes an event to all subscribes of the specific event type with a specified dispatch behavior.

### Event dispatch behavior

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
