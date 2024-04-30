---
title: Standalone .NET Aspire dashboard
description: How to use the .NET Aspire dashboard standalone.
ms.date: 04/30/2024
ms.topic: reference
---

# Standalone .NET Aspire dashboard

The .NET Aspire dashboard is also shipped as a Docker image and can be used standalone, without the rest of .NET Aspire. The standalone dashboard provides a great UI for viewing telemetry and can be used by any application.

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

When the dashboard is run from a standalone container the login token is printed to the container logs. After copying the highlighted token into the login page, select the *Login* button.

![Screenshot of the .NET Aspire dashboard container logs](./images/aspire-dashboard-container-log.png)

For more information about logging into the dashboard, see [Dashboard authentication](https://learn.microsoft.com/dotnet/aspire/fundamentals/dashboard/explore#dashboard-authentication).

## Explore the dashboard

The dashboard provides UI for viewing telemetry. Explore telemetry functionality in the documentation:

- [Structured logs page](explore.md#structured-logs-page)
- [Traces page](explore.md#traces-page)
- [Metrics page](explore.md#metrics-page)

The dashboard also have functionality for viewing .NET Aspire resources. The dashboard's UI for resources is disabled when it is run in standalone mode. To enable resources UI, [add configuration for a resource service](configuration.md#resources).

## Send telemetry to the dashboard

Apps send telemetry to the dashboard using [OpenTelemetry Protocol (OTLP)](https://opentelemetry.io/docs/specs/otlp/). The dashboard container should be launched with ports exposed to view the dashboard UI and receive OpenTelemetry data from apps.

The earlier Docker command [used to start the dashboard](#start-the-dashboard) configured two endpoints:

- Port `18888` has the dashboard UI.
- Port `4317` receives OpenTelemetry data from apps. Apps send data using [OpenTelemetry Protocol (OTLP)](https://opentelemetry.io/docs/specs/otlp/). Its full address is `http://localhost:4317` and is what apps are configured with to send telemetry to the dashboard.

### Configure apps OpenTelemetry SDK

Apps collect and send telemetry using [their language's OpenTelemetry SDK](https://opentelemetry.io/docs/languages/). OpenTelemetry SDK options to configure:

- OTLP endpoint. The address should match the dashboard's configuration. For example, `http://localhost:4317`.
- OTLP protocol. The dashboard currently only supports the [OTLP/gRPC protocol](https://opentelemetry.io/docs/specs/otlp/#otlpgrpc). Apps sending telemetry to the dashboard should be configured to use the `grpc` protocol.

There are a couple of options for configuring apps:

- Configure the OpenTelemetry SDK inside the app to using its SDK APIs, or
- Start the app with [known environment variables](https://opentelemetry.io/docs/specs/otel/protocol/exporter/#configuration-options):
  - `OTEL_EXPORTER_OTLP_PROTOCOL` with a value of `grpc`.
  - `OTEL_EXPORTER_OTLP_ENDPOINT` with a value of `http://localhost:4317`.

## Sample

For a sample of using the standalone dashboard, see the [Standalone .NET Aspire dashboard sample app](/samples/dotnet/aspire-samples/aspire-standalone-dashboard).
