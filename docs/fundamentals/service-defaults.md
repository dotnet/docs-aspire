---
title: .NET Aspire service defaults
description: Learn about the .NET Aspire service defaults project.
ms.date: 12/08/2023
ms.topic: reference
---

# .NET Aspire service defaults

Cloud-native applications often require extensive configurations to ensure they work across different environments reliably and securely. .NET Aspire provides many helper methods and tools to streamline the management of configurations for OpenTelemetry, health checks, environment variables, and more.

## Explore the service defaults project

When you either [**Enlist in .NET Aspire orchestration**](setup-tooling.md#enlist-in-orchestration) or [create a new .NET Aspire project](../get-started/build-your-first-aspire-app.md), the _YourAppName.ServiceDefaults_ project is added to your solution. For example, when building an API, you call the `AddServiceDefaults` method in the _Program.cs_ file of your apps:

```csharp
builder.AddServiceDefaults();
```

The `AddServiceDefaults` method handles the following concerns for you:

- Configures OpenTelemetry metrics and tracing.
- Add default health check endpoints.
- Add service discovery functionality.
- Configures <xref:System.Net.Http.HttpClient> to work with service discovery.

For more information, see [Provided extension methods](#provided-extension-methods) for details on the `AddServiceDefaults` method.

> [!IMPORTANT]
> The .NET Aspire service defaults project is specifically designed for sharing the _Extensions.cs_ file and its functionality. Please refrain from including other shared functionality or models in this project, use a conventional shared class library project for those purposes.

## Project characteristics

The _YourAppName.ServiceDefaults_ project is a .NET 8.0 library that contains the following XML:

:::code language="xml" source="snippets/template/YourAppName/YourAppName.ServiceDefaults.csproj":::

The `IsAspireSharedProject` property is set to `true`, which indicates that this project is a shared project. The .NET Aspire tooling uses this project as a reference for other projects added to a .NET Aspire solution. When you enlist the new project for orchestration, it automatically references the _YourAppName.ServiceDefaults_ project and updates the _Program.cs_ file to call the `AddServiceDefaults` method.

## Provided extension methods

The _YourAppName.ServiceDefaults_ project exposes a single _Extensions.cs_ file that contains several opinionated extension methods:

- `AddServiceDefaults`: Adds service defaults functionality.
- `ConfigureOpenTelemetry`: Configures OpenTelemetry metrics and tracing.
- `AddDefaultHealthChecks`: Adds default health check endpoints.
- `MapDefaultEndpoints`: Maps the health checks endpoint to `/health` and the liveness endpoint to `/alive`.

### Add service defaults functionality

The `AddServiceDefaults` method defines default configurations with the following opinionated functionality:

:::code source="snippets/template/YourAppName/Extensions.cs" id="addservicedefaults":::

The preceding code:

- Configures OpenTelemetry metrics and tracing, by calling the `ConfigureOpenTelemetry` method.
- Adds default health check endpoints, by calling the `AddDefaultHealthChecks` method.
- Adds [service discovery](../service-discovery/overview.md) functionality, by calling the `AddServiceDiscovery` method.
- Configures <xref:System.Net.Http.HttpClient> defaults, by calling the `ConfigureHttpClientDefaults` method—which is based on [Build resilient HTTP apps: Key development patterns](/dotnet/core/resilience/http-resilience):
  - Adds the standard HTTP resilience handler, by calling the `AddStandardResilienceHandler` method.
  - Specifies that the <xref:Microsoft.Extensions.DependencyInjection.IHttpClientBuilder> should use service discovery, by calling the `UseServiceDiscovery` method.
- Returns the `IHostApplicationBuilder` instance to allow for method chaining.

### OpenTelemetry configuration

Telemetry is a critical part of any cloud-native application. .NET Aspire provides a set of opinionated defaults for OpenTelemetry, which are configured with the `ConfigureOpenTelemetry` method:

:::code source="snippets/template/YourAppName/Extensions.cs" id="configureotel":::

The `ConfigureOpenTelemetry` method:

- Adds [.NET Aspire telemetry](telemetry.md) logging to include formatted messages and scopes.
- Adds OpenTelemetry metrics and tracing that include:
  - Runtime instrumentation metrics.
  - Builtin meters.
  - In a development environment, the `AlwaysOnSampler` is used to view all traces.
  - Tracing details for ASP.NET Core, gRPC and HTTP instrumentation.
- Adds OpenTelemetry exporters, by calling `AddOpenTelemetryExporters`.

The `AddOpenTelemetryExporters` method is defined privately as follows:

:::code source="snippets/template/YourAppName/Extensions.cs" id="addotelexporters":::

The `AddOpenTelemetryExporters` method adds OpenTelemetry exporters based on the following conditions:

- If the `OTEL_EXPORTER_OTLP_ENDPOINT` environment variable is set, the OpenTelemetry exporter is added.
- Optionally consumers of .NET Aspire service defaults can uncomment some code to enable the Prometheus exporter, or the Azure Monitor exporter.

For more information, see [.NET Aspire telemetry](telemetry.md).

### Health checks configuration

Health checks are used by various tools and systems to assess the readiness of your app. .NET Aspire provides a set of opinionated defaults for health checks, which are configured with the `AddDefaultHealthChecks` method:

:::code source="snippets/template/YourAppName/Extensions.cs" id="addhealthchecks":::

The `AddDefaultHealthChecks` method adds a default liveness check to ensure the app is responsive. The call to <xref:Microsoft.Extensions.DependencyInjection.HealthCheckServiceCollectionExtensions.AddHealthChecks%2A> registers the <xref:Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckService>. For more information, see [.NET Aspire health checks](health-checks.md).

#### Web app health checks configuration

To expose health checks in a web app, .NET Aspire automatically determines the type of project being referenced within the solution, and adds the appropriate call to `MapDefaultEndpoints`:

:::code source="snippets/template/YourAppName/Extensions.cs" id="mapdefaultendpoints":::

The `MapDefaultEndpoints` method:

- Allows consumers to optionally uncomment some code to enable the Prometheus endpoint.
- Maps the health checks endpoint to `/health`.
- Maps the liveness endpoint to `/alive` route where the health check tag contains `live`.

For more information, see [.NET Aspire health checks](health-checks.md).

## Next steps

This code is derived from the .NET Aspire Starter Application template and is intended as a starting point. You're free to modify this code however you deem necessary to meet your needs. It's important to know that service defaults project and its functionality are automatically applied to all project resources in a .NET Aspire solution.

- [Service discovery in .NET Aspire](../service-discovery/overview.md)
- [Health checks in .NET Aspire](health-checks.md)
- [.NET Aspire telemetry](telemetry.md)
- [Build resilient HTTP apps: Key development patterns](/dotnet/core/resilience/http-resilience)
