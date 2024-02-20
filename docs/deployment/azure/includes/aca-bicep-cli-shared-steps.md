---
ms.topic: include
---

1. Install the latest [Azure CLI](/cli/azure/install-azure-cli) & [sign in](/cli/azure/authenticate-azure-cli)

    # [Windows](#tab/install-az-windows)

    ```powershell
    winget install -e --id Microsoft.AzureCLI
    # Restart the terminal session after installing the az CLI before running the next command
    az login
    ```

    # [macOS](#tab/install-macos-windows)

    ```bash
    brew update && brew install azure-cli
    # Restart the terminal session after installing the az CLI before running the next command
    az login
    ```

    # [Linux](#tab/linux)

    ```bash
    curl -sL https://aka.ms/InstallAzureCLIDeb | sudo bash
    # Restart the terminal session after installing the az CLI before running the next command
    az login
    ```

    ---

1. Login using the `az login` command, which will prompt you to open a browser.

1. If you have access to more than one Azure subscription, get your subscription list as a table so you can find the right subscription and matching subscription id in which you want to experiment.

    ```azurecli
    az account list --output table
    ```

1. Copy the string from the "SubscriptionId" column for the Azure subscription you wish to use, then paste that string to your command prompt as the parameter to the `az account set` command:

    ```azurecli
    az account set --subscription your-subscription-id-pasted-here
    ```

1. Add the Azure CLI `az containerapp` extension, which provides capabilities specific to Azure Container Apps. For more information, see the [az containerapp docs](/cli/azure/containerapp?view=azure-cli-latest).

    ```azurecli
    az extension add --name containerapp --upgrade
    ```

1. Register required `az` namespaces. For more information, see the [az provider register docs](/cli/azure/provider?view=azure-cli-latest#az-provider-register):

    ```azurecli
    az provider register --namespace Microsoft.App
    az provider register --namespace Microsoft.OperationalInsights
    az provider register --namespace Microsoft.ContainerRegistry
    ```

## Set the environment variables

Declare a set of environment variables to store commonly used values for the app deployment process. Setting these variables simplifies working with command-line parameters:

> [!NOTE]
> You will need to customize the `SOLUTION` and `LOCATION` variables per your own needs. To get a list of the available Azure regions to which you can deploy, use the command `az account list-locations --output table`.

# [PowerShell](#tab/powershell)

```powershell
$env:SOLUTION="YOUR_APP_NAME"                           # Your app's name (e.g., "aspiresample42")
$env:LOCATION="YOUR_REGION"                             # Your desired Azure region (e.g., "westus")
$env:RESOURCE_GROUP="$($env:SOLUTION.ToLower())rg"      # Resource Group name, e.g. eshopliterg
$env:CONTAINER_REGISTRY="$($env:SOLUTION.ToLower())cr"  # Azure Container Registry name, e.g. eshoplitecr
$env:IMAGE_PREFIX="$($env:SOLUTION.ToLower())"          # Container image name prefix, e.g. eshoplite
$env:IDENTITY="$($env:SOLUTION.ToLower())id"            # Azure Managed Identity, e.g. eshopliteid
$env:ENVIRONMENT="$($env:SOLUTION.ToLower())cae"        # Azure Container Apps Environment name, e.g. eshoplitecae
```

# [Bash](#tab/bash)

```bash
SOLUTION="YOUR_APP_NAME"                # Your app's name (e.g., "aspiresample42")
LOCATION="YOUR_REGION"                  # Your desired Azure region (e.g., "westus")
RESOURCE_GROUP="${SOLUTION,,}rg"        # Resource Group name, e.g. eshopliterg
CONTAINER_REGISTRY="${SOLUTION,,}cr"    # Azure Container Registry name, e.g. eshoplitecr
IMAGE_PREFIX="${SOLUTION,,}"            # Container image name prefix, e.g. eshoplite
IDENTITY="${SOLUTION,,}id"              # Azure Managed Identity, e.g. eshopliteid
ENVIRONMENT="${SOLUTION,,}cae"          # Azure Container Apps Environment name, e.g. eshoplitecae
```

---

## Provision the Azure resources

Azure Container Apps (ACA) is an ideal hosting platform for .NET Aspire apps. You can use Bicep or the Azure CLI to create resources in Azure to host the .NET Aspire app code along with supporting services:

- An Azure Container Apps Environment to host your code and tertiary containers
- A pair of Azure Container Apps, hosting your code
- A Redis container inside the ACA Environment used by the output caching subsystem
- An Azure Log Analytics instance to host the log output from your apps
- An Azure Container Registry (ACR) instance for publishing your containers into the cloud

1. Create the Azure resource group that will hold the provision resources:

    # [PowerShell](#tab/powershell)

    ```powershell
    az group create --location $env:LOCATION --name $env:RESOURCE_GROUP
    ```

    # [Bash](#tab/bash)

    ```bash
    az group create --location $LOCATION --name $RESOURCE_GROUP
    ```

    ---
