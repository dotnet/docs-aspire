---
ms.topic: include
---

[!INCLUDE [file-new-aspire](../../../includes/file-new-aspire.md)]

[!INCLUDE [aca-bicep-cli-shared-steps](aca-bicep-cli-shared-steps.md)]

## Create supporting resources in Azure

1. Execute the following command to provision the Azure Container Registry (ACR):

    ### [PowerShell](#tab/powershell)

    ```azurecli
    az acr create --resource-group $env:RESOURCE_GROUP --name $env:CONTAINER_REGISTRY --sku Basic --admin-enabled true
    ```

    ### [Bash](#tab/bash)

    ```azurecli
    az acr create --resource-group $RESOURCE_GROUP --name $CONTAINER_REGISTRY --sku Basic --admin-enabled true
    ```

    ---

    > [!NOTE]
    > Before the next step, ensure that the Docker Desktop engine is running on your local computer.

1. Log into the new container registry:

    ### [PowerShell](#tab/powershell)

    ```azurecli
    az acr login --name $env:CONTAINER_REGISTRY
    ```

    ### [Bash](#tab/bash)

    ```azurecli
    az acr login --name $CONTAINER_REGISTRY
    ```

## Create an Azure Container app and containers

You can use the Azure CLI `containerapp up` command to create container apps for the application's API. This command also creates the required Log Analytics workspace and the Container Apps environment.

1. Create the container app by running this command:

    ### [PowerShell](#tab/powershell)

    ```azurecli
    az containerapp up `
      --name apiservice `
      --resource-group $env:RESOURCE_GROUP `
      --location $env:LOCATION `
      --environment $env:ENVIRONMENT `
      --registry-server "$env:CONTAINER_REGISTRY.azurecr.io" `
      --image mcr.microsoft.com/azuredocs/containerapps-helloworld:latest `
      --target-port 80 `
      --ingress external `
      --query properties.configuration.ingress.fqdn
    ```

    ### [Bash](#tab/bash)

    ```azurecli
    az containerapp up \
      --name apiservice \
      --resource-group $RESOURCE_GROUP \
      --location $LOCATION \
      --environment $ENVIRONMENT \
      --registry-server "$CONTAINER_REGISTRY.azurecr.io" \
      --image mcr.microsoft.com/azuredocs/containerapps-helloworld:latest \
      --target-port 80 \
      --ingress external \
      --query properties.configuration.ingress.fqdn
    ```

    ---

1. Now, let's create a Redis service for the front end web app to use:

    ### [PowerShell](#tab/powershell)

    ```azurecli
    az containerapp add-on redis create `
      --environment $env:ENVIRONMENT `
      --name redisca `
      --resource-group $env:RESOURCE_GROUP
    ```

    ### [Bash](#tab/bash)

    ```azurecli
    az containerapp add-on redis create \
      --environment $ENVIRONMENT \
      --name redisca \
      --resource-group $RESOURCE_GROUP
    ```

    ---

1. Next, create a container app for the web application's front end and configure it to use the Redis cache:

    ### [PowerShell](#tab/powershell)

    ```azurecli
    az containerapp create `
      --name web `
      --resource-group $env:RESOURCE_GROUP `
      --environment $env:ENVIRONMENT `
      --registry-server "$env:CONTAINER_REGISTRY.azurecr.io" `
      --image mcr.microsoft.com/azuredocs/containerapps-helloworld:latest `
      --target-port 80 `
      --ingress external `
      --query properties.configuration.ingress.fqdn
    ```

    ### [Bash](#tab/bash)

    ```azurecli
    az containerapp create \
      --name web \
      --resource-group $RESOURCE_GROUP \
      --environment $ENVIRONMENT \
      --registry-server "$CONTAINER_REGISTRY.azurecr.io" \
      --image mcr.microsoft.com/azuredocs/containerapps-helloworld:latest \
      --target-port 80 \
      --ingress external \
      --query properties.configuration.ingress.fqdn
    ```

    ---

## Connect the web containner app to the Redis cache

1. Log into the Azure Portal and select **Resource groups** > **\<Your resource group\>** > **web**.
1. In the menu on the left, select **Containers**.
1. Select the **Bindings** tab, and then select **Edit and deploy**.
1. Select the **Bindings** tab, select **Add**, and then select **Add-on binding**.
1. In the **Add-on** drop-down list, select **redisca**, select **Connect**, and then select **Create**.

## Publish the app container images into a container registry

1. Login to the ACR instance we'll be pushing the app container images to and get the server's FQDN into a parameter you'll use in a moment when you `dotnet build` the Aspire solution:

    ### [PowerShell](#tab/powershell)

    ```powershell
    $loginServer = (az acr show --name $env:CONTAINER_REGISTRY --query loginServer --output tsv)
    ```

    ### [Bash](#tab/bash)

    ```bash
    $loginServer = (az acr show --name $CONTAINER_REGISTRY --query loginServer --output tsv)
    ```

    ---

1. .NET's `publish` command supports the `ContainerRegistry` parameter. Setting this results in the output of the `dotnet publish` command being packaged into a container and pushed directly into your ACR instance in the cloud. Publish the solution projects to ACR using the `dotnet publish` command in the solution directory:

    ```dotnetcli
    dotnet publish -r linux-x64 -p:PublishProfile=DefaultContainer -p:ContainerRegistry=$loginServer
    ```

[!INCLUDE [aca-configure-post-deployment](aca-configure-post-deployment.md)]
