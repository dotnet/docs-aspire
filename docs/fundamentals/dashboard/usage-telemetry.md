---
title: .NET Aspire dashboard telemetry
description: Learn about what telemetry the .NET Aspire dashboard sends and how to opt out.
ms.date: 03/05/2025
---

# .NET Aspire dashboard telemetry

The Aspire dashboard includes a telemetry feature that collects usage data when the dashboard is launched through Visual Studio or Visual Studio Code as part of a running Aspire application. This information is sent to Microsoft to help the Aspire team understand how the dashboard is used and help improve the product. Exception information is also sent when unhandled exceptions occur in the dashboard.

## Scope

.NET Aspire dashboard usage telemetry is supported when using Visual Studio or Visual Studio Code to run an Aspire application containing a dashboard resource.

Telemetry is collected only when the Aspire dashboard is open in the browser and the instance of Visual Studio or Visual Studio Code has not opted out of telemetry collection.

### How to opt out

.NET Aspire dashboard telemetry is enabled by default for `Aspire >= 9.2` when launched through `Visual Studio >= 17.14` or `C# Dev Kit >= [VSC RELEASE VERSION]`.

To opt out, you may either:

- Uncheck the "Enable dashboard telemetry" setting in the Aspire dashboard settings page. This setting will apply to the current browser and is saved in browser localStorage.
- Set the `DOTNET_DASHBOARD_ENABLE_TELEMETRY` environment variable to `false`. This will apply to all users accessing the Aspire dashboard.

#### Configure OTLP HTTP with app host

If the dashboard and your app are started by the app host, the dashboard OTLP endpoints are configured in the app host's _launchSettings.json_ file.

Consider the following example JSON file:

:::code language="json" source="snippets/BrowserTelemetry/BrowserTelemetry.AppHost/Properties/launchSettings.json" highlight="12,25":::

The preceding launch settings JSON file configures all profiles to include the `DOTNET_DASHBOARD_OTLP_HTTP_ENDPOINT_URL` environment variable.

#### Configure OTLP HTTP with standalone dashboard

If the dashboard is used standalone, without the rest of .NET Aspire, the OTLP HTTP endpoint is enabled by default on port `18890`. However, the port must be mapped when the dashboard container is started:

### [Bash](#tab/bash)

```bash
docker run --rm -it -d \
    -p 18888:18888 \
    -p 4317:18889 \
    -p 4318:18890 \
    --name aspire-dashboard \
    mcr.microsoft.com/dotnet/aspire-dashboard:9.0
```

### [PowerShell](#tab/powershell)

```powershell
docker run --rm -it -d `
    -p 18888:18888 `
    -p 4317:18889 `
    -p 4318:18890 `
    --name aspire-dashboard `
    mcr.microsoft.com/dotnet/aspire-dashboard:9.0
```

---

The preceding command runs the dashboard container and maps gRPC OTLP to port `4317` and HTTP OTLP to port `4318`.

### CORS configuration

By default, browser apps are restricted from making cross domain API calls. This impacts sending telemetry to the dashboard because the dashboard and the browser app are always on different domains. Configuring CORS in the .NET Aspire dashboard removes the restriction.

If the dashboard and your app are started by the app host, no CORS configuration is required. .NET Aspire automatically configures the dashboard to allow all resource origins.

If the dashboard is used standlone then CORS must be configured manually. The domain used to view the browser app must be configured as an allowed origin by specifing the `DASHBOARD__OTLP__CORS__ALLOWEDORIGINS` environment variable when the dashboard container is started:

### [Bash](#tab/bash)

```bash
docker run --rm -it -d \
    -p 18888:18888 \
    -p 4317:18889 \
    -p 4318:18890 \
    -e DASHBOARD__OTLP__CORS__ALLOWEDORIGINS=https://localhost:8080 \
    --name aspire-dashboard \
    mcr.microsoft.com/dotnet/aspire-dashboard:9.0
```

### [PowerShell](#tab/powershell)

```powershell
docker run --rm -it -d `
    -p 18888:18888 `
    -p 4317:18889 `
    -p 4318:18890 `
    -e DASHBOARD__OTLP__CORS__ALLOWEDORIGINS=https://localhost:8080 `
    --name aspire-dashboard `
    mcr.microsoft.com/dotnet/aspire-dashboard:9.0
```

