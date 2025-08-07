---
title: Enable browser telemetry
description: Learn how to enable browser telemetry in the .NET Aspire dashboard.
ms.date: 04/15/2025
---

# Enable browser telemetry

The .NET Aspire dashboard can be configured to receive telemetry sent from browser apps. This feature is useful for monitoring client-side performance and user interactions. Browser telemetry requires additional dashboard configuration and that the [JavaScript OTEL SDK](https://opentelemetry.io/docs/languages/js/getting-started/browser/) is added to the browser apps.

This article discusses how to enable browser telemetry in the .NET Aspire dashboard.

## Dashboard configuration

Browser telemetry requires the dashboard to enable these features:

- OTLP HTTP endpoint. This endpoint is used by the dashboard to receive telemetry from browser apps.
- Cross-origin resource sharing (CORS). CORS allows browser apps to make requests to the dashboard.

### OTLP configuration

The .NET Aspire dashboard receives telemetry through OTLP endpoints. [HTTP OTLP endpoints](https://opentelemetry.io/docs/specs/otlp/#otlphttp) and gRPC OTLP endpoints are supported by the dashboard. Browser apps must use HTTP OLTP to send telemetry to the dashboard because browser apps don't support gRPC.

To configure the gPRC or HTTP endpoints, specify the following environment variables:

- `ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL`: The gRPC endpoint to which the dashboard connects for its data.
- `ASPIRE_DASHBOARD_OTLP_HTTP_ENDPOINT_URL`: The HTTP endpoint to which the dashboard connects for its data.

Configuration of the HTTP OTLP endpoint depends on whether the dashboard is started by the AppHost or is run standalone.

#### Configure OTLP HTTP with app host

If the dashboard and your app are started by the app host, the dashboard OTLP endpoints are configured in the app host's _launchSettings.json_ file.

Consider the following example JSON file:

:::code language="json" source="snippets/BrowserTelemetry/BrowserTelemetry.AppHost/Properties/launchSettings.json" highlight="12,25":::

The preceding launch settings JSON file configures all profiles to include the `ASPIRE_DASHBOARD_OTLP_HTTP_ENDPOINT_URL` environment variable.

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

API key authentication is automatically enabled when the dashboard is run from the app host. Dashboard authentication can be disabled by setting `ASPIRE_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS` to `true` in the app host's _launchSettings.json_ file.

OTLP endpoints are unsecured by default in the standalone dashboard.

## Browser app configuration

A browser app uses the [JavaScript OTEL SDK](https://opentelemetry.io/docs/languages/js/getting-started/browser/) to send telemetry to the dashboard. Successfully sending telemetry to the dashboard requires the SDK to be correctly configured.

### OTLP exporter

OTLP exporters must be included in the browser app and configured with the SDK. For example, exporting distributed tracing with OTLP uses the [@opentelemetry/exporter-trace-otlp-proto](https://www.npmjs.com/package/@opentelemetry/exporter-trace-otlp-proto) package.

When OTLP is added to the SDK, OTLP options must be specified. OTLP options includes:

- `url`: The address that HTTP OTLP requests are made to. The address should be the dashboard HTTP OTLP endpoint and the path to the OTLP HTTP API. For example, `https://localhost:4318/v1/traces` for the trace OTLP exporter. If the browser app is launched by the AppHost then the HTTP OTLP endpoint is available from the `OTEL_EXPORTER_OTLP_ENDPOINT` environment variable.

- `headers`: The headers sent with requests. If OTLP endpoint API key authentication is enabled the `x-otlp-api-key` header must be sent with OTLP requests. If the browser app is launched by the AppHost then the API key is available from the `OTEL_EXPORTER_OTLP_HEADERS` environment variable.

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
