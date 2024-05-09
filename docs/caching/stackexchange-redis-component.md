---
title: .NET Aspire StackExchange Redis component
description: This article describes the .NET Aspire StackExchange Redis component features and capabilities
ms.topic: how-to
ms.date: 04/29/2024
---

# .NET Aspire StackExchange Redis component

In this article, you learn how to use the .NET Aspire StackExchange Redis component. The `Aspire.StackExchange.Redis` library is used to register an [IConnectionMultiplexer](https://stackexchange.github.io/StackExchange.Redis/Basics) in the DI container for connecting to a [Redis](https://redis.io/) server. It enables corresponding health checks, logging and telemetry.

## Get started

To get started with the .NET Aspire StackExchange Redis component, install the [Aspire.StackExchange.Redis](https://www.nuget.org/packages/Aspire.StackExchange.Redis) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.StackExchange.Redis --prerelease
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.StackExchange.Redis"
                  Version="[SelectVersion]" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

In the _Program.cs_ file of your component-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireRedisExtensions.AddRedisClient%2A> extension to register a `IConnectionMultiplexer` for use via the dependency injection container.

```csharp
builder.AddRedisClient("cache");
```

You can then retrieve the `IConnectionMultiplexer` instance using dependency injection. For example, to retrieve the connection multiplexer from a service:

```csharp
public class ExampleService(IConnectionMultiplexer connectionMultiplexer)
{
    // Use connection multiplexer...
}
```

## App host usage

In your app host project, register the .NET Aspire Stack Exchange Redis component using the <xref:Aspire.Hosting.RedisBuilderExtensions.AddRedis%2A> method and consume the service using the following methods:

```csharp
// Service registration
var redis = builder.AddRedis("redis");

// Service consumption
builder.AddProject<Projects.ExampleProject>()
       .WithReference(redis)
```

The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method configures a connection in the `ExampleProject` project named `redis`. In the _Program.cs_ file of `ExampleProject`, the Redis connection can be consumed using:

```csharp
builder.AddRedis("cache");
```

## Configuration

The .NET Aspire StackExchange Redis component provides multiple options to configure the Redis connection based on the requirements and conventions of your project.

### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddRedis`:

```csharp
builder.AddRedis("RedisConnection");
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

The .NET Aspire StackExchange Redis component supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the <xref:Aspire.StackExchange.Redis.StackExchangeRedisSettings> from configuration by using the `Aspire:StackExchange:Redis` key. Example `appsettings.json` that configures some of the options:

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
builder.AddRedis(
    "cache",
    settings => settings.DisableTracing = true);
```

[!INCLUDE [component-health-checks](../includes/component-health-checks.md)]

The .NET Aspire StackExchange Redis component handles the following:

- Adds the `StackExchange.Redis` health check, tries to open the connection and throws when it fails.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic

[!INCLUDE [component-observability-and-telemetry](../includes/component-observability-and-telemetry.md)]

### Logging

The .NET Aspire StackExchange Redis component uses the following log categories:

- `Aspire.StackExchange.Redis`

### Tracing

The .NET Aspire StackExchange Redis component will emit the following tracing activities using OpenTelemetry:

- "OpenTelemetry.Instrumentation.StackExchangeRedis"

### Metrics

The .NET Aspire StackExchange Redis component currently doesn't support metrics by default due to limitations with the `StackExchange.Redis` library.

## See also

- [StackExchange Redis docs](https://stackexchange.github.io/StackExchange.Redis/)
- [.NET Aspire components](../fundamentals/components-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
