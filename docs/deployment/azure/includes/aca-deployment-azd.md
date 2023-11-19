---
ms.topic: include
---

## Deploy .NET Aspire apps with AZD

With .NET Aspire and Azure Container Apps (ACA), you have a great hosting scenario for building out your cloud-native apps with .NET. We built some great new features into the Azure Developer CLI (AZD) specific for making .NET Aspire development and deployment to Azure a friction-free experience. You can still use the Azure CLI and/or Bicep options when you need a granular level of control over your deployments. But for new projects, you won't find an easier path to success for getting a new microservice topology deployed into the cloud.

[!INCLUDE [file-new-aspire](../../../includes/file-new-aspire.md)]

## Install the Azure Developer CLI

The process for installing AZD varies based on your operating system, but it is widely available via `winget`, `brew`, `apt`, or directly via `curl`. To install AZD, see [Install Azure Developer CLI](/azure/developer/azure-developer-cli/install-azd).

## Create a new AZD environment

Once you've created your .NET Aspire sample project, open your terminal window and `cd` into the root of the solution. Then, execute the command `azd init`. AZD will first inspect the local directory structure to determine the type of app being deployed.

:::image type="content" source="../media/aspire-azd-01.png" lightbox="../media/aspire-azd-01.png" alt-text="AZD inspecting the local source tree":::

In a few seconds, AZD will figure out that this is a .NET Aspire app, and locate the App Host project. It will then suggest a deployment to Azure Container Apps. Select the `Confirm and continue...` option to move on.

:::image type="content" source="../media/aspire-azd-02.png" lightbox="../media/aspire-azd-02.png" alt-text="Accepting the .NET app type and Azure Container Apps deployment target":::

Next, AZD will present each of the projects in the .NET Aspire solution and provide you the opportunity to identify which project(s) will be deployed with HTTP ingress open publicly to all internet traffic. Since you'll want the API to be private only to the Azure Container Apps environment and *not* available publicly, select only the `webfrontend`.

:::image type="content" source="../media/aspire-azd-03.png" lightbox="../media/aspire-azd-03.png" alt-text="Specifying the containers that are publicly available to the Internet":::

The final step in the `init` phase is setting the environment name. This is helpful when you want to have multiple environments, like `dev` and `prod`, or one-off test environments you plan on deleting momentarily. Provide the name of the environment, then allow AZD to continue.

:::image type="content" source="../media/aspire-azd-04.png" lightbox="../media/aspire-azd-04.png" alt-text="Providing the name of the AZD environment to be created":::

AZD will then complete the initialization and render a markdown file for you to review in case you want to learn more.

:::image type="content" source="../media/aspire-azd-05.png" lightbox="../media/aspire-azd-05.png" alt-text="Post-initialization of AZD environment":::

Now, it's time to deploy the app with `azd up`.

## Deploy the app

Once AZD has been initialized, the provisioning and deployment process can be executed as a single command, `azd up`. First, the projects will be packaged into containers during the `azd package`phase, followed by the `azd provision` phase during which all of the Azure resources the app will need are provisioned.

:::image type="content" source="../media/aspire-azd-06.png" lightbox="../media/aspire-azd-06.png" alt-text="AZD provisioning the Azure resources":::

Once `provision` is complete, `azd deploy` will take place. During this phase, the projects are pushed as containers into an Azure Container Registry instance, and then used to create new revisions of Azure Container Apps in which the code will be hosted.

:::image type="content" source="../media/aspire-azd-06.png" lightbox="../media/aspire-azd-07.png" alt-text="AZD deploying the application code":::

At this point the app has been deployed and configured, and you can open the Azure portal and explore the resources.
