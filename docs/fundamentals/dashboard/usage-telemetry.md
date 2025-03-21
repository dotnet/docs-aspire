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

## How to opt out

.NET Aspire dashboard telemetry is enabled by default for `Aspire >= 9.2` when launched through Visual Studio `>= 17.14` or C# Dev Kit `>= [VSC RELEASE VERSION]`.

To opt out, you may either:

- Set the `DOTNET_DASHBOARD_ENABLE_TELEMETRY` environment variable to `false`. This will apply to all users accessing the Aspire dashboard.
- Disable telemetry collection in the host IDE.

## Disclosure

If dashboard telemetry is enabled, there will be a disclosure statement at the bottom of the settings panel informing that telemetry is enabled. This statement will appear any time telemetry collection is enabled.

:::image type="content" source="media/explore/dashboard-settings-drawer.png" lightbox="media/explore/dashboard-settings-drawer.png" alt-text="A screenshot of the .NET Aspire dashboard settings drawer.":::

## Data points

Aspire dashboard telemetry does not collect personal data like IP addresses or use browser fingerprinting. It does not scan your code and does not extract source code, authorship, or deployment configuration. The data is sent securely to Microsoft using [https://azure.microsoft.com/services/monitor/](Azure Monitor) through existing telemetry APIs in Visual Studio and Visual Studio Code.

Protecting your privacy is important to us. If you suspect that telemetry is collecting sensitive data or the data is being insecurely or inappropriately handled, file an issue in the [dotnet/aspire](https://github.com/dotnet/aspire) repository for investigation.

The Aspire dashboard does not collect telemetry on Visual Studio versions `< 17.14` or C# Dev Kit versions `< [VSC RELEASE VERSION`. It collects the following data:

| Aspire dashboard versions | Data | Notes |
|--------------|------|------|
| 9.2          | Page navigation history. | Includes page settings. |
| 9.3          | Resource types being used. | |
| 9.3          | Request user agent. | |
| 9.3          | Invoked dashboard commands. | Command name is securely hashed. |
| 9.3          | Request language and set dashboard language. | |
| 9.3          | Resource restart times. | |
| 9.3          | OTel data processing times. | |
| 9.3          | Dashboard-related unhandled exceptions. | |
