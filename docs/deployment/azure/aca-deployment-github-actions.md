---
title: Deploy a .NET Aspire app to Azure Container Apps using the Azure Developer CLI and GitHub Actions
description: Learn how to use `azd` to deploy .NET Aspire apps to Azure Container Apps.
ms.date: 12/18/2023
---

# Deploy a .NET Aspire app to Azure Container Apps using the Azure Developer CLI and GitHub Actions

The Azure Developer CLI (`azd`) enables you to deploy .NET Aspire applications using GitHub actions by automatically configuring the required Authentication and environment settings.  In this quickstart you'll walk through the process of creating and deploying a .NET Aspire application to Azure Container Apps using `azd` and GitHub actions. You'll learn the following concepts:

> [!div class="checklist"]
>
> - Explore how `azd` integration works with .NET Aspire apps and GitHub actions
> - Create and configure a GitHub repository for a .NET Aspire app using `azd`
> - Add a GitHub actions workflow file to your .NET Aspire project
> - Monitor and explore GitHub Actions workflow execution and Azure deployment

[!INCLUDE [aspire-prereqs](../../includes/aspire-prereqs.md)]

You will also need to have the Azure Developer CLI [installed locally](/azure/developer/azure-developer-cli/install-azd). Common install options include the following:

# [Windows](#tab/windows)

```powershell
winget install microsoft.azd
```

# [macOS](#tab/macos)

```bash
brew tap azure/azd && brew install azd
```

# [Linux](#tab/linux)

```bash
curl -fsSL https://aka.ms/install-azd.sh | bash
```

---

[!INCLUDE [file-new-aspire](../../includes/file-new-aspire.md)]

## Initialize the template

Open a new terminal window and `cd` into the root of the solution you created. Execute the `azd init` command to instruct `azd` to inspect the local directory structure and determine the type of app.

:::image type="content" source="media/aspire-azd-01.png" lightbox="media/aspire-azd-01.png" alt-text="A screenshot of azd inspecting the local directory structure.":::

`azd` determines that this is a .NET Aspire app and locates the App Host project. It will then suggest a deployment to Azure Container Apps. Select the `Confirm and continue...` option to proceed.

:::image type="content" source="media/aspire-azd-02.png" lightbox="media/aspire-azd-02.png" alt-text="A screenshot of accepting the .NET app type and Azure Container Apps deployment target":::

`azd` presents each of the projects in the .NET Aspire solution and allows you to identify which project(s) will be deployed with HTTP ingress open publicly to all internet traffic. Since you'll want the API to be private only to the Azure Container Apps environment and *not* available publicly, select only the `webfrontend`.

:::image type="content" source="media/aspire-azd-03.png" lightbox="media/aspire-azd-03.png" alt-text="Specifying the containers that are publicly available to the Internet":::

Finally, specify the the environment name, which is used for naming provisioned resources in Azure. The environment is also helpful for managing multiple environments, such as `dev` and `prod`.

:::image type="content" source="media/aspire-azd-04.png" lightbox="media/aspire-azd-04.png" alt-text="Providing the name of the AZD environment to be created":::

## Add the GitHub Actions workflow file

Complete the following steps to add CI/CD support in your template using GitHub actions:

1. Create an empty `.github` folder at the root of your project. `azd` uses this directory by default to discover GitHub Actions workflow files.

1. Inside the new `.github` folder, creating another folder called `workflows`.

