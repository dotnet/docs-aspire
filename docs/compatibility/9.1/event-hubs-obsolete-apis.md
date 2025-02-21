---
title: Azure Event Hubs hosting integration obsolete APIs
description: Method AddEventHub is being obsoleted in Azure.Hosting.EventHubs.
ms.date: 02/13/2025
ai-usage: ai-assisted
ms.custom: https://github.com/dotnet/docs-aspire/issues/2327
---

# Azure Service Bus hosting integration obsolete APIs

In .NET Aspire 9.1, the method `AddEventHub` is being obsoleted. This change introduces new methods that better reflect the intended usage and improve the API's clarity.

## Version introduced

.NET Aspire 9.1

## Previous behavior

Previously, the method `AddEventHub` was used to add a hub to the Azure Event Hubs namespace.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var serviceBus = builder.AddAzureEventHubs("hubs");

serviceBus.AddEventHub("myhub");
```

## New behavior

The new method also uses the `Add` prefix. The `Add` prefix indicates that a child resource is created and returned, aligning with the intended usage.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var eventHubs = builder.AddAzureEventHubs("hubs");

var myhub = serviceBus.AddHub("myhub");
```

## Type of breaking change

This change is a [source incompatible](../categories.md#source-compatibility).

## Reason for change

A better API is provided, as the names `Add` reflect that a child resource is created and returned. `Add` should be used when it returns the new resource (not the parent resource). For more information, see [EventHubs, ServiceBus, and CosmosDB Hosting integrations should create Resources for children](https://github.com/dotnet/aspire/issues/7407).

## Recommended action

Replace any usage of the obsolete methods with the new methods.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var serviceBus = builder.AddAzureEventHubs("messaging");

serviceBus.AddHub("myhub");
```

## Affected APIs

- `Aspire.Hosting.AzureEventHubsExtensions.AddEventHub`
