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
> - Enable the (preview) `aspire deploy` feature.
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

## Enable the deploy command (preview)

The `aspire deploy` command is currently in preview and disabled by default. To enable it:

```Aspire
aspire config set features.deployCommandEnabled true
```

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

## Create or adapt a .NET Aspire project for Azure Container Apps

Start from the **.NET Aspire Starter Application** template (or an existing Aspire AppHost). Then add the Azure AppContainers package so the Azure integration (including ACA support) is available:

```xml
<PackageReference Include="Aspire.Hosting.Azure.AppContainers" Version="9.5.0" />
```

In your AppHost code, define an Azure Container Apps environment and your services:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Registers an ACA environment so the Azure integration deploys compute to Container Apps
var containerAppEnv = builder.AddAzureContainerAppEnvironment("aspire-env");

// Example service topology
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

### Provisioning infrastructure

Provisioning uses generated Bicep templates produced by the Azure deployment integration (not by the generic CLI alone). Example output (truncated):

```Output
Step 3: Deploying Azure resources
    ✓ DONE: Deploying aca-env                              00:01:08
    ...
```

### Building and pushing images

Images are tagged (by convention) with an `aspire-deploy-<timestamp>` suffix before being pushed to ACR.

### Deploying compute (Container Apps)

Each project with external endpoints (or appropriate deployment metadata) becomes a Container App:

```Output
Step 7: Deploying compute resources
    ✓ DONE: Deploying apiservice ...
    ✓ DONE: Deploying webfrontend ...
```

### Completion

```Output
✅ Deployment completed successfully. View Aspire dashboard at https://aspire-env.proudwater-12345678.eastus.azurecontainerapps.io
```

## Monitor deployment

You can monitor through:

1. **Console output** – Integration-provided progress.
2. **Azure portal** – Resource group activity.
3. **Azure CLI**:

   ```azurecli
   az containerapp list --resource-group <resource-group>
   ```

## Troubleshooting

### Authentication issues

```bash
❌ Azure CLI authentication failed. Please run 'az login' to authenticate before deploying.
```

Run `az login` and ensure you have the necessary permissions.

### Missing Azure integration behavior

If nothing is provisioned for Azure:

- Confirm an Azure hosting package is referenced (for example `Aspire.Hosting.Azure.AppContainers`).
- Confirm `builder.AddAzureContainerAppEnvironment(...)` is present for ACA compute.

### Resource naming conflicts

Adjust names in your AppHost code:

```csharp
var containerAppEnv = builder.AddAzureContainerAppEnvironment("my-unique-env-name");
```

### Container build failures

Check:

- Docker is running.
- Local build succeeds: `dotnet build`.
- No conflicting Docker daemon permissions.

## Access your deployed application

1. **Aspire Dashboard**: URL from deployment output.
2. **Application endpoints**: Visible in the Container Apps blade per app.
3. **CLI**:

   ```azurecli
   az containerapp show --name <app-name> --resource-group <resource-group> --query properties.configuration.ingress.fqdn
   ```

## Other deployment integrations

`aspire deploy` is designed to orchestrate multiple platform integrations. While this article demonstrates Azure Container Apps, the same model can enable additional targets (for example Kubernetes-based environments) through their own integrations. Each integration is responsible for:

- Describing provisionable infrastructure and dependencies.
- Building/publishing artifacts needed by its platform.
- Applying platform-specific deployment operations.
- Emitting post-deployment outputs (endpoints, dashboards, connection info).

Consult the documentation for any additional integration you add to understand its specific configuration and prerequisites.

## Clean up resources

Delete the resource group to remove provisioned Azure resources:

```azurecli
az group delete --name <resource-group-name>
```

## Next steps

- [`aspire deploy` command reference](../../cli-reference/aspire-deploy.md)
- [Customize Azure deployments in Aspire](customize-deployments.md)
- [Azure security best practices for Aspire apps](azure-security-best-practices.md)
