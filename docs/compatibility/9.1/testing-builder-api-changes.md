---
title: IDistributedApplicationTestingBuilder API changes
description: "IDistributedApplicationTestingBuilder now inherits from IDistributedApplicationBuilder, IAsyncDisposable, and IDisposable."
ms.date: 2/13/2025
ai-usage: ai-assisted
ms.custom: https://github.com/dotnet/docs-aspire/issues/2616
---

# IDistributedApplicationTestingBuilder API changes

In Aspire 9.1, <xref:Aspire.Hosting.Testing.IDistributedApplicationTestingBuilder> was changed to inherit from <xref:Aspire.Hosting.IDistributedApplicationBuilder>, <xref:System.IAsyncDisposable>, and <xref:System.IDisposable>. This change affects binary compatibility and requires recompilation of existing binaries.

## Version introduced

Aspire 9.1

## Previous behavior

Previously, `IDistributedApplicationTestingBuilder` inherited from nothing.

## New behavior

`IDistributedApplicationTestingBuilder` now inherits from `IDistributedApplicationBuilder`, `IAsyncDisposable`, and `IDisposable`.

## Type of breaking change

This change is a [binary incompatible](../categories.md#binary-compatibility) change.

## Reason for change

Methods accepting a parameter of type `IDistributedApplicationBuilder` now work with `IDistributedApplicationTestingBuilder`, giving developers a consistent API surface.

## Recommended action

Recompile your code if necessary. If you're implementing `IDistributedApplicationTestingBuilder` yourself (which would be highly unusual), then you need to implement the new interfaces also.

## Affected APIs

- `IDistributedApplicationTestingBuilder`
