---
title: .NET Aspire health checks
description: Explore .NET Aspire health checks
ms.date: 09/24/2024
ms.topic: quickstart
uid: dotnet/aspire/health-checks
---

# Health checks in .NET Aspire

Health checks provide availability and state information about an app. Health checks are often exposed as HTTP endpoints, but can also be used internally by the app to write logs or perform other tasks based on the current health. Health checks are typically used in combination with an external monitoring service or container orchestrator to check the status of an app. The data reported by health checks can be used for various scenarios:

- Influence decisions made by container orchestrators, load balancers, API gateways, and other management services. For instance, if the health check for a containerized app fails, it might be skipped by a load balancer routing traffic.
- Verify that underlying dependencies are available, such as a database or cache, and return an appropriate status message.
- Trigger alerts or notifications when an app isn't responding as expected.

## .NET Aspire health check endpoints

.NET Aspire exposes two default health check HTTP endpoints in **Development** environments when the `AddServiceDefaults` and `MapDefaultEndpoints` methods are called from the _:::no-loc text="Program.cs":::_ file:

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

The `AddServiceDefaults` and `MapDefaultEndpoints` methods also apply various configurations to your app beyond just health checks, such as [OpenTelemetry](telemetry.md) and [service discovery](../service-discovery/overview.md) configurations.

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

In addition to the updated `AddDefaultHealthChecks` and `MapDefaultEndpoints` methods, you must also add the corresponding services for both request timeouts and output caching.

In the appropriate consuming app's entry point (usually the _:::no-loc text="Program.cs":::_ file), add the following code:

```csharp
// Wherever your services are being registered.
// Before the call to Build().
builder.Services.AddRequestTimeouts();
builder.Services.AddOutputCache();

var app = builder.Build();

// Wherever your app has been built, before the call to Run().
app.UseRequestTimeouts();
app.UseOutputCache();

app.Run();
```

For more information, see [Request timeouts middleware in ASP.NET Core](/aspnet/core/performance/timeouts) and [Output caching middleware in ASP.NET Core](/aspnet/core/performance/caching/output).

## Integration health checks

.NET Aspire integrations can also register additional health checks for your app. These health checks contribute to the returned status of the `/health` and `/alive` endpoints. For example, the .NET Aspire PostgreSQL integration automatically adds a health check to verify the following conditions:

- A database connection could be established
- A database query could be executed successfully

If either of these operations fail, the corresponding health check also fails.

### Configure health checks

You can disable health checks for a given integration using one of the available configuration options. .NET Aspire integrations support [Microsoft.Extensions.Configurations](/dotnet/api/microsoft.extensions.configuration) to apply settings through config files such as _:::no-loc text="appsettings.json":::_:

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

## AppHost resource health checks

AppHost resource health checks are different from the health check endpoints described earlier. These health checks are configured in the AppHost project and determine the readiness of resources from the orchestrator's perspective. They're particularly important for controlling when dependent resources start via the [`WaitFor`](orchestrate-resources.md#waiting-for-resources) functionality and are displayed in the Aspire dashboard.

### Resource readiness with health checks

When a resource has health checks configured, the AppHost uses them to determine if the resource is ready before starting dependent resources. If no health checks are registered for a resource, the AppHost waits for the resource to be in the <xref:Aspire.Hosting.ApplicationModel.KnownResourceStates.Running> state.

### HTTP health checks for resources

For resources that expose HTTP endpoints, you can add health checks that poll specific paths:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var catalogApi = builder.AddContainer("catalog-api", "catalog-api")
                        .WithHttpEndpoint(targetPort: 8080)
                        .WithHttpHealthCheck("/health");

builder.AddProject<Projects.WebApp>("webapp")
       .WithReference(catalogApi)
       .WaitFor(catalogApi); // Waits for /health to return HTTP 200
```

The `WithHttpHealthCheck` method can also be applied to project resources:

```csharp
var backend = builder.AddProject<Projects.Backend>("backend")
                     .WithHttpHealthCheck("/health");
                     
builder.AddProject<Projects.Frontend>("frontend")
       .WithReference(backend)
       .WaitFor(backend);
```

### Custom resource health checks

You can create custom health checks for more complex readiness scenarios. Start by defining the health check in the AppHost's service collection, then associate it with resources:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var startAfter = DateTime.Now.AddSeconds(30);

builder.Services.AddHealthChecks().AddCheck("mycheck", () =>
    {
        return DateTime.Now > startAfter 
            ? HealthCheckResult.Healthy() 
            : HealthCheckResult.Unhealthy();
    });

var pg = builder.AddPostgres("pg")
    .WithHealthCheck("mycheck");

builder.AddProject<Projects.MyApp>("myapp")
    .WithReference(pg)
    .WaitFor(pg); // Waits for both the Postgres container to be running
                  // AND the custom "mycheck" health check to be healthy
```

The <xref:Microsoft.Extensions.DependencyInjection.HealthChecksBuilderAddCheckExtensions.AddCheck*> method registers the health check, and <xref:Aspire.Hosting.ResourceBuilderExtensions.WithHealthCheck*> associates it with specific resources. For more details about creating and registering custom health checks, see [Create health checks](/aspnet/core/host-and-deploy/health-checks#create-health-checks).

### Dashboard integration

Resource health check status is displayed in the Aspire dashboard, providing real-time visibility into resource readiness. When resources are waiting for health checks to pass, the dashboard shows the current status and any failure details.

For more information about using health checks with resource dependencies, see [Waiting for resources](orchestrate-resources.md#waiting-for-resources).

## See also

- [.NET app health checks in C#](/dotnet/core/diagnostics/diagnostic-health-checks)
- [Health checks in ASP.NET Core](/aspnet/core/host-and-deploy/health-checks)
