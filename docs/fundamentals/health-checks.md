---
title: .NET Aspire health checks
description: Explore .NET Aspire health checks
ms.date: 04/02/2024
ms.topic: quickstart
---

# Health checks in .NET Aspire

Health checks provide availability and state information about an app. Health checks are often exposed as HTTP endpoints, but can also be used internally by the app to write logs or perform other tasks based on the current health. Health checks are typically used in combination with an external monitoring service or container orchestrator to check the status of an app. The data reported by health checks can be used for various scenarios:

- Influence decisions made by container orchestrators, load balancers, API gateways, and other management services. For instance, if the health check for a containerized app fails, it might be skipped by a load balancer routing traffic.
- Verify that underlying dependencies are available, such as a database or cache, and return an appropriate status message.
- Trigger alerts or notifications when an app isn't responding as expected.

## .NET Aspire health check endpoints

.NET Aspire exposes two default health check HTTP endpoints in **Development** environments when the `AddServiceDefaults` and `MapDefaultEndpoints` methods are called from the _Program.cs_ file:

- The `/health` endpoint indicates if the app is running normally where it's ready to receive requests. All health checks must pass for app to be considered ready to accept traffic after starting.

    ```http
    GET /health
    ```

    The `/health` endpoint returns an HTTP status code 200 and a `text/plain` value of <xref:Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy> when the app is _healthy_.

- The `/alive` indicates if an app is running or has crashed and must be restarted. Only health checks tagged with the _live_ tag must pass for app to be considered alive.

    ```http
    GET /alive
    ```

    The `/alive` endpoint returns an HTTP status code 200 and a `text/plain` value of <xref:Microsoft.Extensions.Diagnostics.HealthChecks.HealthStatus.Healthy> when the app is _alive_.

The `AddServiceDefaults` and `MapDefaultEndpoints` methods also apply various configurations to your app beyond just health checks, such as OpenTelemetry and service discovery configurations.

### Non-development environments

In non-development environments, the `/health` and `/alive` endpoints are disabled by default. If you need to enable them, its recommended to protect these endpoints with various routing features, such as host filtering and/or authorization. For more information, see [Health checks in ASP.NET Core](/aspnet/core/host-and-deploy/health-checks#use-health-checks-routing).

Additionally, it may be advantageous to configure request timeouts and output caching for these endpoints to prevent abuse or denial-of-service attacks. To do so, consider the following modified `AddDefaultHealthChecks` method:

:::code language="csharp" source="snippets/healthz/Healthz.ServiceDefaults/Extensions.cs" id="healthchecks":::

The preceding code:

- Adds a timeout of 5 seconds to the health check requests with a policy named `HealthChecks`.
- Adds a 10-second cache to the health check responses with a policy named `HealthChecks`.

Now consider the updated `MapDefaultEndpoints` method:

:::code language="csharp" source="snippets/healthz/Healthz.ServiceDefaults/Extensions.cs" id="mapendpoints":::

The preceding code:

- Groups the health check endpoints under the `/` path.
- Caches the output and specifies a request time with the corresponding `HealthChecks` policy.

## Component health checks

.NET Aspire components can also register additional health checks for your app. These health checks contribute to the returned status of the `/health` and `/alive` endpoints. For example, the .NET Aspire PostgreSQL component automatically adds a health check to verify the following conditions:

- A database connection could be established
- A database query could be executed successfully

If either of these operations fail, the corresponding health check also fails.

### Configure health checks

You can disable health checks for a given component using one of the available configuration options. .NET Aspire components support [Microsoft.Extensions.Configurations](/dotnet/api/microsoft.extensions.configuration) to apply settings through config files such as `appsettings.json`:

```json
{
  "Aspire": {
    "Npgsql": {
      "DisableHealthChecks": true,
    }
  }
}
```

You can also use an inline delegate to configure health checks:

```csharp
builder.AddNpgsqlDbContext<MyDbContext>(
    "postgresdb",
    static settings => settings.DisableHealthChecks  = true);
```

## See also

- [.NET app health checks in C#](/dotnet/core/diagnostics/diagnostic-health-checks)
- [Health checks in ASP.NET Core](/aspnet/core/host-and-deploy/health-checks)
