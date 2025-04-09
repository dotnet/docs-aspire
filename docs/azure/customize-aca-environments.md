---
title: Configure Azure Container Apps environments
description: Learn how to configure Azure Container Apps environments in .NET Aspire.
ms.topic: how-to
ms.date: 04/09/2025
---

# Configure Azure Container Apps environments

The [.NET Aspire app host](../fundamentals/app-host-overview.md) generates infrastructure code to provision Azure resources for your application. This allows user to model and configure most things involving deployment in C#, rather than dropping down to Bicep, for example. This includes Azure Container Apps (ACA) environments, which are used to run containerized applications in a serverless environment.

Using the [📦 Azure.Provisioning](https://www.nuget.org/packages/Azure.Provisioning) NuGet package, you're able to define and configure ACA environments and related resources, such as container registries and volume file shares. In this article, you learn how to customize ACA environments in your .NET Aspire solutions.

## Prerequisites

Before you begin, ensure you have:

- An existing .NET Aspire application.
- Azure CLI installed and logged in.
- Basic familiarity with Azure Container Apps and Azure Developer CLI (`azd`).

## Customize your ACA environment

Follow these steps to customize your ACA environment:

1. Open your Aspire application's AppHost project.

2. In your AppHost configuration, call the `AddAzureContainerAppEnvironment` method to define your ACA environment:

```csharp
// Program.cs
builder.AddAzureContainerAppEnvironment("my-aca-environment", aca =>
{
    aca.Location = "eastus";
    aca.ContainerRegistry = "myregistry.azurecr.io";
    aca.VolumeFileShares.Add("my-volume-share");
});
```

This example creates an ACA environment named `my-aca-environment` in the `eastus` region, specifies a container registry, and adds a volume file share.

3. Customize additional properties as needed. For example, you can configure logging, scaling, or networking settings directly in your C# code.

## Handle naming conventions

By default, `AddAzureContainerAppEnvironment` uses a different Azure resource naming scheme than the Azure Developer CLI (`azd`). If you're upgrading an existing deployment that previously used `azd`, you might see duplicate Azure resources.

To avoid this issue, call the `WithAzdResourceNaming` method to revert to the naming convention used by `azd`:

```csharp
// Program.cs
builder.AddAzureContainerAppEnvironment("my-aca-environment", aca =>
{
    aca.WithAzdResourceNaming();
    // Additional configuration...
});
```

This ensures your existing Azure resources remain consistent and prevents duplication.

## Deploy your changes

After customizing your ACA environment, deploy your Aspire application using your usual deployment process. The Aspire AppHost automatically provisions and configures the ACA environment and related resources based on your defined settings.

## Additional resources

For more information, see:

- [Azure Container Apps overview](https://learn.microsoft.com/azure/container-apps/overview)
- [Azure Developer CLI (`azd`) documentation](https://learn.microsoft.com/azure/developer/azure-developer-cli/overview)
- [Azure.Provisioning APIs documentation](../azure-provisioning/overview.md)
