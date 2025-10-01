---
ms.topic: include
---

[!INCLUDE [client-integration-health-checks](../../includes/client-integration-health-checks.md)]

The Aspire Redis distributed caching integration handles the following:

- Adds the health check when <xref:Aspire.StackExchange.Redis.StackExchangeRedisSettings.DisableHealthChecks?displayProperty=nameWithType> is `false`, which attempts to connect to the container instance.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic.

[!INCLUDE [integration-observability-and-telemetry](../../includes/integration-observability-and-telemetry.md)]

#### Logging

The Aspire Redis distributed caching integration uses the following Log categories:

- `Aspire.StackExchange.Redis`
- `Microsoft.Extensions.Caching.StackExchangeRedis`

#### Tracing

The Aspire Redis distributed caching integration will emit the following Tracing activities using OpenTelemetry:

- `OpenTelemetry.Instrumentation.StackExchangeRedis`

#### Metrics

The Aspire Redis Distributed caching integration currently doesn't support metrics by default due to limitations with the `StackExchange.Redis` library.
