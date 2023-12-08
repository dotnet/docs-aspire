---
title: .NET Aspire dashboard overview
description: Explore the .NET Aspire dashboard features through the .NET Aspire Starter app.
ms.date: 11/13/2023
ms.topic: reference
---

# .NET Aspire dashboard overview

[.NET Aspire](get-started/aspire-overview.md) project templates offer a sophisticated dashboard for comprehensive app monitoring and inspection. This dashboard allows you to closely track various aspects of your application, including logs, traces, and environment configurations, in real-time. It's purpose-built to enhance the local development experience, providing an insightful overview of your app's state and structure.

In the upcoming sections, you'll discover how to create a .NET Aspire app and embark on the following tasks:

> [!div class="checklist"]
>
> - Investigate the dashboard's capabilities by using the app generated from the project template as explained in the [Quickstart: Build your first .NET Aspire app.](get-started/build-your-first-aspire-app.md)
>
> - Delve into the features of the .NET Aspire dashboard app.

The screenshots featured in this article showcase the dark theme. For more details on theme selection, refer to [Theme selection](#theme-selection).

## Explore the .NET Aspire dashboard

The .NET Aspire dashboard is only visible while the AppHost app is running and will launch automatically when you start the project. The left navigation provides links to the different parts of the dashboard, each of which you'll explore in the following sections. Additionally, the cog icon in the upper right of the dashboard provides access to the settings page, which allows you to configure your dashboard experience.

## Projects page

The **Projects** page is the default home page of the .NET Aspire dashboard. This page lists all of the .NET projects included in your .NET Aspire app. For example, the starter application includes two projects:

- **apiservice**: A back-end API for the .NET Aspire app built using Minimal APIs.
- **webfrontend**: The front-end UI for the .NET Aspire app built using Blazor.

The dashboard also provides essential details about each project:

- **Name**: The name of the .NET project.
- **State**: Displays whether or not the app is currently running.
  - **Errors**: Within the **State** column, errors are displayed as a badge with the error count. It's useful to quickly understand what projects are reporting errors. Selecting the badge takes you to the [semantic logs](#structured-logs-page) for that resource with the filter at an error level.
- **Start Time**: When the application started running.
- **Process Id**: The process ID of the running project.
- **Source Location**: The location of the project on the device.
- **Endpoints**: The URL(s) to reach the running project directly.
- **Environment**: The environment variables that were loaded during startup.
- **Logs**: A link to the project logs page.

The search bar in the upper right of the dashboard also provides the option to filter the list of projects, which is useful for .NET Aspire apps with many resources. Consider the following screenshot of the projects page:

:::image type="content" source="media/dashboard/projects.png" lightbox="media/dashboard/projects.png" alt-text="A screenshot of the .NET Aspire dashboard Projects page.":::

The following screenshot shows an example of a project that has errors:

:::image type="content" source="media/dashboard/projects-errors.png" lightbox="media/dashboard/projects-errors.png" alt-text="A screenshot of the .NET Aspire dashboard Projects page, showing a project with errors.":::

Selecting the error count badge navigates to the [Structured logs](#structured-logs-page) page with a filter applied to show only the logs relevant to the project:

:::image type="content" source="media/dashboard/structured-logs-errors.png" lightbox="media/dashboard/structured-logs-errors.png" alt-text="A screenshot of the .NET Aspire dashboard Structured logs page, showing a filter applied to show only the logs relevant to the project.":::

To see the log entry in detail for the error, select the **View** button to open a modal dialog with the structured log entry details:

:::image type="content" source="media/dashboard/structured-logs-errors-view.png" lightbox="media/dashboard/structured-logs-errors-view.png" alt-text="A screenshot of the .NET Aspire dashboard Structured logs page, showing a modal dialog with the structured log entry details.":::

For more information and examples of Structured logs, see the [Structured logs page](#structured-logs-page) section.

## Containers page

Navigate to the **Containers** page to view a list of all of the container resources that are part of your .NET Aspire app. For example, if you enable **Use Redis for caching** when creating a .NET Aspire project, you should see a Redis container listed.

The dashboard also provides essential details about each container:

- **Name**: The name of the container.
- **Container Id**: The ID of the container.
- **State**: Displays whether or not the app is currently running.
  - **Errors**: Within the **State** column, errors are displayed as a badge with the error count. It's useful to quickly understand what containers are reporting errors. Selecting the badge takes you to the [semantic logs](#structured-logs-page) for that resource with the filter at an error level.
- **Start Time**: When the application started running.
- **Container Image**: The tag of the container image.
- **Ports**: The configured port for the container.
- **Endpoints**: The URL(s) to reach the running container directly.
- **Environment**: The environment variables that were loaded during startup.
- **Logs**: A link to the container logs page.

Consider the following screenshot of the containers page:

:::image type="content" source="media/dashboard/containers.png" lightbox="media/dashboard/containers.png" alt-text="A screenshot of the .NET Aspire dashboard Containers page.":::

## Executables page

Navigate to the **Executables** page to see a list of all of the executable resources that are part of your .NET Aspire app. You can configure a .NET Aspire app to run a stand-alone executable during startup, though the default starter templates do not include any executables by default.

The dashboard also provides essential details about each executable:

- **Name**: The name of the container.
- **State**: Displays whether or not the app is currently running.
- **Start Time**: When the application started running.
- **Process Id**: The process ID of the running executable.
- **Path**: The location of the executable on the device.
- **Working Directory**: The working directory of the executable.
- **Arguments**: The arguments passed to the executable.
- **Endpoints**: The URL(s) to reach the running executable directly.
- **Environment**: The environment variables that were loaded during startup.
- **Logs**: A link to the executable logs page.

Consider the following screenshot of the executables page:

:::image type="content" source="media/dashboard/executables.png" lightbox="media/dashboard/executables.png" alt-text="A screenshot of the .NET Aspire dashboard Executables page.":::

## Log pages

The .NET Aspire dashboard provides a variety of ways to view logs for your app. Logs are a useful way to monitor the health of your app and diagnose issues. Logs are divided into various groups based on the source of the log, such as project, container, or executable. The dashboard also provides a semantic log view that displays logs in a structured format.

### Project logs page

Navigate to the **Project logs** page to view the output from the various logging providers in your .NET projects. The log window watches for new logs and updates in real time as you use the app. The live logs are rendered with a stylized set of colors that correspond to the severity of the log; green for information as an example. Switch between the different projects in your app using the dropdown at the top of the page.

Consider the following example screenshot of project logs with the `apiservice` project selected:

:::image type="content" source="media/dashboard/project-logs.png" lightbox="media/dashboard/project-logs.png" alt-text="A screenshot of the .NET Aspire dashboard Project logs page.":::

When errors occur, they're styled in the logs such that they're easy to identify. Consider the following example screenshot of project logs with errors:

:::image type="content" source="media/dashboard/project-logs-error.png" lightbox="media/dashboard/project-logs-error.png" alt-text="A screenshot of the .NET Aspire dashboard Project logs page, showing logs with errors.":::

Using the project dropdown, you toggle between the different projects in your app. Consider the following example screenshot of project logs with the `webfrontend` project selected:

:::image type="content" source="media/dashboard/project-logs-web.png" lightbox="media/dashboard/project-logs-web.png" alt-text="A screenshot of the .NET Aspire dashboard Project logs page, showing logs from the webfrontend project.":::

### Container logs page

Navigate to the **Container logs** page to view the logs output by the containers in your .NET Aspire app. When the page first loads, you should see some initial logs from the Redis container. You can also switch between different containers using the drop down at the top of the page.

Consider the following example screenshot of container logs with the `cache` container selected:

:::image type="content" source="media/dashboard/container-logs.png" lightbox="media/dashboard/container-logs.png" alt-text="A screenshot of the .NET Aspire dashboard Container logs page.":::

### Executable logs page

Navigate to the **Executable logs** page to view the logs output by stand-alone executables in your .NET Aspire app. The .NET Aspire starter template doesn't include any executables, so the output view is empty. Consider the following example screenshot of executable logs:

:::image type="content" source="media/dashboard/executable-logs.png" lightbox="media/dashboard/executable-logs.png" alt-text="A screenshot of the .NET Aspire dashboard Executable logs page.":::

### Structured logs page

.NET Aspire automatically configures your projects with logging using OpenTelemetry. Navigate to the **Semantic logs** page to view the semantic logs for your .NET Aspire app. [Semantic, or structured logging](https://github.com/NLog/NLog/wiki/How-to-use-structured-logging) makes it easier to store and query log-events, as the log-event message-template and message-parameters are preserved, instead of just transforming them into a formatted message. You'll notice a clean structure for the different logs displayed on the page using columns:

- **Service**: The service the log originated from.
- **Level**: The log level of the entry, such as information, warning, or error.
- **Timestamp**: The time that the log occurred.
- **Message**: The details of the log.
- **Trace**: A link to the relevant trace for the log, if applicable.
- **Details**: Additional details or metadata about the log entry.

Consider the following example screenshot of semantic logs:

:::image type="content" source="media/dashboard/structured-logs.png" lightbox="media/dashboard/structured-logs.png" alt-text="A screenshot of the .NET Aspire dashboard Semantic logs page.":::

#### Filter structured logs

The structured logs page also provides a search bar to filter the logs by service, level, or message. You use the **Level** drop down to filter by log level. For advanced filtering options, select the filter icon button to open the advanced filter dialog.

Consider the following screenshots showing the dropdown/text input used to filter the **Field** and **Value** of the structured logs, given a specific comparison operator:

:::image type="content" source="media/dashboard/structured-logs-filter-field.png" lightbox="media/dashboard/structured-logs-filter-field.png" alt-text="A screenshot of the .NET Aspire dashboard Semantic logs page, showing the dropdown/text input used to filter the Field.":::

:::image type="content" source="media/dashboard/structured-logs-filter-compare.png" lightbox="media/dashboard/structured-logs-filter-compare.png" alt-text="A screenshot of the .NET Aspire dashboard Semantic logs page, showing the dropdown/text input used to filter the Value with a comparison operator.":::

## Traces page

Navigate to the **Traces** page to view all of the traces for your app. .NET Aspire automatically configures tracing for the different projects in your app. Distributed tracing is a diagnostic technique that helps engineers localize failures and performance issues within applications, especially those that may be distributed across multiple machines or processes. For more information, see [.NET distributed tracing](/dotnet/core/diagnostics/distributed-tracing). This technique tracks requests through an application and correlates work done by different application components. Traces also help identify how long different stages of the request took to complete. The traces page displays the following information:

- **Timestamp**: When the trace completed.
- **Name**: The name of the trace, prefixed with the project name.
- **Spans**: The resources involved in the request.
- **Duration**: The time it took to complete the request.

### Filter traces

The traces page also provides a search bar to filter the traces by name or span. Apply a filter, and notice the trace results are updated immediately. Consider the following screenshot of traces with a filter applied to `weather` and notice how the search term is highlighted in the results:

:::image type="content" source="media/dashboard/trace-view-filter.png" lightbox="media/dashboard/trace-view-filter.png" alt-text="A screenshot of the .NET Aspire dashboard Traces page, showing a filter applied to show only traces with the term 'weather'.":::

### Trace details

The trace details page contains various details pertinent to the request, including:

- **Trace Start**: When the trace started.
- **Duration**: The time it took to complete the request.
- **Services**: The number of services involved in the request.
- **Depth**: The number of layers involved in the request.
- **Total Spans**: The total number of spans involved in the request.

Each span is represented as a row in the table, and contains a **Name**. Spans also display the error icon if an error occurred within that particular span of a the trace. Spans that have a type of client/consumer, but don't have a span on the server, show an arrow icon and then the destination address. This represents a client call to a system outside of the .NET Aspire application. For example, an HTTP request an external web API, or a database call.

Within the trace details page, there's a **View Logs** button that takes you to the structured logs page with a filter applied to show only the logs relevant to the request. Consider an example screenshot depicting the structured logs page with a filter applied to show only the logs relevant to the trace:

:::image type="content" source="media/dashboard/structured-logs-trace-errors.png" lightbox="media/dashboard/structured-logs-trace-errors.png" alt-text="A screenshot of the .NET Aspire dashboard Structured logs page, showing a filter applied to show only the logs relevant to the trace.":::

The structured logs page is discussed in more detail in the [Structured logs page](#structured-logs-page) section.

### Trace examples

Each trace has a color, which is generated to help differentiate between spans—one color per resource. The colors are reflected in both the _traces page_ and the _trace detail view_. When traces depict an arrow icon, those icons are colorized as well to match the span of the target trace. Consider the following example screenshot of traces:

:::image type="content" source="media/dashboard/traces.png" lightbox="media/dashboard/traces.png" alt-text="A screenshot of the .NET Aspire dashboard Traces page.":::

You can also select the **View** button to navigate to a detailed view of the request and the duration of time it spent traveling through each application layer. Consider an example selection of a trace to view its details:

:::image type="content" source="media/dashboard/trace-view.png" lightbox="media/dashboard/trace-view.png" alt-text="A screenshot of the .NET Aspire dashboard Trace details page.":::

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
