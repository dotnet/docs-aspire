---
title: Use .NET Aspire with Application Insights
description: Learn how to send .NET Aspire telemetry to Application Insights.
ms.date: 11/11/2023
ms.topic: how-to
---

# Use Application Insights for .NET Aspire telemetry

Azure Application Insights, a feature of Azure Monitor, excels in Application Performance Management (APM) for live web applications. .NET Aspire apps are designed to use OpenTelemetry for application telemetry. OpenTelemetry supports an extension model to support sending data to different APMs. .NET Aspire uses OTLP by default for telemetry export, which is used by the dashboard during development. Azure Monitor doesn't (yet) support OTLP, so the applications need to be modified to use the Azure Monitor exporter, and configured with the connection string.

To use Application insights, you will need to specify its configuration in the AppHost project *and* use the [Azure Monitor distro in the service defaults project](#use-the-azure-monitor-distro).

## Choosing how Application Insights is provisioned

Aspire has the capability to deploy cloud resources as part of development or cloud provisioning, including Application Insights. In your Aspire app, you can decide if you want Aspire to provision an Application Insights resource at debug time, or when deploying to Azure. You can also select to use an existing Application Insights resource by providing its connection string. The connection information is managed by the resource configuration in the AppHost project.

### Automatically provisioning Application Insights during development

With this option, an instance of Application Insights will be created for you the first time you debug an app with the Application Insights resource definition in the AppHost project. An instance of Application Insights will be created in a resource group based on the name of the AppHost project. Each different Aspire solution will create an independent Application Insights resource.

> Note: There is no automatic deletion of the Azure resources created this way, so you'll need to manually remove them when they are no longer needed.

To use automatic provisioning you will need to specify a dependency in the AppHost project, and reference it in each project/resource that needs to send telemetry to Application Insights. The steps include:

- Adding a Nuget package reference to `Aspire.Hosting.Azure.Provisioning` in the AppHost project. This will also implicitly include the `Aspire.Hosting.Azure` package.

- Add [app secrets](/aspnet/core/security/app-secrets) to tell it where to create the Application Insights Resource. If using Visual Studio, right click on the AppHost project and choose *Manage User Secrets* to create and open the secrets file. Add the following keys, replacing the values as applicable:

``` json
{
  "Azure:Location": "WestUS",
  "Azure:SubscriptionId": "__Replace with your subscription GUID__"
}
```

- Update the AppHost code to use the Application Insights resource, and reference it from each project:

``` csharp

var builder = DistributedApplication.CreateBuilder(args);

// Required for Azure provisioning
builder.AddAzureProvisioning();

// Automatically provision an Application Insights resource
var insights = builder.AddAzureApplicationInsights("MyApplicationInsights");

// Reference the resource from each project 
var apiService = builder.AddProject<Projects.aspire_app11_ApiService>("apiservice")
    .WithReference(insights);

builder.AddProject<Projects.aspire_app11_Web>("webfrontend")
    .WithReference(apiService)
    .WithReference(insights);

builder.Build().Run();
```

When you debug the application, Aspire will create the Application Insights resource on the first time that it sees the resource. The Application Insights Resource will feed the connection information to each project that references it. This can be seen as an `APPLICATIONINSIGHTS_CONNECTION_STRING` environment variable in the details view for the resource in the Aspire dashboard.

:::image type="content" source="../media/dashboard-with-connection-string.png" lightbox="../media/dashboard-with-connection-string.png" alt-text="Connection string environment variables in the Aspire dashboard":::

The settings for the created Application Insights resource will be persisted in the app secrets file.

### Provisioning Application insights at deployment time

Using the Application Insights resource above will cause an Application Insights resource to be provisioned when the application is deployed using the Azure Developer CLI (AZD).

Follow the steps in [Deploy a .NET Aspire app to Azure Container Apps using the Azure Developer CLI (in-depth guide)](./aca-deployment-azd-in-depth.md) to deploy the application to Azure Container Apps. AZD will create an Application Insights resource as part of the same resource group, and configure the connection string for each container.

### Manual deployment of Application Insights resource

Application insights use a connection string to tell the OpenTelemetry exporter where to send the telemetry data. The connection string is specific to the instance of Application Insights you want to send the telemetry to. It can be found in the Overview page for the application insights instance.

:::image type="content" source="../media/app-insights-connection-string.png" lightbox="../media/app-insights-connection-string.png" alt-text="Connection string placement in the Azure Application Insights portal UI.":::

If you wish to use an instance of Application Insights that you have provisioned manually, then you should use the Connection String resource in the AppHost project to tell the projects/containers where to send the telemetry data. The Azure Monitor distro expects the environment variable to be `APPLICATIONINSIGHTS_CONNECTION_STRING`, so that needs to be explicitly set when defining the connection string.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureProvisioning();

var insights = builder.AddConnectionString("myInsightsResource", "APPLICATIONINSIGHTS_CONNECTION_STRING");

var apiService = builder.AddProject<Projects.aspire_app11_ApiService>("apiservice")
    .WithReference(insights);

builder.AddProject<Projects.aspire_app11_Web>("webfrontend")
    .WithReference(apiService)
    .WithReference(insights);

builder.Build().Run();
```

#### Resource usage during developemnt

When running the Aspire application locally, the preceding code reads the connection string from configuration. As this is a secret we should store the value in [app secrets](/aspnet/core/security/app-secrets). Right click on the AppHost project and choose Manage Secrets from the context menu to open the secrets file for the AppHost project. In the file add the key and your specific connection string, the example below is for illustration purposes.

```json
{
  "ConnectionStrings": {
    "myInsightsResource": "InstrumentationKey=12345678-abcd-1234-abcd-1234abcd5678;IngestionEndpoint=https://westus3-1.in.applicationinsights.azure.com"
  }
}
```

> Note: The keyname specified in the AppHost code needs to match a key inside the `ConnectionStrings` section in the settings file.

#### Resource usage during deployment

When deploying an Aspire application with Azure Developer CLI (AZD), it will recognize the connection string resource and prompt for a value. This enables a different resource to be used for the deployment from the value used for local development.

### Mixed deployment

If you wish to use a different mechanism at developemnt from deployment, you can use a combination of Application Insights and Connection String resources, with a conditional. For example the following code uses an automatically provisioned resource at development time, and a pre-supplied connection at deployment time.

```
var builder = DistributedApplication.CreateBuilder(args);

builder.AddAzureProvisioning();

var insights = builder.ExecutionContext.IsPublishMode ?
    builder.AddConnectionString("ai", "APPLICATIONINSIGHTS_CONNECTION_STRING") :
    builder.AddAzureApplicationInsights("ai");

var apiService = builder.AddProject<Projects.aspire_app11_ApiService>("apiservice")
    .WithReference(insights);

builder.AddProject<Projects.aspire_app11_Web>("webfrontend")
    .WithReference(apiService)
    .WithReference(insights);

builder.Build().Run();
```

> Note: With the above you will need to supply the `Azure:Location` and `Azure:Subscription` information in app secrets for development time usage, and will be prompted for the connection string by AZD at deployment time.

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
    // (requires the Azure.Monitor.OpenTelemetry.Exporter package)
    builder.Services.AddOpenTelemetry().UseAzureMonitor();

    return builder;
}
```

It's possible to further customize the Azure Monitor exporter, including customizing the resource name and changing the sampling. For more information, see [Customize the Azure Monitor exporter](/azure/azure-monitor/app/opentelemetry-configuration?tabs=aspnetcore). Using the parameterless version of `UseAzureMonitor()`, will pickup the connection string from the `APPLICATIONINSIGHTS_CONNECTION_STRING` environment variable, we configured via the AppHost project.
