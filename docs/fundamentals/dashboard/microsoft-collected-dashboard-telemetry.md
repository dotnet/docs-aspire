---
title: Microsoft-collected dashboard telemetry
description: Learn about what telemetry the .NET Aspire dashboard sends and how to opt out.
ms.date: 05/08/2025
---

# Microsoft-collected dashboard telemetry

The .NET Aspire dashboard collects diagnostic data to help developers monitor and analyze their applications. Separately, when the dashboard is launched through Visual Studio or Visual Studio Code as part of a running .NET Aspire application, Microsoft collects usage data about the dashboard itself. This data helps the .NET Aspire team improve the product. Additionally, unhandled exception details from the dashboard are sent to Microsoft to assist in identifying and resolving issues.

## Scope

.NET Aspire dashboard usage telemetry is collected only when running the dashboard from Visual Studio or Visual Studio Code. Telemetry is gathered while the dashboard is open in the browser, unless you have [opted out](#how-to-opt-out) of telemetry collection.

## How to opt-out

Starting with .NET Aspire 9.3, dashboard usage telemetry is enabled by default. Dashboard usage telemetry isn't collected in early version. This feature aligns with the following IDE versions:

- Visual Studio: 17.14 or later.
- C# Dev Kit: 1.18.25 or later.

To opt-out of telemetry collection, set the `ASPIRE_DASHBOARD_TELEMETRY_OPTOUT` environment variable to `true`. This will apply to all users accessing the .NET Aspire dashboard:

### [PowerShell](#tab/powershell)

```powershell
$env:ASPIRE_DASHBOARD_TELEMETRY_OPTOUT= "true"
```

### [Bash](#tab/bash)

```bash
export ASPIRE_DASHBOARD_TELEMETRY_OPTOUT=true
```

---

Alternatively, you can opt-out of telemetry collection by disabling telemetry collection in the host IDE. Learn how to opt out in [Visual Studio](/visualstudio/ide/visual-studio-experience-improvement-program) or [Visual Studio Code](https://code.visualstudio.com/docs/configure/telemetry#_disable-telemetry-reporting).

## Disclosure

When dashboard usage telemetry is enabled, there's a disclosure statement at the bottom of the settings panel, informing you that telemetry is enabled. This disclosure statement appears when usage telemetry is enabled.

## Data points

.NET Aspire dashboard usage telemetry doesn't collect personal data, such as, IP addresses or use browser fingerprinting. It doesn't scan your code and doesn't extract source code, authorship, or deployment configuration. The data is sent securely to Microsoft using [Azure Monitor](/azure/azure-monitor/) through existing telemetry APIs in Visual Studio and Visual Studio Code.

Protecting your privacy is important to Microsoft. If you suspect that telemetry is collecting sensitive data or the data is being insecurely or inappropriately handled, file an issue in the :::image type="icon" source="../../media/github-mark.svg" border="false"::: [GitHub dotnet/aspire](https://github.com/dotnet/aspire) repository for investigation.

The .NET Aspire dashboard doesn't collect telemetry on Visual Studio versions `< 17.14` or C# Dev Kit versions `< 1.18.25`. It collects the following data:

| .NET Aspire dashboard versions | Data | Notes |
|--|--|--|
| 9.3 | Page navigation history. | Includes page settings. |
| 9.3 | Resource types being used. | Custom resource type names are not sent to Microsoft. |
| 9.3 | Request user agent. | — |
| 9.3 | Invoked dashboard commands. | Custom command names are not sent to Microsoft. |
| 9.3 | Request language and set dashboard language. | — |
| 9.3 | Resource restart times. | — |
| 9.3 | OTel data processing times. | — |
| 9.3 | Dashboard-related unhandled exceptions. | — |
| 9.3 | GitHub Copilot usage statistics. | — |
| 9.3 | GitHub Copilot help/unhelp feedback. | — |

## See also

- [.NET Aspire dashboard](overview.md)
- [.NET Aspire dashboard configuration](configuration.md)
