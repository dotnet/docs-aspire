---
title: Aspire dashboard configuration
description: Aspire dashboard configuration options
ms.date: 10/10/2025
ms.topic: reference
---

# Dashboard configuration

The dashboard is configured when it starts up. Configuration includes frontend and OpenTelemetry Protocol (OTLP) addresses, the resource service endpoint, authentication, telemetry limits, and more.

When the dashboard is launched with the Aspire AppHost project, it's automatically configured to display the app's resources and telemetry. Configuration is provided when launching the dashboard in [standalone mode](overview.md#standalone-mode).

There are many ways to provide configuration:

- Command line arguments.
- Environment variables. The `:` delimiter should be replaced with double underscore (`__`) in environment variable names.
- Optional JSON configuration file. The `ASPIRE_DASHBOARD_CONFIG_FILE_PATH` setting can be used to specify a JSON configuration file.

Consider the following example, which shows how to configure the dashboard when started from a Docker container:

## [Bash](#tab/bash)

```bash
docker run --rm -it -p 18888:18888 -p 4317:18889 -d --name aspire-dashboard \
    -e DASHBOARD__TELEMETRYLIMITS__MAXLOGCOUNT='1000' \
    -e DASHBOARD__TELEMETRYLIMITS__MAXTRACECOUNT='1000' \
    -e DASHBOARD__TELEMETRYLIMITS__MAXMETRICSCOUNT='1000' \
    mcr.microsoft.com/dotnet/aspire-dashboard:9.5
```

## [PowerShell](#tab/powershell)

```powershell
docker run --rm -it -p 18888:18888 -p 4317:18889 -d --name aspire-dashboard `
    -e DASHBOARD__TELEMETRYLIMITS__MAXLOGCOUNT='1000' `
    -e DASHBOARD__TELEMETRYLIMITS__MAXTRACECOUNT='1000' `
    -e DASHBOARD__TELEMETRYLIMITS__MAXMETRICSCOUNT='1000' `
    mcr.microsoft.com/dotnet/aspire-dashboard:9.5
```

---

Alternatively, these same values could be configured using a JSON configuration file that is specified using `ASPIRE_DASHBOARD_CONFIG_FILE_PATH`:

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

> [!NOTE]
> Configuration described on this page is for the standalone dashboard. To configure an Aspire AppHost project, see [AppHost configuration](../../app-host/configuration.md).

## Common configuration

| Option | Default value | Description |
|--|--|--|
| `ASPNETCORE_URLS` | `http://localhost:18888` | One or more HTTP endpoints through which the dashboard frontend is served. The frontend endpoint is used to view the dashboard in a browser. When the dashboard is launched by the Aspire AppHost this address is secured with HTTPS. Securing the dashboard with HTTPS is recommended. |
| `ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL` | `http://localhost:18889` | The [OTLP/gRPC](https://opentelemetry.io/docs/specs/otlp/#otlpgrpc) endpoint. This endpoint hosts an OTLP service and receives telemetry using gRPC. When the dashboard is launched by the Aspire AppHost this address is secured with HTTPS. Securing the dashboard with HTTPS is recommended. |
| `ASPIRE_DASHBOARD_OTLP_HTTP_ENDPOINT_URL` | `http://localhost:18890` | The [OTLP/HTTP](https://opentelemetry.io/docs/specs/otlp/#otlphttp) endpoint. This endpoint hosts an OTLP service and receives telemetry using Protobuf over HTTP. When the dashboard is launched by the Aspire AppHost the OTLP/HTTP endpoint isn't configured by default. To configure an OTLP/HTTP endpoint with the AppHost, set an `ASPIRE_DASHBOARD_OTLP_HTTP_ENDPOINT_URL` env var value in _launchSettings.json_. Securing the dashboard with HTTPS is recommended. |
| `ASPIRE_DASHBOARD_MCP_ENDPOINT_URL` | `http://localhost:18891` | The [Aspire MCP](mcp-server.md) endpoint. When this value isn't specified then the MCP server is hosted with an `ASPNETCORE_URLS` endpoint. The MCP server can be disabled by configuring `Dashboard:Mcp:Disabled` to `true`. When the dashboard is launched by the Aspire AppHost this address is secured with HTTPS. Securing the dashboard with HTTPS is recommended. |
| `ASPIRE_DASHBOARD_UNSECURED_ALLOW_ANONYMOUS` | `false` | Configures the dashboard to not use authentication and accepts anonymous access. This setting is a shortcut to configuring `Dashboard:Frontend:AuthMode`, `Dashboard:Otlp:AuthMode` and `Dashboard:Mcp:AuthMode` to `Unsecured`. |
| `ASPIRE_DASHBOARD_CONFIG_FILE_PATH` | `null` | The path for a JSON configuration file. If the dashboard is being run in a Docker container, then this is the path to the configuration file in a mounted volume. This value is optional. |
| `ASPIRE_DASHBOARD_FILE_CONFIG_DIRECTORY` | `null` | The directory where the dashboard looks for key-per-file configuration. This value is optional. |
| `ASPIRE_RESOURCE_SERVICE_ENDPOINT_URL` | `null` | The gRPC endpoint to which the dashboard connects for its data. If this value is unspecified, the dashboard shows telemetry data but no resource list or console logs. This setting is a shortcut to `Dashboard:ResourceServiceClient:Url`. |

## Frontend

The dashboard frontend endpoint authentication is configured with `Dashboard:Frontend:AuthMode`. The frontend can be secured with OpenID Connect (OIDC) or browser token authentication.

Browser token authentication works by the frontend asking for a token. The token can either be entered in the UI or provided as a query string value to the login page. For example, `https://localhost:1234/login?t=TheToken`. When the token is successfully authenticated an auth cookie is persisted to the browser, and the browser is redirected to the app.

| Option | Default value | Description |
|--|--|--|
| `Dashboard:Frontend:AuthMode` | `BrowserToken` | Can be set to `BrowserToken`, `OpenIdConnect` or `Unsecured`. `Unsecured` should only be used during local development. It's not recommended when hosting the dashboard publicly or in other settings. |
| `Dashboard:Frontend:BrowserToken` | `null` | Specifies the browser token. If the browser token isn't specified, then the dashboard generates one. Tooling that wants to automate logging in with browser token authentication can specify a token and open a browser with the token in the query string. A new token should be generated each time the dashboard is launched. |
| `Dashboard:Frontend:PublicUrl` | `null` | Specifies the public URL used to access the dashboard frontend. The public URL is used when constructing links to the dashboard frontend. If a public URL isn't specified, the frontend endpoint is used instead. This setting is important when the dashboard is accessed through a proxy and the dashboard endpoint isn't directly reachable. |
| `Dashboard:Frontend:OpenIdConnect:NameClaimType` | `name` | Specifies one or more claim types that should be used to display the authenticated user's full name. Can be a single claim type or a comma-delimited list of claim types. |
| `Dashboard:Frontend:OpenIdConnect:UsernameClaimType` | `preferred_username` | Specifies one or more claim types that should be used to display the authenticated user's username. Can be a single claim type or a comma-delimited list of claim types. |
| `Dashboard:Frontend:OpenIdConnect:RequiredClaimType` | `null` | Specifies the claim that must be present for authorized users. Authorization fails without this claim. This value is optional. |
| `Dashboard:Frontend:OpenIdConnect:RequiredClaimValue` | `null` | Specifies the value of the required claim. Only used if `Dashboard:Frontend:OpenIdConnect:RequireClaimType` is also specified. This value is optional. |
| `Dashboard:Frontend:OpenIdConnect:ClaimActions` | `null` | A collection of claim actions to configure how claims are mapped from the OpenID Connect user info endpoint. Each claim action can map JSON properties to claims. This value is optional. |
| `Authentication:Schemes:OpenIdConnect:Authority` | `null` | URL to the identity provider (IdP). |
| `Authentication:Schemes:OpenIdConnect:ClientId` | `null` | Identity of the relying party (RP). |
| `Authentication:Schemes:OpenIdConnect:ClientSecret` | `null` | A secret that only the real RP would know. |
| Other properties of <xref:Microsoft.AspNetCore.Builder.OpenIdConnectOptions> | `null` | Values inside configuration section `Authentication:Schemes:OpenIdConnect:*` are bound to `OpenIdConnectOptions`, such as `Scope`. |

> [!NOTE]
> Additional configuration may be required when using `OpenIdConnect` as authentication mode behind a reverse-proxy that terminates SSL. Check if you need `ASPIRE_DASHBOARD_FORWARDEDHEADERS_ENABLED` to be set to `true`.
>
> For more information, see [Configure ASP.NET Core to work with proxy servers and load balancers](/aspnet/core/host-and-deploy/proxy-load-balancer).

### Claim actions

Claim actions configure how claims are mapped from the JSON returned by the OpenID Connect user info endpoint to the user's claims identity. Each claim action in the `Dashboard:Frontend:OpenIdConnect:ClaimActions` collection supports the following properties:

| Property | Description |
|--|--|
| `ClaimType` (required) | The claim type to create. |
| `JsonKey` (required) | The JSON key to map from. |
| `SubKey` (optional) | The sub-key within the JSON key to map from. Used when the value is nested within another JSON object. |
| `IsUnique` (optional) | When `true`, ensures only one claim of this type exists. If a claim already exists, it won't be added again. Defaults to `false`. |
| `ValueType` (optional) | The claim value type. Defaults to `string`. |

The following example shows how to configure claim actions using JSON configuration:

```json
{
  "Dashboard": {
    "Frontend": {
      "OpenIdConnect": {
        "ClaimActions": [
          {
            "ClaimType": "role",
            "JsonKey": "role"
          }
        ]
      }
    }
  }
}
```

Or using environment variables for configuration:

```bash
export Dashboard__Frontend__OpenIdConnect__ClaimActions__0__ClaimType="role"
export Dashboard__Frontend__OpenIdConnect__ClaimActions__0__JsonKey="role"
```

## OTLP

The OTLP endpoint authentication is configured with `Dashboard:Otlp:AuthMode`. The OTLP endpoint can be secured with an API key or client certificate authentication.

API key authentication works by requiring each OTLP request to have a valid `x-otlp-api-key` header value. It must match either the primary or secondary key.

Client certificate authentication validates the TLS connection's client certificate. When a request with a client certificate is received, two sets of validation are performed:

- **ASP.NET Core certificate authentication validation:** By default this verifies that the certificate chains to a trusted root on the machine, the certificate hasn't expired, and that its Extended Key Usage value is appropriate for Client Authentication. For more information on this validation and how to configure it, see [Configure ASP.NET Core certificate validation](/aspnet/core/security/authentication/certauth#configure-certificate-validation).
- **Optional explicit certificate allowlist:** You can optionally configure an explicit list of allowed certificates using `AllowedCertificates`. If `AllowedCertificates` is configured and a client certificate does not match any of the listed thumbprints, the request is rejected. If no allowed certificates are specified, all certificates that pass the minimum validation are accepted.

| Option | Default value | Description |
|--|--|--|
| `Dashboard:Otlp:AuthMode` | `Unsecured` | Can be set to `ApiKey`, `Certificate` or `Unsecured`. `Unsecured` should only be used during local development. It's not recommended when hosting the dashboard publicly or in other settings. |
| `Dashboard:Otlp:PrimaryApiKey` | `null` | Specifies the primary API key. The API key can be any text, but a value with at least 128 bits of entropy is recommended. This value is required if auth mode is API key. |
| `Dashboard:Otlp:SecondaryApiKey` | `null` | Specifies the secondary API key. The API key can be any text, but a value with at least 128 bits of entropy is recommended. This value is optional. If a second API key is specified, then the incoming `x-otlp-api-key` header value can match either the primary or secondary key. |
| `Dashboard:Otlp:SuppressUnsecuredMessage` | `false` | Suppresses the unsecured message displayed in the dashboard when `Dashboard:Otlp:AuthMode` is `Unsecured`. This message should only be suppressed if an external frontdoor proxy is securing access to the endpoint. |

## OTLP CORS

Cross-origin resource sharing (CORS) can be configured to allow browser apps to send telemetry to the dashboard.

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

| Option | Default value | Description |
|--|--|--|
| `Dashboard:Otlp:Cors:AllowedOrigins` | `null` | Specifies the allowed origins for CORS. It's a comma-delimited string and can include the `*` wildcard to allow any domain. This option is optional and can be set using the `DASHBOARD__OTLP__CORS__ALLOWEDORIGINS` environment variable. |
| `Dashboard:Otlp:Cors:AllowedHeaders` | `null` | A comma-delimited string representing the allowed headers for CORS. This setting is optional and can be set using the `DASHBOARD__OTLP__CORS__ALLOWEDHEADERS` environment variable. |

> [!NOTE]
> The dashboard only supports the `POST` method for sending telemetry and doesn't allow configuration of the _allowed methods_ (`Access-Control-Allow-Methods`) for CORS.

## MCP

The MCP endpoint authentication is configured with `Dashboard:Mcp:AuthMode`. The MCP endpoint can be secured with API key authentication.

API key authentication works by requiring each MCP request to have a valid `x-mcp-api-key` header value. It must match either the primary or secondary key.

| Option | Default value | Description |
|--|--|--|
| `Dashboard:Mcp:AuthMode` | `Unsecured` | Can be set to `ApiKey` or `Unsecured`. `Unsecured` should only be used during local development. It's not recommended when hosting the dashboard publicly or in other settings. |
| `Dashboard:Mcp:PrimaryApiKey` | `null` | Specifies the primary API key. The API key can be any text, but a value with at least 128 bits of entropy is recommended. This value is required if auth mode is API key. |
| `Dashboard:Mcp:SecondaryApiKey` | `null` | Specifies the secondary API key. The API key can be any text, but a value with at least 128 bits of entropy is recommended. This value is optional. If a second API key is specified, then the incoming `x-mcp-api-key` header value can match either the primary or secondary key. |
| `Dashboard:Mcp:SuppressUnsecuredMessage` | `false` | Suppresses the unsecured message displayed in the dashboard when `Dashboard:Mcp:AuthMode` is `Unsecured`. This message should only be suppressed if an external frontdoor proxy is securing access to the endpoint. |
| `Dashboard:Mcp:PublicUrl` | `null` | Specifies the public URL used to access the MCP server. The public URL is used when constructing links to the MCP server. If a public URL isn't specified, the MCP endpoint is used instead. This setting is important when the dashboard is accessed through a proxy and the dashboard endpoint isn't directly reachable. |
| `Dashboard:Mcp:Disabled` | `false` | Disables the MCP server and remove MCP UI in the dashboard. |

## Resources

The dashboard connects to a resource service to load and display resource information. The client is configured in the dashboard for how to connect to the service.

The resource service client authentication is configured with `Dashboard:ResourceServiceClient:AuthMode`. The client can be configured to support API key or client certificate authentication.

| Option | Default value | Description |
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

### Telemetry limits

Telemetry is stored in memory. To avoid excessive memory usage, the dashboard has limits on the count and size of stored telemetry. When a count limit is reached, new telemetry is added, and the oldest telemetry is removed. When a size limit is reached, data is truncated to the limit.

Telemetry limits have different scopes depending upon the telemetry type:

- `MaxLogCount` and `MaxTraceCount` are shared across resources. For example, a `MaxLogCount` value of 5,000 configures the dashboard to store up to 5,000 total log entries for all resources.
- `MaxMetricsCount` is per-resource. For example, a `MaxMetricsCount` value of 10,000 configures the dashboard to store up to 10,000 metrics data points per-resource.

| Option | Default value | Description |
|--|--|--|
| `Dashboard:TelemetryLimits:MaxLogCount` | 10,000 | The maximum number of log entries. Limit is shared across resources. |
| `Dashboard:TelemetryLimits:MaxTraceCount` | 10,000 | The maximum number of log traces. Limit is shared across resources. |
| `Dashboard:TelemetryLimits:MaxMetricsCount` | 50,000 | The maximum number of metric data points. Limit is per-resource. |
| `Dashboard:TelemetryLimits:MaxAttributeCount` | 128 | The maximum number of attributes on telemetry. |
| `Dashboard:TelemetryLimits:MaxAttributeLength` | `null` | The maximum length of attributes. |
| `Dashboard:TelemetryLimits:MaxSpanEventCount` | `null` | The maximum number of events on span attributes. |

## Other

| Option | Default value | Description |
|--|--|--|
| `Dashboard:ApplicationName` | `Aspire` | The application name to be displayed in the UI. This applies only when no resource service URL is specified. When a resource service exists, the service specifies the application name. |
| `Dashboard:UI:DisableResourceGraph` | `false` | Disables displaying the resource graph UI in the dashboard. |

## Next steps

> [!div class="nextstepaction"]
> [Security considerations for running the Aspire dashboard](security-considerations.md)
