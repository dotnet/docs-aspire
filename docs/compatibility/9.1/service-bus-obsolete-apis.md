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

The new methods use the `AddServiceBus` prefix. The `Add` prefix indicates that a child resource is created and returned, aligning with the intended usage.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var serviceBus = builder.AddAzureServiceBus("messaging");

var queue = serviceBus.AddServiceBusQueue("queueName");

var topic = serviceBus.AddServiceBusTopic("topicName");
```

## Type of breaking change

This change is a [source incompatible](../categories.md#source-compatibility).

## Reason for change

A better API is provided, as the names `Add` reflect that a child resource is created and returned. `Add` should be used when it returns the new resource (not the parent resource). For more information, see [EventHubs, ServiceBus, and CosmosDB Hosting integrations should create Resources for children](https://github.com/dotnet/aspire/issues/7407).

## Recommended action

Replace any usage of the obsolete methods with the new methods.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var serviceBus = builder.AddAzureServiceBus("messaging");

serviceBus.AddServiceBusQueue("queueName");
serviceBus.AddServiceBusTopic("topicName");
```

## Affected APIs

- `Aspire.Hosting.AzureServiceBusExtensions.AddQueue`
- `Aspire.Hosting.AzureServiceBusExtensions.AddTopic`
- `Aspire.Hosting.AzureServiceBusExtensions.AddSubscription`
