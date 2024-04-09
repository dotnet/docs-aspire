---
title: Deploy a .NET Aspire app using the Azure Developer CLI and GitHub Actions
description: Learn how to use `azd` and GitHub Actions to deploy .NET Aspire apps.
ms.date: 12/18/2023
zone_pivot_groups: deployment-platform
ms.custom: devx-track-extended-azdevcli
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

:::zone pivot="azure-pipelines"

- [Create an Azure DevOps organization](/azure/devops/organizations/accounts/create-organization) or choose an existing organization
- [Create an Azure DevOps Personal Access Token (PAT)](/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate#create-a-pat) and save it for later use. Configure the token with the following permissions:
  - Agent Pools (read, manage)
  - Build (read and execute)
  - Code (full)
  - Project and team (read, write and manage)
  - Release (read, write, execute and manage)
  - Service Connections (read, query and manage)

:::zone-end

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

:::zone pivot="github-actions"

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

## Create the GitHub repository and pipeline

The Azure Developer CLI enables you to automatically create CI/CD pipelines with the correct configurations and permissions to provision and deploy resources to Azure. `azd` can also create a GitHub repository for your app if it doesn't exist already.

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

:::zone-end

:::zone pivot="azure-pipelines"

## Configure the workflow file

Although `azd` generated some essential template files for you, the project still needs an Azure Pipelines workflow file to support provisioning and deployments using CI/CD.

1. Create an empty _.azdo_ folder at the root of your project. `azd` uses this directory by default to discover Azure Pipelines workflow files.

1. Inside the new _.azdo_ folder, create another folder called _pipelines_ (you'll end up with _.azdo/pipelines_).

1. Add a new Azure Pipelines workflow file into the new folder named _azure-dev.yml_. The `azd` starter template provides a [Sample Azure Pipelines workflow file](https://github.com/Azure-Samples/azd-starter-bicep/blob/main/.azdo/pipelines/azure-dev.yml) that you can copy into your project.

1. Update the sample Azure Pipelines workflow to include a step to install the .NET Aspire workload. The completed workflow file should match the following:

```yml
trigger:
  - main
  - master

pool:
  vmImage: ubuntu-latest

steps:

  - task: Bash@3
    displayName: Install azd
    inputs:
      targetType: 'inline'
      script: |
        curl -fsSL https://aka.ms/install-azd.sh | bash

  # azd delegate auth to az to use service connection with AzureCLI@2
  - pwsh: |
      azd config set auth.useAzCliAuth "true"
    displayName: Configure `azd` to Use AZ CLI Authentication.

  - task: Install .NET Aspire workload
    inputs: 
        inlineScript: |
            dotnet workload install aspire

  - task: AzureCLI@2
    displayName: Provision Infrastructure
    inputs:
      azureSubscription: azconnection
      scriptType: bash
      scriptLocation: inlineScript
      inlineScript: |
        azd provision --no-prompt
    env:
      AZURE_SUBSCRIPTION_ID: $(AZURE_SUBSCRIPTION_ID)
      AZURE_ENV_NAME: $(AZURE_ENV_NAME)
      AZURE_LOCATION: $(AZURE_LOCATION)

  - task: AzureCLI@2
    displayName: Deploy Application
    inputs:
      azureSubscription: azconnection
      scriptType: bash
      scriptLocation: inlineScript
      inlineScript: |
        azd deploy --no-prompt
    env:
      AZURE_SUBSCRIPTION_ID: $(AZURE_SUBSCRIPTION_ID)
      AZURE_ENV_NAME: $(AZURE_ENV_NAME)
      AZURE_LOCATION: $(AZURE_LOCATION)
```

## Create the Azure DevOps repository and pipeline

> [!IMPORTANT]
> As mentioned in the prerequisites, you'll need to [create an Azure DevOps organization](/azure/devops/organizations/accounts/create-organization) or select an existing organization to complete the steps ahead. You will also need to [create a Personal Access Token (PAT)](/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate#create-a-pat) with the permissions listed in the prerequisites.

The Azure Developer CLI enables you to automatically create pipelines with the correct configurations and permissions to provision and deploy resources to Azure. `azd` can also create an Azure Pipelines repository for your app if it doesn't exist already.

1. Run the `azd pipeline config` command to configure your deployment pipeline and securely connect it to Azure. Include the `--provider azdo` option to use Azure Pipelines instead of the default GitHub Actions configuration.

    ```azdeveloper
    azd pipeline config --provider azdo
    ```

1. Select the subscription to provision and deploy the app resources to.

1. Select the Azure location to use for the resources.

1. Paste the Personal Access Token you created earlier.

1. Enter the Azure DevOps Organization name you created or selected.

1. When prompted to create a new repository in the current directory, enter <kbd>y</kbd> and press <kbd>Enter</kbd>.

1. When prompted to configure the git remote, select **Create a new Azure DevOps Project**.

1. Enter a unique name of your choice for the new repository, such as `aspireazd`. `azd` creates a new repository in Azure Repos and configures it with the necessary secrets required to authenticate to Azure.

    :::image type="content" source="media/pipeline-configuration.png" lightbox="media/pipeline-configuration.png" alt-text="A screenshot showing the pipeline configuration steps.":::

1. Enter <kbd>y</kbd> to proceed when `azd` prompts you to commit and push your local changes to start the configured pipeline.

## Explore the pipeline and deployed app

1. Navigate to your new Azure Pipeline using the status link output by `azd`.

    :::image type="content" source="media/azure-pipeline-run.png" lightbox="media/azure-pipeline-run.png" alt-text="A screenshot showing the successful Azure Pipelines run.":::

1. Select the completed pipeline run to view the summary.

    :::image type="content" source="media/azure-pipeline-summary.png" lightbox="media/azure-pipeline-summary.png" alt-text="A screenshot showing the summary view of the Azure Pipelines run.":::

1. Select the job link at the bottom of the view to navigate to the job details.

    :::image type="content"  source="media/azure-pipeline-run-details.png" lightbox="media/azure-pipeline-run-details.png" alt-text="A screenshot showing the detailed view of the Azure Pipelines run." :::

1. The job details page shows the status of all the individual stages. Select **Provision Infrastructure** to view the logs for that stage, which detail all of the provisioning steps completed by `azd`. At the bottom of the logs take note of the final status message and link to the provisioned Azure resouce group.

1. Select the link at the bottom of the provisioning output logs to navigate to the new Azure resource group.

    :::image type="content" source="media/azure-pipeline-resource-group.png" lightbox="media/azure-pipeline-resource-group.png" alt-text="A screenshot showing the deployed Azure resources.":::

    > [!NOTE]
    > You can also navigate directly to your new resource group by searching for it in the Azure Portal. Your resource group name will be the environment name you provided to `azd` prefixed with `rg-`.

1. Select the **webfrontend** container app, which hosts the public facing portion of your site.

1. On the **webfrontend** details page, select the **Application Url** link to open your site in the browser.

> [!IMPORTANT]
> If you encounter a `403 Forbidden` error when viewing your site in the browser, make sure the ingress settings are configured correctly. On the **webfrontend** app page in the Azure Portal, navigate to **Ingress** on the left navigation. Make sure **Ingress traffic** is set to **Accepting traffic from anywhere** and save your changes.

Congratulations! You successfully deployed a .NET Aspire app using the Azure Developer CLI and Azure Pipelines.

:::zone-end

[!INCLUDE [clean-up-resources](../../includes/clean-up-resources.md)]
