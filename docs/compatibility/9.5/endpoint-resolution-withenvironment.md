---
title: "Breaking change - Fix endpoint resolution for Host/Port in WithEnvironment"
description: "Learn about the breaking change in .NET Aspire 9.5 where endpoint Host and Port properties resolve independently inside WithEnvironment."
ms.date: 09/23/2025
ai-usage: ai-assisted
ms.custom: https://github.com/dotnet/docs-aspire/issues/4741
---

# Fix endpoint resolution for Host/Port in WithEnvironment

In .NET Aspire 9.5, endpoint property resolution inside `WithEnvironment` delegates is corrected so that `Host` and `Port` each resolve accurately when accessed independently. Previously, accessing only `Host` returned `localhost` and only `Port` returned the host (published) port unless both properties were accessed together.

## Version introduced

.NET Aspire 9.5

## Previous behavior

Inside a `WithEnvironment` delegate, accessing `Host` alone produced `localhost` instead of the container (resource) name. Accessing `Port` alone produced the published (host) port. To coerce correct values, some code accessed multiple endpoint properties in a single expression or used combined properties as a workaround.

```csharp
var redis = builder.AddRedis("redis");

var app = builder.AddContainer("myapp", "mycontainer")
  .WithEnvironment(context =>
  {
    var endpoint = redis.GetEndpoint("tcp");

    // Workaround: force evaluation of multiple properties
    _ = endpoint.Property(EndpointProperty.HostAndPort);

    // Still: Host would resolve to "localhost" unless both Host and Port
    // were referenced in certain combinations.
    var host = endpoint.Property(EndpointProperty.Host);   // "localhost" (incorrect)
    var port = endpoint.Property(EndpointProperty.Port);   // Published host port, not container port

    context.EnvironmentVariables["REDIS_HOST"] = host;
    context.EnvironmentVariables["REDIS_PORT"] = port;
  })
  .WithReference(redis);
```

## New behavior

Each endpoint property now resolves independently and correctly:

- `Host` resolves to the container (resource) name (for example, `redis`).
- `Port` resolves to the container's internal (target) port.
- No combined or chained property access is required.

```csharp
var redis = builder.AddRedis("redis");

var app = builder.AddContainer("myapp", "mycontainer")
  .WithEnvironment(context =>
  {
      var endpoint = redis.GetEndpoint("tcp");
    
      var host = endpoint.Property(EndpointProperty.Host); // "redis"
      var port = endpoint.Property(EndpointProperty.Port); // Container port (e.g., 6379)
    
      context.EnvironmentVariables["REDIS_HOST"] = host;
      context.EnvironmentVariables["REDIS_PORT"] = port;
  })
  .WithReference(redis);
```

## Type of breaking change

This is a [behavioral](../categories.md#behavioral-change) change.

## Reason for change

The previous resolution logic forced workarounds that accessed multiple properties to obtain correct values, making configuration unintuitive, and error prone. The fix aligns per-property resolution with developer expectations and eliminates reliance on indirect or combined property accesses.

## Recommended action

Remove any workarounds that:

1. Access `EndpointProperty.HostAndPort` solely to force evaluation.
1. Access both `Host` and `Port` together to coerce correct `Host` behavior.
1. Depend on the published host port being returned from `EndpointProperty.Port`.

Use the individual properties directly:

```csharp
var endpoint = redis.GetEndpoint("tcp");
var host = endpoint.Property(EndpointProperty.Host); // Container name
var port = endpoint.Property(EndpointProperty.Port); // Container port
```

If you previously stored the published port for external binding, explicitly retrieve that value via the appropriate API (for example, a published ports collection) instead of relying on `EndpointProperty.Port`'s former behavior.

## Affected APIs

- <xref:Aspire.Hosting.EndpointProperty.Host?displayProperty=fullName>
- <xref:Aspire.Hosting.EndpointProperty.Port?displayProperty=fullName>
