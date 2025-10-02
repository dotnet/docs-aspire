---
title: Azure Cache for Redis integration
description: Learn how to integrate Azure Cache for Redis with the Aspire stack.
ms.date: 02/05/2025
---

# Aspire Azure Cache for Redis&reg;<sup>**[*](#registered)**</sup> integration

<a name="heading"></a>

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[!INCLUDE [azure-redis-intro](includes/azure-redis-intro.md)]

The Aspire Azure Cache for Redis integration enables you to connect to existing Azure Cache for Redis instances, or create new instances, or run as a container locally from .NET with the [`docker.io/library/redis` container image](https://hub.docker.com/_/redis/).

## Hosting integration

[!INCLUDE [azure-redis-app-host](includes/azure-redis-app-host.md)]

## Client integration

[!INCLUDE [redis-client-nuget](includes/redis-client-nuget.md)]

### Add Redis client

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireRedisExtensions.AddRedisClient*> extension method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register an `IConnectionMultiplexer` for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddRedisClient(connectionName: "cache");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the Azure Cache for Redis resource in the AppHost project. For more information, see [Add Azure Cache for Redis resource](#add-azure-cache-for-redis-resource).

You can then retrieve the `IConnectionMultiplexer` instance using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(IConnectionMultiplexer connectionMux)
{
    // Use connection multiplexer...
}
```

For more information on dependency injection, see [.NET dependency injection](/dotnet/core/extensions/dependency-injection).

[!INCLUDE [azure-redis-client](includes/azure-redis-client.md)]

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

The Aspire Stack Exchange Redis client integration provides multiple options to configure the Redis connection based on the requirements and conventions of your project.

#### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling <xref:Aspire.Hosting.RedisBuilderExtensions.AddRedis*>:

```csharp
builder.AddRedis("cache");
```

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

[!INCLUDE [redis-client-health-checks-and-diagnostics](includes/redis-client-health-checks-and-diagnostics.md)]

## See also

- [Azure Cache for Redis docs](/azure/azure-cache-for-redis/)
- [Stack Exchange Redis docs](https://stackexchange.github.io/StackExchange.Redis/)
- [Aspire integrations](../fundamentals/integrations-overview.md)
- [Aspire GitHub repo](https://github.com/dotnet/aspire)

[!INCLUDE [redis-trademark](includes/redis-trademark.md)]
