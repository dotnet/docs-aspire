---
title: .NET Aspire dashboard overview
description: Explore the .NET Aspire dashboard features through the .NET Aspire Starter app.
ms.date: 12/08/2023
ms.topic: reference
---

# .NET Aspire dashboard overview

[.NET Aspire](../get-started/aspire-overview.md) project templates offer a sophisticated dashboard for comprehensive app monitoring and inspection. This dashboard allows you to track closely various aspects of your application, including logs, traces, and environment configurations, in real-time. It's purpose-built to enhance the local development experience, providing an insightful overview of your app's state and structure.

In the upcoming sections, you'll discover how to create a .NET Aspire app and embark on the following tasks:

> [!div class="checklist"]
>
> - Investigate the dashboard's capabilities by using the app generated from the project template as explained in the [Quickstart: Build your first .NET Aspire app.](../get-started/build-your-first-aspire-app.md)
>
> - Delve into the features of the .NET Aspire dashboard app.

The screenshots featured in this article showcase the dark theme. For more details on theme selection, refer to [Theme selection](#theme-selection).

## Explore the .NET Aspire dashboard

The .NET Aspire dashboard is only visible while the **AppHost** app is running and will launch automatically when you start the project. The left navigation provides links to the different parts of the dashboard, each of which you'll explore in the following sections. Additionally, the cog icon in the upper right of the dashboard provides access to the settings page, which allows you to configure your dashboard experience.

## Resources page

The **Resources** page is the default home page of the .NET Aspire dashboard. This page lists all of the .NET projects, containers, and executables included in your .NET Aspire app. For example, the starter application includes two projects:

- **apiservice**: A back-end API for the .NET Aspire app built using Minimal APIs.
- **webfrontend**: The front-end UI for the .NET Aspire app built using Blazor.

The dashboard also provides essential details about each resource:

- **Type**: Displays whether the resource is a project, container, or executable.
- **Name**: The name of the resource.
- **State**: Displays whether or not the resource is currently running.
  - **Errors**: Within the **State** column, errors are displayed as a badge with the error count. It's useful to understand quickly what resources are reporting errors. Selecting the badge takes you to the [semantic logs](#structured-logs-page) for that resource with the filter at an error level.
- **Start Time**: When the resource started running.
- **Source**: The location of the resource on the device.
- **Endpoints**: The URL(s) to reach the running resource directly.
- **Environment**: The environment variables that were loaded during startup.
- **Logs**: A link to the resource logs page.

 Consider the following screenshot of the resources page:

:::image type="content" source="media/dashboard/projects.png" lightbox="media/dashboard/projects.png" alt-text="A screenshot of the .NET Aspire dashboard Resources page.":::

The search bar in the upper right of the dashboard also provides the option to filter the list, which is useful for .NET Aspire apps with many resources. To select the types of resources that are displayed, drop down the arrow to the left of the filter textbox:

:::image type="content" source="media/dashboard/select-resource-type.png" alt-text="A screenshot of the resource type selector list in the .NET Aspire dashboard Resources page.":::

In this example, only containers are displayed in the list. For example, if you enable **Use Redis for caching** when creating a .NET Aspire project, you should see a Redis container listed:

:::image type="content" source="media/dashboard/resources-filtered-containers.png" lightbox="media/dashboard/resources-filtered-containers.png" alt-text="A screenshot of the .NET Aspire dashboard Resources page filtered to show only containers.":::

Executables are stand-alone processes. You can configure a .NET Aspire app to run a stand-alone executable during startup, though the default starter templates do not include any executables by default.

The following screenshot shows an example of a project that has errors:

:::image type="content" source="media/dashboard/projects-errors.png" lightbox="media/dashboard/projects-errors.png" alt-text="A screenshot of the .NET Aspire dashboard Resources page, showing a project with errors.":::

Selecting the error count badge navigates to the [Structured logs](#structured-logs-page) page with a filter applied to show only the logs relevant to the resource:

:::image type="content" source="media/dashboard/structured-logs-errors.png" lightbox="media/dashboard/structured-logs-errors.png" alt-text="A screenshot of the .NET Aspire dashboard Structured logs page, showing a filter applied to show only the logs relevant to the resource.":::

To see the log entry in detail for the error, select the **View** button to open a window below the list with the structured log entry details:

:::image type="content" source="media/dashboard/structured-logs-errors-view.png" lightbox="media/dashboard/structured-logs-errors-view.png" alt-text="A screenshot of the .NET Aspire dashboard Structured logs page, showing a lower window with the structured log entry details.":::

For more information and examples of Structured logs, see the [Structured logs page](#structured-logs-page) section.

## Monitoring pages

The .NET Aspire dashboard provides a variety of ways to view logs, traces, and metrics for your app. This information enables you to track the behavior and performance of your app and to diagnose any issues that arise.

### Console logs page

The **Console logs** page displays text that each resource in you app has sent to standard output. Logs are a useful way to monitor the health of your app and diagnose issues. Logs are displayed differently depending on the source, such as project, container, or executable.

When you open the Console logs page, you must select a source in the **Select a resource** drop-down list.

If you select a project, the live logs are rendered with a stylized set of colors that correspond to the severity of the log; green for information as an example. Consider the following example screenshot of project logs with the `apiservice` project selected:

:::image type="content" source="media/dashboard/project-logs.png" lightbox="media/dashboard/project-logs.png" alt-text="A screenshot of the .NET Aspire dashboard Console logs page with a project source selected.":::

When errors occur, they're styled in the logs such that they're easy to identify. Consider the following example screenshot of project logs with errors:

:::image type="content" source="media/dashboard/project-logs-error.png" lightbox="media/dashboard/project-logs-error.png" alt-text="A screenshot of the .NET Aspire dashboard Project logs page, showing logs with errors.":::

If you select a container or executable, formatting is different from a project but verbose behavior information is still available. Consider the following example screenshot of a container log with the `cache` container selected:

:::image type="content" source="media/dashboard/container-logs.png" lightbox="media/dashboard/container-logs.png" alt-text="A screenshot of the .NET Aspire dashboard Console logs page with a container source selected.":::

### Structured logs page

.NET Aspire automatically configures your projects with logging using OpenTelemetry. Navigate to the **Structured logs** page to view the semantic logs for your .NET Aspire app. [Semantic, or structured logging](https://github.com/NLog/NLog/wiki/How-to-use-structured-logging) makes it easier to store and query log-events, as the log-event message-template and message-parameters are preserved, instead of just transforming them into a formatted message. You'll notice a clean structure for the different logs displayed on the page using columns:

- **Resource**: The resource the log originated from.
- **Level**: The log level of the entry, such as information, warning, or error.
- **Timestamp**: The time that the log occurred.
- **Message**: The details of the log.
- **Trace**: A link to the relevant trace for the log, if applicable.
- **Details**: Additional details or metadata about the log entry.

Consider the following example screenshot of semantic logs:

:::image type="content" source="media/dashboard/structured-logs.png" lightbox="media/dashboard/structured-logs.png" alt-text="A screenshot of the .NET Aspire dashboard Semantic logs page.":::

#### Filter structured logs

The structured logs page also provides a search bar to filter the logs by service, level, or message. You use the **Level** drop down to filter by log level. For advanced filtering options, select the filter icon button to open the advanced filter dialog.

Consider the following screenshots showing the structured logs, filtered to display items with "Hosting" in the message text:

:::image type="content" source="media/dashboard/structured-logs-filtered.png" lightbox="media/dashboard/structured-logs-filtered.png" alt-text="A screenshot of the .NET Aspire dashboard Structured logs page, showing a filter that displayed only items with Hosting in the message text.":::

### Traces page

Navigate to the **Traces** page to view all of the traces for your app. .NET Aspire automatically configures tracing for the different projects in your app. Distributed tracing is a diagnostic technique that helps engineers localize failures and performance issues within applications, especially those that may be distributed across multiple machines or processes. For more information, see [.NET distributed tracing](/dotnet/core/diagnostics/distributed-tracing). This technique tracks requests through an application and correlates work done by different application components. Traces also help identify how long different stages of the request took to complete. The traces page displays the following information:

- **Timestamp**: When the trace completed.
- **Name**: The name of the trace, prefixed with the project name.
- **Spans**: The resources involved in the request.
- **Duration**: The time it took to complete the request.

#### Filter traces

The traces page also provides a search bar to filter the traces by name or span. Apply a filter, and notice the trace results are updated immediately. Consider the following screenshot of traces with a filter applied to `SET` and notice how the search term is highlighted in the results:

:::image type="content" source="media/dashboard/trace-view-filter.png" lightbox="media/dashboard/trace-view-filter.png" alt-text="A screenshot of the .NET Aspire dashboard Traces page, showing a filter applied to show only traces with the term 'weather'.":::

#### Trace details

The trace details page contains various details pertinent to the request, including:

- **Trace Detail**: When the trace started.
- **Duration**: The time it took to complete the request.
- **Resources**: The number of resources involved in the request.
- **Depth**: The number of layers involved in the request.
- **Total Spans**: The total number of spans involved in the request.

Each span is represented as a row in the table, and contains a **Name**. Spans also display the error icon if an error occurred within that particular span of a the trace. Spans that have a type of client/consumer, but don't have a span on the server, show an arrow icon and then the destination address. This represents a client call to a system outside of the .NET Aspire application. For example, an HTTP request an external web API, or a database call.

Within the trace details page, there's a **View Logs** button that takes you to the structured logs page with a filter applied to show only the logs relevant to the request. Consider an example screenshot depicting the structured logs page with a filter applied to show only the logs relevant to the trace:

:::image type="content" source="media/dashboard/structured-logs-trace-errors.png" lightbox="media/dashboard/structured-logs-trace-errors.png" alt-text="A screenshot of the .NET Aspire dashboard Structured logs page, showing a filter applied to show only the logs relevant to the trace.":::

The structured logs page is discussed in more detail in the [Structured logs page](#structured-logs-page) section.

#### Trace examples

Each trace has a color, which is generated to help differentiate between spans—one color per resource. The colors are reflected in both the _traces page_ and the _trace detail view_. When traces depict an arrow icon, those icons are colorized as well to match the span of the target trace. Consider the following example screenshot of traces:

:::image type="content" source="media/dashboard/traces.png" lightbox="media/dashboard/traces.png" alt-text="A screenshot of the .NET Aspire dashboard Traces page.":::

You can also select the **View** button to navigate to a detailed view of the request and the duration of time it spent traveling through each application layer. Consider an example selection of a trace to view its details:

:::image type="content" source="media/dashboard/trace.png" lightbox="media/dashboard/trace.png" alt-text="A screenshot of the .NET Aspire dashboard Trace details page.":::

When errors are present, the page renders an error icon next to the trace name. Consider an example screenshot of traces with errors:

:::image type="content" source="media/dashboard/traces-errors.png" lightbox="media/dashboard/traces-errors.png" alt-text="A screenshot of the .NET Aspire dashboard Traces page, showing traces with errors.":::

And the corresponding detailed view of the trace with errors:

:::image type="content" source="media/dashboard/trace-view-errors.png" lightbox="media/dashboard/trace-view-errors.png" alt-text="A screenshot of the .NET Aspire dashboard Trace details page, showing a trace with errors.":::

## Metrics page

Navigate to the **Metrics** page to view the metrics for your app. .NET Aspire automatically configures metrics for the different projects in your app. Metrics are a way to measure the health of your application and can be used to monitor the performance of your app over time.

Each metric-publishing project in your app will have its own metrics. The metrics page displays a selection pane for each top-level meter and the corresponding instruments that you can select to view the metric.

Consider the following example screenshot of the metrics page, with the `webfrontend` project selected and the `System.Net.Http` meter's `http.client.request.duration` metric selected:

:::image type="content" source="media/dashboard/metrics-view.png" lightbox="media/dashboard/metrics-view.png" alt-text="A screenshot of the .NET Aspire dashboard Metrics page.":::

For more information about metrics, see [Built-in Metrics in .NET](/dotnet/core/diagnostics/built-in-metrics).

## Theme selection

By default, the theme is set to follow the System theme, which means the dashboard will use the same theme as your operating system. You can also select the **Light** or **Dark** theme to override the system theme. Theme selections are persisted.

The following screenshot shows the theme selection dialog, with the default System theme selected:

:::image type="content" source="media/dashboard/theme-selection.png" lightbox="media/dashboard/theme-selection.png" alt-text="The .NET Aspire dashboard Settings dialog, showing the System theme default selection.":::

If you prefer the Light theme, you can select it from the theme selection dialog:

:::image type="content" source="media/dashboard/theme-selection-light.png" lightbox="media/dashboard/theme-selection-light.png" alt-text="The .NET Aspire dashboard Settings dialog, showing the Light theme selection.":::
