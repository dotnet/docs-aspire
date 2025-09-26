---
title: Deploy .NET Aspire projects to Azure Container Apps using the Aspire CLI
description: Learn how to use the Aspire CLI command to deploy .NET Aspire projects to Azure Container Apps.
ms.date: 09/25/2025
ai-usage: ai-assisted
---

# Deploy a .NET Aspire project to Azure Container Apps using the Aspire CLI

The [`aspire deploy`](../../cli-reference/aspire-deploy.md) command is a **generic deployment entrypoint** for .NET Aspire applications. It doesn't intrinsically "know" how to deploy to specific platforms. Instead, it discovers and executes **deployment integrations** contributed by packages referenced in your AppHost project.

When your application includes Azure-related hosting packages or resources (for example referencing `Aspire.Hosting.Azure.*` packages, or adding Azure resources with `builder.AddAzure*` methods), the **Azure deployment integration** is automatically registered. That integration contributes:

- Infrastructure as code templates (Bicep)
- Azure resource dependency resolution
- Container image build & push targeting Azure Container Registry (ACR)
- Compute deployment to Azure Container Apps (ACA) when an ACA environment is defined

This article focuses on the Azure Container Apps scenario enabled by that Azure deployment integration.

> [!IMPORTANT]
> Using `aspire deploy` alone does **not** result in an Azure Container Apps deployment. ACA deployment occurs only if all of the following are true:
>
> 1. The Azure deployment integration is present (automatically when Azure packages/resources are referenced).
> 2. You define an Azure Container Apps environment via `builder.AddAzureContainerAppEnvironment(...)`.
> 3. You have one or more project or container resources suitable for deployment as Container Apps.

> [!div class="checklist"]
>
> - Reference Azure packages or add Azure resources (activates Azure integration).
> - Define an Azure Container Apps environment.
> - Run `aspire deploy` to orchestrate provision + build + push + deploy.
> - Monitor progress and access the deployed application.

[!INCLUDE [aspire-prereqs](../../includes/aspire-prereqs.md)]

## Prerequisites

For this Azure Container Apps scenario ensure you have:

- **Azure CLI**: Installed and authenticated.
- **Docker**: Installed and running (for building container images).
- **Azure subscription**: Permissions to create resource groups, Container Apps, ACR, etc.

## How `aspire deploy` discovers deployment integrations

When you run `aspire deploy`, the CLI:

1. Loads your AppHost project and evaluates the application model.
2. Detects deployment integrations registered via referenced SDKs/packages.
3. Executes each integration's contributed pipeline stages (for example: parameter collection, infrastructure provisioning, image build/publish, platform-specific deployment, post-deploy outputs).

For Azure specifically:

- Referencing `Aspire.Hosting.Azure.AppContainers` (or adding any other Azure resource extension) registers the Azure deployment integration.
- Adding `builder.AddAzureContainerAppEnvironment("env-name")` indicates Container Apps is a compute target.
- Adding resources like `builder.AddRedis("cache")` plus Azure backing services triggers generation of the necessary provisioning templates.

## Authenticate with Azure

Authenticate before deploying:

```azurecli
az login
```

`aspire deploy` validates Azure CLI authentication at the start of the Azure integration pipeline.

- [Sign in with Azure CLI](/cli/azure/authenticate-azure-cli)
- [`aspire deploy` command reference](/dotnet/aspire/cli-reference/aspire-deploy)

## Create a .NET Aspire project

As a starting point, this article assumes you've created a .NET Aspire project from the **.NET Aspire Starter Application** template. For more information, see [Quickstart: Build your first .NET Aspire project](../../get-started/build-your-first-aspire-app.md).

