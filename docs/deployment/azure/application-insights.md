---
title: Use .NET Aspire with Application Insights
description: Learn how to send .NET Aspire telemetry to Application Insights.
ms.date: 11/11/2023
ms.topic: how-to
---

# Use Application Insights for .NET Aspire telemetry

Azure Application Insights, a feature of Azure Monitor, excels in Application Performance Management (APM) for live web applications. .NET Aspire apps are designed to use OpenTelemetry for application telemetry. OpenTelemetry supports an extension model to support sending data to different APMs. .NET Aspire uses OTLP by default for telemetry export, which is used by the dashboard during development. Azure Monitor doesn't (yet) support OTLP, so the applications need to be modified to use the Azure Monitor exporter, and configured with the connection string.

To use Application insights, you specify its configuration in the app host project *and* use the [Azure Monitor distro in the service defaults project](#use-the-azure-monitor-distro).

## Choosing how Application Insights is provisioned

.NET Aspire has the capability to provision cloud resources as part of cloud deployment, including Application Insights. In your .NET Aspire app, you can decide if you want .NET Aspire to provision an Application Insights resource when deploying to Azure. You can also select to use an existing Application Insights resource by providing its connection string. The connection information is managed by the resource configuration in the app host project.

### Provisioning Application insights during Azure deployment

With this option, an instance of Application Insights will be created for you when the application is deployed using the Azure Developer CLI (`azd`).

To use automatic provisioning, you specify a dependency in the app host project, and reference it in each project/resource that needs to send telemetry to Application Insights. The steps include:

- Add a Nuget package reference to `Aspire.Hosting.Azure` in the app host project.

- Update the app host code to use the Application Insights resource, and reference it from each project:

``` csharp
var builder = DistributedApplication.CreateBuilder(args);

// Automatically provision an Application Insights resource
var insights = builder.AddAzureApplicationInsights("MyApplicationInsights");

// Reference the resource from each project 
var apiService = builder.AddProject<Projects.ApiService>("apiservice")
    .WithReference(insights);

builder.AddProject<Projects.Web>("webfrontend")
    .WithReference(apiService)
    .WithReference(insights);

builder.Build().Run();
```

Follow the steps in [Deploy a .NET Aspire app to Azure Container Apps using the Azure Developer CLI (in-depth guide)](./aca-deployment-azd-in-depth.md) to deploy the application to Azure Container Apps. `azd` will create an Application Insights resource as part of the same resource group, and configure the connection string for each container.

### Manual provisioning of Application Insights resource

Application Insights uses a connection string to tell the OpenTelemetry exporter where to send the telemetry data. The connection string is specific to the instance of Application Insights you want to send the telemetry to. It can be found in the Overview page for the application insights instance.

:::image type="content" source="../media/app-insights-connection-string.png" lightbox="../media/app-insights-connection-string.png" alt-text="Connection string placement in the Azure Application Insights portal UI.":::

If you wish to use an instance of Application Insights that you have provisioned manually, then you should use the `AddConnectionString` API in the app host project to tell the projects/containers where to send the telemetry data. The Azure Monitor distro expects the environment variable to be `APPLICATIONINSIGHTS_CONNECTION_STRING`, so that needs to be explicitly set when defining the connection string.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var insights = builder.AddConnectionString("myInsightsResource", "APPLICATIONINSIGHTS_CONNECTION_STRING");

var apiService = builder.AddProject<Projects.ApiService>("apiservice")
    .WithReference(insights);

builder.AddProject<Projects.Web>("webfrontend")
    .WithReference(apiService)
    .WithReference(insights);

builder.Build().Run();
```

#### Resource usage during developemnt

When running the .NET Aspire app locally, the preceding code reads the connection string from configuration. As this is a secret, you should store the value in [app secrets](/aspnet/core/security/app-secrets). Right click on the app host project and choose **Manage Secrets** from the context menu to open the secrets file for the app host project. In the file add the key and your specific connection string, the example below is for illustration purposes.

```json
{
  "ConnectionStrings": {
    "myInsightsResource": "InstrumentationKey=12345678-abcd-1234-abcd-1234abcd5678;IngestionEndpoint=https://westus3-1.in.applicationinsights.azure.com"
  }
}
```

> [!NOTE]
> The `name` specified in the app host code needs to match a key inside the `ConnectionStrings` section in the settings file.

#### Resource usage during deployment

When [deploying an Aspire application with Azure Developer CLI (`azd`)](./aca-deployment-azd-in-depth.md), it will recognize the connection string resource and prompt for a value. This enables a different resource to be used for the deployment from the value used for local development.

### Mixed deployment

If you wish to use a different deployment mechanism per execution context, use the appropriate API conditionally. For example, the following code uses a pre-supplied connection at development time, and an automatically provisioned resource at deployment time.

``` csharp
var builder = DistributedApplication.CreateBuilder(args);

var insights = builder.ExecutionContext.IsPublishMode
    ? builder.AddAzureApplicationInsights("myInsightsResource")
    : builder.AddConnectionString("myInsightsResource", "APPLICATIONINSIGHTS_CONNECTION_STRING");
var apiService = builder.AddProject<Projects.ApiService>("apiservice")
    .WithReference(insights);

builder.AddProject<Projects.Web>("webfrontend")
    .WithReference(apiService)
    .WithReference(insights);

builder.Build().Run();
```

> [!TIP]
> The preceding code requires you to supply the connection string information in app secrets for development time usage, and will be prompted for the connection string by `azd` at deployment time.

## Use the Azure Monitor distro

To make exporting to Azure Monitor simpler, this example uses the Azure Monitor Exporter Repo. This is a wrapper package around the Azure Monitor OpenTelemetry Exporter package that makes it easier to export to Azure Monitor with a set of common defaults.

Add the following package to the `ServiceDefaults` project, so that it will be included in each of the .NET Aspire services. For more information, see [.NET Aspire service defaults](../../fundamentals/service-defaults.md).

``` xml
<PackageReference Include="Azure.Monitor.OpenTelemetry.AspNetCore" 
                  Version="[SelectVersion]" />
```

Add a using statement to the top of the project.

``` csharp
using Azure.Monitor.OpenTelemetry.AspNetCore;
```

Uncomment the line in `AddOpenTelemetryExporters` to use the Azure Monitor exporter:

```csharp
private static IHostApplicationBuilder AddOpenTelemetryExporters(
    this IHostApplicationBuilder builder)
{
    // Omitted for brevity...

    // Uncomment the following lines to enable the Azure Monitor exporter 
    // (requires the Azure.Monitor.OpenTelemetry.AspNetCore package)
    if (!string.IsNullOrEmpty(builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"]))
    {
        builder.Services.AddOpenTelemetry().UseAzureMonitor();
    }
    return builder;
}
```

It's possible to further customize the Azure Monitor exporter, including customizing the resource name and changing the sampling. For more information, see [Customize the Azure Monitor exporter](/azure/azure-monitor/app/opentelemetry-configuration?tabs=aspnetcore). Using the parameterless version of `UseAzureMonitor()`, will pickup the connection string from the `APPLICATIONINSIGHTS_CONNECTION_STRING` environment variable, we configured via the app host project.
