---
title: Aspire Redis distributed caching integration
description: Learn how to use the Aspire Redis distributed caching integration, which includes both hosting and client integrations.
ms.date: 02/05/2025
zone_pivot_groups: resp-host
ms.custom: sfi-ropc-nochange
---

# Aspire Redis&reg;<sup>**[*](#registered)**</sup> distributed caching integration

<a name="heading"></a>

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

:::zone pivot="redis"

Learn how to use the Aspire Redis distributed caching integration. The `Aspire.StackExchange.Redis.DistributedCaching` library is used to register an [IDistributedCache](https://stackexchange.github.io/StackExchange.Redis/Basics) provider backed by a [Redis](https://redis.io/) server with the [`docker.io/library/redis` container image](https://hub.docker.com/_/redis/).

:::zone-end
:::zone pivot="garnet"

Learn how to use the Aspire Redis distributed caching integration. The `Aspire.StackExchange.Redis.DistributedCaching` library is used to register an [IDistributedCache](https://stackexchange.github.io/StackExchange.Redis/Basics) provider backed by a [Garnet](https://microsoft.github.io/garnet/) server with the [`ghcr.io/microsoft/garnet` container image](https://github.com/microsoft/garnet/pkgs/container/garnet).

:::zone-end
:::zone pivot="valkey"

Learn how to use the Aspire Redis distributed caching integration. The `Aspire.StackExchange.Redis.DistributedCaching` library is used to register an [IDistributedCache](https://stackexchange.github.io/StackExchange.Redis/Basics) provider backed by a [Valkey](https://valkey.io/) server with the [`docker.io/valkey/valkey` container image](https://hub.docker.com/r/valkey/valkey/).

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

[!INCLUDE [redis-distributed-client-nuget](includes/redis-distributed-client-nuget.md)]

### Add Redis client

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireRedisDistributedCacheExtensions.AddRedisDistributedCache%2A> extension to register the required services for distributed caching and add a <xref:Microsoft.Extensions.Caching.Distributed.IDistributedCache> for use via the dependency injection container.

```csharp
builder.AddRedisDistributedCache(connectionName: "cache");
```

:::zone pivot="redis"

> [!TIP]
> The `connectionName` parameter must match the name used when adding the Redis resource in the AppHost project. For more information, see [Add Redis resource](#add-redis-resource).

:::zone-end
:::zone pivot="garnet"

> [!TIP]
> The `connectionName` parameter must match the name used when adding the Garnet resource in the AppHost project. For more information, see [Add Garnet resource](#add-garnet-resource).

:::zone-end
:::zone pivot="valkey"

> [!TIP]
> The `connectionName` parameter must match the name used when adding the Valkey resource in the AppHost project. For more information, see [Add Valkey resource](#add-valkey-resource).

:::zone-end

You can then retrieve the `IDistributedCache` instance using dependency injection. For example, to retrieve the cache from a service:

```csharp
public class ExampleService(IDistributedCache cache)
{
    // Use cache...
}
```

For more information on dependency injection, see [.NET dependency injection](/dotnet/core/extensions/dependency-injection).

### Add keyed Redis client

Due to its limitations, you cannot register multiple `IDistributedCache` instances simultaneously. However, there may be scenarios where you need to register multiple Redis clients and use a specific `IDistributedCache` instance for a particular connection name. To register a keyed Redis client that will be used for the `IDistributedCache` service, call the <xref:Microsoft.Extensions.Hosting.AspireRedisDistributedCacheExtensions.AddKeyedRedisDistributedCache*> method:

```csharp
builder.AddKeyedRedisClient(name: "chat");
builder.AddKeyedRedisDistributedCache(name: "product");
```

Then you can retrieve the `IDistributedCache` instance using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(
    [FromKeyedServices("chat")] IConnectionMultiplexer chatConnectionMux,
    IDistributedCache productCache)
{
    // Use product cache...
}
```

For more information on keyed services, see [.NET dependency injection: Keyed services](/dotnet/core/extensions/dependency-injection#keyed-services).

### Configuration

The Aspire Redis distributed caching integration provides multiple options to configure the Redis connection based on the requirements and conventions of your project.

#### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddRedisDistributedCache`:

```csharp
builder.AddRedisDistributedCache("cache");
```

And then the connection string will be retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "cache": "localhost:6379"
  }
}
```

For more information on how to format this connection string, see the [Stack Exchange Redis configuration docs](https://stackexchange.github.io/StackExchange.Redis/Configuration.html#basic-configuration-strings).

#### Use configuration providers

[!INCLUDE [redis-distributed-client-json-settings](includes/redis-distributed-client-json-settings.md)]

#### Use inline delegates

You can also pass the `Action<StackExchangeRedisSettings>` delegate to set up some or all the options inline, for example to configure `DisableTracing`:

```csharp
builder.AddRedisDistributedCache(
    "cache",
    settings => settings.DisableTracing = true);
```

You can also set up the [ConfigurationOptions](https://stackexchange.github.io/StackExchange.Redis/Configuration.html#configuration-options) using the `Action<ConfigurationOptions> configureOptions` delegate parameter of the `AddRedisDistributedCache` method. For example to set the connection timeout:

```csharp
builder.AddRedisDistributedCache(
    "cache",
     null,
     static options => options.ConnectTimeout = 3_000);
```

[!INCLUDE [redis-distributed-client-health-checks-and-diagnostics](includes/redis-distributed-client-health-checks-and-diagnostics.md)]

## See also

- [Stack Exchange Redis docs](https://stackexchange.github.io/StackExchange.Redis/)
- [Aspire integrations](../fundamentals/integrations-overview.md)
- [Aspire GitHub repo](https://github.com/dotnet/aspire)

[!INCLUDE [redis-trademark](includes/redis-trademark.md)]
