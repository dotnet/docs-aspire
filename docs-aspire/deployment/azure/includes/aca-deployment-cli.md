[!INCLUDE [file-new-aspire](../../../includes/file-new-aspire.md)]

[!INCLUDE [aca-bicep-cli-shared-steps](aca-bicep-cli-shared-steps.md)]

## Create supporting resources in Azure

1. Execute the following command to provision the Azure Container Registry (ACR):

    # [PowerShell](#tab/powershell)

    ```azurecli
    az acr create --resource-group $env:RESOURCE_GROUP --name $env:CONTAINER_REGISTRY --sku Basic
    ```

    # [Bash](#tab/bash)

    ```azurecli
    az acr create --resource-group $RESOURCE_GROUP --name $CONTAINER_REGISTRY --sku Basic
    ```

1. Execute the following command to create a user-assigned Azure identity. Each of your apps will run as this identity, which will be given access to the ACR instance so the Azure Container Apps hosting your code can securely access your ACR instance and pull containers as they're updated:

    # [PowerShell](#tab/powershell)

    ```azurecli
    az identity create -g $env:RESOURCE_GROUP -n $env:IDENTITY
    ```

    # [Bash](#tab/bash)

    ```azurecli
    az identity create -g $RESOURCE_GROUP -n $IDENTITY
    ```

    - Allow acr pulls to that identity

## Create an Azure Container app and containers

You can use the Azure CLI `containerapp up` command to create the required Log Analytics workspace and the Container Apps environment:

# [PowerShell](#tab/powershell)

```azurecli
az containerapp up `
  --name aspirecontainerapp `
  --resource-group $env:RESOURCE_GROUP `
  --location $env:LOCATION `
  --environment $env:ENVIRONMENT `
  --image mcr.microsoft.com/azuredocs/containerapps-helloworld:latest `
  --target-port 80 `
  --ingress external `
  --query properties.configuration.ingress.fqdn
```

# [Bash](#tab/bash)

```azurecli
az containerapp up `
  --name aspirecontainerapp `
  --resource-group $RESOURCE_GROUP `
  --location $LOCATION `
  --environment $ENVIRONMENT `
  --image mcr.microsoft.com/azuredocs/containerapps-helloworld:latest `
  --target-port 80 `
  --ingress external `
  --query properties.configuration.ingress.fqdn
```

## Publish the app container images into a container registry

1. Login to the ACR instance we'll be pushing the app container images to and get the server's FQDN into a parameter you'll use in a moment when you `dotnet build` the Aspire solution:

    # [PowerShell](#tab/powershell)

    ```powershell
    az acr login --name $env:CONTAINER_REGISTRY
    $loginServer = (az acr show --name $env:CONTAINER_REGISTRY --query loginServer --output tsv)
    ```

    # [Bash](#tab/bash)

    ```bash
    az acr login --name $CONTAINER_REGISTRY
    $loginServer = (az acr show --name $CONTAINER_REGISTRY --query loginServer --output tsv)
    ```

    ---

1. .NET's `publish` command supports the `ContainerRegistry` parameter. Setting this results in the output of the `dotnet publish` command being packaged into a container and pushed directly into your ACR instance in the cloud. Publish the solution projects to ACR using the `dotnet publish` command in the solution directory:

    ```dotnetcli
    dotnet publish -r linux-x64 -p:PublishProfile=DefaultContainer -p:ContainerRegistry=$loginServer
    ```

[!INCLUDE [aca-configure-post-deployment](aca-configure-post-deployment.md)]
