---
title: .NET Aspire dashboard configuration
description: .NET Aspire dashboard configuration options
ms.date: 04/23/2024
ms.topic: reference
---

# Dashboard configuration

The dashboard is configured when it starts up. Configuration includes frontend and OTLP addresses, the resource service endpoint, authentication, telemetry limits, and more.

If the dashboard is launched by the .NET Aspire app host project, then it's automatically configured to display the app's resources and telemetry. Configuration is provided when launching the dashboard in [standalone mode](overview.md#standalone-mode).

There are a number of ways to provide configuration:

- Command line arguments.
- Environment variables. The `:` delimiter should be replaced with double underscore (`__`) in environment variable names.
- Optional JSON configuration file. The `DOTNET_DASHBOARD_CONFIG_FILE_PATH` setting can be used to specify a JSON configuration file.

Consider the following example, which shows how to configure the dashboard when started from a Docker container:

```bash
docker run --rm -it -p 18888:18888 -p 4317:18889 -d --name aspire-dashboard \
    -e DASHBOARD__TELEMETRYLIMITS__MAXLOGCOUNT='1000' \
    -e DASHBOARD__TELEMETRYLIMITS__MAXTRACECOUNT='1000' \
    -e DASHBOARD__TELEMETRYLIMITS__MAXMETRICSCOUNT='1000' \
    mcr.microsoft.com/dotnet/nightly/aspire-dashboard:8.0.0-preview.6
```

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

### Common configuration

| Option | Default Value | Description |
|--|--|--|
| `ASPNETCORE_URLS` | `http://localhost:18888` | One or more HTTP endpoints through which the dashboard frontend is served. The frontend endpoint is used to view the dashboard in a browser. When the dashboard is launched by the .NET Aspire app host this address is secured with HTTPS. Securing the dashboard with HTTPS is recommended. |
| `DOTNET_DASHBOARD_OTLP_ENDPOINT_URL` | `http://localhost:18889` | The OTLP endpoint. This endpoint hosts an OTLP service and receives telemetry. When the dashboard is launched by the .NET Aspire app host this address is secured with HTTPS. Securing the dashboard with HTTPS is recommended. |
| `DOTNET_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS` | `false` | Configures the dashboard to not use authentication and accepts anonymous access. This setting is a shortcut to configuring `Dashboard:Frontend:AuthMode` and `Dashboard:Otlp:AuthMode` to `Unsecured`. |
| `DOTNET_DASHBOARD_CONFIG_FILE_PATH` | `null` | The path for a JSON configuration file. If the dashboard is being run in a Docker container, then this is the path to the configuration file in a mounted volume. This value is optional. |
| `DOTNET_RESOURCE_SERVICE_ENDPOINT_URL` | `null` | The gRPC endpoint to which the dashboard connects for its data. If this value is unspecified, the dashboard shows telemetry data but no resource list or console logs. This setting is a shortcut to `Dashboard:ResourceServiceClient:Url`. |

### Frontend authentication

The dashboard frontend endpoint authentication is configured with `Dashboard:Frontend:AuthMode`. The frontend can be secured with OpenID Connect (OIDC) or browser token authentication.

Browser token authentication works by the frontend asking for a token. The token can either be entered in the UI or provided as a query string value to the login page. For example, `https://localhost:1234/login?t=TheToken`. When the token is successfully authenticated an auth cookie is persisted to the browser and the browser is redirected to the app.

| Option | Default Value | Description |
|--|--|--|
| `Dashboard:Frontend:AuthMode` | `BrowserToken` | Can be set to `BrowserToken`, `OpenIdConnect` or `Unsecured`. `Unsecured` should only be used during local development. It's not recommended when hosting the dashboard publicly or in other settings. |
| `Dashboard:Frontend:BrowserToken` | `null` | Specifies the browser token. If the browser token isn't specified, then the dashboard will generate one. Tooling that wants to automate logging in with browser token authentication can specify a token and open a browser with the token in the query string. A new token should be generated each time the dashboard is launched. |
| `Dashboard:Frontend:OpenIdConnect:NameClaimType` | `name` | Specifies the claim type(s) that should be used to display the authenticated user's full name. Can be a single claim type or a comma-delimited list of claim types. |
| `Dashboard:Frontend:OpenIdConnect:UsernameClaimType` | `preferred_username` | Specifies the claim type(s) that should be used to display the authenticated user's username. Can be a single claim type or a comma-delimited list of claim types. |
| `Dashboard:Frontend:OpenIdConnect:RequiredClaimType` | `null` | Specifies the claim that must be present for authorized users. Authorization fails without this claim. This value is optional. |
| `Dashboard:Frontend:OpenIdConnect:RequiredClaimValue` | `null` | Specifies the value of the required claim. Only used if `Dashboard:Frontend:OpenIdConnect:RequireClaimType` is also specified. This value is optional. |
| `Authentication:Schemes:OpenIdConnect:Authority` | `null` | URL to the identity provider (IdP). |
| `Authentication:Schemes:OpenIdConnect:ClientId` | `null` | Identity of the relying party (RP). |
| `Authentication:Schemes:OpenIdConnect:ClientSecret` | `null` | A secret that only the real RP would know. |
| Other properties of <xref:Microsoft.AspNetCore.Builder.OpenIdConnectOptions> | `null` | Values inside configuration section `Authentication:Schemes:OpenIdConnect:*` are bound to `OpenIdConnectOptions`, such as `Scope`. |

### OTLP authentication

The OTLP endpoint authentication is configured with `Dashboard:Otlp:AuthMode`. The OTLP endpoint can be secured with an API key or [client certificate](/aspnet/core/security/authentication/certauth) authentication.

API key authentication works by requiring each OTLP request to have a valid `x-otlp-api-key` header value. It must match either the primary or secondary key.

| Option | Default Value | Description |
|--|--|--|
| `Dashboard:Otlp:AuthMode` | `Unsecured` | Can be set to `ApiKey`, `Certificate` or `Unsecured`. `Unsecured` should only be used during local development. It's not recommended when hosting the dashboard publicly or in other settings. |
| `Dashboard:Otlp:PrimaryApiKey` | `null` | Specifies the primary API key. The API key can be any text, but a value with at least 128 bits of entropy is recommended. This value is required if auth mode is API key. |
| `Dashboard:Otlp:SecondaryApiKey` | `null` | Specifies the secondary API key. The API key can be any text, but a value with at least 128 bits of entropy is recommended. This value is optional. If a second API key is specified then the incoming `x-otlp-api-key` header value can match either the primary or secondary key. |

### Resources

The dashboard connects to a resource service to load and display resource information. The client is configured in the dashboard for how to connect to the service.

The resource service client authentication is configured with `Dashboard:ResourceServiceClient:AuthMode`. The client can be configured to support API key or client certificate authentication.

| Option | Default Value | Description |
|--|--|--|
| `Dashboard:ResourceServiceClient:Url` | `null` | The gRPC endpoint to which the dashboard connects for its data. If this value is unspecified, the dashboard shows telemetry data but no resource list or console logs. |
| `Dashboard:ResourceServiceClient:AuthMode` | `null` | Can be set to `ApiKey`, `Certificate` or `Unsecured`. `Unsecured` should only be used during local development. It's not recommended when hosting the dashboard publicly or in other settings. This value is required if a resource service URL is specified. |
| `Dashboard:ResourceServiceClient:ApiKey` | `null` | The API to send to the resource service in the `x-resource-service-api-key` header. This value is required if auth mode is API key. |
| `Dashboard:ResourceServiceClient:ClientCertificate:Source` | `null` | Can be set to `File` or `KeyStore`. This value is required if auth mode is client certificate. |
| `Dashboard:ResourceServiceClient:ClientCertificate:FilePath` | `null` | The certificate file path. This value is required if source is `File`. |
| `Dashboard:ResourceServiceClient:ClientCertificate:Password` | `null` | The password for the certificate file. This value is optional. |
| `Dashboard:ResourceServiceClient:ClientCertificate:Subject` | `null` | The certificate subject. This value is required if source is `KeyStore`. |
| `Dashboard:ResourceServiceClient:ClientCertificate:Store` | `My` | The certificate <xref:System.Security.Cryptography.X509Certificates.StoreName>. |
| `Dashboard:ResourceServiceClient:ClientCertificate:Location` | `CurrentUser` | The certificate <xref:System.Security.Cryptography.X509Certificates.StoreLocation>. |

#### Telemetry limits

Telemetry is stored in memory. To avoid excessive memory usage, the dashboard has limits on the count and size of stored telemetry. When a count limit is reached, new telemetry is added, and the oldest telemetry is removed. When a size limit is reached, data is truncated to the limit.

Limits are per-resource. For example, a `MaxLogCount` value of 10,000 configures the dashboard to store up to 10,000 log entries per-resource.

| Option | Default Value | Description |
|--|--|--|
| `Dashboard:TelemetryLimits:MaxLogCount` | 10,000 | The maximum number of log entries. |
| `Dashboard:TelemetryLimits:MaxTraceCount` | 10,000 | The maximum number of log traces. |
| `Dashboard:TelemetryLimits:MaxMetricsCount` | 50,000 | The maximum number of metric data points. |
| `Dashboard:TelemetryLimits:MaxAttributeCount` | 128 | The maximum number of attributes on telemetry. |
| `Dashboard:TelemetryLimits:MaxAttributeLength` | `null` | The maximum length of attributes. |
| `Dashboard:TelemetryLimits:MaxSpanEventCount` | `null` | The maximum number of events on span attributes. |

### Other

| Option | Default Value | Description |
|--|--|--|
| `Dashboard:ApplicationName` | `Aspire` | The application name to be displayed in the UI. This applies only when no resource service URL is specified. When a resource service exists, the service specifies the application name. |
