---
title: Aspire dashboard overview
description: Overview of Aspire dashboard and getting started.
ms.date: 04/04/2025
ms.topic: reference
---

# Aspire dashboard overview

[Aspire](../../get-started/aspire-overview.md) project templates include a sophisticated dashboard for comprehensive app monitoring and inspection. The dashboard is also available in [standalone mode](#standalone-mode).

The dashboard enables real-time tracking of key aspects of your app, including logs, traces, and environment configurations. It's designed to enhance the development experience by providing a clear and insightful view of your app's state and structure.

Key features of the dashboard include:

- Real-time tracking of logs, traces, and environment configurations.
- User interface to [stop, start, and restart resources](explore.md#resource-actions).
- Collects and displays logs and telemetry; [view structured logs, traces, and metrics](explore.md#monitoring-pages) in an intuitive UI.
- Enhanced debugging with [GitHub Copilot](copilot.md), your AI-powered assistant built into the dashboard.

## Use the dashboard with Aspire projects

The dashboard is integrated into the [Aspire _*.AppHost_](../app-host-overview.md). During development the dashboard is automatically launched when you start the project. It's configured to display the Aspire project's resources and telemetry.

:::image type="content" source="media/explore/projects.png" lightbox="media/explore/projects.png" alt-text="A screenshot of the Aspire dashboard Resources page.":::

For more information about using the dashboard during Aspire development, see [Explore dashboard features](explore.md).

## Standalone mode

The Aspire dashboard is also shipped as a Docker image and can be used standalone, without the rest of Aspire. The standalone dashboard provides a great UI for viewing telemetry and can be used by any application.

## [Bash](#tab/bash)

```bash
docker run --rm -it -p 18888:18888 -p 4317:18889 -d --name aspire-dashboard \
    mcr.microsoft.com/dotnet/aspire-dashboard:9.5
```

## [PowerShell](#tab/powershell)

```powershell
docker run --rm -it -p 18888:18888 -p 4317:18889 -d --name aspire-dashboard `
    mcr.microsoft.com/dotnet/aspire-dashboard:9.5
```

---

The preceding Docker command:

- Starts a container from the `mcr.microsoft.com/dotnet/aspire-dashboard:9.5` image.
- The container instance exposing two ports:
  - Maps the dashboard's OTLP port `18889` to the host's port `4317`. Port `4317` receives OpenTelemetry data from apps. Apps send data using [OpenTelemetry Protocol (OTLP)](https://opentelemetry.io/docs/specs/otlp/).
  - Maps the dashboard's port `18888` to the host's port `18888`. Port `18888` has the dashboard UI. Navigate to `http://localhost:18888` in the browser to view the dashboard.

For more information, see the [Standalone Aspire dashboard](standalone.md).

## Configuration

The dashboard is configured when it starts up. Configuration includes frontend and OTLP addresses, the resource service endpoint, authentication, telemetry limits and more.

For more information, see [Aspire dashboard configuration](configuration.md).

## Architecture

The dashboard user experience is built with a variety of technologies. The frontend is built with [📦 Grpc.AspNetCore](https://www.nuget.org/packages/Grpc.AspNetCore) NuGet package to the resource server. Consider the following diagram that illustrates the architecture of the Aspire dashboard:

:::image type="content" source="media/architecture-diagram.png" lightbox="media/architecture-diagram.png" alt-text="A diagram showing the architecture of the Aspire dashboard.":::

## Security

The Aspire dashboard offers powerful insights to your apps. The UI displays information about resources, including their configuration, console logs and in-depth telemetry.

Data displayed in the dashboard can be sensitive. For example, configuration can include secrets in environment variables, and telemetry can include sensitive runtime data. Care should be taken to secure access to the dashboard.

For more information, see [Aspire dashboard security considerations](security-considerations.md).

## Next steps

> [!div class="nextstepaction"]
> [Explore the Aspire dashboard](explore.md)
