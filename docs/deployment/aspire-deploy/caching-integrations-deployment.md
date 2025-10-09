---
title: Deploy an Aspire project that connects to Redis Cache to Azure
description: Learn how to deploy an Aspire project that connects to Redis Cache to Azure
ms.date: 09/30/2025
ms.topic: how-to
---

# Tutorial: Deploy an Aspire project with a Redis Cache to Azure

In this tutorial, you learn to configure an Aspire project with a Redis Cache for deployment to Azure. Aspire provides multiple caching integration configurations that provision different Redis services in Azure. You'll learn how to:

> [!div class="checklist"]
>
> - Configure the app to provision an Azure Cache for Redis
> - Configure the app to provision a containerized Redis Cache

> [!NOTE]
> This document focuses specifically on Aspire configurations to provision and deploy Redis Cache resources in Azure. For more information and to learn more about the full Aspire deployment process, see the [Azure Container Apps deployment](/dotnet/aspire/deployment/azure/aca-deployment?pivots=azure-azd) tutorial.

[!INCLUDE [aspire-prereqs](../../includes/aspire-prereqs.md)]

## Create the sample solution

Follow the [Tutorial: Implement caching with Aspire integrations](../../caching/caching-integrations.md) to create the sample project.

## Configure the app for Redis cache deployment

Aspire provides two built-in configuration options to streamline Redis Cache deployment on Azure:

- Provision a containerized Redis Cache using Azure Container Apps
- Provision an Azure Cache for Redis instance

### Add the Aspire integration to the app

Add the appropriate Aspire integration to the _AspireRedis.AppHost_ project for your desired hosting service.

# [Azure Cache for Redis](#tab/azure-redis)

Add the [📦 Aspire.Hosting.Azure.Redis](https://www.nuget.org/packages/Aspire.Hosting.Azure.Redis) NuGet package to the _AspireRedis.AppHost_ project:

```dotnetcli
dotnet add package Aspire.Hosting.Azure.Redis
```

## [Redis Container](#tab/redis-container)

Add the [📦 Aspire.Hosting.Redis](https://www.nuget.org/packages/Aspire.Hosting.Redis) NuGet package to the _AspireRedis.AppHost_ project:

```dotnetcli
dotnet add package Aspire.Hosting.Redis
```

---

### Configure the AppHost project

Configure the _AspireRedis.AppHost_ project for your desired Redis service.

# [Azure Cache for Redis](#tab/azure-redis)

Replace the contents of the _:::no-loc text="Program.cs":::_ file in the _AspireRedis.AppHost_ project with the following code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddAzureRedis("cache");

var apiService = builder.AddProject<Projects.AspireRedis_ApiService>("apiservice")
                        .WithReference(cache);

builder.AddProject<Projects.AspireRedis_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
```

The preceding code adds an Azure Cache for Redis resource to your app and configures a connection called `cache`. The `AddAzureRedis` method ensures that tools such as the Azure Developer CLI or Visual Studio create an Azure Cache for Redis resource during the deployment process.

## [Redis Container](#tab/redis-container)

Replace the contents of the _:::no-loc text="Program.cs":::_ file in the _AspireRedis.AppHost_ project with the following code:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var apiService = builder.AddProject<Projects.RedisSample_ApiService>("apiservice")
                        .WithReference(cache);

builder.AddProject<Projects.RedisSample_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(cache)
    .WaitFor(cache)
    .WithReference(apiService)
    .WaitFor(apiService);

builder.Build().Run();
```

The preceding code adds a Redis Container resource to your app and configures a connection called `cache`. This configuration also ensures that tools such as the Azure Developer CLI or Visual Studio create a containerized Redis instance during the deployment process.

---

## Deploy the app

The `aspire deploy` command supports Aspire Redis integration configurations to streamline deployments. The command consumes these settings and provisions properly configured resources for you.

> [!NOTE]
> You can also use the [Azure CLI](/dotnet/aspire/deployment/azure/aca-deployment?pivots=azure-cli) or [Bicep](/dotnet/aspire/deployment/azure/aca-deployment?pivots=azure-bicep) to provision and deploy Aspire project resources. These options require more manual steps, but provide more granular control over your deployments. Aspire projects can also connect to an existing Redis instance through manual configurations.

To deploy your app to Azure Container Apps, run the following command from the _AspireRedis.AppHost_ directory:

```Aspire
aspire deploy
```

When you run the `aspire deploy` command for the first time, you'll be prompted to:

1. **Sign in to Azure**: Follow the authentication prompts to sign in to your Azure account.
1. **Select a subscription**: Choose the Azure subscription you want to use for deployment.
1. **Select or create a resource group**: Choose an existing resource group or create a new one.
1. **Select a location**: Choose the Azure region where you want to deploy your resources.

The deployment process will provision the necessary Azure resources and deploy your Aspire app. The process may take a few minutes to complete.

When the deployment finishes, the command output will provide information about the deployed resources that you can view in the Azure portal.

## [Azure Cache for Redis](#tab/azure-redis)

The deployment process provisioned an Azure Cache for Redis resource due to the **.AppHost** configuration you provided.

:::image type="content" loc-scope="azure" source="../../caching/media/resources-azure-redis.png" alt-text="A screenshot showing the deployed Azure Cache for Redis.":::

## [Redis Container](#tab/redis-container)

The deployment process created a Redis app container due to the **.AppHost** configuration you provided.

:::image type="content" loc-scope="azure" source="../../caching/media/resources-azure-redis-container.png" alt-text="A screenshot showing the containerized Redis.":::

---

[!INCLUDE [clean-up-resources](../../includes/clean-up-resources.md)]

## See also

- [Aspire deployment via Azure Container Apps](../azd/aca-deployment.md)
- [Aspire Azure Container Apps deployment deep dive](../azd/aca-deployment-azd-in-depth.md)
- [Deploy an Aspire project using GitHub Actions](../azd/aca-deployment-github-actions.md)
