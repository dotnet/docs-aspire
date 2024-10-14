---
title: .NET Aspire Redis integration
description: Learn how to use the .NET Aspire Redis integration, which includes both hosting and client integrations.
ms.date: 10/14/2024
zone_pivot_groups: resp-host
---

# .NET Aspire Redis integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

:::zone pivot="redis"

[!INCLUDE [redis-intro](includes/redis-intro.md)]

:::zone-end
:::zone pivot="garnet"

[!INCLUDE [garnet-intro](includes/garnet-intro.md)]

:::zone-end
:::zone pivot="valkey"

[!INCLUDE [valkey-intro](includes/valkey-intro.md)]

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

The Redis hosting integration automatically adds a health check for the appropriate resource type. The health check verifies that the server is running and that a connection can be established to it.

The hosting integration relies on the [📦 AspNetCore.HealthChecks.Redis](https://www.nuget.org/packages/AspNetCore.HealthChecks.Redis) NuGet package.

## Client integration

To get started with the .NET Aspire Stack Exchange Redis client integration, install the [📦 Aspire.StackExchange.Redis](https://www.nuget.org/packages/Aspire.StackExchange.Redis) NuGet package in the client-consuming project, that is, the project for the application that uses the Redis client. The Redis client integration registers an  an [IConnectionMultiplexer](https://stackexchange.github.io/StackExchange.Redis/Basics) instance that you can use to interact with Redis.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.StackExchange.Redis
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.StackExchange.Redis"
                  Version="*" />
```

---

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

## Configuration

The .NET Aspire Stack Exchange Redis client integration provides multiple options to configure the Redis connection based on the requirements and conventions of your project.

### Use a connection string

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

[!INCLUDE [valkey-app-host](includes/valkey-app-host.md)]

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

### Use configuration providers

The .NET Aspire Stack Exchange Redis integration supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the <xref:Aspire.StackExchange.Redis.StackExchangeRedisSettings> from configuration by using the `Aspire:StackExchange:Redis` key. Example _:::no-loc text="appsettings.json":::_ that configures some of the options:

```json
{
  "Aspire": {
    "StackExchange": {
      "Redis": {
        "ConnectionString": "localhost:6379",
        "DisableHealthChecks": true,
        "DisableTracing": false
      }
    }
  }
}
```

For the complete Redis client integration JSON schema, see [Aspire.StackExchange.Redis/ConfigurationSchema.json](https://github.com/dotnet/aspire/blob/v8.2.1/src/Components/Aspire.StackExchange.Redis/ConfigurationSchema.json).

### Use inline delegates

You can also pass the `Action<StackExchangeRedisSettings>` delegate to set up some or all the options inline, for example to configure `DisableTracing`:

```csharp
builder.AddRedisClient(
    "cache",
    static settings => settings.DisableTracing = true);
```

[!INCLUDE [integration-health-checks](../includes/integration-health-checks.md)]

The .NET Aspire Stack Exchange Redis integration handles the following:

- Adds the `StackExchange.Redis` health check, tries to open the connection and throws when it fails.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

### Logging

The .NET Aspire Stack Exchange Redis integration uses the following log categories:

- `Aspire.StackExchange.Redis`

### Tracing

The .NET Aspire Stack Exchange Redis integration will emit the following tracing activities using OpenTelemetry:

- `OpenTelemetry.Instrumentation.StackExchangeRedis`

### Metrics

The .NET Aspire Stack Exchange Redis integration currently doesn't support metrics by default due to limitations with the `StackExchange.Redis` library.

## See also

- [Stack Exchange Redis docs](https://stackexchange.github.io/StackExchange.Redis/)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)

**<a name="registered">*</a>**: _Redis is a registered trademark of Redis Ltd. Any rights therein are reserved to Redis Ltd. Any use by Microsoft is for referential purposes only and does not indicate any sponsorship, endorsement or affiliation between Redis and Microsoft._
