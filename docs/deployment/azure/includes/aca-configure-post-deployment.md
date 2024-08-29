---
ms.topic: include
---

## Configure the app for the deployed environment

Now that the infrastructure has been provisioned, you need to set a few configuration settings in the Azure Container Apps before your code can be published into them from their new ACR repositories.

> [!NOTE]
> These last few steps will be mitigated in a future release of Bicep and ACA.

Configure the front end with the correct Redis connection configuration:

1. First, use the `az containerapp exec` command to log into the Container App:

    # [PowerShell](#tab/powershell)

    ```powershell
    az containerapp exec --resource-group $env:RESOURCE_GROUP --name web
    ```

    # [Bash](#tab/bash)

    ```bash
    az containerapp exec --resource-group $RESOURCE_GROUP --name web
    ```

    ---

1. Next, use `env` to view the `REDIS_ENDPOINT` and `REDIS_PASSWORD` environment variables injected by the Azure Container Apps service binding, then `exit` to close the connection to the Container App:

    ```bash
    env | grep "^REDIS_ENDPOINT"
    env | grep "^REDIS_PASSWORD"
    exit
    ```

1. The `web` Container App is using the .NET Aspire integration for Redis, which loads the connection information from the app's configuration with the key `ConnectionStrings__cache`. Update the app configuration in ACA so that an environment variable with this name contains a valid connection string constructed using the details retrieved in the previous step. Note that .NET expects comma delimited values in the Redis connection string. For example,  `redis:6379,password=jH7DePUiK5E...`:

    # [PowerShell](#tab/powershell)

    ```powershell
    az containerapp update --name web --resource-group $env:RESOURCE_GROUP --set-env-vars 'ConnectionStrings__cache="redis:6379,password=<your password here>"'
    ```

    # [Bash](#tab/bash)

    ```bash
    az containerapp update --name web --resource-group $RESOURCE_GROUP --set-env-vars 'ConnectionStrings__cache="redis:6379,password=<your password here>"'
    ```

    ---

1. Then, update the target ports for the `web` and `apiservice` Container Apps as the default "Hello World" app and our app have different target ports:

    > [!NOTE]
    > The string `aspiretoaca` is associated with the name of the .NET Aspire solution, in lowercase form. If, when you created the .NET Aspire solution with a different name, you'll need to tweak this string in the code below. For example, if you created an app in a directory named `MyNewAspireApp`, you'd swap the string `aspiretoaca` with `mynewaspireapp` or the command will fail.

    # [PowerShell](#tab/powershell)

    ```powershell
    az containerapp ingress update --name web --resource-group $env:RESOURCE_GROUP --target-port 8080
    az containerapp ingress update --name apiservice --resource-group $env:RESOURCE_GROUP --target-port 8080
    az containerapp update --name web --resource-group $env:RESOURCE_GROUP --image "$($env:CONTAINER_REGISTRY).azurecr.io/aspiretoaca-web:latest"
    az containerapp update --name apiservice --resource-group $env:RESOURCE_GROUP --image "$($env:CONTAINER_REGISTRY).azurecr.io/aspiretoaca-apiservice:latest"
    ```

    # [Bash](#tab/bash)

    ```bash
    az containerapp ingress update --name web --resource-group $RESOURCE_GROUP --target-port 8080
    az containerapp ingress update --name apiservice --resource-group $RESOURCE_GROUP --target-port 8080
    az containerapp update --name web --resource-group $RESOURCE_GROUP --image "$($CONTAINER_REGISTRY).azurecr.io/aspiretoaca-web:latest"
    az containerapp update --name apiservice --resource-group $RESOURCE_GROUP --image "$($CONTAINER_REGISTRY).azurecr.io/aspiretoaca-apiservice:latest"
    ```

    ---
