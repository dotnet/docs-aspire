---
title: .NET Aspire dashboard overview
description: Overview of .NET Aspire dashboard and getting started.
ms.date: 04/23/2024
ms.topic: reference
---

# .NET Aspire dashboard overview

[.NET Aspire](../../get-started/aspire-overview.md) project templates offer a sophisticated dashboard for comprehensive app monitoring and inspection. This dashboard allows you to closely track various aspects of your app, including logs, traces, and environment configurations, in real-time. It's purpose-built to enhance the local development experience, providing an insightful overview of your app's state and structure.

## Using the dashboard with .NET Aspire projects

The dashboard is integrated into the .NET Aspire **AppHost**. During development the dashboard is automatically launched when you start the project. It's configured to display the .NET Aspire app's resources and telemetry.

:::image type="content" source="media/explore/projects.png" lightbox="media/explore/projects.png" alt-text="A screenshot of the .NET Aspire dashboard Resources page.":::

For more information about using the dashboard during .NET Aspire development, see [Explore dashboard features](explore.md).

## Standalone mode

The .NET Aspire dashboard is also shipped as a Docker image and can be used standalone, without the rest of .NET Aspire. The standalone dashboard provides a great UI for viewing telemetry and can be used by any application.

```bash
docker run --rm -it -p 18888:18888 -p 4317:18889 -d --name aspire-dashboard \
    mcr.microsoft.com/dotnet/nightly/aspire-dashboard:8.0.0-preview.6
```

The preceding Docker command:

- Starts a container from the `mcr.microsoft.com/dotnet/nightly/aspire-dashboard:8.0.0-preview.6` image.
- The container has two ports:
  - Port `4317` receives OpenTelemetry data from apps. Apps send data using [OpenTelemetry Protocol (OTLP)](https://opentelemetry.io/docs/specs/otlp/).
  - Port `18888` has the dashboard UI. Navigate to `http://localhost:18888` in the browser to view the dashboard.

For more information, see the [Standalone .NET Aspire dashboard sample app](/samples/dotnet/aspire-samples/aspire-standalone-dashboard).

> [!NOTE]
> The dashboard currently only supports the [OTLP/gRPC protocol](https://opentelemetry.io/docs/specs/otlp/#otlpgrpc). Apps sending telemetry to the dashboard must be configured to use the `grpc` protocol. There are a couple of options for configuring apps:
>
> - Configure the OpenTelemetry SDK inside the app to use the gRPC OTLP protocol, or
> - Start the app with the [`OTEL_EXPORTER_OTLP_PROTOCOL` environment variable](https://opentelemetry.io/docs/specs/otel/protocol/exporter/#configuration-options) with a value of `grpc`.

## Configuration

The dashboard is configured when it starts up. Configuration includes frontend and OTLP addresses, the resource service endpoint, authentication, telemetry limits and more.

For more information, see [.NET Aspire dashboard configuration](configuration.md).

## Architecture

The dashboard user experience is built with a variety of technologies. The frontend is built with [Microsoft's Fluent UI Blazor component library](https://github.com/microsoft/fluentui-blazor). Each app communicates to the dashboard using the [OpenTelemetry Protocol (OTLP)](https://opentelemetry.io/docs/specs/otlp/). A resource server exists within this architecture to provide information about the app's resources, such as a resource listing, console logs, and command execution. The dashboard communicates using gRPC (specifically with the [Grpc.AspNetCore](https://www.nuget.org/packages/Grpc.AspNetCore) NuGet package) to the resource server. Consider the following diagram that illustrates the architecture of the .NET Aspire dashboard:

:::image type="content" source="media/architecture-diagram.png" lightbox="media/architecture-diagram.png" alt-text="A diagram showing the architecture of the .NET Aspire dashboard.":::

## Security

The .NET Aspire dashboard offers powerful insights to your apps. The UI displays information about resources, including their configuration, console logs and in-depth telemetry.

Data displayed in the dashboard can be sensitive. For example, configuration can include secrets in environment variables, and telemetry can include sensitive runtime data. Care should be taken to secure access to the dashboard.

For more information, see [.NET Aspire dashboard security considerations](security-considerations.md).
