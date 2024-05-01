---
title: Standalone .NET Aspire dashboard
description: How to use the .NET Aspire dashboard standalone.
ms.date: 04/30/2024
ms.topic: reference
---

# Standalone .NET Aspire dashboard

The [.NET Aspire dashboard](overview.md) is also shipped as a Docker image and can be used standalone, without the rest of .NET Aspire. The standalone dashboard provides a great UI for viewing telemetry and can be used by any application.

:::image type="content" source="media/explore/trace.png" lightbox="media/explore/trace.png" alt-text="A screenshot of the .NET Aspire dashboard Trace details page.":::

## Start the dashboard

The dashboard is started using the Docker command line.

```bash
docker run --rm -it -p 18888:18888 -p 4317:18889 -d --name aspire-dashboard \
    mcr.microsoft.com/dotnet/nightly/aspire-dashboard:8.0.0-preview.6
```

The preceding Docker command:

- Starts a container from the `mcr.microsoft.com/dotnet/nightly/aspire-dashboard:8.0.0-preview.6` image.
- The container has two ports:
  - Port `4317` receives OpenTelemetry data from apps. Apps send data using [OpenTelemetry Protocol (OTLP)](https://opentelemetry.io/docs/specs/otlp/).
  - Port `18888` has the dashboard UI. Navigate to `http://localhost:18888` in the browser to view the dashboard.

## Login to the dashboard

Data displayed in the dashboard can be sensitive. By default, the dashboard is secured with authentication that requires a token to login.

When the dashboard is run from a standalone container, the login token is printed to the container logs. After copying the highlighted token into the login page, select the *Login* button.

:::image type="content" source="media/standalone/aspire-dashboard-container-log.png" lightbox="media/standalone/aspire-dashboard-container-log.png" alt-text="Screenshot of the .NET Aspire dashboard container logs.":::

For more information about logging into the dashboard, see [Dashboard authentication](explore.md#dashboard-authentication).

## Explore the dashboard

The dashboard offers a UI for viewing telemetry. Refer to the documentation to explore the telemetry functionality:

- [Structured logs page](explore.md#structured-logs-page)
- [Traces page](explore.md#traces-page)
- [Metrics page](explore.md#metrics-page)

Although there is no restriction on where the dashboard is run, the dashboard is designed as a development and short-term diagnosic tool. The dashboard persists telemetry in-memory which creates some limitations:

- Telemetry is automatically removed if [telemetry limits](configuration.md#telemetry-limits) are exceeded.
- No telemetry is persisted when the dashboard is restarted.

The dashboard also has functionality for viewing .NET Aspire resources. The dashboard resource features are disabled when it is run in standalone mode. To enable the resources UI, [add configuration for a resource service](configuration.md#resources).

## Send telemetry to the dashboard

Apps send telemetry to the dashboard using [OpenTelemetry Protocol (OTLP)](https://opentelemetry.io/docs/specs/otlp/). The dashboard must expose a port for receiving OpenTelemetry data, and apps are configured to send data to that address.

A Docker command was shown earlier to [start the dashboard](#start-the-dashboard). It configured the container to receive OpenTelemetry data on port `4317`. The OTLP endpoint's full address is `http://localhost:4317`.

### Configure OpenTelemetry SDK

Apps collect and send telemetry using [their language's OpenTelemetry SDK](https://opentelemetry.io/docs/languages/).

Important OpenTelemetry SDK options to configure:

- OTLP endpoint, which should match the dashboard's configuration, e.g., `http://localhost:4317`.
- OTLP protocol, with the dashboard currently supporting only the [OTLP/gRPC protocol](https://opentelemetry.io/docs/specs/otlp/#otlpgrpc). Configure applications to use the `grpc` protocol.

To configure applications:

- Use the OpenTelemetry SDK APIs within the application, or
- Start the app with [known environment variables](https://opentelemetry.io/docs/specs/otel/protocol/exporter/#configuration-options):
  - `OTEL_EXPORTER_OTLP_PROTOCOL` with a value of `grpc`.
  - `OTEL_EXPORTER_OTLP_ENDPOINT` with a value of `http://localhost:4317`.

## Sample

For a sample of using the standalone dashboard, see the [Standalone .NET Aspire dashboard sample app](/samples/dotnet/aspire-samples/aspire-standalone-dashboard).
