---
title: Use .NET Aspire with Application Insights
description: Learn how to send .NET Aspire telemetry to Application Insights.
ms.date: 11/11/2023
ms.topic: how-to
---

# Use Application Insights for .NET Aspire telemetry

Azure Application Insights, a feature of Azure Monitor, excels in Application Performance Management (APM) for live web applications. .NET Aspire apps are designed to use OpenTelemetry for application telemetry. OpenTelemetry supports an extension model to support sending data to different APMs. .NET Aspire uses OTLP by default for telemetry export, which is used by the dashboard during development. Azure Monitor doesn't (yet) support OTLP, so the applications need to be modified to use the Azure Monitor exporter, and configured with the connection string.

## Connection string management

Application insights use a connection string to tell OpenTelemetry where to send the telemetry data. The connection string is specific to the instance of Application Insights you want to send the telemetry to. It can be found in the Overview page for the application insights instance.

:::image type="content" source="../media/app-insights-connection-string.png" lightbox="../media/app-insights-connection-string.png" alt-text="Connection string placement in the Azure Application Insights portal UI.":::

The connection string is typically supplied to the apps as an env var. There are multiple ways an env var can be set, but the easiest way to set those in .NET Aspire is in the AppHost project using the `WithEnvironment` method, That ensures that the variables are included in the manifest for deployment. The values can be changed after deployment by modifying the environment variable in the deployed environment.

For example, for the .NET Aspire Starter Application that has two projects, it needs to be set on each project you wish to receive the telemetry from.

```csharp
using Aspire.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var appInsightsConnectionString =
    builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];

var cache = builder.AddRedisContainer("cache");

var apiservice = builder.AddProject<Projects.AspireApp_ApiService>("apiservice")
    .WithEnvironment(
        "APPLICATIONINSIGHTS_CONNECTION_STRING",
        appInsightsConnectionString);

builder.AddProject<Projects.AspireApp_Web>("webfrontend")
    .WithReference(cache)
    .WithReference(apiservice)
    .WithEnvironment(
        "APPLICATIONINSIGHTS_CONNECTION_STRING",
        appInsightsConnectionString);

builder.Build().Run();
```

The preceding reads the connection string from configuration, as this is a secret we should store the value in [app secrets](/aspnet/core/security/app-secrets). Right click on the AppHost project and choose Manage Secrets from the context menu to open the secrets file for the AppHost project. In the file add the key and your specific connection string, the example below is for illustration purposes.

```json
{
  "APPLICATIONINSIGHTS_CONNECTION_STRING": "InstrumentationKey=12345678-abcd-1234-abcd-1234abcd5678;IngestionEndpoint=https://westus3-1.in.applicationinsights.azure.com"
}
```

## Use the Azure Monitor distro

To make exporting to Azure Monitor simpler, this example uses the Azure Monitor Exporter Repo. This is a wrapper package around the Azure Monitor OpenTelemetry Exporter package that makes it easier to export to Azure Monitor with a set of defaults.

Add the following package to the `ServiceDefaults` project, so that it will be included in all the .NET Aspire applications. For more information, see [.NET Aspire service defaults](../../fundamentals/service-defaults.md).

``` xml
<PackageReference Include="Azure.Monitor.OpenTelemetry.AspNetCore" 
                  Version="[SelectVersion]" />
```

Uncomment the line in `AddOpenTelemetryExporters` to use the Azure Monitor exporter:

```csharp
private static IHostApplicationBuilder AddOpenTelemetryExporters(
    this IHostApplicationBuilder builder)
{
    // Omitted for brevity...

    // Uncomment the following lines to enable the Azure Monitor exporter 
    // (requires the Azure.Monitor.OpenTelemetry.Exporter package)
    builder.Services.AddOpenTelemetry().UseAzureMonitor();

    return builder;
}
```

It's possible to further customize the Azure Monitor exporter, including customizing the resource name and changing the sampling. For more information, see [Customize the Azure Monitor exporter](/azure/azure-monitor/app/opentelemetry-configuration?tabs=aspnetcore).
