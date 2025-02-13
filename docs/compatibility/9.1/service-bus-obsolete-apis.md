---
title: Azure Service Bus hosting integration obsolete APIs
description: Methods AddQueue, AddTopic, and AddSubscription are being obsoleted in Azure.Hosting.ServiceBus.
ms.date: 02/13/2025
ai-usage: ai-assisted
ms.custom: https://github.com/dotnet/docs-aspire/issues/2327
---

# Azure Service Bus hosting integration obsolete APIs

In .NET Aspire 9.1, the methods `AddQueue`, `AddTopic`, and `AddSubscription` are being obsoleted. This change introduces new methods that better reflect the intended usage and improve the API's clarity.

## Version introduced

.NET Aspire 9.1

## Previous behavior

Previously, the methods `AddQueue`, `AddTopic`, and `AddSubscription` were used to add resources to the Azure Service Bus.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var serviceBus = builder.AddAzureServiceBus("messaging");

serviceBus.AddQueue("queueName");
serviceBus.AddTopic("topicName");
serviceBus.AddSubscription("topicName", "subscriptionName");
```

## New behavior

The new methods use the `With` prefix to indicate that no specific resource is created, aligning with the intended usage.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var serviceBus = builder.AddAzureServiceBus("messaging");

serviceBus.WithQueue(
    name: "queueName", 
    configure: (ServiceBusQueue queue) => { /* configure queue */ });

serviceBus.WithTopic(
    name: "topicName",
    configure: (ServiceBusTopic topic) => { /* configure topic */ });
```

The `configure` parameter is optional. If not provided, the default configuration is used.

## Type of breaking change

This change is a [source incompatible](../categories.md#source-incompatible).

## Reason for change

A better API is provided, as the names `With` reflect that no specific resource is created. `Add` should be used when it returns an actual resource (not a resource builder). For more information, see [Add Service Bus emulator support](https://github.com/dotnet/aspire/pull/6737).

## Recommended action

Replace any usage of the obsolete methods with the new methods.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var serviceBus = builder.AddAzureServiceBus("messaging");

serviceBus.WithQueue("queueName");
serviceBus.WithTopic("topicName");
```

## Affected APIs

- `Aspire.Hosting.AzureServiceBusExtensions.AddQueue`
- `Aspire.Hosting.AzureServiceBusExtensions.AddTopic`
- `Aspire.Hosting.AzureServiceBusExtensions.AddSubscription`
