---
title: Deploy .NET Aspire projects to Azure Container Apps using the Aspire CLI
description: Learn how to use the Aspire CLI command to deploy .NET Aspire projects to Azure Container Apps.
ms.date: 09/22/2025
ai-usage: ai-assisted
---

# Deploy a .NET Aspire project to Azure Container Apps using Aspire CLI

The `aspire deploy` CLI command provides a streamlined way to deploy .NET Aspire applications directly to Azure Container Apps. This command automates the entire deployment process, from building container images to provisioning Azure infrastructure and deploying your applications. This article will walk you through using the `aspire deploy` command to deploy a .NET Aspire solution to Microsoft Azure Container Apps. You'll learn how to complete the following tasks:

> [!div class="checklist"]
>
> - Use the `aspire deploy` command to deploy to Azure Container Apps
> - Validate Azure CLI authentication for deployment
> - Provision Azure infrastructure using Bicep templates
> - Build and push container images to Azure Container Registry
> - Deploy compute resources to Azure Container Apps
> - Monitor deployment progress and access the deployed application

[!INCLUDE [aspire-prereqs](../../includes/aspire-prereqs.md)]

## Prerequisites

Before using the `aspire deploy` command, ensure you have the following:

- **Azure CLI**: Installed and authenticated with your Azure account
- **Docker**: Installed for building container images
- **Azure subscription**: With appropriate permissions to create resources

## Enable the deploy command

The `aspire deploy` command is currently in preview and disabled by default. To enable it:

```bash
aspire config set features.deployCommandEnabled true
```

## Authenticate with Azure

Before deploying, you must authenticate with Azure CLI. Run the following command:

```bash
az login
```

This command opens a web browser for you to sign in with your Azure credentials. The `aspire deploy` command automatically validates your Azure CLI authentication before proceeding with deployment.

## Create a .NET Aspire project

As a starting point, this article assumes you've created a .NET Aspire project from the **.NET Aspire Starter Application** template. For more information, see [Quickstart: Build your first .NET Aspire project](../../get-started/build-your-first-aspire-app.md).

To configure your project for Azure Container Apps deployment, add Azure hosting support to your AppHost project:

### Configure Azure environment

In your AppHost project's `AppHost.cs` file, add the Azure Container Apps environment:

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

### Resource naming

[!INCLUDE [azure-container-app-naming](../../includes/azure-container-app-naming.md)]

## Deploy with aspire deploy

Once your project is configured, deploy it using the `aspire deploy` command:

```bash
aspire deploy
```

You can also specify additional options:

```bash
aspire deploy --project ./MyApp.AppHost/MyApp.AppHost.csproj
```

## Deployment process

The `aspire deploy` command performs the following steps automatically. First, the command validates that you're authenticated with Azure CLI. If you're not logged in, you'll see an error message prompting you to run `az login`.

Next, Aspire prompts you for the Azure subscription, location, and resource group name that the deployment should be targeted to. The subscriptions and locations that are accessible depend on the Azure account that is authenticated with the Azure CLI from the step above.

Next, Aspire analyzes your application model and prompts for any required deployment parameters that don't have values set. You'll see prompts like:

```
There are unresolved parameters that need to be set. Please provide values for them. [y/n] (n): y
Please provide values for the unresolved parameters. Parameters can be saved to user secrets for future use.
weatherApiKey: 
Save to user secrets: [y/n] (n): n
```

> The CLI will continuously prompt until all unresolved parameters are provided with values. While Azure deployment is in preview, the CLI will prompt to save values in user secrets but not use them, as [deployment state is not supported](https://github.com/dotnet/aspire/issues/11444).

Once parameters are collected, Azure infrastructure is provisioned using Bicep templates. This step creates the necessary Azure resources including the Container Apps environment, Container Registry, and any backing services like Redis caches:

```
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

```
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

Finally, your applications are deployed to Azure Container Apps using the container images from the previous step:

```
Step 7: Deploying compute resources

    ✓ DONE: Deploying apiservice                                                                                   00:00:35
        Successfully deployed apiservice to https://apiservice.proudplant-5c457a5d.westus2.azurecontainerapps.io
    ✓ DONE: Deploying webfrontend                                                                                  00:00:19
        Successfully deployed webfrontend to https://webfrontend.proudplant-5c457a5d.westus2.azurecontainerapps.io

✅ COMPLETED: Successfully deployed 2 compute resources
```

Upon successful deployment, you'll see the Aspire dashboard URL where you can monitor and manage your deployed application:

```
✅ Deployment completed successfully. View Aspire dashboard at https://aspire-env.proudwater-12345678.eastus.azurecontainerapps.io
```

## Command options

The `aspire deploy` command supports the following options:

- **`--project`**: Path to the AppHost project file (optional)
- **`--output-path`**: Directory for deployment artifacts (optional)
- **`--`**: Delimiter for arguments passed to the AppHost

### Examples

Deploy with specific project file:

```bash
aspire deploy --project ./src/MyApp.AppHost/MyApp.AppHost.csproj
```

Deploy with custom output path:

```bash
aspire deploy --output-path ./my-deployment-artifacts
```

Deploy with AppHost arguments:

```bash
aspire deploy -- --environment Production
```

## Monitor deployment

During deployment, you can monitor progress through:

1. **Console output**: Real-time progress updates and status messages
2. **Azure portal**: View resources being created in your resource group
3. **Azure CLI**: Use `az containerapp list` to see deployed applications

## Troubleshooting deployment

### Authentication issues

If you encounter authentication errors:

```bash
❌ Azure CLI authentication failed. Please run 'az login' to authenticate before deploying.
```

Run `az login` and ensure you have the necessary permissions in your Azure subscription.

### Resource naming conflicts

If resource names conflict with existing Azure resources, modify your resource names in the AppHost:

```csharp
var containerAppEnv = builder.AddAzureContainerAppEnvironment("my-unique-env-name");
```

### Container build failures

If container builds fail, ensure:

- Docker is running and accessible
- Your project builds successfully locally: `dotnet build`
- Container runtime is properly configured

## Access your deployed application

After successful deployment:

1. **Aspire Dashboard**: Access the dashboard URL provided in the deployment output
2. **Application endpoints**: Find your application URLs in the Azure portal under Container Apps
3. **Azure CLI**: List endpoints using:

   ```bash
   az containerapp show --name <app-name> --resource-group <resource-group> --query properties.configuration.ingress.fqdn
   ```

## Clean up resources

To remove deployed resources, delete the resource group:

```bash
az group delete --name <resource-group-name>
```

## Next steps

- [aspire deploy command reference](../../cli-reference/aspire-deploy.md)
- [Customize .NET Aspire Azure deployments](customize-deployments.md)
- [Azure security best practices for .NET Aspire apps](azure-security-best-practices.md)
