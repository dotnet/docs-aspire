---
title: Azure App Configuration integration
description: Learn how to use Azure App Configuration with .NET Aspire.
ms.date: 05/15/2025
---

# .NET Aspire Azure App Configuration integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[Azure App Configuration](/azure/azure-app-configuration/overview) provides a service to centrally manage application settings and feature flags. Modern programs, especially programs running in a cloud, generally have many components that are distributed in nature. Spreading configuration settings across these components can lead to hard-to-troubleshoot errors during an application deployment. The .NET Aspire Azure App Configuration integration enables you to connect to existing App Configuration instances or create new instances all from your app host.

## Hosting integration

The .NET Aspire Azure App Configuration hosting integration models the App Configuration resource as the <xref:Aspire.Hosting.Azure.AzureAppConfigurationResource> type. To access this type and APIs fro expressing the resource, add the [📦 Aspire.Hosting.Azure.AppConfiguration](https://www.nuget.org/packages/Aspire.Hosting.Azure.AppConfiguration) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure.AppConfiguration
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure.AppConfiguration"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add Azure App Configuration resource

In your app host project, call <xref:Aspire.Hosting.AzureAppConfigurationExtensions.AddAzureAppConfiguration*> to add and return an Azure App Configuration resource builder.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var appConfig = builder.AddAzureAppConfiguration("config");

// After adding all resources, run the app...

builder.Build().Run();
```

When you add an <xref:Aspire.Hosting.Azure.AzureAppConfigurationResource> to the app host, it exposes other useful APIs.

> [!IMPORTANT]
> When you call <xref:Aspire.Hosting.AzureAppConfigurationExtensions.AddAzureAppConfiguration*>, it implicitly calls <xref:Aspire.Hosting.AzureProvisionerExtensions.AddAzureProvisioning(Aspire.Hosting.IDistributedApplicationBuilder)>—which adds support for generating Azure resources dynamically during app startup. The app must configure the appropriate subscription and location. For more information, see [Local provisioning: Configuration](local-provisioning.md#configuration).

#### Provisioning-generated Bicep

If you're new to [Bicep](/azure/azure-resource-manager/bicep/overview), it's a domain-specific language for defining Azure resources. With .NET Aspire, you don't need to write Bicep by-hand, instead the provisioning APIs generate Bicep for you. When you publish your app, the generated Bicep is output alongside the manifest file. When you add an Azure App Configuration resource, the following Bicep is generated:

:::code language="bicep" source="../snippets/azure/AppHost/config.module.bicep":::

The preceding Bicep is a module that provisions an Azure App Configuration resource. Additionally, role assignments are created for the Azure resource in a separate module:

:::code language="bicep" source="../snippets/azure/AppHost/config-roles.module.bicep":::

The generated Bicep is a starting point and is influenced by changes to the provisioning infrastructure in C#. Customizations to the Bicep file directly will be overwritten, so make changes through the C# provisioning APIs to ensure they're reflected in the generated files.

#### Customize provisioning infrastructure

All .NET Aspire Azure resources are subclasses of the <xref:Aspire.Hosting.Azure.AzureProvisioningResource> type. This type enables the customization of the generated Bicep by providing a fluent API to configure the Azure resources—using the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure``1(Aspire.Hosting.ApplicationModel.IResourceBuilder{``0},System.Action{Aspire.Hosting.Azure.AzureResourceInfrastructure})> API. For example, you can configure the `sku`, purge protection, and more. The following example demonstrates how to customize the Azure App Configuration resource:

:::code language="csharp" source="../snippets/azure/AppHost/Program.ConfigureAppConfigInfra.cs" id="configure":::

The preceding code:

- Chains a call to the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure*> API:
  - The `infra` parameter is an instance of the <xref:Aspire.Hosting.Azure.AzureResourceInfrastructure> type.
  - The provisionable resources are retrieved by calling the <xref:Azure.Provisioning.Infrastructure.GetProvisionableResources> method.
  - The single <xref:Azure.Provisioning.AppConfiguration.AppConfigurationStore> is retrieved.
  - The <xref:Azure.Provisioning.AppConfiguration.AppConfigurationStore.SkuName?displayProperty=nameWithType> is assigned to `Free`.
  - A tag is added to the App Configuration store with a key of `ExampleKey` and a value of `Example value`.

There are many more configuration options available to customize the Azure App Configuration resource. For more information, see <xref:Azure.Provisioning.AppConfiguration>. For more information, see [Azure.Provisioning customization](integrations-overview.md#azureprovisioning-customization). For deployment-focused customization patterns, see [Customize Azure resources](customize-azure-resources.md).

### Use existing Azure App Configuration resource

You might have an existing Azure App Configuration store that you want to connect to. If you want to use an existing Azure App Configuration store, you can do so by calling the <xref:Aspire.Hosting.ExistingAzureResourceExtensions.AsExisting*> method. This method accepts the config store and resource group names as parameters, and uses it to connect to the existing Azure App Configuration store resource.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var configName = builder.AddParameter("configName");
var configResourceGroupName = builder.AddParameter("configResourceGroupName");

var appConfig = builder.AddAzureAppConfiguration("config")
                       .AsExisting(configName, configResourceGroupName);

// After adding all resources, run the app...

builder.Build().Run();
```

For more information, see [Use existing Azure resources](integrations-overview.md#use-existing-azure-resources).

### Connect to an existing Azure App Configuration store

An alternative approach to using the `*AsExisting` APIs enables the addition of a connection string instead, where the app host uses configuration to resolve the connection information. To add a connection to an existing Azure App Configuration store, call the <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var config = builder.AddConnectionString("config");

builder.AddProject<Projects.WebApplication>("web")
       .WithReference(config);

// After adding all resources, run the app...
```

[!INCLUDE [connection-strings-alert](../includes/connection-strings-alert.md)]

The connection string is configured in the app host's configuration, typically under [User Secrets](/aspnet/core/security/app-secrets), under the `ConnectionStrings` section. The app host injects this connection string as an environment variable into all dependent resources, for example:

```json
{
    "ConnectionStrings": {
        "config": "https://{store_name}.azconfig.io"
    }
}
```

The dependent resource can access the injected connection string by calling the <xref:Microsoft.Extensions.Configuration.ConfigurationExtensions.GetConnectionString*> method, and passing the connection name as the parameter, in this case `"config"`. The `GetConnectionString` API is shorthand for `IConfiguration.GetSection("ConnectionStrings")[name]`.

## Client integration

To get started with the .NET Aspire Azure App Configuration client integration, install the [📦 Aspire.Microsoft.Extensions.Configuration.AzureAppConfiguration](https://www.nuget.org/packages/Aspire.Microsoft.Extensions.Configuration.AzureAppConfiguration) NuGet package in the client-consuming project, that is, the project for the application that uses the App Configuration client. The App Configuration client integration registers a <xref:Microsoft.Azure.Cosmos.CosmosClient> instance that you can use to interact with App Configuration.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Microsoft.Extensions.Configuration.AzureAppConfiguration
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Microsoft.Extensions.Configuration.AzureAppConfiguration"
                  Version="*" />
```

---

In the :::no-loc text="Program.cs"::: file of your client-consuming project, call the `AddAzureAppConfiguration` extension method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register the required services to flow Azure App Configuration values into the <xref:Microsoft.Extensions.Configuration.IConfiguration> instance for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddAzureAppConfiguration(connectionName: "config");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the App Configuration resource in the app host project. In other words, when you call `AddAzureAppConfiguration` in the app host and provide a name of `config` that same name should be used when calling `AddAzureAppConfiguration` in the client-consuming project. For more information, see [Add Azure App Configuration resource](#add-azure-app-configuration-resource).

You can then retrieve the <xref:Microsoft.Extensions.Configuration.IConfiguration> instance using dependency injection. For example, to retrieve the client from an example service:

```csharp
public class ExampleService(IConfiguration configuration)
{
    private readonly string _someValue = configuration["SomeKey"];
}
```

### Use feature flags

To use feature flags, install the [📦 Microsoft.FeatureManagement](https://www.nuget.org/packages/Microsoft.FeatureManagement) NuGet package:

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Microsoft.FeatureManagement
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Microsoft.FeatureManagement"
                  Version="*" />
```

---

App Configuration doesn't load feature flags by default. To load feature flags, you pass the `Action<AzureAppConfigurationOptions> configureOptions` delegate when calling `builder.AddAzureAppConfiguration`.

```csharp
builder.AddAzureAppConfiguration(
    "config",
    configureOptions: options => options.UseFeatureFlags());

// Register feature management services
builder.Services.AddFeatureManagement();
```

You can then use <xref:Microsoft.FeatureManagement.IFeatureManager> to evaluate feature flags in your app. Consider the following example ASP.NET Core Minimal API app:

```csharp
using Microsoft.Extensions.Hosting;
using Microsoft.FeatureManagement;

var builder = WebApplication.CreateBuilder(args);

builder.AddAzureAppConfiguration(
    "config",
    configureOptions: options => options.UseFeatureFlags());

// Register feature management services
builder.Services.AddFeatureManagement();

var app = builder.Build();

app.MapGet("/", async (IFeatureManager featureManager) =>
{
    if (await featureManager.IsEnabledAsync("NewFeature"))
    {
        return Results.Ok("New feature is enabled!");
    }

    return Results.Ok("Using standard implementation.");
});

app.Run();
```

For more information, see [.NET Feature Management](/azure/azure-app-configuration/feature-management-dotnet-reference).

### Configuration

The .NET Aspire Azure App Configuration library provides multiple options to configure the Azure App Configuration connection based on the requirements and conventions of your project. The App Config endpoint is required to be supplied, either in `AzureAppConfigurationSettings.Endpoint` or using a connection string.

#### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddAzureAppConfiguration()`:

```csharp
builder.AddAzureAppConfiguration("config");
```

Then the App Configuration endpoint is retrieved from the `ConnectionStrings` configuration section. The App Configuration store URI works with the `AzureAppConfigurationSettings.Credential` property to establish a connection. If no credential is configured, the <xref:Azure.Identity.DefaultAzureCredential> is used.

```json
{
  "ConnectionStrings": {
    "config": "https://{store_name}.azconfig.io"
  }
}
```

#### Use configuration providers

The .NET Aspire Azure App Configuration library supports <xref:Microsoft.Extensions.Configuration>. It loads the `AzureAppConfigurationSettings` from configuration by using the `Aspire:Microsoft:Extensions:Configuration:AzureAppConfiguration` key. Example _appsettings.json_ that configures some of the options:

```json
{
  "Aspire": {
    "Microsoft": {
      "Extensions": {
        "Configuration": {
          "AzureAppConfiguration": {
            "Endpoint": "YOUR_APPCONFIGURATION_ENDPOINT_URI"
          }
        }
      }
    }
  }
}
```

For the complete App Configuration client integration JSON schema, see [./ConfigurationSchema.json](https://github.com/dotnet/aspire/blob/main/src/Components/Aspire.Microsoft.Extensions.Configuration.AzureAppConfiguration/ConfigurationSchema.json).

#### Use inline delegates

You can also pass the `Action<AzureAppConfigurationSettings> configureSettings` delegate to set up some or all the options inline, for example to set App Configuration endpoint from code:

```csharp
builder.AddAzureAppConfiguration(
    "config",
    configureSettings: settings => settings.Endpoint = "http://YOUR_URI");
```

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

#### Logging

The .NET Aspire Azure App Configuration integration uses the following log categories:

- `Microsoft.Extensions.Configuration.AzureAppConfiguration.Refresh`

#### Tracing

The .NET Aspire Azure App Configuration integration doesn't make use any activity sources thus no tracing is available.

#### Metrics

The .NET Aspire Azure App Configuration integration currently doesn't support metrics.

## See also

- [Azure App Configuration documentation](/azure/azure-app-configuration/)
- [.NET Aspire integrations overview](../fundamentals/integrations-overview.md)
- [.NET Aspire Azure integrations overview](../azure/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