---

The preceding command runs the dashboard container and configures `https://localhost:8080` as an allowed origin. That means a browser app that is accessed using `https://localhost:8080` has permission to send the dashboard telemetry.

Multiple origins can be allowed with a comma separated value. Or all origins can be allowed with the `*` wildcard. For example, `DASHBOARD__OTLP__CORS__ALLOWEDORIGINS=*`.

For more information, see [.NET Aspire dashboard configuration: OTLP CORS](configuration.md#otlp-cors).

### OTLP endpoint security

Dashboard OTLP endpoints can be secured with API key authentication. When enabled, HTTP OTLP requests to the dashboard must include the API key as the `x-otlp-api-key` header. By default a new  API key is generated each time the dashboard is run.

API key authentication is automatically enabled when the dashboard is run from the app host. Dashboard authentication can be disabled by setting `DOTNET_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS` to `true` in the app host's _launchSettings.json_ file.

OTLP endpoints are unsecured by default in the standalone dashboard.

## Browser app configuration

A browser app uses the [JavaScript OTEL SDK](https://opentelemetry.io/docs/languages/js/getting-started/browser/) to send telemetry to the dashboard. Successfully sending telemetry to the dashboard requires the SDK to be correctly configured.

### OTLP exporter

OTLP exporters must be included in the browser app and configured with the SDK. For example, exporting distributed tracing with OTLP uses the [@opentelemetry/exporter-trace-otlp-proto](https://www.npmjs.com/package/@opentelemetry/exporter-trace-otlp-proto) package.

When OTLP is added to the SDK, OTLP options must be specified. OTLP options includes:

- `url`: The address that HTTP OTLP requests are made to. The address should be the dashboard HTTP OTLP endpoint and the path to the OTLP HTTP API. For example, `https://localhost:4318/v1/traces` for the trace OTLP exporter. If the browser app is launched by the app host then the HTTP OTLP endpoint is available from the `OTEL_EXPORTER_OTLP_ENDPOINT` environment variable.

- `headers`: The headers sent with requests. If OTLP endpoint API key authentication is enabled the `x-otlp-api-key` header must be sent with OTLP requests. If the browser app is launched by the app host then the API key is available from the `OTEL_EXPORTER_OTLP_HEADERS` environment variable.

### Browser metadata

When a browser app is configured to collect distributed traces, the browser app can set the trace parent a browser's spans using the `meta` element in the HTML. The value of the `name="traceparent"` meta element should correspond to the current trace.

In a .NET app, for example, the trace parent value would likely be assigned from the <xref:System.Diagnostics.Activity.Current?displayProperty=nameWithType> and passing its <xref:System.Diagnostics.Activity.Id?displayProperty=nameWithType> value as the `content`. For example, consider the following Razor code:

```razor
<head>
    @if (Activity.Current is { } currentActivity)
    {
        <meta name="traceparent" content="@currentActivity.Id" />
    }
    <!-- Other elements omitted for brevity... -->
</head>
```

The preceding code sets the `traceparent` meta element to the current activity ID.

## Example browser telemetry code

The following JavaScript code demonstrates the initialization of the OpenTelemetry JavaScript SDK and the sending of telemetry data to the dashboard:

:::code language="javascript" source="snippets/BrowserTelemetry/BrowserTelemetry.Web/Scripts/index.js":::

The preceding JavaScript code defines an `initializeTelemetry` function that expects the OTLP endpoint URL, the headers, and the resource attributes. These parameters are provided by the consuming browser app that pulls them from the environment variables set by the app host. Consider the following Razor code:

:::code language="razor" source="snippets/BrowserTelemetry/BrowserTelemetry.Web/Pages/Shared/_Layout.cshtml" highlight="31-38":::

> [!TIP]
> The bundling and minification of the JavaScript code is beyond the scope of this article.

For the complete working example of how to configure the JavaScript OTEL SDK to send telemetry to the dashboard, see the [browser telemetry sample](https://github.com/dotnet/aspire/tree/main/playground/BrowserTelemetry).

## See also

- [.NET Aspire dashboard configuration](configuration.md)
- [Standalone .NET Aspire dashboard](standalone.md)
- [Browser telemetry sample](https://github.com/dotnet/aspire/tree/main/playground/BrowserTelemetry)