To configure your project for Azure Container Apps deployment, add a package reference to your AppHost project that includes the [`📦Aspire.Hosting.Azure.AppContainers](https://www.nuget.org/packages/Aspire.Hosting.Azure.AppContainers) NuGet package:

```xml
<Project Sdk="Microsoft.NET.Sdk">

    <Sdk Name="Aspire.AppHost.Sdk" Version="9.5.0" />

    <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net9.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
    <UserSecretsId>7b352f08-305b-4032-9a21-90deb02efc04</UserSecretsId>
    </PropertyGroup>

    <ItemGroup>
    <PackageReference Include="Aspire.Hosting.AppHost" Version="9.5.0" />
    <PackageReference Include="Aspire.Hosting.Azure.AppContainers" Version="9.5.0" />
    </ItemGroup>

</Project>
```

In your AppHost project's _AppHost.cs_ file, add the Container Apps environment:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Add Azure Container Apps environment
var containerAppEnv = builder.AddAzureContainerAppEnvironment("aspire-env");

// Add your services
var cache = builder.AddRedis("cache");

var apiService = builder.AddProject<Projects.AspireApp_ApiService>("apiservice")
    .WithExternalHttpEndpoints();

builder.AddProject<Projects.AspireApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WithReference(apiService);

builder.Build().Run();
```

> [!NOTE]
> If you omit `AddAzureContainerAppEnvironment`, the Azure integration may still provision supporting Azure resources (like storage) but will not deploy your projects as Azure Container Apps.

For more information, see [Configure Azure Container Apps environments](../../azure/configure-aca-environments.md).

### Resource naming

[!INCLUDE [azure-container-app-naming](../../includes/azure-container-app-naming.md)]

## Deploy with `aspire deploy`

Run:

```Aspire
aspire deploy
```

> [!TIP]
> To run non-interactively, set:
>
> - `Azure__SubscriptionId`
> - `Azure__Location` (for example, `eastus`)
> - `Azure__ResourceGroup`
>
> The Azure integration consumes these values to skip prompts.

Specify a project explicitly if needed:

```Aspire
aspire deploy --project ./MyApp.AppHost/MyApp.AppHost.csproj
```

## Deployment process (Azure integration stages)

The following conceptual stages are executed by the Azure deployment integration (surfaced through the generic `aspire deploy` pipeline):

1. Authentication validation – Ensures `az login` has occurred.
2. Subscription / location / resource group selection – Interactive unless overridden.
3. Parameter resolution – Prompts for unresolved application model parameters.
4. Infrastructure provisioning (Bicep) – Creates ACA environment, backing stores, identities, etc.
5. Container image build – Builds project/container resources locally.
6. Registry authentication – Logs in to the target Azure Container Registry (ACR).
7. Image push – Pushes tagged images to ACR.
8. Compute deployment – Creates/updates Azure Container Apps for each deployable resource.
9. Dashboard & endpoint discovery – Emits Aspire dashboard and app URLs.

(Your existing console output format corresponds to these conceptual steps.)

### Parameter prompts

Example:

```Output
There are unresolved parameters that need to be set. Please provide values for them. [y/n] (n): y
Please provide values for the unresolved parameters. Parameters can be saved to user secrets for future use.
weatherApiKey:
Save to user secrets: [y/n] (n): n
```

> [!NOTE]
> During preview, user secret persistence may be limited; future releases may streamline reuse.

Once parameters are collected, Azure infrastructure is provisioned using Bicep templates. This step creates the necessary Azure resources including the Container Apps environment, Container Registry, and any backing services like Redis caches:

```Output
Step 3: Deploying Azure resources

    ✓ DONE: Deploying aca-env                              00:01:08
        Successfully provisioned aca-env
    ✓ DONE: Deploying storage                              00:00:04
        Successfully provisioned storage
    ✓ DONE: Deploying cosmosdb                             00:01:08
        Successfully provisioned cosmosdb
    ✓ DONE: Deploying apiservice-identity                  00:00:03
        Successfully provisioned apiservice-identity
    ✓ DONE: Deploying apiservice-roles-storage             00:00:11
        Successfully provisioned apiservice-roles-storage
    ✓ DONE: Deploying apiservice-roles-cosmosdb            00:01:10
        Successfully provisioned apiservice-roles-cosmosdb
```

After infrastructure provisioning, your application projects are built as container images and pushed to Azure Container Registry.

```Output
Step 4: Building container images for resources

    ✓ DONE: Checking Docker health               00:00:00
        Docker is healthy.
    ✓ DONE: Building image: apiservice           00:00:04
        Building image for apiservice completed
    ✓ DONE: Building image: webfrontend          00:00:05
        Building image for webfrontend completed

✅ COMPLETED: Building container images completed

Step 5: Authenticating to container registries

    ✓ DONE: Logging in to acrname         00:00:08
        Successfully logged in to acrname

✅ COMPLETED: Successfully authenticated to 1 container registries

Step 6: Pushing 2 images to container registries

    ✓ DONE: Pushing apiservice to acrname                                                              00:00:04
        Successfully pushed apiservice to acrname.azurecr.io/apiservice:aspire-deploy-20250922203320
    ✓ DONE: Pushing webfrontend to acrname                                                             00:00:04
        Successfully pushed webfrontend to acrname.azurecr.io/webfrontend:aspire-deploy-20250922203320

✅ COMPLETED: Successfully pushed 2 images to container registries
```

Finally, all .NET projects that were built as container images from the previous step, are pushed to provisioned Azure Container Registry—where they'll live, making them available to Container Apps:

```Output
Step 7: Deploying compute resources

    ✓ DONE: Deploying apiservice                                                                                   00:00:35
        Successfully deployed apiservice to https://apiservice.proudplant-5c457a5d.westus2.azurecontainerapps.io
    ✓ DONE: Deploying webfrontend                                                                                  00:00:19
        Successfully deployed webfrontend to https://webfrontend.proudplant-5c457a5d.westus2.azurecontainerapps.io

✅ COMPLETED: Successfully deployed 2 compute resources
```

Upon successful deployment, you'll see the Aspire dashboard URL where you can monitor and manage your deployed application:

```Output
✅ Deployment completed successfully. View Aspire dashboard at https://aspire-env.proudwater-12345678.eastus.azurecontainerapps.io
```

For more information on what options are available on the `aspire deploy` command and examples of how to supply them on the CLI, see the [`aspire deploy` command reference](../../cli-reference/aspire-deploy.md).

## Monitor deployment

During deployment, you can monitor progress through:

1. **Console output**: Real-time progress updates and status messages.
1. **Azure portal**: View resources being created in your resource group.
1. **Azure CLI**: Use `az containerapp list` to see deployed applications.

## Troubleshooting deployment

When using the `aspire deploy` command, you may encounter various issues during deployment. Use this section to learn common problems and we provide you with tips for troubleshooting, helping you quickly identify and fix errors. With the right approach and understanding of common issues, you can ensure a smoother deployment process.

### Authentication issues

If you encounter authentication errors:

```bash
❌ Azure CLI authentication failed. Please run 'az login' to authenticate before deploying.
```

Run `az login` and ensure you have the necessary permissions in your Azure subscription. For more information, see [Sign in with Azure CLI](/cli/azure/authenticate-azure-cli).

### Resource naming conflicts

If resource names conflict with existing Azure resources, modify your resource names in the AppHost:

```csharp
var containerAppEnv = builder.AddAzureContainerAppEnvironment(
        "my-unique-env-name"
    );
```

### Container build failures

If container builds fail, ensure:

- Docker is running and accessible.
- Your project builds successfully locally: `dotnet build`.
- Container runtime is properly configured.

## Access your deployed application

After successful deployment:

1. **Aspire Dashboard**: Access the dashboard URL provided in the deployment output.
1. **Application endpoints**: Find your application URLs in the Azure portal under Container Apps.
1. **Azure CLI**: List endpoints using:

   ```azurecli
   az containerapp show --name <app-name> --resource-group <resource-group> --query properties.configuration.ingress.fqdn
   ```

## Clean up resources

Delete the resource group to remove provisioned Azure resources:

```azurecli
az group delete --name <resource-group-name>
```

## Next steps

- [aspire deploy command reference](../../cli-reference/aspire-deploy.md).
- [Customize Azure deployments in Aspire](customize-deployments.md).
- [Azure security best practices for Aspire apps](azure-security-best-practices.md).