1. Add a new GitHub Actions workflow file into the new folder named `azure-dev.yaml`. The `azd` starter template provides a [Sample GitHub Actions workflow file](https://github.com/Azure-Samples/azd-starter-bicep/blob/main/.github/workflows/azure-dev.yml) that you can copy into your project and modify as needed.

1. Update the sample GitHub Actions workflow to include a step to install the .NET Aspire workload. The completed workflow file should match the following:

```yml
on:
  workflow_dispatch:
  push:
    # Run when commits are pushed to mainline branch (main or master)
    # Set this to the mainline branch you are using
    branches:
      - main
      - master

permissions:
  id-token: write
  contents: read

jobs:
  build:
    runs-on: ubuntu-latest
    env:
      AZURE_CLIENT_ID: ${{ vars.AZURE_CLIENT_ID }}
      AZURE_TENANT_ID: ${{ vars.AZURE_TENANT_ID }}
      AZURE_SUBSCRIPTION_ID: ${{ vars.AZURE_SUBSCRIPTION_ID }}
      AZURE_CREDENTIALS: ${{ secrets.AZURE_CREDENTIALS }}
    steps:
      - name: Checkout
        uses: actions/checkout@v3

      - name: Install azd
        uses: Azure/setup-azd@v0.1.0

      - name: Install .NET Aspire workload
        run: dotnet workload install aspire

      - name: Log in with Azure (Federated Credentials)
        if: ${{ env.AZURE_CLIENT_ID != '' }}
        run: |
          azd auth login `
            --client-id "$Env:AZURE_CLIENT_ID" `
            --federated-credential-provider "github" `
            --tenant-id "$Env:AZURE_TENANT_ID"
        shell: pwsh

      - name: Log in with Azure (Client Credentials)
        if: ${{ env.AZURE_CREDENTIALS != '' }}
        run: |
          $info = $Env:AZURE_CREDENTIALS | ConvertFrom-Json -AsHashtable;
          Write-Host "::add-mask::$($info.clientSecret)"

          azd auth login `
            --client-id "$($info.clientId)" `
            --client-secret "$($info.clientSecret)" `
            --tenant-id "$($info.tenantId)"
        shell: pwsh
        env:
          AZURE_CREDENTIALS: ${{ secrets.AZURE_CREDENTIALS }}

      - name: Provision Infrastructure
        run: azd provision --no-prompt
        env:
          AZURE_ENV_NAME: ${{ vars.AZURE_ENV_NAME }}
          AZURE_LOCATION: ${{ vars.AZURE_LOCATION }}
          AZURE_SUBSCRIPTION_ID: ${{ vars.AZURE_SUBSCRIPTION_ID }}

      - name: Deploy Application
        run: azd deploy --no-prompt
        env:
          AZURE_ENV_NAME: ${{ vars.AZURE_ENV_NAME }}
          AZURE_LOCATION: ${{ vars.AZURE_LOCATION }}
          AZURE_SUBSCRIPTION_ID: ${{ vars.AZURE_SUBSCRIPTION_ID }}

```

## Create the repository and pipeline

The Azure Developer CLI enables you to automatically create pipelines with the correct configurations and permissions to provision and deploy resources to Azure. `azd` can also help you create a GitHub repository for you app, if it doesn't exist already.

1. Use the `azd pipeline config` command to manage your deployment pipeline and configure it to connect securely to Azure:

    ```azdeveloper
    azd pipeline config
    ```

1. Select the subscription to provision and deploy the app resources to.

1. Select the Azure location to use for the resources.

1. When prompted whether to create a new Git repository in the directory, enter `y` and press enter.

1. Select **Create a new private GitHub repository** when prompted to configure the git remote.

1. Enter a name of your choicei for the new GitHub repository or press enter to use the default name.

`azd` will create a new repository in GitHub and configure it with the necessary secrets required to authenticate to Azure.

:::image type="content" source="media/pipeline-configuration.png" alt-text="A screenshot showing the pipeline configuration steps.":::

1. Enter `y` to proceed when `azd` prompts you to commit and push your local changes to start the configured pipeline.

## Explore the GitHub Actions workflow

1. Navigate to you new repository in GitHub and select the **Actions** tab to view the repository workflows. You should see the new workflow either running or already completed. Select the workflow to view the details of the run. The job steps are visible in the logs of the run. For example, you can expand steps such as **Install .NET Aspire Workload** or **Deploy application** to see the details of the completed action. Select the  

:::image type="content" source="media/github-action.png" alt-text="A screenshot showing the GitHub Action workflow steps.":::

1. Select **Deploy Application** to expand the logs for that step. You should see two endpoint urls printed out for the `apiservice` and `webfrontend`. Select either of these links to open them in another browser tab and explore the deployed application.

:::image type="content" source="media/deploying-links.png" alt-text="A screenshot showing the deployed app links.":::

Congratulations! You successfully deployed a .NET Aspire app using the Azure Developer CLI and GitHub Actions.
