---
title: .NET Aspire Stack Exchange Redis distributed caching component
description: This article describes the .NET Aspire Stack Exchange Redis distributed caching component features and capabilities
ms.topic: how-to
ms.date: 07/23/2024
zone_pivot_groups: resp-host
---

# .NET Aspire Stack Exchange Redis distributed caching component

In this article, you learn how to use the .NET Aspire Stack Exchange Redis distributed caching component. The `Aspire.StackExchange.Redis.DistributedCaching` library is used to register an [IDistributedCache](https://stackexchange.github.io/StackExchange.Redis/Basics) provider for connecting to [Redis](https://redis.io/) server. It enables corresponding health checks, logging and telemetry.

## Get started

To get started with the .NET Aspire Stack Exchange Redis distributed caching component, install the [Aspire.StackExchange.Redis.DistributedCaching](https://www.nuget.org/packages/Aspire.StackExchange.Redis.DistributedCaching) NuGet package in the consuming client project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.StackExchange.Redis.DistributedCaching
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.StackExchange.Redis.DistributedCaching"
                  Version="[SelectVersion]" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

In the _:::no-loc text="Program.cs":::_ file of your component-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireRedisDistributedCacheExtensions.AddRedisDistributedCache%2A> extension to register the required services for distributed caching and add a <xref:Microsoft.Extensions.Caching.Distributed.IDistributedCache> for use via the dependency injection container.

```csharp
builder.AddRedisDistributedCache("cache");
```

You can then retrieve the `IDistributedCache` instance using dependency injection. For example, to retrieve the cache from a service:

```csharp
public class ExampleService(IDistributedCache cache)
{
    // Use cache...
}
```

## App host usage

:::zone pivot="redis"

[!INCLUDE [redis-app-host](includes/redis-app-host.md)]

:::zone-end
:::zone pivot="garnet"

[!INCLUDE [garnet-app-host](includes/garnet-app-host.md)]

:::zone-end
:::zone pivot="valkey"

[!INCLUDE [valkey-app-host](includes/valkey-app-host.md)]

:::zone-end

## Configuration

The .NET Aspire Stack Exchange Redis distributed caching component provides multiple options to configure the Redis connection based on the requirements and conventions of your project.

### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddRedisDistributedCache`:

```csharp
builder.AddRedisDistributedCache("RedisConnection");
```

And then the connection string will be retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "RedisConnection": "localhost:6379"
  }
}
```

For more information on how to format this connection string, see the [Stack Exchange Redis configuration docs](https://stackexchange.github.io/StackExchange.Redis/Configuration.html#basic-configuration-strings).

### Use configuration providers

The .NET Aspire Stack Exchange Redis distributed caching component supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the <xref:Aspire.StackExchange.Redis.StackExchangeRedisSettings> from configuration by using the `Aspire:StackExchange:Redis` key. Example _:::no-loc text="appsettings.json":::_ that configures some of the options:

```json
{
  "Aspire": {
    "StackExchange": {
      "Redis": {
        "ConfigurationOptions": {
          "ConnectTimeout": 3000,
          "ConnectRetry": 2
        },
        "DisableHealthChecks": true,
        "DisableTracing": false
      }
    }
  }
}
```

### Use inline delegates

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
    configureOptions: options => options.ConnectTimeout = 3000);
```

[!INCLUDE [component-health-checks](../includes/component-health-checks.md)]

The .NET Aspire Stack Exchange Redis distributed caching component handles the following:

- Adds the `StackExchange.Redis` health check, tries to open the connection and throws when it fails.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic

[!INCLUDE [component-observability-and-telemetry](../includes/component-observability-and-telemetry.md)]

### Logging

The .NET Aspire Stack Exchange Redis Distributed Caching component uses the following Log categories:

- `Aspire.StackExchange.Redis`
- `Microsoft.Extensions.Caching.StackExchangeRedis`

### Tracing

The .NET Aspire Stack Exchange Redis Distributed Caching component will emit the following Tracing activities using OpenTelemetry:

- "OpenTelemetry.Instrumentation.StackExchangeRedis"

### Metrics

The .NET Aspire Stack Exchange Redis Distributed Caching component currently doesn't support metrics by default due to limitations with the `StackExchange.Redis` library.

## See also

- [Stack Exchange Redis docs](https://stackexchange.github.io/StackExchange.Redis/)
- [.NET Aspire components](../fundamentals/components-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
