---
title: Allow unsecure transport in .NET Aspire
description: Learn how to allow unsecure transport in .NET Aspire applications.
ms.date: 04/09/2024
---

# Allow unsecure transport in .NET Aspire

Starting with .NET Aspire preview 5, the app host will crash if an `applicationUrl` is configured with an unsecure transport (non-TLS `http`) protocol. This is a security feature to prevent accidental exposure of sensitive data. However, there are scenarios where you might need to allow unsecure transport. This article explains how to allow unsecure transport in .NET Aspire applications.

## Symptoms

When you run a .NET Aspire application with an `applicationUrl` configured with an unsecure transport protocol, you might see the following error message:

```Output
The 'applicationUrl' setting must be an https address unless the
'ASPIRE_ALLOW_UNSECURED_TRANSPORT' environment variable is set to true.

This configuration is commonly set in the launch profile.
```

## How to allow unsecure transport

To allow an unsecure transport in .NET Aspire, set the `ASPIRE_ALLOW_UNSECURED_TRANSPORT` environment variable to `true`. This environment variable is used to control the behavior of the app host when an `applicationUrl` is configured with an insecure transport protocol:

## [Unix](#tab/unix)

```bash
export ASPIRE_ALLOW_UNSECURED_TRANSPORT=true
```

## [Windows](#tab/windows)

```powershell
$env:ASPIRE_ALLOW_UNSECURED_TRANSPORT = "true"
```

---

Alternatively, you can control this via the launch profile as it exposes the ability to configure environment variables per profile. To do this, consider the following example settings in the `launchSettings.json` file:

```json
{
  "$schema": "http://json.schemastore.org/launchsettings.json",
  "profiles": {
    "https": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "https://localhost:15015;http://localhost:15016",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "DOTNET_ENVIRONMENT": "Development",
        "DOTNET_DASHBOARD_OTLP_ENDPOINT_URL": "https://localhost:16099",
        "DOTNET_RESOURCE_SERVICE_ENDPOINT_URL": "https://localhost:17037"
      }
    },
    "http": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "launchBrowser": true,
      "applicationUrl": "http://localhost:15016",
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development",
        "DOTNET_ENVIRONMENT": "Development",
        "DOTNET_DASHBOARD_OTLP_ENDPOINT_URL": "http://localhost:16099",
        "DOTNET_RESOURCE_SERVICE_ENDPOINT_URL": "http://localhost:17038",
        "ASPIRE_ALLOW_UNSECURED_TRANSPORT": "true"
      }
    }
  }
}
```

The preceding example shows two profiles, `https` and `http`. The `https` profile is configured with a secure transport protocol, while the `http` profile is configured with an insecure transport protocol. The `ASPIRE_ALLOW_UNSECURED_TRANSPORT` environment variable is set to `true` in the `http` profile to allow unsecure transport.
