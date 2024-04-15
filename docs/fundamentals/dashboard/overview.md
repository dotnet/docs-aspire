---
title: .NET Aspire dashboard overview
description: Overview of .NET Aspire dashboard and getting started.
ms.date: 03/13/2024
ms.topic: reference
---

# .NET Aspire dashboard overview

[.NET Aspire](../../get-started/aspire-overview.md) project templates offer a sophisticated dashboard for comprehensive app monitoring and inspection. This dashboard allows you to track closely various aspects of your application, including logs, traces, and environment configurations, in real-time. It's purpose-built to enhance the local development experience, providing an insightful overview of your app's state and structure.

## Using the dashboard with Aspire projects

The dashboard is integrated into the Aspire **AppHost**. During development the dashboard is autommatically launched when you start the project. See the Aspire app's resources and view collected telemetry.

:::image type="content" source="media/explore/projects.png" lightbox="media/explore/projects.png" alt-text="A screenshot of the .NET Aspire dashboard Resources page.":::

[Explore dashboard features](explore.md) to find out more about using dashboard during Aspire development.

## Standalone mode

The .NET Aspire dashboard is also shipped as a docker image and can be used standalone, without the rest of .NET Aspire. The standalone dashboard provides a great UI for viewing telemetry and can be used by any application.

```bash
docker run --rm -it -p 18888:18888 -p 4317:18889 -d --name aspire-dashboard \
    mcr.microsoft.com/dotnet/nightly/aspire-dashboard:8.0.0-preview.6
```

The preceeding docker command:

- Starts a container from the `mcr.microsoft.com/dotnet/nightly/aspire-dashboard:8.0.0-preview.6` image.
- The container has two ports:
  - Port `4317` receives OpenTelemetry data from apps. Apps send data using [OpenTelemetry Protocol (OTLP)](https://opentelemetry.io/docs/specs/otlp/).
  - Port `18888` has the dashboard UI. Navigate to <http://localhost:18888> in the browser to view the dashboard.

For more information, see the [Standalone .NET Aspire dashboard sample app](/samples/dotnet/aspire-samples/aspire-standalone-dashboard).

## Configuration

The dashboard is configured when it starts up. Configuration includes frontend and OTLP addresses, the resource service endpoint, authentication, telemetry limits and more.

For more information, see [.NET Aspire dashboard configuration](configuration.md).

## Security

The .NET Aspire dashboard offers powerful insights to your apps. The UI displays information about resources, including their configuration, console logs and in-depth telemtry.

Data displayed in the dashboard can be sensitive. For example, configuration can include secrets in environment variables, and telemetry can include sensitive runtime data. Care should be taken to secure access to the dashboard.

For more information, see [.NET Aspire dashboard security considerations](security-considerations.md).
