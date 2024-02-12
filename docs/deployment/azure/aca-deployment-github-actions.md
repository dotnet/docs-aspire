---
title: Deploy a .NET Aspire app using the Azure Developer CLI and GitHub Actions
description: Learn how to use `azd` and GitHub Actions to deploy .NET Aspire apps.
ms.date: 12/18/2023
---

# Tutorial: Deploy a .NET Aspire app using the Azure Developer CLI and GitHub Actions

The Azure Developer CLI (`azd`) enables you to deploy .NET Aspire applications using GitHub Actions by automatically configuring the required authentication and environment settings. This article walks you through the process of creating and deploying a .NET Aspire app on Azure Container Apps using `azd` and GitHub Actions. You learn the following concepts:

> [!div class="checklist"]
>
> - Explore how `azd` integration works with .NET Aspire apps and GitHub Actions
> - Create and configure a GitHub repository for a .NET Aspire app using `azd`
> - Add a GitHub Actions workflow file to your .NET Aspire solution
> - Monitor and explore GitHub Actions workflow executions and Azure deployments

[!INCLUDE [aspire-prereqs](../../includes/aspire-prereqs.md)]

You also need to have the Azure Developer CLI [installed locally](/azure/developer/azure-developer-cli/install-azd) (version 1.5.1 or higher). Common install options include the following:

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

[!INCLUDE [init workflow](includes/init-workflow.md)]

## Add the GitHub Actions workflow file

Although `azd` generated some essential template files for you, the project still needs a GitHub Actions workflow file to support provisioning and deployments using CI/CD.

1. Create an empty _.github_ folder at the root of your project. `azd` uses this directory by default to discover GitHub Actions workflow files.

1. Inside the new _.github_ folder, create another folder called _workflows_ (you'll end up with _.github/workflows_).

1. Add a new GitHub Actions workflow file into the new folder named _azure-dev.yml_. The `azd` starter template provides a [Sample GitHub Actions workflow file](https://github.com/Azure-Samples/azd-starter-bicep/blob/main/.github/workflows/azure-dev.yml) that you can copy into your project.

1. Update the sample GitHub Actions workflow to include a step to install the .NET Aspire workload. This ensures the .NET Aspire tooling and commands are available to the job running your GitHub Actions. The completed workflow file should match the following:

    ```yml
    on:
      workflow_dispatch:
      push:
        # Run when commits are pushed to mainline branch (main or master)
        # Set this to the mainline branch you are using
        branches:
          - main
    
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
          AZURE_ENV_NAME: ${{ vars.AZURE_ENV_NAME }}
          AZURE_LOCATION: ${{ vars.AZURE_LOCATION }}
        steps:
          - name: Checkout
            uses: actions/checkout@v4
    
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
    
          - name: Provision Infrastructure
            run: azd provision --no-prompt
    
          - name: Deploy Application
            run: azd deploy --no-prompt
    ```

## Create the repository and pipeline

The Azure Developer CLI enables you to automatically create CICD pipelines with the correct configurations and permissions to provision and deploy resources to Azure. `azd` can also create a GitHub repository for your app if it doesn't exist already.

1. Run the `azd pipeline config` command to configure your deployment pipeline and securely connect it to Azure:

    ```azdeveloper
    azd pipeline config
    ```

1. Select the subscription to provision and deploy the app resources to.

1. Select the Azure location to use for the resources.

1. When prompted whether to create a new Git repository in the directory, enter <kbd>y</kbd> and press <kbd>Enter</kbd>.

1. Select **Create a new private GitHub repository** to configure the git remote.

1. Enter a name of your choice for the new GitHub repository or press enter to use the default name. `azd` creates a new repository in GitHub and configures it with the necessary secrets required to authenticate to Azure.

    :::image type="content" source="media/pipeline-configuration.png" alt-text="A screenshot showing the pipeline configuration steps.":::

1. Enter <kbd>y</kbd> to proceed when `azd` prompts you to commit and push your local changes to start the configured pipeline.

## Explore the GitHub Actions workflow and deployment

1. Navigate to your new GitHub repository using the link output by `azd`.

1. Select the **Actions** tab to view the repository workflows. You should see the new workflow either running or already completed. Select the workflow to view the job steps and details in the logs of the run. For example, you can expand steps such as **Install .NET Aspire Workload** or **Deploy application** to see the details of the completed action.

    :::image type="content" source="media/github-action.png" alt-text="A screenshot showing the GitHub Action workflow steps.":::

1. Select **Deploy Application** to expand the logs for that step. You should see two endpoint urls printed out for the `apiservice` and `webfrontend`. Select either of these links to open them in another browser tab and explore the deployed application.

    :::image type="content" source="media/deployment-links.png" alt-text="A screenshot showing the deployed app links.":::

Congratulations! You successfully deployed a .NET Aspire app using the Azure Developer CLI and GitHub Actions.

[!INCLUDE [clean-up-resources](../../includes/clean-up-resources.md)]
