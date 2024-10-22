---
title: .NET Aspire Redis distributed caching integration
description: Learn how to use the .NET Aspire Redis distributed caching integration, which includes both hosting and client integrations.
ms.date: 10/15/2024
zone_pivot_groups: resp-host
---

# .NET Aspire Redis&reg;<sup>**[*](#registered)**</sup> distributed caching integration

<a name="heading"></a>

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

:::zone pivot="redis"

Learn how to use the .NET Aspire Redis distributed caching integration. The `Aspire.StackExchange.Redis.DistributedCaching` library is used to register an [IDistributedCache](https://stackexchange.github.io/StackExchange.Redis/Basics) provider backed by a [Redis](https://redis.io/) server with the [`docker.io/library/redis` container image](https://hub.docker.com/_/redis/).

:::zone-end
:::zone pivot="garnet"

Learn how to use the .NET Aspire Redis distributed caching integration. The `Aspire.StackExchange.Redis.DistributedCaching` library is used to register an [IDistributedCache](https://stackexchange.github.io/StackExchange.Redis/Basics) provider backed by a [Garnet](https://microsoft.github.io/garnet/) server with the [`ghcr.io/microsoft/garnet` container image](https://github.com/microsoft/garnet/pkgs/container/garnet).

:::zone-end
:::zone pivot="valkey"

Learn how to use the .NET Aspire Redis distributed caching integration. The `Aspire.StackExchange.Redis.DistributedCaching` library is used to register an [IDistributedCache](https://stackexchange.github.io/StackExchange.Redis/Basics) provider backed by a [Valkey](https://valkey.io/) server with the [`docker.io/valkey/valkey` container image](https://hub.docker.com/r/valkey/valkey/).

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

To get started with the .NET Aspire Redis distributed caching integration, install the [📦 Aspire.StackExchange.Redis.DistributedCaching](https://www.nuget.org/packages/Aspire.StackExchange.Redis.DistributedCaching) NuGet package in the client-consuming project, i.e., the project for the application that uses the Redis distributed caching client.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.StackExchange.Redis.DistributedCaching
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.StackExchange.Redis.DistributedCaching"
                  Version="*" />
```

---

### Add Redis client

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireRedisDistributedCacheExtensions.AddRedisDistributedCache%2A> extension to register the required services for distributed caching and add a <xref:Microsoft.Extensions.Caching.Distributed.IDistributedCache> for use via the dependency injection container.

```csharp
builder.AddRedisDistributedCache(connectionName: "cache");
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

You can then retrieve the `IDistributedCache` instance using dependency injection. For example, to retrieve the cache from a service:

```csharp
public class ExampleService(IDistributedCache cache)
{
    // Use cache...
}
```

For more information on dependency injection, see [.NET dependency injection](/dotnet/core/extensions/dependency-injection).

### Add keyed Redis client

There might be situations where you want to register multiple `IDistributedCache` instances with different connection names. To register keyed Redis clients, call the <xref:Microsoft.Extensions.Hosting.AspireRedisDistributedCacheExtensions.AddKeyedRedisDistributedCache*> method:

```csharp
builder.AddKeyedRedisDistributedCache(name: "chat");
builder.AddKeyedRedisDistributedCache(name: "product");
```

Then you can retrieve the `IDistributedCache` instances using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(
    [FromKeyedServices("chat")] IDistributedCache chatCache,
    [FromKeyedServices("product")] IDistributedCache productCache)
{
    // Use caches...
}
```

For more information on keyed services, see [.NET dependency injection: Keyed services](/dotnet/core/extensions/dependency-injection#keyed-services).

### Configuration

The .NET Aspire Redis distributed caching integration provides multiple options to configure the Redis connection based on the requirements and conventions of your project.

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

[!INCLUDE [redis-client-json-settings](includes/redis-client-json-settings.md)]

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
    static settings => settings.ConnectTimeout = 3_000);
```

[!INCLUDE [integration-health-checks](../includes/integration-health-checks.md)]

The .NET Aspire Redis distributed caching integration handles the following:

- Adds the `StackExchange.Redis` health check, tries to open the connection and throws when it fails.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

#### Logging

The .NET Aspire Redis Distributed Caching integration uses the following Log categories:

- `Aspire.StackExchange.Redis`
- `Microsoft.Extensions.Caching.StackExchangeRedis`

#### Tracing

The .NET Aspire Redis Distributed Caching integration will emit the following Tracing activities using OpenTelemetry:

- `OpenTelemetry.Instrumentation.StackExchangeRedis`

#### Metrics

The .NET Aspire Redis Distributed Caching integration currently doesn't support metrics by default due to limitations with the `StackExchange.Redis` library.

## See also

- [Stack Exchange Redis docs](https://stackexchange.github.io/StackExchange.Redis/)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)

[!INCLUDE [redis-trademark](includes/redis-trademark.md)]
