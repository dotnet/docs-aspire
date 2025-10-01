---
title: Azure Cache for Redis distributed caching integration
description: Learn how to integrate Azure Cache for Redis as a distributed caching solution with the Aspire stack.
ms.date: 02/05/2025
---

# Aspire Azure Cache for Redis&reg;<sup>**[*](#registered)**</sup> distributed caching integration

<a name="heading"></a>

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[!INCLUDE [azure-redis-intro](includes/azure-redis-intro.md)]

The Aspire Azure Cache for Redis integration enables you to connect to existing Azure Cache for Redis instances, or create new instances from .NET with the [`docker.io/library/redis` container image](https://hub.docker.com/_/redis/).

## Hosting integration

[!INCLUDE [azure-redis-app-host](includes/azure-redis-app-host.md)]

## Client integration

[!INCLUDE [redis-distributed-client-nuget](includes/redis-distributed-client-nuget.md)]

### Add Redis distributed cache client

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireRedisDistributedCacheExtensions.AddRedisDistributedCache%2A> extension to register the required services for distributed caching and add a <xref:Microsoft.Extensions.Caching.Distributed.IDistributedCache> for use via the dependency injection container.

```csharp
builder.AddRedisDistributedCache(connectionName: "cache");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the Azure Cache for Redis resource in the AppHost project. For more information, see [Add Azure Cache for Redis resource](#add-azure-cache-for-redis-resource).

You can then retrieve the `IDistributedCache` instance using dependency injection. For example, to retrieve the cache from a service:

```csharp
public class ExampleService(IDistributedCache cache)
{
    // Use cache...
}
```

For more information on dependency injection, see [.NET dependency injection](/dotnet/core/extensions/dependency-injection).

[!INCLUDE [azure-redis-distributed-client](includes/azure-redis-distributed-client.md)]

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
    static settings => settings.ConnectTimeout = 3_000);
```

[!INCLUDE [redis-distributed-client-health-checks-and-diagnostics](includes/redis-distributed-client-health-checks-and-diagnostics.md)]

## See also

- [Azure Cache for Redis docs](/azure/azure-cache-for-redis/)
- [Stack Exchange Redis docs](https://stackexchange.github.io/StackExchange.Redis/)
- [Aspire integrations](../fundamentals/integrations-overview.md)
- [Aspire GitHub repo](https://github.com/dotnet/aspire)

[!INCLUDE [redis-trademark](includes/redis-trademark.md)]
