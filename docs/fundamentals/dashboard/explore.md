---
title: Explore .NET Aspire dashboard
description: Explore the .NET Aspire dashboard features through the .NET Aspire Starter app.
ms.date: 04/23/2024
ms.topic: reference
---

# Explore the .NET Aspire dashboard

In the upcoming sections, you'll discover how to create a .NET Aspire app and embark on the following tasks:

> [!div class="checklist"]
>
> - Investigate the dashboard's capabilities by using the app generated from the project template as explained in the [Quickstart: Build your first .NET Aspire app.](../../get-started/build-your-first-aspire-app.md)
>
> - Delve into the features of the .NET Aspire dashboard app.

The screenshots featured in this article showcase the dark theme. For more details on theme selection, refer to [Theme selection](#theme-selection).

## Dashboard authentication

When you run a .NET Aspire app host, the orchestrator starts up all the app's dependent resources and then opens a browser window to the dashboard. The .NET Aspire dashboard requires token-based authentication for its users because it displays environment variables and other sensitive information.

When the dashboard is launched from Visual Studio or Visual Studio Code (with the [C# Dev Kit extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)), the login page is bypassed, and the dashboard opens directly. This is the typical developer <kbd>F5</kbd> experience, and the authentication login flow is automated by the .NET Aspire tooling.

However, if you start the app host from the command line, you're presented with the login page. The console window displays a URL that you can click on to open the dashboard in your browser.

:::image type="content" source="media/explore/dotnet-run-login-url.png" lightbox="media/explore/dotnet-run-login-url.png" alt-text=".NET CLI run command output, showing the login URL with token query string.":::

The URL contains a token query string (with the token value mapped to the `t` name part) that's used to _log in_ to the dashboard. If your console supports it, you can hold the <kbd>Ctrl</kbd> key and then click the link to open the dashboard in your browser. This method is easier than copying the token from the console and pasting it into the login page. If you end up on the dashboard login page without either of the previously described methods, you can always return to the console to copy the token.

:::image type="content" source="media/explore/aspire-login.png" lightbox="media/explore/aspire-login.png" alt-text=".NET Aspire dashboard login page.":::

The login page accepts a token and provides helpful instructions on how to obtain the token, as shown in the following screenshot:

:::image type="content" source="media/explore/aspire-login-help.png" lightbox="media/explore/aspire-login-help.png" alt-text=".NET Aspire dashboard login page with instructions on how to obtain the token.":::

After copying the token from the console and pasting it into the login page, select the **Log in** button.

:::image type="content" source="media/explore/aspire-login-filled.png" lightbox="media/explore/aspire-login-filled.png" alt-text=".NET Aspire dashboard login page with the token pasted into the textbox.":::

The dashboard persists the token as a browser session cookie. Session cookies are temporary and only valid for the session. If you close the browser, the session cookie is deleted, and you'd need to log in again. For more information, see [Security considerations for running the .NET Aspire dashboard](security-considerations.md).

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

:::image type="content" source="media/explore/projects.png" lightbox="media/explore/projects.png" alt-text="A screenshot of the .NET Aspire dashboard Resources page.":::

You can obtain full details about each resource by selecting the **View** link in the **Details** column:

:::image type="content" source="media/explore/resource-details.png" lightbox="media/explore/resource-details.png" alt-text="A screenshot of the .NET Aspire dashboard Resources page with the details of a selected resource displayed..":::

The search bar in the upper right of the dashboard also provides the option to filter the list, which is useful for .NET Aspire apps with many resources. To select the types of resources that are displayed, drop down the arrow to the left of the filter textbox:

:::image type="content" source="media/explore/select-resource-type.png" alt-text="A screenshot of the resource type selector list in the .NET Aspire dashboard Resources page.":::

In this example, only containers are displayed in the list. For example, if you enable **Use Redis for caching** when creating a .NET Aspire project, you should see a Redis container listed:

:::image type="content" source="media/explore/resources-filtered-containers.png" lightbox="media/explore/resources-filtered-containers.png" alt-text="A screenshot of the .NET Aspire dashboard Resources page filtered to show only containers.":::

Executables are stand-alone processes. You can configure a .NET Aspire app to run a stand-alone executable during startup, though the default starter templates do not include any executables by default.

The following screenshot shows an example of a project that has errors:

:::image type="content" source="media/explore/projects-errors.png" lightbox="media/explore/projects-errors.png" alt-text="A screenshot of the .NET Aspire dashboard Resources page, showing a project with errors.":::

Selecting the error count badge navigates to the [Structured logs](#structured-logs-page) page with a filter applied to show only the logs relevant to the resource:

:::image type="content" source="media/explore/structured-logs-errors.png" lightbox="media/explore/structured-logs-errors.png" alt-text="A screenshot of the .NET Aspire dashboard Structured logs page, showing a filter applied to show only the logs relevant to the resource.":::

To see the log entry in detail for the error, select the **View** button to open a window below the list with the structured log entry details:

:::image type="content" source="media/explore/structured-logs-errors-view.png" lightbox="media/explore/structured-logs-errors-view.png" alt-text="A screenshot of the .NET Aspire dashboard Structured logs page, showing a lower window with the structured log entry details.":::

For more information and examples of Structured logs, see the [Structured logs page](#structured-logs-page) section.

> [!NOTE]
> The resources page isn't available if the dashboard is started without a configured resource service. The dashboard starts on the [Structured logs page](#structured-logs-page) instead. This is the default experience when the dashboard is run in standalone mode without additional configuration.
>
> For more information about configuring a resource service, see [Dashboard configuration](configuration.md).

## Monitoring pages

The .NET Aspire dashboard provides a variety of ways to view logs, traces, and metrics for your app. This information enables you to track the behavior and performance of your app and to diagnose any issues that arise.

### Console logs page

The **Console logs** page displays text that each resource in you app has sent to standard output. Logs are a useful way to monitor the health of your app and diagnose issues. Logs are displayed differently depending on the source, such as project, container, or executable.

When you open the Console logs page, you must select a source in the **Select a resource** drop-down list.

If you select a project, the live logs are rendered with a stylized set of colors that correspond to the severity of the log; green for information as an example. Consider the following example screenshot of project logs with the `apiservice` project selected:

:::image type="content" source="media/explore/project-logs.png" lightbox="media/explore/project-logs.png" alt-text="A screenshot of the .NET Aspire dashboard Console Logs page with a source selected.":::

When errors occur, they're styled in the logs such that they're easy to identify. Consider the following example screenshot of project logs with errors:

:::image type="content" source="media/explore/project-logs-error.png" lightbox="media/explore/project-logs-error.png" alt-text="A screenshot of the .NET Aspire dashboard Console Logs page, showing logs with errors.":::

If you select a container or executable, formatting is different from a project but verbose behavior information is still available. Consider the following example screenshot of a container log with the `cache` container selected:

:::image type="content" source="media/explore/container-logs.png" lightbox="media/explore/container-logs.png" alt-text="A screenshot of the .NET Aspire dashboard Console logs page with a container source selected.":::

### Structured logs page

.NET Aspire automatically configures your projects with logging using OpenTelemetry. Navigate to the **Structured logs** page to view the semantic logs for your .NET Aspire app. [Semantic, or structured logging](https://github.com/NLog/NLog/wiki/How-to-use-structured-logging) makes it easier to store and query log-events, as the log-event message-template and message-parameters are preserved, instead of just transforming them into a formatted message. You'll notice a clean structure for the different logs displayed on the page using columns:

- **Resource**: The resource the log originated from.
- **Level**: The log level of the entry, such as information, warning, or error.
- **Timestamp**: The time that the log occurred.
- **Message**: The details of the log.
- **Trace**: A link to the relevant trace for the log, if applicable.
- **Details**: Additional details or metadata about the log entry.

Consider the following example screenshot of semantic logs:

:::image type="content" source="media/explore/structured-logs.png" lightbox="media/explore/structured-logs.png" alt-text="A screenshot of the .NET Aspire dashboard Semantic logs page.":::

#### Filter structured logs

The structured logs page also provides a search bar to filter the logs by service, level, or message. You use the **Level** drop down to filter by log level. You can also filter by any log property by selecting the filter icon button, which will open the advanced filter dialog.

Consider the following screenshots showing the structured logs, filtered to display items with "Hosting" in the message text:

:::image type="content" source="media/explore/structured-logs-filtered.png" lightbox="media/explore/structured-logs-filtered.png" alt-text="A screenshot of the .NET Aspire dashboard Structured logs page, showing a filter that displayed only items with Hosting in the message text.":::

### Traces page

Navigate to the **Traces** page to view all of the traces for your app. .NET Aspire automatically configures tracing for the different projects in your app. Distributed tracing is a diagnostic technique that helps engineers localize failures and performance issues within applications, especially those that may be distributed across multiple machines or processes. For more information, see [.NET distributed tracing](/dotnet/core/diagnostics/distributed-tracing). This technique tracks requests through an application and correlates work done by different application components. Traces also help identify how long different stages of the request took to complete. The traces page displays the following information:

- **Timestamp**: When the trace completed.
- **Name**: The name of the trace, prefixed with the project name.
- **Spans**: The resources involved in the request.
- **Duration**: The time it took to complete the request. This column includes a radial icon that illustrates the duration of the request in comparison with the others in the list.

:::image type="content" source="media/explore/traces.png" lightbox="media/explore/traces.png" alt-text="A screenshot of the .NET Aspire dashboard Traces page.":::

#### Filter traces

The traces page also provides a search bar to filter the traces by name or span. Apply a filter, and notice the trace results are updated immediately. Consider the following screenshot of traces with a filter applied to `weather` and notice how the search term is highlighted in the results:

:::image type="content" source="media/explore/trace-view-filter.png" lightbox="media/explore/trace-view-filter.png" alt-text="A screenshot of the .NET Aspire dashboard Traces page, showing a filter applied to show only traces with the term 'weather'.":::

#### Trace details

The trace details page contains various details pertinent to the request, including:

- **Trace Detail**: When the trace started.
- **Duration**: The time it took to complete the request.
- **Resources**: The number of resources involved in the request.
- **Depth**: The number of layers involved in the request.
- **Total Spans**: The total number of spans involved in the request.

Each span is represented as a row in the table, and contains a **Name**. Spans also display the error icon if an error occurred within that particular span of the trace. Spans that have a type of client/consumer, but don't have a span on the server, show an arrow icon and then the destination address. This represents a client call to a system outside of the .NET Aspire application. For example, an HTTP request an external web API, or a database call.

Within the trace details page, there's a **View Logs** button that takes you to the structured logs page with a filter applied to show only the logs relevant to the request. Consider an example screenshot depicting the structured logs page with a filter applied to show only the logs relevant to the trace:

:::image type="content" source="media/explore/structured-logs-trace-errors.png" lightbox="media/explore/structured-logs-trace-errors.png" alt-text="A screenshot of the .NET Aspire dashboard Structured logs page, showing a filter applied to show only the logs relevant to the trace.":::

The structured logs page is discussed in more detail in the [Structured logs page](#structured-logs-page) section.

#### Trace examples

Each trace has a color, which is generated to help differentiate between spans — one color for each resource. The colors are reflected in both the _traces page_ and the _trace detail page_. When traces depict an arrow icon, those icons are colorized as well to match the span of the target trace. Consider the following example screenshot of traces:

:::image type="content" source="media/explore/traces.png" lightbox="media/explore/traces.png" alt-text="A screenshot of the .NET Aspire dashboard Traces page.":::

You can also select the **View** button to navigate to a detailed view of the request and the duration of time it spent traveling through each application layer. Consider an example selection of a trace to view its details:

:::image type="content" source="media/explore/trace.png" lightbox="media/explore/trace.png" alt-text="A screenshot of the .NET Aspire dashboard Trace details page.":::

For each span in the trace, select **View** to see more details:

:::image type="content" source="media/explore/trace-span-details.png" lightbox="media/explore/trace-span-details.png" alt-text="A screenshot of the .NET Aspire dashboard Trace details page with the details of a span displayed.":::

Scroll down in the span details pain to see full information. At the bottom of the span details pane, some span types, such as this call to a cache, show span event timings:

:::image type="content" source="media/explore/trace-span-event-details.png" lightbox="media/explore/trace-span-event-details.png" alt-text="A screenshot of the .NET Aspire dashboard Trace details page with the event timings for a span displayed.":::

When errors are present, the page renders an error icon next to the trace name. Consider an example screenshot of traces with errors:

:::image type="content" source="media/explore/traces-errors.png" lightbox="media/explore/traces-errors.png" alt-text="A screenshot of the .NET Aspire dashboard Traces page, showing traces with errors.":::

And the corresponding detailed view of the trace with errors:

:::image type="content" source="media/explore/trace-view-errors.png" lightbox="media/explore/trace-view-errors.png" alt-text="A screenshot of the .NET Aspire dashboard Trace details page, showing a trace with errors.":::

### Metrics page

Navigate to the **Metrics** page to view the metrics for your app. .NET Aspire automatically configures metrics for the different projects in your app. Metrics are a way to measure the health of your application and can be used to monitor the performance of your app over time.

Each metric-publishing project in your app will have its own metrics. The metrics page displays a selection pane for each top-level meter and the corresponding instruments that you can select to view the metric.

Consider the following example screenshot of the metrics page, with the `webfrontend` project selected and the `System.Net.Http` meter's `http.client.request.duration` metric selected:

:::image type="content" source="media/explore/metrics-view.png" lightbox="media/explore/metrics-view.png" alt-text="A screenshot of the .NET Aspire dashboard Metrics page.":::

In addition to the metrics chart, the metrics page includes an option to view the data as a table instead. Consider the following screenshot of the metrics page with the table view selected:

:::image type="content" source="media/explore/metrics-table-view.png" lightbox="media/explore/metrics-table-view.png" alt-text="A screenshot of the .NET Aspire dashboard Metrics page with the table view selected.":::

Under the chart, there is a list of filters you can apply to focus on the data that interests you. For example, in the following screenshot, the **http.request.method** field has been filtered to show only **GET** requests:

:::image type="content" source="media/explore/metrics-view-filtered.png" lightbox="media/explore/metrics-view-filtered.png" alt-text="A screenshot of the .NET Aspire dashboard Metrics page with a filter applied to the chart.":::

You can also choose to select the count of the displayed metric on the vertical access, instead of its values:

:::image type="content" source="media/explore/metrics-view-count.png" lightbox="media/explore/metrics-view-count.png" alt-text="A screenshot of the .NET Aspire dashboard Metrics page with the count option applied.":::

For more information about metrics, see [Built-in Metrics in .NET](/dotnet/core/diagnostics/built-in-metrics).

## Theme selection

By default, the theme is set to follow the System theme, which means the dashboard will use the same theme as your operating system. You can also select the **Light** or **Dark** theme to override the system theme. Theme selections are persisted.

The following screenshot shows the theme selection dialog, with the default System theme selected:

:::image type="content" source="media/explore/theme-selection.png" lightbox="media/explore/theme-selection.png" alt-text="The .NET Aspire dashboard Settings dialog, showing the System theme default selection.":::

If you prefer the Light theme, you can select it from the theme selection dialog:

:::image type="content" source="media/explore/theme-selection-light.png" lightbox="media/explore/theme-selection-light.png" alt-text="The .NET Aspire dashboard Settings dialog, showing the Light theme selection.":::

## Dashboard shortcuts

The .NET Aspire dashboard provides a variety of shortcuts to help you navigate the different parts of the dashboard. To display the keyboard shortcuts, press <kbd>Shift</kbd> + <kbd>?</kbd>. The following shortcuts are available:

**Panels**:

- <kbd>+</kbd>: Increase panel size.
- <kbd>-</kbd>: Decrease panel size.
- <kbd>Shift</kbd> + <kbd>r</kbd>: Reset panel size.
- <kbd>Shift</kbd> + <kbd>t</kbd>: Toggle panel orientation.
- <kbd>Shift</kbd> + <kbd>x</kbd>: Close panel.

**Page navigation**:

- <kbd>r</kbd>: Go to **Resources**.
- <kbd>c</kbd>: Go to **Console Logs**.
- <kbd>s</kbd>: Go to **Structured Logs**.
- <kbd>t</kbd>: Go to **Traces**.
- <kbd>m</kbd>: Go to **Metrics**.

**Site-wide navigation**:

- <kbd>?</kbd>: Got to **Help**.
- <kbd>Shift</kbd> + <kbd>s</kbd>: Go to **Settings**.
