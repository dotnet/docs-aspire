---
title: Azure App Service integration (Preview)
description: Learn how to integrate Azure App Service with Aspire for hosting web applications.
ms.date: 10/02/2025
ai-usage: ai-generated
---

# Aspire Azure App Service integration (Preview)

[!INCLUDE [includes-hosting](../includes/includes-hosting.md)]

> [!IMPORTANT]
> The Aspire Azure App Service integration is currently in preview and is subject to change.

[Azure App Service](/azure/app-service) is a fully managed platform for building, deploying, and scaling web apps. Aspire apps often run locally during development but require scalable, production-ready hosting environments for staging and production. The Aspire integration allows you to provision or reference an existing Azure App Service environment (App Service Plan) and seamlessly publish your container, executable, and project resources as Azure App Service websites. When you add an App Service environment, the integration automatically provisions an Azure Container Registry for container-based deployments and grants fine-grained role assignments to enable secure access between Azure resources.

## Hosting integration

The Azure App Service integration is part of the Aspire hosting model. It allows you to define and manage your app's resources in a declarative manner. The integration is available in the [📦 Aspire.Hosting.Azure.AppService](https://www.nuget.org/packages/Aspire.Hosting.Azure.AppService) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure.AppService
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure.AppService"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Add Azure App Service environment

To use Azure App Service with Aspire, you first add an App Service environment to your AppHost project. The environment represents the hosting infrastructure (App Service Plan) where your apps will run.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var appServiceEnv = builder.AddAzureAppServiceEnvironment("app-service-env");

// Add other resources to the app model

builder.Build().Run();
```

The preceding code creates a new Azure App Service environment named `app-service-env`. When you run your app locally, this environment is provisioned in your Azure subscription with the following resources:

- An App Service Plan with a Premium P0V3 tier on Linux.
- An Azure Container Registry (Basic SKU) for container image storage.
- A user-assigned managed identity for secure access to the container registry.

> [!IMPORTANT]
> When you call <xref:Aspire.Hosting.AzureAppServiceEnvironmentExtensions.AddAzureAppServiceEnvironment*>, it implicitly calls <xref:Aspire.Hosting.AzureProvisionerExtensions.AddAzureProvisioning*>—which adds support for generating Azure resources dynamically during app startup. The app must configure the appropriate subscription and location. For more information, see [Local provisioning: Configuration](local-provisioning.md#configuration).

## Connect to an existing Azure App Service plan

You might have an existing Azure App Service plan that you want to use. Chain a call to annotate that your <xref:Aspire.Hosting.Azure.AzureAppServiceEnvironmentResource> is an existing resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var existingAppServicePlanName = builder.AddParameter("existingAppServicePlanName");
var existingResourceGroup = builder.AddParameter("existingResourceGroup");

var appServiceEnv = builder.AddAzureAppServiceEnvironment("app-service-env")
                           .AsExisting(existingAppServicePlanName, existingResourceGroup);

builder.AddProject<Projects.WebApi>("api")
       .PublishAsAzureAppServiceWebsite((infra, website) =>
       {
           // Optional: customize the Azure App Service website here
       });

// After adding all resources, run the app...
```

[!INCLUDE [azure-configuration](includes/azure-configuration.md)]

