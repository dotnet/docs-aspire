---
title: .NET Aspire Redis output caching integration
description: Learn how to use the .NET Aspire  Redis output caching integration to register an ASP.NET Core Output Caching provider backed by a Redis server.
ms.date: 02/05/2025
zone_pivot_groups: resp-host
ms.custom: sfi-ropc-nochange
---

# .NET Aspire Redis&reg;<sup>**[*](#registered)**</sup> output caching integration

<a name="heading"></a>

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

:::zone pivot="redis"

Learn how to use the .NET Aspire Redis output caching integration. The `Aspire.StackExchange.Redis.OutputCaching` client integration is used to register an [ASP.NET Core Output Caching](/aspnet/core/performance/caching/output) provider backed by a [Redis](https://redis.io/) server with the [`docker.io/library/redis` container image](https://hub.docker.com/_/redis/).

:::zone-end
:::zone pivot="garnet"

Learn how to use the .NET Aspire Redis output caching integration. The `Aspire.StackExchange.Redis.OutputCaching` client integration is used to register an [ASP.NET Core Output Caching](/aspnet/core/performance/caching/output) provider backed by a [Garnet](https://microsoft.github.io/garnet/) server with the [`ghcr.io/microsoft/garnet` container image](https://github.com/microsoft/garnet/pkgs/container/garnet).

:::zone-end
:::zone pivot="valkey"

Learn how to use the .NET Aspire Redis output caching integration. The `Aspire.StackExchange.Redis.OutputCaching` client integration is used to register an [ASP.NET Core Output Caching](/aspnet/core/performance/caching/output) provider backed by a [Valkey](https://valkey.io/) server with the [`docker.io/valkey/valkey` container image](https://hub.docker.com/r/valkey/valkey/).

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

[!INCLUDE [redis-output-client-nuget](includes/redis-output-client-nuget.md)]

### Add output caching

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireRedisOutputCacheExtensions.AddRedisOutputCache%2A> extension method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register the required services for output caching.

```csharp
builder.AddRedisOutputCache(connectionName: "cache");
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

Add the middleware to the request processing pipeline by calling <xref:Microsoft.AspNetCore.Builder.OutputCacheApplicationBuilderExtensions.UseOutputCache(Microsoft.AspNetCore.Builder.IApplicationBuilder)>:

```csharp
var app = builder.Build();

app.UseOutputCache();
```

For [minimal API apps](/aspnet/core/fundamentals/minimal-apis/overview), configure an endpoint to do caching by calling <xref:Microsoft.Extensions.DependencyInjection.OutputCacheConventionBuilderExtensions.CacheOutput%2A>, or by applying the <xref:Microsoft.AspNetCore.OutputCaching.OutputCacheAttribute>, as shown in the following examples:

```csharp
app.MapGet("/cached", () => "Hello world!")
   .CacheOutput();

app.MapGet(
    "/attribute",
    [OutputCache] () => "Hello world!");
```

For apps with controllers, apply the `[OutputCache]` attribute to the action method. For Razor Pages apps, apply the attribute to the Razor page class.

### Configuration

The .NET Aspire Stack Exchange Redis output caching integration provides multiple options to configure the Redis connection based on the requirements and conventions of your project.

#### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling <xref:Microsoft.Extensions.Hosting.AspireRedisOutputCacheExtensions.AddRedisOutputCache*>:

```csharp
builder.AddRedisOutputCache(connectionName: "cache");
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

[!INCLUDE [redis-output-client-json-settings](includes/redis-output-client-json-settings.md)]

#### Use inline delegates

You can also pass the `Action<StackExchangeRedisSettings> configurationSettings` delegate to set up some or all the options inline, for example to disable health checks from code:

```csharp
builder.AddRedisOutputCache(
    "cache",
    static settings => settings.DisableHealthChecks  = true);
```

You can also set up the [ConfigurationOptions](https://stackexchange.github.io/StackExchange.Redis/Configuration.html#configuration-options) using the `Action<ConfigurationOptions> configureOptions` delegate parameter of the <xref:Microsoft.Extensions.Hosting.AspireRedisOutputCacheExtensions.AddRedisOutputCache%2A> method. For example to set the connection timeout:

```csharp
builder.AddRedisOutputCache(
    "cache",
    static settings => settings.ConnectTimeout = 3_000);
```

[!INCLUDE [redis-output-client-health-checks-and-diagnostics](includes/redis-output-client-health-checks-and-diagnostics.md)]

## See also

- [Stack Exchange Redis docs](https://stackexchange.github.io/StackExchange.Redis/)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)

[!INCLUDE [redis-trademark](includes/redis-trademark.md)]
