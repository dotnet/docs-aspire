---
title: Deploy Aspire projects to Azure Container Apps using Visual Studio
description: Learn how to deploy Aspire projects to Azure Container Apps using Visual Studio.
ms.date: 09/23/2025
ms.custom: sfi-image-nochange
---

# Deploy a Aspire project to Azure Container Apps using Visual Studio

Aspire projects are designed to run in containerized environments. Azure Container Apps is a fully managed environment that enables you to run microservices and containerized applications on a serverless platform. This article will walk you through creating a new Aspire solution and deploying it to Microsoft Azure Container Apps using the Visual Studio. You'll learn how to complete the following tasks:

> [!div class="checklist"]
>
> - Provision an Azure resource group and Container Registry
> - Publish the Aspire projects as container images in Azure Container Registry
> - Provision a Redis container in Azure
> - Deploy the apps to an Azure Container Apps environment
> - View application console logs to troubleshoot application issues

[!INCLUDE [aspire-prereqs](../../includes/aspire-prereqs.md)]

## Create a Aspire project

As a starting point, this article assumes that you've created a Aspire project from the **Aspire Starter Application** template. For more information, see [Quickstart: Build your first Aspire project](../../get-started/build-your-first-aspire-app.md).

### Resource naming

[!INCLUDE [azure-container-app-naming](../../includes/azure-container-app-naming.md)]

### Deploy the app

1. In the solution explorer, right-click on the **.AppHost** project and select **Publish** to open the **Publish** dialog.

1. Select **Azure Container Apps for Aspire** as the publishing target.

    :::image type="content" loc-scope="visual-studio" source="../media/visual-studio-deploy.png" alt-text="A screenshot of the publishing dialog workflow.":::

1. On the **AzDev Environment** step, select your desired **Subscription** and **Location** values and then enter an **Environment name** such as *aspire-vs*. The environment name determines the naming of Azure Container Apps environment resources.

1. Select **Finish** to close the dialog workflow and view the deployment environment summary.

1. Select **Publish** to provision and deploy the resources on Azure. This process may take several minutes to complete. Visual Studio provides status updates on the deployment progress.

1. When the publish completes, Visual Studio displays the resource URLs at the bottom of the environment screen. Use these links to view the various deployed resources. Select the **webfrontend** URL to open a browser to the deployed app.

    :::image type="content" loc-scope="visual-studio" source="../media/visual-studio-deploy-complete.png" alt-text="A screenshot of the completed publishing process and deployed resources.":::

[!INCLUDE [test-deployed-app](../includes/test-deployed-app.md)]

[!INCLUDE [azd-dashboard](../includes/azd-dashboard.md)]

[!INCLUDE [clean-up-resources](../../includes/clean-up-resources-visual-studio.md)]
