---
title: .NET Aspire telemetry
description: Learn about essential telemetry concepts for .NET Aspire.
ms.date: 12/08/2023
---

# .NET Aspire telemetry

One of the primary objectives of .NET Aspire is to ensure that apps are straightforward to debug and diagnose. .NET Aspire integrations automatically set up Logging, Tracing, and Metrics configurations, which are sometimes known as the pillars of observability, using the [.NET OpenTelemetry SDK](https://github.com/open-telemetry/opentelemetry-dotnet).

- **[Logging](/dotnet/core/diagnostics/logging-tracing)**: Log events describe what's happening as an app runs. A baseline set is enabled for .NET Aspire integrations by default and more extensive logging can be enabled on-demand to diagnose particular problems.

- **[Tracing](/dotnet/core/diagnostics/distributed-tracing)**: Traces correlate log events that are part of the same logical activity (e.g. the handling of a single request), even if they're spread across multiple machines or processes.

- **[Metrics](/dotnet/core/diagnostics/metrics)**: Metrics expose the performance and health characteristics of an app as simple numerical values. As a result, they have low performance overhead and many services configure them as always-on telemetry. This also makes them suitable for triggering alerts when potential problems are detected.

Together, these types of telemetry allow you to gain insights into your application's behavior and performance using various monitoring and analysis tools. Depending on the backing service, some integrations may only support some of these features.

## .NET Aspire OpenTelemetry integration

The [.NET OpenTelemetry SDK](https://github.com/open-telemetry/opentelemetry-dotnet) includes features for gathering data from several .NET APIs, including <xref:Microsoft.Extensions.Logging.ILogger>, <xref:System.Activities.Activity>, <xref:System.Diagnostics.Metrics.Meter>, and <xref:System.Diagnostics.Metrics.Instrument%601>. These APIs correspond to telemetry features like logging, tracing, and metrics. .NET Aspire projects define OpenTelemetry SDK configurations in the _ServiceDefaults_ project. For more information, see [.NET Aspire service defaults](service-defaults.md).

By default, the `ConfigureOpenTelemetry` method enables logging, tracing, and metrics for the app. It also adds exporters for these data points so they can be collected by other monitoring tools.

## Export OpenTelemetry data for monitoring

The .NET OpenTelemetry SDK facilitates the export of this telemetry data to a data store or reporting tool. The telemetry export mechanism relies on the [OpenTelemetry protocol (OTLP)](https://opentelemetry.io/docs/specs/otel/protocol), which serves as a standardized approach for transmitting telemetry data through REST or gRPC. The `ConfigureOpenTelemetry` method also registers exporters to provide your telemetry data to other monitoring tools, such as Prometheus or Azure Monitor. For more information, see [OpenTelemetry configuration](service-defaults.md#opentelemetry-configuration).

## OpenTelemetry environment variables

OpenTelemetry has a [list of known environment variables](https://opentelemetry.io/docs/specs/otel/configuration/sdk-environment-variables/) that configure the most important behavior for collecting and exporting telemetry. OpenTelemetry SDKs, including the .NET SDK, support reading these variables.

.NET Aspire projects launch with environment variables that configure the name and ID of the app in exported telemetry and set the address endpoint of the OTLP server to export data. For example:

- `OTEL_SERVICE_NAME` = myfrontend
- `OTEL_RESOURCE_ATTRIBUTES` = service.instance.id=1a5f9c1e-e5ba-451b-95ee-ced1ee89c168
- `OTEL_EXPORTER_OTLP_ENDPOINT` = `http://localhost:4318`

The environment variables are automatically set in local development.

## .NET Aspire local development

When you create a .NET Aspire project, the .NET Aspire dashboard provides a UI for viewing app telemetry by default. Telemetry data is sent to the dashboard using OTLP, and the dashboard implements an OTLP server to receive telemetry data and store it in memory. The .NET Aspire debugging workflow is as follows:

- Developer starts the .NET Aspire project with debugging, presses <kbd>F5</kbd>.
- .NET Aspire dashboard and developer control plane (DCP) start.
- App configuration is run in the _AppHost_ project.
  - OpenTelemetry environment variables are automatically added to .NET projects during app configuration.
  - DCP provides the name (`OTEL_SERVICE_NAME`) and ID (`OTEL_RESOURCE_ATTRIBUTES`) of the app in exported telemetry.
  - The OTLP endpoint is an HTTP/2 port started by the dashboard. This endpoint is set in the `OTEL_EXPORTER_OTLP_ENDPOINT` environment variable on each project. That tells projects to export telemetry back to the dashboard.
  - Small export intervals (`OTEL_BSP_SCHEDULE_DELAY`, `OTEL_BLRP_SCHEDULE_DELAY`, `OTEL_METRIC_EXPORT_INTERVAL`) so data is quickly available in the dashboard. Small values are used in local development to prioritize dashboard responsiveness over efficiency.
- The DCP starts configured projects, containers, and executables.
- Once started, apps send telemetry to the dashboard.
- Dashboard displays near real-time telemetry of all .NET Aspire projects.

All of these steps happen internally, so in most cases the developer simply needs to run the app to see this process in action.

## .NET Aspire deployment

.NET Aspire deployment environments should configure OpenTelemetry environment variables that make sense for their environment. For example, `OTEL_EXPORTER_OTLP_ENDPOINT` should be configured to the environment's local OTLP collector or monitoring service.

.NET Aspire telemetry works best in environments that support OTLP. OTLP exporting is disabled if `OTEL_EXPORTER_OTLP_ENDPOINT` isn't configured.

For more information, see [.NET Aspire deployments](../deployment/overview.md).
