---
title: Deploy .NET Aspire apps to Azure Container Apps using Visual Studio
description: Learn how to use Bicep, the Azure CLI, and Azure Developer CLI to deploy .NET Aspire apps to Azure using Visual Studio.
ms.date: 05/03/2024
---

# Deploy a .NET Aspire app to Azure Container Apps using Visual Studio

.NET Aspire apps are designed to run in containerized environments. Azure Container Apps is a fully managed environment that enables you to run microservices and containerized applications on a serverless platform. This article will walk you through creating a new .NET Aspire solution and deploying it to Microsoft Azure Container Apps using the Azure Developer CLI (`azd`), the Azure CLI, or Bicep. You'll learn how to complete the following tasks:

> [!div class="checklist"]
>
> - Provision an Azure resource group and Container Registry
> - Publish the .NET Aspire projects as container images in Azure Container Registry
> - Provision a Redis container in Azure
> - Deploy the apps to an Azure Container Apps environment
> - View application console logs to troubleshoot application issues

[!INCLUDE [aspire-prereqs](../../includes/aspire-prereqs.md)]

[!INCLUDE [file-new-aspire](../../includes/file-new-aspire.md)]

### Deploy the app

1. In the solution explorer, right-click on the **.AppHost** project and select **Publish** to open the **Publish** dialog.

1. Select **Azure Container Apps for .NET Aspire** as the publishing target.

    :::image type="content" source="../media/visual-studio-deploy.png" alt-text="A screenshot of the publishing dialog workflow.":::

1. On the **AzDev Environment** step, select your desired **Subscription** and **Location** values and then enter an **Environment name** such as *aspire-vs*. The environment name determines the naming of Azure Container Apps environment resources.

1. Select **Finish** to close the dialog workflow and view the deployment environment summary.

1. Select **Publish** to provision and deploy the resources on Azure. This process may take several minutes to complete. Visual Studio provides status updates on the deployment progress.

1. When the publish completes, Visual Studio displays the resource URLs at the bottom of the environment screen. Use these links to view the various deployed resources. Select the **webfrontend** URL to open a browser to the deployed app.

    :::image type="content" source="../media/visual-studio-deploy-complete.png" alt-text="A screenshot of the completed publishing process and deployed resources.":::

[!INCLUDE [test-deployed-app](includes/test-deployed-app.md)]

[!INCLUDE [clean-up-resources](../../includes/clean-up-resources.md)]