For more information on treating Azure App Service resources as existing resources, see [Use existing Azure resources](integrations-overview.md#use-existing-azure-resources).

> [!NOTE]
> Alternatively, instead of representing an Azure App Service environment resource, you can add a connection string to the AppHost. This approach is weakly-typed, and doesn't work with role assignments or infrastructure customizations. For more information, see [Add existing Azure resources with connection strings](integrations-overview.md#add-existing-azure-resources-with-connection-strings).

## Publish resources as Azure App Service websites

After adding an App Service environment, you can publish compute resources (`IComputeResource`) as Azure App Service websites using the <xref:Aspire.Hosting.AzureAppServiceComputeResourceExtensions.PublishAsAzureAppServiceWebsite*> method.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var appServiceEnv = builder.AddAzureAppServiceEnvironment("app-service-env");

builder.AddProject<Projects.WebApi>("api")
       .PublishAsAzureAppServiceWebsite((infra, website) =>
       {
           // Optional: customize the Azure App Service website here
       });

builder.Build().Run();
```

The preceding code:

- Creates an Azure App Service environment named `app-service-env`.
- Adds a project named `api` to the AppHost.
- Configures the project to be published as an Azure App Service website.
- Provides an optional callback to customize the website configuration.

During local development (when running with <kbd>F5</kbd>), the project runs locally. When you publish your app, the project is deployed as an Azure App Service website within the provisioned environment.

## Customize the App Service website

The `PublishAsAzureAppServiceWebsite` method accepts a callback that allows you to customize the Azure App Service website configuration using the [Azure.Provisioning](/dotnet/api/azure.provisioning) APIs:

```csharp
builder.AddProject<Projects.WebApi>("api")
       .PublishAsAzureAppServiceWebsite((infra, website) =>
       {
           website.AppSettings.Add("ASPNETCORE_ENVIRONMENT", new AppServiceConfigurationSetting
           {
               Name = "ASPNETCORE_ENVIRONMENT",
               Value = "Production"
           });
           
           website.Tags.Add("Environment", "Production");
           website.Tags.Add("Team", "Engineering");
       });
```

The preceding code:

- Chains a call to <xref:Aspire.Hosting.AzureAppServiceComputeResourceExtensions.PublishAsAzureAppServiceWebsite*> with a customization callback.
- Adds an application setting for `ASPNETCORE_ENVIRONMENT`.
- Adds multiple tags for metadata and organization.

> [!NOTE]
> You can configure many other properties of the App Service website using this approach. For a complete list of available configuration options, see <xref:Azure.Provisioning.AppService.WebSite>.

## Provisioning-generated Bicep

If you're new to [Bicep](/azure/azure-resource-manager/bicep/overview), it's a domain-specific language for defining Azure resources. With .NET Aspire, you don't need to write Bicep by-hand, instead the provisioning APIs generate Bicep for you. When you publish your app, the generated Bicep is output alongside the manifest file.

When you add an Azure App Service environment, the following key resources are provisioned:

- **App Service Plan**: A Premium P0V3 Linux-based hosting plan.
- **Azure Container Registry**: A Basic SKU registry for storing container images.
- **User-assigned Managed Identity**: For secure access between App Service and Container Registry.
- **Role Assignments**: ACR Pull role assigned to the managed identity.

The generated Bicep is a starting point and is influenced by changes to the provisioning infrastructure in C#. Customizations to the Bicep file directly are overwritten, so make changes through the C# provisioning APIs to ensure they're reflected in the generated files.

### Customize provisioning infrastructure

All Aspire Azure resources are subclasses of the <xref:Aspire.Hosting.Azure.AzureProvisioningResource> type. This type enables the customization of the generated Bicep by providing a fluent API to configure the Azure resources—using the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure``1(Aspire.Hosting.ApplicationModel.IResourceBuilder{``0},System.Action{Aspire.Hosting.Azure.AzureResourceInfrastructure})> API. For example, you can configure the App Service Plan SKU, location, and more. The following example demonstrates how to customize the Azure App Service environment:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var appServiceEnv = builder.AddAzureAppServiceEnvironment("app-service-env")
                           .ConfigureInfrastructure(infra =>
                           {
                               var resources = infra.GetProvisionableResources();
                               var plan = resources.OfType<AppServicePlan>().Single();
                               
                               plan.Sku = new AppServiceSkuDescription
                               {
                                   Name = "P1V3",
                                   Tier = "Premium"
                               };
                               
                               plan.Tags.Add("Environment", "Production");
                               plan.Tags.Add("CostCenter", "Engineering");
                           });

builder.Build().Run();
```

The preceding code:

- Chains a call to the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure*> API:
  - The `infra` parameter is an instance of the <xref:Aspire.Hosting.Azure.AzureResourceInfrastructure> type.
  - The provisionable resources are retrieved by calling the <xref:Azure.Provisioning.Infrastructure.GetProvisionableResources> method.
  - The single <xref:Azure.Provisioning.AppService.AppServicePlan> is retrieved.
  - The <xref:Azure.Provisioning.AppService.AppServicePlan.Sku?displayProperty=nameWithType> is changed to a P1V3 tier.
  - Tags are added to the App Service Plan for metadata and organization.

There are many more configuration options available to customize the App Service environment. For more information, see <xref:Azure.Provisioning.AppService> and [Azure.Provisioning customization](customize-azure-resources.md#azureprovisioning-customization).

## See also

- [Azure App Service documentation](/azure/app-service)
- [.NET Aspire Azure integrations overview](integrations-overview.md)
- [Configure Azure Container Apps environments](configure-aca-environments.md)
