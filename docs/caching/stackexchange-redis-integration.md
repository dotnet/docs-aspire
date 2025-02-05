---
title: .NET Aspire Redis integration
description: Learn how to use the .NET Aspire Redis integration, which includes both hosting and client integrations.
ms.date: 02/05/2025
zone_pivot_groups: resp-host
---

# .NET Aspire Redis&reg;<sup>**[*](#registered)**</sup> integration

<a name="heading"></a>

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

:::zone pivot="redis"

[Redis](https://redis.io/) is the world's fastest data platform for caching, vector search, and NoSQL databases. The .NET Aspire Redis integration enables you to connect to existing Redis instances, or create new instances from .NET with the [`docker.io/library/redis` container image](https://hub.docker.com/_/redis/).

:::zone-end
:::zone pivot="garnet"

[Garnet](https://microsoft.github.io/garnet/) is a a high-performance cache-store from Microsoft Research and complies with the [Redis serialization protocol](stackexchange-redis-caching-overview.md#redis-serialization-protocol-resp) (RESP). The .NET Aspire Redis integration enables you to connect to existing Garnet instances, or create new instances from .NET with the [`ghcr.io/microsoft/garnet` container image](https://github.com/microsoft/garnet/pkgs/container/garnet).

:::zone-end
:::zone pivot="valkey"

[Valkey](https://valkey.io/) is a Redis fork and complies with the [Redis serialization protocol](stackexchange-redis-caching-overview.md#redis-serialization-protocol-resp) (RESP). It's a high-performance key/value datastore that supports a variety of workloads such as caching, message queues, and can act as a primary database. The .NET Aspire Redis integration enables you to connect to existing Valkey instances, or create new instances from .NET with the [`docker.io/valkey/valkey` container image](https://hub.docker.com/r/valkey/valkey/).

:::zone-end

## Hosting integration

:::zone pivot="redis"

[!INCLUDE [redis-app-host](includes/redis-app-host.md)]

:::zone-end
:::zone pivot="garnet"

[!INCLUDE [garnet-app-host](includes/garnet-app-host.md)]

:::zone-end
:::zone pivot="valkey"

[!INCLUDE [valkey-app-host](includes/valkey-app-host.md)]

:::zone-end

### Hosting integration health checks

[!INCLUDE [redis-hosting-health-checks](includes/redis-hosting-health-checks.md)]

## Client integration

[!INCLUDE [redis-client-nuget](includes/redis-client-nuget.md)]

### Add Redis client

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireRedisExtensions.AddRedisClient*> extension method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register an `IConnectionMultiplexer` for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddRedisClient(connectionName: "cache");
```

:::zone pivot="redis"

> [!TIP]
> The `connectionName` parameter must match the name used when adding the Redis resource in the app host project. For more information, see [Add Redis resource](#add-redis-resource).

:::zone-end
:::zone pivot="garnet"

> [!TIP]
> The `connectionName` parameter must match the name used when adding the Garnet resource in the app host project. For more information, see [Add Garnet resource](#add-garnet-resource).

:::zone-end
:::zone pivot="valkey"

> [!TIP]
> The `connectionName` parameter must match the name used when adding the Valkey resource in the app host project. For more information, see [Add Valkey resource](#add-valkey-resource).

:::zone-end

You can then retrieve the `IConnection` instance using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(IConnectionMultiplexer connectionMux)
{
    // Use connection multiplexer...
}
```

For more information on dependency injection, see [.NET dependency injection](/dotnet/core/extensions/dependency-injection).

### Add keyed Redis client

There might be situations where you want to register multiple `IConnectionMultiplexer` instances with different connection names. To register keyed Redis clients, call the <xref:Microsoft.Extensions.Hosting.AspireRedisExtensions.AddKeyedRedisClient*> method:

```csharp
builder.AddKeyedRedisClient(name: "chat");
builder.AddKeyedRedisClient(name: "queue");
```

Then you can retrieve the `IConnectionMultiplexer` instances using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(
    [FromKeyedServices("chat")] IConnectionMultiplexer chatConnectionMux,
    [FromKeyedServices("queue")] IConnectionMultiplexer queueConnectionMux)
{
    // Use connections...
}
```

For more information on keyed services, see [.NET dependency injection: Keyed services](/dotnet/core/extensions/dependency-injection#keyed-services).

### Configuration

The .NET Aspire Stack Exchange Redis client integration provides multiple options to configure the Redis connection based on the requirements and conventions of your project.

#### Use a connection string

:::zone pivot="redis"

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling <xref:Aspire.Hosting.RedisBuilderExtensions.AddRedis*>:

```csharp
builder.AddRedis("cache");
```

:::zone-end
:::zone pivot="garnet"

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling <xref:Aspire.Hosting.GarnetBuilderExtensions.AddGarnet*>:

```csharp
builder.AddGarnet("cache");
```

:::zone-end
:::zone pivot="valkey"

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling <xref:Aspire.Hosting.ValkeyBuilderExtensions.AddValkey*>:

```csharp
builder.AddValkey("cache");
```

:::zone-end

Then the connection string will be retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "cache": "localhost:6379"
  }
}
```

For more information on how to format this connection string, see the [Stack Exchange Redis configuration docs](https://stackexchange.github.io/StackExchange.Redis/Configuration.html#basic-configuration-strings).

#### Use configuration providers

[!INCLUDE [redis-client-json-settings](includes/redis-client-json-settings.md)]

#### Use inline delegates

You can also pass the `Action<StackExchangeRedisSettings>` delegate to set up some or all the options inline, for example to configure `DisableTracing`:

```csharp
builder.AddRedisClient(
    "cache",
    static settings => settings.DisableTracing = true);
```

### Client integration health checks

By default, .NET Aspire integrations enable [health checks](../fundamentals/health-checks.md) for all services. For more information, see [.NET Aspire integrations overview](../fundamentals/integrations-overview.md).

The .NET Aspire Stack Exchange Redis integration handles the following:

- Adds the health check when <xref:Aspire.StackExchange.Redis.StackExchangeRedisSettings.DisableHealthChecks?displayProperty=nameWithType> is `false`, which attempts to connect to the container instance.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic.

### Observability and telemetry

.NET Aspire integrations automatically set up Logging, Tracing, and Metrics configurations, which are sometimes known as *the pillars of observability*. For more information about integration observability and telemetry, see [.NET Aspire integrations overview](../fundamentals/integrations-overview.md). Depending on the backing service, some integrations might only support some of these features. For example, some integrations support logging and tracing, but not metrics. Telemetry features can also be disabled using the techniques presented in the [Configuration](#configuration) section.

#### Logging

The .NET Aspire Stack Exchange Redis integration uses the following log categories:

- `Aspire.StackExchange.Redis`

#### Tracing

The .NET Aspire Stack Exchange Redis integration will emit the following tracing activities using OpenTelemetry:

- `OpenTelemetry.Instrumentation.StackExchangeRedis`

#### Metrics

The .NET Aspire Stack Exchange Redis integration currently doesn't support metrics by default due to limitations with the `StackExchange.Redis` library.

## See also

- [Stack Exchange Redis docs](https://stackexchange.github.io/StackExchange.Redis/)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)

[!INCLUDE [redis-trademark](includes/redis-trademark.md)]
