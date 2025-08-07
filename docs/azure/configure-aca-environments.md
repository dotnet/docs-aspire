---
title: Configure Azure Container Apps environments
description: Learn how to configure Azure Container Apps environments in .NET Aspire.
ms.topic: how-to
ms.date: 04/09/2025
---

# Configure Azure Container Apps environments

It's easy to [publish resources as Azure Container Apps (ACA)](integrations-overview.md#publish-as-azure-container-app) using any of the supported APIs:

- <xref:Aspire.Hosting.AzureContainerAppProjectExtensions.PublishAsAzureContainerApp*?displayProperty=nameWithType>
- <xref:Aspire.Hosting.AzureContainerAppContainerExtensions.PublishAsAzureContainerApp*?displayProperty=nameWithType>
- <xref:Aspire.Hosting.AzureContainerAppExecutableExtensions.PublishAsAzureContainerApp*?displayProperty=nameWithType>

These APIs automatically create a default ACA environment when you publish your app. While this default setup works well for most scenarios, you might need to customize the ACA environment to meet specific requirements. To achieve this, use the `AddAzureContainerAppEnvironment` method.

The [.NET Aspire app host](../fundamentals/app-host-overview.md) simplifies infrastructure provisioning by generating code to create Azure resources for your applications. This approach enables you to model and configure deployment-related aspects directly in C#, reducing the need to rely on tools like Bicep. These aspects include configuring ACA environments, which provide a serverless platform for running containerized applications.

By using the <xref:Azure.Provisioning> APIs (explained in [Customize Azure resources](customize-azure-resources.md)), you can configure and customize ACA environments along with related resources, such as container registries and file share volumes. Any available deployment setting can be configured. For more information on the available settings, see [Microsoft.App managedEnvironments](/azure/templates/microsoft.app/managedenvironments).

This article guides you through the process of tailoring ACA environments for your .NET Aspire solutions.

## Add an ACA environment

The `AzureContainerAppEnvironmentResource` type models an ACA environment resource. When you call the `AddAzureContainerAppEnvironment` method, it creates an instance of this type (wrapped in the <xref:Aspire.Hosting.ApplicationModel.IResourceBuilder`1>).

:::code language="csharp" source="snippets/aca/AspireAca.AppHost/AspireApp.AppHost/Program.cs":::

By default, the calling this API to add an ACA environment generates the following provisioning Bicep module:

:::code language="bicep" source="snippets/aca/AspireAca.AppHost/AspireApp.AppHost/aca-env.module.bicep":::

This module configures:

- A user-assigned managed identity for the ACA environment.
- An Azure Container Registry (ACR) for the ACA environment.
- A Log Analytics workspace for the ACA environment.
- An Azure Container Apps environment.
- The [.NET Aspire dashboard](../fundamentals/dashboard/overview.md) for the ACA environment.
- A role assignment for the user principal ID to the ACA environment.
- Various outputs for the ACA environment.

Using the `acaEnv` variable, you can chain a call to the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure*> API to customize the ACA environment to your liking. For more information, see [Configure infrastructure](customize-azure-resources.md#configure-infrastructure).

## Handle naming conventions

By default, <xref:Aspire.Hosting.AzureContainerAppExtensions.AddAzureContainerAppEnvironment*> uses a different Azure resource naming scheme than the [Azure Developer CLI (`azd`)](/azure/developer/azure-developer-cli/). If you're upgrading an existing deployment that previously used `azd`, you might see duplicate Azure resources. To avoid this issue, call the <xref:Aspire.Hosting.AzureContainerAppExtensions.WithAzdResourceNaming*> method to revert to the naming convention used by `azd`:

```csharp
var builder = DistributionApplicationBuilder.Create(args);

var acaEnv = builder.AddAzureContainerAppEnvironment("aca-env")
                    .WithAzdResourceNaming();

// Omitted for brevity...

builder.Build().Run();
```

Calling this API ensures your existing Azure resources remain consistent and prevents duplication.

## Customize provisioning infrastructure

All .NET Aspire Azure resources are subclasses of the <xref:Aspire.Hosting.Azure.AzureProvisioningResource> type. This enables customization of the generated Bicep by providing a fluent API to configure the Azure resources—using the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure``1(Aspire.Hosting.ApplicationModel.IResourceBuilder{``0},System.Action{Aspire.Hosting.Azure.AzureResourceInfrastructure})> API:

```csharp
var builder = DistributionApplicationBuilder.Create(args);

var acaEnv = builder.AddAzureContainerAppEnvironment(Config.ContainEnvironmentName);

acaEnv.ConfigureInfrastructure(config =>
{
    var resources = config.GetProvisionableResources();
    var containerEnvironment = resources.OfType<ContainerAppManagedEnvironment>().FirstOrDefault();

    containerEnvironment.Tags.Add("ExampleKey", "Example value");
});
```

The preceding code:

- Chains a call to the <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure*> API:
  - The `infra` parameter is an instance of the <xref:Aspire.Hosting.Azure.AzureResourceInfrastructure> type.
  - The provisionable resources are retrieved by calling the <xref:Azure.Provisioning.Infrastructure.GetProvisionableResources> method.
  - The single <xref:Azure.Provisioning.AppContainers.ContainerAppManagedEnvironment> resource is retrieved.
  - A tag is added to the Azure Container Apps environment resource with a key of `ExampleKey` and a value of `Example value`.

## See also

- [.NET Aspire Azure integrations overview](integrations-overview.md)
- [Azure Container Apps overview](/azure/container-apps/overview)
