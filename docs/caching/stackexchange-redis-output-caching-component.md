---
title: .NET Aspire StackExchange Redis output caching Component
description: This article describes the .NET Aspire StackExchange Redis output caching component features and capabilities
ms.topic: how-to
ms.date: 04/29/2024
---

# .NET Aspire StackExchange Redis output caching component

In this article, you learn how to use the .NET Aspire StackExchange Redis output caching component. The `Aspire.StackExchange.Redis.OutputCaching` library is used to register an [ASP.NET Core Output Caching](/aspnet/core/performance/caching/output) provider backed by a [Redis](https://redis.io/) server. It enables corresponding health check, logging, and telemetry.

## Get started

To get started with the .NET Aspire StackExchange Redis output caching component, install the [Aspire.StackExchange.Redis.OutputCaching](https://www.nuget.org/packages/Aspire.StackExchange.Redis.OutputCaching) NuGet package.

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

In the _Program.cs_ file of your component-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireRedisOutputCacheExtensions.AddRedisOutputCache%2A> extension to register the required services for output caching.

```csharp
builder.AddRedisOutputCache();
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

In your app host project, register the .NET Aspire StackExchange Redis output caching component using the <xref:Aspire.Hosting.RedisBuilderExtensions.AddRedis%2A> method and consume the service using the following methods:

```csharp
// Service registration
var redis = builder.AddRedis("redis");

// Service consumption
var basket = builder.AddProject<Projects.ExampleProject>()
                    .WithReference(redis)
```

The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method configures a connection in the `ExampleProject` project named `redis`. In the _Program.cs_ file of `ExampleProject`, the Redis connection can be consumed using:

```csharp
builder.AddRedisOutputCache("messaging");
```

## Configuration

The .NET Aspire StackExchange Redis output caching component provides multiple options to configure the Redis connection based on the requirements and conventions of your project.

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

For more information on how to format this connection string, see the [StackExchange Redis configuration docs](https://stackexchange.github.io/StackExchange.Redis/Configuration.html#basic-configuration-strings).

### Use configuration providers

The .NET Aspire StackExchange Redis output caching component supports <xref:Microsoft.Extensions.Configuration>. It loads the <xref:Aspire.StackExchange.Redis.StackExchangeRedisSettings> from configuration by using the `Aspire:StackExchange:Redis` key. Example _appsettings.json_ that configures some of the options:

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

[!INCLUDE [component-health-checks](../includes/component-health-checks.md)]

The .NET Aspire StackExchange Redis output caching component handles the following:

- Adds the `StackExchange.Redis` health check, tries to open the connection and throws when it fails.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic.

[!INCLUDE [component-observability-and-telemetry](../includes/component-observability-and-telemetry.md)]

### Logging

The .NET Aspire StackExchange Redis output caching component uses the following Log categories:

- `Aspire.StackExchange.Redis`
- `Microsoft.AspNetCore.OutputCaching.StackExchangeRedis`

### Tracing

The .NET Aspire StackExchange Redis output caching component will emit the following Tracing activities using OpenTelemetry:

- "OpenTelemetry.Instrumentation.StackExchangeRedis"

### Metrics

The .NET Aspire StackExchange Redis output caching component currently doesn't support metrics by default due to limitations with the `StackExchange.Redis` library.

## See also

- [StackExchange Redis docs](https://stackexchange.github.io/StackExchange.Redis/)
- [.NET Aspire components](../fundamentals/components-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
