## Deploy the template

1. Once an `azd` template is initialized, the provisioning and deployment process can be executed as a single command from the _AppHost_ project directory using [azd up](/azure/developer/azure-developer-cli/reference#azd-up):

    ```azdeveloper
    azd up
    ```

1. Select the subscription you'd like to deploy to from the list of available options:

    ```output
    Select an Azure Subscription to use:  [Use arrows to move, type to filter]
      1. SampleSubscription01 (xxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxx)
      2. SamepleSubscription02 (xxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxx)
    ```

1. Select the desired Azure location to use from the list of available options:

    ```output
    Select an Azure location to use:  [Use arrows to move, type to filter]
      42. (US) Central US (centralus)
      43. (US) East US (eastus)
    > 44. (US) East US 2 (eastus2)
      46. (US) North Central US (northcentralus)
      47. (US) South Central US (southcentralus)
    ```

After you make your selections, `azd` executes the provisioning and deployment process.

[!INCLUDE [azd-up-output](azd-up-output.md)]

The `azd up` command acts as wrapper for the following individual `azd` commands to provision and deploy your resources in a single step:

1. [`azd package`](/azure/developer/azure-developer-cli/reference#azd-package): The app projects and their dependencies are packaged into containers.
1. [`azd provision`](/azure/developer/azure-developer-cli/reference#azd-provision): The Azure resources the app will need are provisioned.
1. [`azd deploy`](/azure/developer/azure-developer-cli/reference#azd-deploy): The projects are pushed as containers into an Azure Container Registry instance, and then used to create new revisions of Azure Container Apps in which the code will be hosted.

When the `azd up` stages complete, your app will be available on Azure, and you can open the Azure portal to explore the resources. `azd` also outputs URLs to access the deployed apps directly.
