---
title: Deploy .NET Aspire apps to Azure Container Apps
description: Learn how to use Bicep, the Azure CLI, and Azure Developer CLI to deploy .NET Aspire apps to Azure.
ms.date: 11/13/2023
zone_pivot_groups: azure-development-tool
ms.custom: devx-track-extended-azdevcli
---

# Deploy a .NET Aspire app to Azure Container Apps

.NET Aspire apps are designed to run in containerized environments. Azure Container Apps is a fully managed environment that enables you to run microservices and containerized applications on a serverless platform. This article will walk you through creating a new .NET Aspire solution and deploying it to Microsoft Azure Container Apps using the Azure Developer CLI (`azd`), the Azure CLI, or Bicep. You'll learn how to complete the following tasks:

> [!div class="checklist"]
>
> - Provision an Azure resource group and Container Registry
> - Publish the .NET Aspire projects as container images in Azure Container Registry
> - Provision a Redis container in Azure
> - Deploy the apps to an Azure Container Apps environment
> - View application console logs to troubleshoot application issues

[!INCLUDE [aspire-prereqs](../../includes/aspire-prereqs.md)]

:::zone pivot="azure-azd"

As an alternative to this tutorial and for a more in-depth guide, see [Deploy a .NET Aspire app to Azure Container Apps using AZD (in-depth guide)](aca-deployment-azd-in-depth.md).

[!INCLUDE [aca-deployment-azd](includes/aca-deployment-azd.md)]

:::zone-end
:::zone pivot="azure-cli"

[!INCLUDE [aca-deployment-cli](includes/aca-deployment-cli.md)]

:::zone-end
:::zone pivot="azure-bicep"

[!INCLUDE [aca-deployment-bicep](includes/aca-deployment-bicep.md)]

:::zone-end

## Test the deployed app

Now that the app has been provisioned and deployed, you can browse to the Azure portal. In the resource group where you deployed the app, you'll see the three container apps and other resources.

:::image type="content" source="../../media/portal-screenshot-post-deploy.png" alt-text="A screenshot of the .NET Aspire app's resource group in the Azure portal.":::

Click on the `web` Container App to open it up in the portal.

:::image type="content" source="../../media/portal-screens-web-container-app.png" alt-text="A screenshot of the .NET Aspire app's front end in the Azure portal.":::

Click the **Application URL** link to open the front end in the browser.

:::image type="content" source="../../media/front-end-open.png" alt-text="A screenshot of the .NET Aspire app's front end in the browser.":::

When you click the "Weather" node in the navigation bar, the front end `web` container app makes a call to the `apiservice` container app to get data. The front end's output will be cached using the `redis` container app and the [.NET Aspire Redis Output Caching component](../../caching/stackexchange-redis-output-caching-component.md). As you refresh the front end a few times, you'll notice that the weather data is cached. It will update after a few seconds.

[!INCLUDE [clean-up-resources](../../includes/clean-up-resources.md)]
