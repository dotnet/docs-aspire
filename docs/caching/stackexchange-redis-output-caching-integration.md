---
title: .NET Aspire Stack Exchange Redis output caching Component
description: This article describes the .NET Aspire Stack Exchange Redis output caching integration features and capabilities
ms.topic: how-to
ms.date: 08/12/2024
zone_pivot_groups: resp-host
---

# .NET Aspire Stack Exchange Redis output caching integration

In this article, you learn how to use the .NET Aspire Stack Exchange Redis output caching integration. The `Aspire.StackExchange.Redis.OutputCaching` library is used to register an [ASP.NET Core Output Caching](/aspnet/core/performance/caching/output) provider backed by a [Redis](https://redis.io/) server. It enables corresponding health check, logging, and telemetry..

## Get started

To get started with the .NET Aspire Stack Exchange Redis output caching integration, install the [Aspire.StackExchange.Redis.OutputCaching](https://www.nuget.org/packages/Aspire.StackExchange.Redis.OutputCaching) NuGet package in the client-consuming project, i.e., the project for the application that uses the Stack Exchange Redis output caching client.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.StackExchange.Redis.OutputCaching
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.StackExchange.Redis.OutputCache"
                  Version="[SelectVersion]" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireRedisOutputCacheExtensions.AddRedisOutputCache%2A> extension to register the required services for output caching.

```csharp
builder.AddRedisOutputCache("cache");
```

Add the middleware to the request processing pipeline by calling [UseOutputCache](/dotnet/api/microsoft.aspnetcore.builder.outputcacheapplicationbuilderextensions.useoutputcache).

```csharp
var app = builder.Build();
app.UseOutputCache();
```

For minimal API apps, configure an endpoint to do caching by calling <xref:Microsoft.Extensions.DependencyInjection.OutputCacheConventionBuilderExtensions.CacheOutput%2A>, or by applying the <xref:Microsoft.AspNetCore.OutputCaching.OutputCacheAttribute>, as shown in the following examples:

```csharp
app.MapGet("/cached", () => { return "Hello world!"; }).CacheOutput();
app.MapGet("/attribute", [OutputCache] () => { return "Hello world!"; });
```

For apps with controllers, apply the `[OutputCache]` attribute to the action method. For Razor Pages apps, apply the attribute to the Razor page class.

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

The .NET Aspire Stack Exchange Redis output caching integration provides multiple options to configure the Redis connection based on the requirements and conventions of your project.

### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddRedisOutputCache`:

```csharp
builder.AddRedisOutputCache("RedisConnection");
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

The .NET Aspire Stack Exchange Redis output caching integration supports <xref:Microsoft.Extensions.Configuration>. It loads the <xref:Aspire.StackExchange.Redis.StackExchangeRedisSettings> from configuration by using the `Aspire:StackExchange:Redis` key. Example _:::no-loc text="appsettings.json":::_ that configures some of the options:

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
    static configureOptions: options => options.ConnectTimeout = 3000);
```

[!INCLUDE [integration-health-checks](../includes/integration-health-checks.md)]

The .NET Aspire Stack Exchange Redis output caching integration handles the following:

- Adds the `StackExchange.Redis` health check, tries to open the connection and throws when it fails.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic.

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

### Logging

The .NET Aspire Stack Exchange Redis output caching integration uses the following Log categories:

- `Aspire.StackExchange.Redis`
- `Microsoft.AspNetCore.OutputCaching.StackExchangeRedis`

### Tracing

The .NET Aspire Stack Exchange Redis output caching integration will emit the following Tracing activities using OpenTelemetry:

- "OpenTelemetry.Instrumentation.StackExchangeRedis"

### Metrics

The .NET Aspire Stack Exchange Redis output caching integration currently doesn't support metrics by default due to limitations with the `StackExchange.Redis` library.

## See also

- [Stack Exchange Redis docs](https://stackexchange.github.io/StackExchange.Redis/)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
