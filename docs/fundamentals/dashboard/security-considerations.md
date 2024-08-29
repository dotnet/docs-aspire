---
title: .NET Aspire dashboard security considerations
description: Security considerations for running the .NET Aspire dashboard
ms.date: 05/31/2024
ms.topic: reference
---

# Security considerations for running the .NET Aspire dashboard

The [.NET Aspire dashboard](overview.md) offers powerful insights to your apps. The dashboard displays information about resources, including their configuration, console logs and in-depth telemetry.

Data displayed in the dashboard can be sensitive. For example, configuration can include secrets in environment variables, and telemetry can include sensitive runtime data. Care should be taken to secure access to the dashboard.

## Scenarios for running the dashboard

The dashboard can be run in different scenarios, such as being automatically starting by .NET Aspire tooling, or as a standalone application that is separate from other .NET Aspire integrations. Steps to secure the dashboard depend on how it's being run.

### .NET Aspire tooling

The dashboard is automatically started when an .NET Aspire app host is run. The dashboard is secure by default when run from .NET Aspire tooling:

- Transport is secured with HTTPS. Using HTTPS is configured by default in _launchSettings.json_. The launch profile includes `https` addresses in `applicationUrl` and `DOTNET_DASHBOARD_OTLP_ENDPOINT_URL` values.
- Browser frontend authenticated with a browser token.
- Incoming telemetry authenticated with an API key.

HTTPS in the dashboard uses the ASP.NET Core development certificate. The certificate must be trusted for the dashboard to work correctly. The steps required to trust the development cert are different depending on the machine's operating system:

- [Trust the ASP.NET Core HTTPS development certificate on Windows and macOS](/aspnet/core/security/enforcing-ssl#trust-the-aspnet-core-https-development-certificate-on-windows-and-macos)
- [Trust HTTPS certificate on Linux](/aspnet/core/security/enforcing-ssl#trust-https-certificate-on-linux)

There are scenarios where you might want to allow an unsecured transport. The dashboard can run without HTTPS from the .NET Aspire app host by configuring the `ASPIRE_ALLOW_UNSECURED_TRANSPORT` setting to `true`. For more information, see [Allow unsecured transport in .NET Aspire](../../troubleshooting/allow-unsecure-transport.md).

### Standalone mode

The dashboard is shipped as a Docker image and can be used without the rest of .NET Aspire. When the dashboard is launched in standalone mode, it defaults to a mix of secure and unsecured settings.

- Browser frontend authenticated with a browser token.
- Incoming telemetry is unsecured. Warnings are displayed in the console and dashboard UI.

The telemetry endpoint accepts incoming OTLP data without authentication. When the endpoint is unsecured, the dashboard is open to receiving telemetry from untrusted apps.

For information about securing the telemetry when running the dashboard in standalone mode, see [Securing the telemetry endpoint](#secure-telemetry-endpoint).

## Secure telemetry endpoint

The .NET Aspire dashboard provides a variety of ways to view logs, traces, and metrics for your app. This information enables you to track the behavior and performance of your app and to diagnose any issues that arise. It's important that you can trust this information, and a warning is displayed in the dashboard UI if telemetry isn't secured.

The dashboard collects telemetry through an [OTLP (OpenTelemetry protocol)](https://opentelemetry.io/docs/specs/otel/protocol/) endpoint. Apps send telemetry to this endpoint, and the dashboard stores the external information it receives in memory, which is then accessible via the UI.

To prevent untrusted apps from sending telemetry to .NET Aspire, the OTLP endpoint should be secured. The OTLP endpoint is automatically secured with an API key when the dashboard is started by .NET Aspire tooling. Additional configuration is required for standalone mode.

API key authentication can be enabled on the telemetry endpoint with some additional configuration:

## [Bash](#tab/bash)

```bash
docker run --rm -it -p 18888:18888 -p 4317:18889 -d --name aspire-dashboard \
    -e DASHBOARD__OTLP__AUTHMODE='ApiKey' \
    -e DASHBOARD__OTLP__PRIMARYAPIKEY='{MY_APIKEY}' \
    mcr.microsoft.com/dotnet/aspire-dashboard:8.0.0
```

## [PowerShell](#tab/powershell)

```powershell
docker run --rm -it -p 18888:18888 -p 4317:18889 -d --name aspire-dashboard `
    -e DASHBOARD__OTLP__AUTHMODE='ApiKey' `
    -e DASHBOARD__OTLP__PRIMARYAPIKEY='{MY_APIKEY}' `
    mcr.microsoft.com/dotnet/aspire-dashboard:8.0.0
```

---

The preceding Docker command:

- Starts the .NET Aspire dashboard image and exposes OTLP endpoint as port 4317
- Configures the OTLP endpoint to use `ApiKey` authentication. This requires that incoming telemetry has a valid `x-otlp-api-key` header value.
- Configures the expected API key. `{MY_APIKEY}` in the example value should be replaced with a real API key. The API key can be any text, but a value with at least 128 bits of entropy is recommended.

When API key authentication is configured, the dashboard validates incoming telemetry has a required API key. Apps that send the dashboard telemetry must be configured to send the API key. This can be configured in .NET with `OtlpExporterOptions.Headers`:

```csharp
builder.Services.Configure<OtlpExporterOptions>(
    o => o.Headers = $"x-otlp-api-key={MY_APIKEY}");
```

Other languages have different OpenTelmetry APIs. Passing the [`OTEL_EXPORTER_OTLP_HEADERS` environment variable](https://opentelemetry.io/docs/specs/otel/protocol/exporter/) to apps is a universal way to configure the header.

## Memory exhaustion

The dashboard stores external information it receives in memory, such as resource details and telemetry. While the number of resources the dashboard tracks are bounded, there isn't a limit to how much telemetry apps send to the dashboard. Limits must be placed on how much information is stored to prevent the dashboard using an excessive amount of memory and exhausting available memory on the current machine.

### Telemetry limits

To help prevent memory exhaustion, the dashboard limits how much telemetry it stores by default. For example, there is a maximum of 10,000 structured log entries per resource. Once the limit is reached, each new new log entry received causes an old entry to be removed.

Configuration can customize telemetry limits.
