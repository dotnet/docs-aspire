---
title: "Breaking change - WithHttpsHealthCheck and WithHttpHealthCheck changes in Aspire 9.3"
description: "Learn about the breaking change in Aspire 9.3 where the WithHttpsHealthCheck method is marked obsolete and WithHttpHealthCheck behavior is updated."
ms.date: 5/7/2025
ai-usage: ai-assisted
ms.custom: https://github.com/dotnet/docs-aspire/issues/3326
---

# With HTTP/S health checks changes

In Aspire 9.3, the `WithHttpsHealthCheck` method is marked as obsolete and will be removed in a future update. The `WithHttpHealthCheck` method is updated to default to selecting an endpoint with an `https` scheme, improving usability and aligning with other endpoint selection behaviors.

## Version introduced

Aspire 9.3

## Previous behavior

Previously, the `WithHttpsHealthCheck` method was used to register a health check for an endpoint with an `https` scheme. The `WithHttpHealthCheck` method defaulted to using an endpoint named "http" if no name was specified.

Example:

```csharp
builder.WithHttpsHealthCheck();
builder.WithHttpHealthCheck(); // Defaults to "http" endpoint
```

## New behavior

The `WithHttpsHealthCheck` method is now obsolete. The `WithHttpHealthCheck` method defaults to selecting the first available endpoint with an `https` scheme. If no `https` endpoint is found, it falls back to an `http` endpoint. You can also specify a specific endpoint name or selector action.

Example:

```csharp
builder.WithHttpHealthCheck(); // Defaults to "https" endpoint if available
```

## Type of breaking change

This is both a [source incompatible](../categories.md#source-compatibility) and [behavioral change](../categories.md#behavioral-change).

## Reason for change

In Aspire 9.2, the starter template included a call to `WithHttpsHealthCheck` by default. This caused issues when launching projects with an "http" profile, resulting in exceptions during startup. The change simplifies endpoint selection and aligns `WithHttpHealthCheck` behavior with the `WithHttpCommand` method, which offers a more user-friendly approach.

## Recommended action

Replace calls to the obsolete `WithHttpsHealthCheck` method with `WithHttpHealthCheck`. Ensure the behavior aligns with your expectations.

Example:

```csharp
// Replace this:
builder.WithHttpsHealthCheck();

// With this:
builder.WithHttpHealthCheck();
```

## Affected APIs

- <xref:Aspire.Hosting.ResourceBuilderExtensions.WithHttpsHealthCheck*>
- <xref:Aspire.Hosting.ResourceBuilderExtensions.WithHttpHealthCheck*>
