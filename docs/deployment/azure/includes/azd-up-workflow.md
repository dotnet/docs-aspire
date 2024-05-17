## Provision and deploy the template

1. Once `azd` is initialized, the provisioning and deployment process can be executed as a single command from the _AppHost_ project directory, [azd up](/azure/developer/azure-developer-cli/reference#azd-up):

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
  41. (South America) Brazil Southeast (brazilsoutheast)
  42. (US) Central US (centralus)
  43. (US) East US (eastus)
> 44. (US) East US 2 (eastus2)
  45. (US) East US STG (eastusstg)
  46. (US) North Central US (northcentralus)
  47. (US) South Central US (southcentralus)
```

After you make your selections, `azd` executes the provisioning and deployment process.

[!INCLUDE [azd-up-output](azd-up-output.md)]

First, the projects will be packaged into containers during the `azd package` phase, followed by the `azd provision` phase during which all of the Azure resources the app will need are provisioned.

Once `provision` is complete, `azd deploy` will take place. During this phase, the projects are pushed as containers into an Azure Container Registry instance, and then used to create new revisions of Azure Container Apps in which the code will be hosted.
