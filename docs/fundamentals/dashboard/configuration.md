---
title: .NET Aspire dashboard configuration
description: .NET Aspire dashboard configuration options
ms.date: 09/11/2024
ms.topic: reference
---

# Dashboard configuration

The dashboard is configured when it starts up. Configuration includes frontend and OpenTelemetry Protocol (OTLP) addresses, the resource service endpoint, authentication, telemetry limits, and more.

When the dashboard is launched with the .NET Aspire app host project, it's automatically configured to display the app's resources and telemetry. Configuration is provided when launching the dashboard in [standalone mode](overview.md#standalone-mode).

There are many ways to provide configuration:

- Command line arguments.
- Environment variables. The `:` delimiter should be replaced with double underscore (`__`) in environment variable names.
- Optional JSON configuration file. The `DOTNET_DASHBOARD_CONFIG_FILE_PATH` setting can be used to specify a JSON configuration file.

Consider the following example, which shows how to configure the dashboard when started from a Docker container:

## [Bash](#tab/bash)

```bash
docker run --rm -it -p 18888:18888 -p 4317:18889 -d --name aspire-dashboard \
    -e DASHBOARD__TELEMETRYLIMITS__MAXLOGCOUNT='1000' \
    -e DASHBOARD__TELEMETRYLIMITS__MAXTRACECOUNT='1000' \
    -e DASHBOARD__TELEMETRYLIMITS__MAXMETRICSCOUNT='1000' \
    mcr.microsoft.com/dotnet/aspire-dashboard:8.0.0
```

## [PowerShell](#tab/powershell)

```powershell
docker run --rm -it -p 18888:18888 -p 4317:18889 -d --name aspire-dashboard `
    -e DASHBOARD__TELEMETRYLIMITS__MAXLOGCOUNT='1000' `
    -e DASHBOARD__TELEMETRYLIMITS__MAXTRACECOUNT='1000' `
    -e DASHBOARD__TELEMETRYLIMITS__MAXMETRICSCOUNT='1000' `
    mcr.microsoft.com/dotnet/aspire-dashboard:8.0.0
```

---

Alternatively, these same values could be configured using a JSON configuration file that is specified using `DOTNET_DASHBOARD_CONFIG_FILE_PATH`:

```json
{
  "Dashboard": {
    "TelemetryLimits": {
      "MaxLogCount": 1000,
      "MaxTraceCount": 1000,
      "MaxMetricsCount": 1000
    }
  }
}
```

> [!IMPORTANT]
> The dashboard displays information about resources, including their configuration, console logs and in-depth telemetry.
>
> Data displayed in the dashboard can be sensitive. For example, secrets in environment variables, and sensitive runtime data in telemetry. Care should be taken to configure the dashboard to secure access.
>
> For more information, see [dashboard security](security-considerations.md).

## Common configuration

| Option | Default Value | Description |
|--|--|--|
| `ASPNETCORE_URLS` | `http://localhost:18888` | One or more HTTP endpoints through which the dashboard frontend is served. The frontend endpoint is used to view the dashboard in a browser. When the dashboard is launched by the .NET Aspire app host this address is secured with HTTPS. Securing the dashboard with HTTPS is recommended. |
| `DOTNET_DASHBOARD_OTLP_ENDPOINT_URL` | `http://localhost:18889` | The [OTLP/gRPC](https://opentelemetry.io/docs/specs/otlp/#otlpgrpc) endpoint. This endpoint hosts an OTLP service and receives telemetry using gRPC. When the dashboard is launched by the .NET Aspire app host this address is secured with HTTPS. Securing the dashboard with HTTPS is recommended. |
| `DOTNET_DASHBOARD_OTLP_HTTP_ENDPOINT_URL` | `http://localhost:18890` | The [OTLP/HTTP](https://opentelemetry.io/docs/specs/otlp/#otlphttp) endpoint. This endpoint hosts an OTLP service and receives telemetry using Protobuf over HTTP. When the dashboard is launched by the .NET Aspire app host the OTLP/HTTP endpoint isn't configured by default. To configure an OTLP/HTTP endpoint with the app host, set an `DOTNET_DASHBOARD_OTLP_HTTP_ENDPOINT_URL` env var value in _launchSettings.json_. Securing the dashboard with HTTPS is recommended. |
| `DOTNET_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS` | `false` | Configures the dashboard to not use authentication and accepts anonymous access. This setting is a shortcut to configuring `Dashboard:Frontend:AuthMode` and `Dashboard:Otlp:AuthMode` to `Unsecured`. |
| `DOTNET_DASHBOARD_CONFIG_FILE_PATH` | `null` | The path for a JSON configuration file. If the dashboard is being run in a Docker container, then this is the path to the configuration file in a mounted volume. This value is optional. |
| `DOTNET_RESOURCE_SERVICE_ENDPOINT_URL` | `null` | The gRPC endpoint to which the dashboard connects for its data. If this value is unspecified, the dashboard shows telemetry data but no resource list or console logs. This setting is a shortcut to `Dashboard:ResourceService:Url`. |

## Frontend authentication

The dashboard frontend endpoint authentication is configured with `Dashboard:Frontend:AuthMode`. The frontend can be secured with OpenID Connect (OIDC) or browser token authentication.

Browser token authentication works by the frontend asking for a token. The token can either be entered in the UI or provided as a query string value to the login page. For example, `https://localhost:1234/login?t=TheToken`. When the token is successfully authenticated an auth cookie is persisted to the browser, and the browser is redirected to the app.

| Option | Default Value | Description |
|--|--|--|
| `Dashboard:Frontend:AuthMode` | `BrowserToken` | Can be set to `BrowserToken`, `OpenIdConnect` or `Unsecured`. `Unsecured` should only be used during local development. It's not recommended when hosting the dashboard publicly or in other settings. |
| `Dashboard:Frontend:BrowserToken` | `null` | Specifies the browser token. If the browser token isn't specified, then the dashboard generates one. Tooling that wants to automate logging in with browser token authentication can specify a token and open a browser with the token in the query string. A new token should be generated each time the dashboard is launched. |
| `Dashboard:Frontend:OpenIdConnect:NameClaimType` | `name` | Specifies one or more claim types that should be used to display the authenticated user's full name. Can be a single claim type or a comma-delimited list of claim types. |
| `Dashboard:Frontend:OpenIdConnect:UsernameClaimType` | `preferred_username` | Specifies one or more claim types that should be used to display the authenticated user's username. Can be a single claim type or a comma-delimited list of claim types. |
| `Dashboard:Frontend:OpenIdConnect:RequiredClaimType` | `null` | Specifies the claim that must be present for authorized users. Authorization fails without this claim. This value is optional. |
| `Dashboard:Frontend:OpenIdConnect:RequiredClaimValue` | `null` | Specifies the value of the required claim. Only used if `Dashboard:Frontend:OpenIdConnect:RequireClaimType` is also specified. This value is optional. |
| `Authentication:Schemes:OpenIdConnect:Authority` | `null` | URL to the identity provider (IdP). |
| `Authentication:Schemes:OpenIdConnect:ClientId` | `null` | Identity of the relying party (RP). |
| `Authentication:Schemes:OpenIdConnect:ClientSecret` | `null` | A secret that only the real RP would know. |
| Other properties of <xref:Microsoft.AspNetCore.Builder.OpenIdConnectOptions> | `null` | Values inside configuration section `Authentication:Schemes:OpenIdConnect:*` are bound to `OpenIdConnectOptions`, such as `Scope`. |

## OTLP authentication

The OTLP endpoint authentication is configured with `Dashboard:Otlp:AuthMode`. The OTLP endpoint can be secured with an API key or [client certificate](/aspnet/core/security/authentication/certauth) authentication.

API key authentication works by requiring each OTLP request to have a valid `x-otlp-api-key` header value. It must match either the primary or secondary key.

| Option | Default Value | Description |
|--|--|--|
| `Dashboard:Otlp:AuthMode` | `Unsecured` | Can be set to `ApiKey`, `Certificate` or `Unsecured`. `Unsecured` should only be used during local development. It's not recommended when hosting the dashboard publicly or in other settings. |
| `Dashboard:Otlp:PrimaryApiKey` | `null` | Specifies the primary API key. The API key can be any text, but a value with at least 128 bits of entropy is recommended. This value is required if auth mode is API key. |
| `Dashboard:Otlp:SecondaryApiKey` | `null` | Specifies the secondary API key. The API key can be any text, but a value with at least 128 bits of entropy is recommended. This value is optional. If a second API key is specified, then the incoming `x-otlp-api-key` header value can match either the primary or secondary key. |

## OTLP CORS

CORS (Cross-Origin Resource Sharing) can be configured to allow browser apps to send telemetry to the dashboard.

By default, browser apps are restricted from making cross domain API calls. This impacts sending telemetry to the dashboard because the dashboard and the browser app are always on different domains. To configure CORS, use the `Dashboard:Otlp:Cors` section and specify the allowed origins and headers:

```json
{
  "Dashboard": {
    "Otlp": {
      "Cors": {
        "AllowedOrigins": "http://localhost:5000,https://localhost:5001"
      }
    }
  }
}
```

Consider the following configuration options:

| Option | Default Value | Description |
|--|--|--|
| `Dashboard:Otlp:Cors:AllowedOrigins` | `null` | Specifies the allowed origins for CORS. It's a comma-delimited string and can include the `*` wildcard to allow any domain. This option is optional and can be set using the `DASHBOARD__OTLP__CORS__ALLOWEDORIGINS` environment variable. |
| `Dashboard:Otlp:Cors:AllowedHeaders` | `null` | A comma-delimited string representing the allowed headers for CORS. This setting is optional and can be set using the `DASHBOARD__OTLP__CORS__ALLOWEDHEADERS` environment variable. |

> [!NOTE]
> The dashboard only supports the `POST` method for sending telemetry and doesn't allow configuration of the _allowed methods_ (`Access-Control-Allow-Methods`) for CORS.

## Enable browser telemetry

In addition to [enabling CORS](#otlp-cors), you also need to configure the OTLP endpoint, headers and resource attributes, use the [JavaScript OTEL SDK](https://opentelemetry.io/docs/languages/js/getting-started/browser/) in your browser app, and configure a few other telemetry settings. In this section, you learn how to enable browser telemetry—sending telemetry directly to the dashboard from a browser. The following sections detail how to configure the OTLP endpoint, headers, resource attributes, and browser metadata.

### OTLP configuration

To configure the gPRC or HTTP endpoints, specify the following environment variables:

- `DOTNET_DASHBOARD_OTLP_ENDPOINT_URL`: The gRPC endpoint to which the dashboard connects for its data.
- `DOTNET_DASHBOARD_OTLP_HTTP_ENDPOINT_URL`: The HTTP endpoint to which the dashboard connects for its data.

While the dashboard supports both gRPC and HTTP endpoints simultaneously, resources are limited to one endpoint when set from environment variables. When both endpoints are set, the gRPC endpoint is used by default. If no endpoint is set, gRPC is used with the default value of `http://localhost:18889`.

The <xref:Aspire.Hosting.OtlpConfigurationExtensions.WithOtlpExporter``1(Aspire.Hosting.ApplicationModel.IResourceBuilder{``0})> method is used to configure the OTLP exporter in the app host when adding various resources to the app model. This is called when adding project resources, Azure Functions resources, Node.js resources, and Python resources.

### OTLP API key header

The OTLP API key `x-otlp-api-key` header is generated by the app host and is passed to apps as a standard OTEL attribute. The app host's configuration value for the `AppHost:OtlpApiKey` setting is used to generate the header value. All exporter headers are configured in the `OTEL_EXPORTER_OTLP_HEADERS` environment variable. Values are comma-delimited and formatted as `key=value`.

The header is added to all telemetry data sent to the dashboard. This header needs to be specified within the browser app so that it can be sent to the dashboard.

### Resource attributes

Resource attributes identify the resource sending telemetry data. The app host manages the `OTEL_RESOURCE_ATTRIBUTES` environment variable, which is a comma-delimited string formatted as `key=value`. For example, `"service.instance.id=71d4a6j4g4"` specifies that a service instance with the given ID can report telemetry to the dashboard.

For more information, see [Resources](#resources).

### Browser metadata

The browser app must set the appropriate `meta` element in the HTML to specify the `name="traceparent"` that corresponds to the current trace. The `traceparent` is used to correlate telemetry data to the current trace. In a .NET app for example, this would likely be assigned from the <xref:System.Diagnostics.Activity.Current?displayProperty=nameWithType> and passing its <xref:System.Diagnostics.Activity.Id?displayProperty=nameWithType> value as the `content`. For example, consider the following Razor code:

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

### Example browser telemetry code

The following JavaScript code demonstrates the initialization of the OpenTelemetry JavaScript SDK and the sending of telemetry data to the dashboard:

:::code language="javascript" source="snippets/BrowserTelemetry/BrowserTelemetry.Web/Scripts/index.js":::

The preceding JavaScript code defines an `initializeTelemetry` function that expects the OTLP endpoint URL, the headers, and the resource attributes. These parameters are provided by the consuming browser app that pulls them from the environment variables set by the app host. Consider the following Razor code:

:::code language="razor" source="snippets/BrowserTelemetry/BrowserTelemetry.Web/Pages/Shared/_Layout.cshtml" highlight="31-38":::

> [!TIP]
> The bundling and minification of the JavaScript code is beyond the scope of this article. For the complete working example of how to configure the JavaScript OTEL SDK to send telemetry to the dashboard, see the [browser telemetry sample](https://github.com/dotnet/aspire/tree/main/playground/BrowserTelemetry).

## Resources

The dashboard connects to a resource service to load and display resource information. The client is configured in the dashboard for how to connect to the service.

The resource service client authentication is configured with `Dashboard:ResourceService:AuthMode`. The client can be configured to support API key or client certificate authentication.

| Option | Default Value | Description |
|--|--|--|
| `Dashboard:ResourceService:Url` | `null` | The gRPC endpoint to which the dashboard connects for its data. If this value is unspecified, the dashboard shows telemetry data but no resource list or console logs. |
| `Dashboard:ResourceService:AuthMode` | `null` | Can be set to `ApiKey`, `Certificate` or `Unsecured`. `Unsecured` should only be used during local development. It's not recommended when hosting the dashboard publicly or in other settings. This value is required if a resource service URL is specified. |
| `Dashboard:ResourceService:ApiKey` | `null` | The API to send to the resource service in the `x-resource-service-api-key` header. This value is required if auth mode is API key. |
| `Dashboard:ResourceService:ClientCertificate:Source` | `null` | Can be set to `File` or `KeyStore`. This value is required if auth mode is client certificate. |
| `Dashboard:ResourceService:ClientCertificate:FilePath` | `null` | The certificate file path. This value is required if source is `File`. |
| `Dashboard:ResourceService:ClientCertificate:Password` | `null` | The password for the certificate file. This value is optional. |
| `Dashboard:ResourceService:ClientCertificate:Subject` | `null` | The certificate subject. This value is required if source is `KeyStore`. |
| `Dashboard:ResourceService:ClientCertificate:Store` | `My` | The certificate <xref:System.Security.Cryptography.X509Certificates.StoreName>. |
| `Dashboard:ResourceService:ClientCertificate:Location` | `CurrentUser` | The certificate <xref:System.Security.Cryptography.X509Certificates.StoreLocation>. |

### Telemetry limits

Telemetry is stored in memory. To avoid excessive memory usage, the dashboard has limits on the count and size of stored telemetry. When a count limit is reached, new telemetry is added, and the oldest telemetry is removed. When a size limit is reached, data is truncated to the limit.

Telemetry limits have different scopes depending upon the telemetry type:

- `MaxLogCount` and `MaxTraceCount` are shared across resources. For example, a `MaxLogCount` value of 5,000 configures the dashboard to store up to 5,000 total log entries for all resources.
- `MaxMetricsCount` is per-resource. For example, a `MaxMetricsCount` value of 10,000 configures the dashboard to store up to 10,000 metrics data points per-resource.

| Option | Default Value | Description |
|--|--|--|
| `Dashboard:TelemetryLimits:MaxLogCount` | 10,000 | The maximum number of log entries. Limit is shared across resources. |
| `Dashboard:TelemetryLimits:MaxTraceCount` | 10,000 | The maximum number of log traces. Limit is shared across resources. |
| `Dashboard:TelemetryLimits:MaxMetricsCount` | 50,000 | The maximum number of metric data points. Limit is per-resource. |
| `Dashboard:TelemetryLimits:MaxAttributeCount` | 128 | The maximum number of attributes on telemetry. |
| `Dashboard:TelemetryLimits:MaxAttributeLength` | `null` | The maximum length of attributes. |
| `Dashboard:TelemetryLimits:MaxSpanEventCount` | `null` | The maximum number of events on span attributes. |

## Other

| Option | Default Value | Description |
|--|--|--|
| `Dashboard:ApplicationName` | `Aspire` | The application name to be displayed in the UI. This applies only when no resource service URL is specified. When a resource service exists, the service specifies the application name. |

## Next steps

> [!div class="nextstepaction"]
> [Security considerations for running the .NET Aspire dashboard](security-considerations.md)
