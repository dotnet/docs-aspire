---
ms.topic: include
---

[!INCLUDE [file-new-aspire](../../../includes/file-new-aspire.md)]

### Deploy the app

1. In the solution explorer, right-click on the **.AppHost** project and select **Publish** to open the **Publish** dialog.

1. Select **Azure Container Apps for .NET Aspire** as the publishing target.

    :::image type="content" source="../../media/visual-studio-deploy.png" alt-text="A screenshot of the publishing dialog workflow.":::

1. On the **AzDev Environment** step, select your desired **Subscription** and **Location** values and then enter an **Environment name** such as *aspire-vs*. The environment name determines the naming of Azure Container Apps environment resources.

1. Select **Finish** to close the dialog workflow and view the deployment environment summary.

1. Select **Publish** to provision and deploy the resources on Azure. This process may take several minutes to complete. Visual Studio provides status updates on the deployment progress.

1. When the publish completes, Visual Studio displays the resource URLs at the bottom of the environment screen. Use these links to view the various deployed resources. Select the **webfrontend** URL to open a browser to the deployed app.

    :::image type="content" source="../../media/visual-studio-deploy-complete.png" alt-text="A screenshot of the completed publishing process and deployed resources.":::
