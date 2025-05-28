---
title: Screen capture automation
description: Details pertaining to the capturing of dashboard screens.
ms.date: 04/10/2025
---

# .NET Aspire Dashboard: Screen Capture Automation

This solution contains the following projects:

- **Aspire.Dashboard.ScreenCapture**: This project contains the screen capture tests for the .NET Aspire Dashboard. It uses Playwright to automate the browser and capture images of the dashboard.
- **AspireSample.AppHost**: This project contains the .NET Aspire app host, which orchestrates the an API and Web frontend.
- **AspireSample.ApiService**: This is an ASP.NET Core Minimal API service that provides a REST API for weather forecasts.
- **AspireSample.Web**: This is a Blazor app that provides a web frontend.
- **AspireSample.ServiceDefaults**: This project contains the default service settings for the API and Web apps.

The _ScreenCapture_ project defines xUnit tests are simply a harness for leveraging Playwright and .NET Aspire together to automate the maintenance of images. Run the following tests individually to generate the corresponding images:

```
dotnet test --filter Capture=help-images
dotnet test --filter Capture=project-resources
dotnet test --filter Capture=themes
dotnet test --filter Capture=stop-start-resources
dotnet test --filter Capture=resource-text-visualizer
dotnet test --filter Capture=resource-details
dotnet test --filter Capture=resource-filtering
dotnet test --filter Capture=resource-errors
dotnet test --filter Capture=structured-logs-errors
dotnet test --filter Capture=structured-logs
dotnet test --filter Capture=trace-logs
```

Each `Fact` (test) has a "Capture" trait with a specific value to help indicate what area of the dashboard is being captured.

> [!TIP]
> There's a script to capture all of these images at the root of the _Aspire.Dashboard.ScreenCapture_ directory:
>
> - **Bash**: `./capture.sh`
> - **PowerShell**: `./capture.ps1`

Running this script took about 2 minutes and 20 seconds on my machine. I haven't considered running these as part of a build system.

> [!NOTE]
> These tests are not written to be run in parallel. While it's not ideal and an opportunity for future improvement, it's easier for now to just run them individually.
