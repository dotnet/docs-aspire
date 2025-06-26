---
title: Azure Container Registry integration (Preview)
description: Learn how to integrate Azure Container Registry with .NET Aspire for secure container image management.
ms.date: 05/09/2025
---

# .NET Aspire Azure Container Registry integration (Preview)

[!INCLUDE [includes-hosting](../includes/includes-hosting.md)]

> [!IMPORTANT]
> The .NET Aspire Azure Container Registry integration is currently in preview and is subject to change.

[Azure Container Registry (ACR)](/azure/container-registry) is a managed Docker container registry service that simplifies the storage, management, and deployment of container images. The .NET Aspire integration allows you to provision or reference an existing Azure Container Registry and seamlessly integrate it with your app's compute environments.

## Overview

.NET Aspire apps often build and run container images locally but require secure registries for staging and production environments. The Azure Container Registry integration provides the following capabilities:

- Provision or reference an existing Azure Container Registry.
- Attach the registry to any compute-environment resource (for example, Azure Container Apps, Docker, Kubernetes) to ensure proper credential flow.
- Grant fine-grained ACR role assignments to other Azure resources.

## Supported scenarios

The Azure Container Registry integration supports the following scenarios:

- **Provisioning a new registry**: Automatically create a new Azure Container Registry for your app.
- **Referencing an existing registry**: Use an existing Azure Container Registry by providing its name and resource group.
- **Credential management**: Automatically flow credentials to compute environments for secure image pulls.
- **Role assignments**: Assign specific roles (for example, `AcrPush`) to enable services to push images to the registry.

## Hosting integration

The Azure Container Registry integration is part of the .NET Aspire hosting model. It allows you to define and manage your app's resources in a declarative manner. The integration is available in the [📦 Aspire.Hosting.Azure.ContainerRegistry](https://www.nuget.org/packages/Aspire.Hosting.Azure.ContainerRegistry) NuGet package.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure.ContainerRegistry
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure.ContainerRegistry"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Provision a new container registry

The following example demonstrates how to provision a new Azure Container Registry and attach it to a container app environment:

:::code source="snippets/acr//AspireAcr.AppHost/AspireAcr.AppHost/Program.cs":::

The preceding code:

- Creates a new Azure Container Registry named `my-acr`.
- Attaches the registry to an Azure Container Apps environment named `env`.
- Optionally grants the `AcrPush` role to a project resource named `api`, allowing it to push images to the registry.

For more information, see [Configure Azure Container Apps environments](configure-aca-environments.md).

> [!IMPORTANT]
> When you call `AddAzureContainerRegistry` or `AddAzureContainerAppEnvironment`, they implicitly call the idempotent <xref:Aspire.Hosting.AzureProvisionerExtensions.AddAzureProvisioning*>—which adds support for generating Azure resources dynamically during app startup. The app must configure the appropriate subscription and location. For more information, see [Local provisioning: Configuration](local-provisioning.md#configuration).

#### Provisioning-generated Bicep

If you're new to [Bicep](/azure/azure-resource-manager/bicep/overview), it's a domain-specific language for defining Azure resources. With .NET Aspire, you don't need to write Bicep by-hand, instead the provisioning APIs generate Bicep for you. When you publish your app, the generated Bicep is output alongside the manifest file. When you add an Azure Container Registry resource, the following Bicep is generated:

:::code language="bicep" source="snippets/acr/AspireAcr.AppHost/AspireAcr.AppHost/my-acr.module.bicep":::

The preceding Bicep provisions an Azure Container Registry resource. Additionally, the added Azure Container App environment resource is also generated:

:::code language="bicep" source="snippets/acr/AspireAcr.AppHost/AspireAcr.AppHost/env.module.bicep":::

The generated Bicep is a starting point and is influenced by changes to the provisioning infrastructure in C#. Customizations to the Bicep file directly are overwritten, so make changes through the C# provisioning APIs to ensure they're reflected in the generated files.

### Reference an existing container registry

To reference an existing Azure Container Registry, use the <xref:Aspire.Hosting.ExistingAzureResourceExtensions.PublishAsExisting*> method with the registry name and resource group:

:::code source="snippets/acr/AspireAcr.AppHost/AspireAcr.AppHost/Program.Existing.cs" id="existing":::

The preceding code:

- References an existing Azure Container Registry named `my-acr` in the specified resource group.
- Attaches the registry to an Azure Container Apps environment named `env`.
- Uses parameters to allow for dynamic configuration of the registry name and resource group.
- Optionally grants the `AcrPush` role to a project resource named `api`, allowing it to push images to the registry.

### Key features

**Automatic credential flow**

When you attach an Azure Container Registry to a compute environment, Aspire automatically ensures that the correct credentials are available for secure image pulls.

**Fine-grained role assignments**

You can assign specific roles to Azure resources to control access to the container registry. For example, the `AcrPush` role allows a service to push images to the registry.

```csharp
builder.AddProject("api", "../Api/Api.csproj")
       .WithRoleAssignments(acr, ContainerRegistryBuiltInRole.AcrPush);
```

## See also

- [Azure Container Registry documentation](/azure/container-registry)
- [.NET Aspire Azure integrations overview](integrations-overview.md)
