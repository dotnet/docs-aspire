---
title: Deploy .NET Aspire apps to Azure Container Apps
description: Learn how to use Bicep, the Azure CLI, and Azure Developer CLI to deploy .NET Aspire apps to Azure.
ms.date: 03/08/2024
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

As an alternative to this tutorial and for a more in-depth guide, see [Deploy a .NET Aspire app to Azure Container Apps using `azd` (in-depth guide)](aca-deployment-azd-in-depth.md).

## Deploy .NET Aspire apps with `azd`

With .NET Aspire and Azure Container Apps (ACA), you have a great hosting scenario for building out your cloud-native apps with .NET. We built some great new features into the Azure Developer CLI (`azd`) specific for making .NET Aspire development and deployment to Azure a friction-free experience. You can still use the Azure CLI and/or Bicep options when you need a granular level of control over your deployments. But for new projects, you won't find an easier path to success for getting a new microservice topology deployed into the cloud.

[!INCLUDE [file-new-aspire](../../includes/file-new-aspire.md)]

## Install the Azure Developer CLI

The process for installing `azd` varies based on your operating system, but it is widely available via `winget`, `brew`, `apt`, or directly via `curl`. To install `azd`, see [Install Azure Developer CLI](/azure/developer/azure-developer-cli/install-azd).

[!INCLUDE [init-workflow](includes/init-workflow.md)]

## Deploy the app

Once `azd` is initialized, the provisioning and deployment process can be executed as a single command, [azd up](/azure/developer/azure-developer-cli/reference#azd-up).

[!INCLUDE [azd-up-output](includes/azd-up-output.md)]

First, the projects will be packaged into containers during the `azd package` phase, followed by the `azd provision` phase during which all of the Azure resources the app will need are provisioned.

Once `provision` is complete, `azd deploy` will take place. During this phase, the projects are pushed as containers into an Azure Container Registry instance, and then used to create new revisions of Azure Container Apps in which the code will be hosted.

At this point the app has been deployed and configured, and you can open the Azure portal and explore the resources.

[!INCLUDE [test-deployed-app](includes/test-deployed-app.md)]

[!INCLUDE [clean-up-resources](../../includes/clean-up-resources.md)]
