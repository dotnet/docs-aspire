---
ms.topic: include
---

## Deploy .NET Aspire apps with AZD

With .NET Aspire and Azure Container Apps (ACA), you have a great hosting scenario for building out your cloud-native apps with .NET. We built some great new features into the Azure Developer CLI (AZD) specific for making .NET Aspire development and deployment to Azure a friction-free experience. You can still use the Azure CLI and/or Bicep options when you need a granular level of control over your deployments. But for new projects, you won't find an easier path to success for getting a new microservice topology deployed into the cloud.

[!INCLUDE [file-new-aspire](../../../includes/file-new-aspire.md)]

## Install the Azure Developer CLI

The process for installing AZD varies based on your operating system, but it is widely available via `winget`, `brew`, `apt`, or directly via `curl`. To install AZD, see [Install Azure Developer CLI](/azure/developer/azure-developer-cli/install-azd).

[!INCLUDE [init workflow](init-workflow.md)]

## Deploy the app

Once AZD has been initialized, the provisioning and deployment process can be executed as a single command, `azd up`. First, the projects will be packaged into containers during the `azd package` phase, followed by the `azd provision` phase during which all of the Azure resources the app will need are provisioned.

:::image type="content" source="../media/aspire-azd-06.png" lightbox="../media/aspire-azd-06.png" alt-text="AZD provisioning the Azure resources":::

Once `provision` is complete, `azd deploy` will take place. During this phase, the projects are pushed as containers into an Azure Container Registry instance, and then used to create new revisions of Azure Container Apps in which the code will be hosted.

:::image type="content" source="../media/aspire-azd-07.png" lightbox="../media/aspire-azd-07.png" alt-text="AZD deploying the application code":::

At this point the app has been deployed and configured, and you can open the Azure portal and explore the resources.
