---
title: Deploy a .NET Aspire project using the Azure Developer CLI
description: Learn how to use `azd` to deploy .NET Aspire projects.
ms.date: 07/17/2025
zone_pivot_groups: deployment-platform
ms.custom: devx-track-extended-azdevcli
---

# Tutorial: Deploy a .NET Aspire project using the Azure Developer CLI

The Azure Developer CLI (`azd`) enables you to deploy .NET Aspire projects using GitHub Actions or Azure Devops pipelines by automatically configuring the required authentication and environment settings. This article walks you through the process of creating and deploying a .NET Aspire project on Azure Container Apps using `azd`. You learn the following concepts:

> [!div class="checklist"]
>
> - Explore how `azd` integration works with .NET Aspire projects
> - Create and configure a GitHub or Azure DevOps repository for a .NET Aspire project using `azd`
> - Monitor and explore GitHub Actions workflow or Azure DevOps pipeline executions and Azure deployments

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

## Create a .NET Aspire solution

As a starting point, this article assumes that you've created a .NET Aspire solution from the **.NET Aspire Starter Application** template. For more information, see [Quickstart: Build your first .NET Aspire app](../../get-started/build-your-first-aspire-app.md).

[!INCLUDE [init workflow](includes/init-workflow.md)]

:::zone pivot="github-actions"

## Create the GitHub repository and pipeline

The Azure Developer CLI enables you to automatically create CI/CD pipelines with the correct configurations and permissions to provision and deploy resources to Azure. `azd` can also create a GitHub repository for your app if it doesn't exist already.

1. Run the `azd pipeline config` command to configure your deployment pipeline and securely connect it to Azure:

    ```azdeveloper
    azd pipeline config
    ```

1. Select the subscription to provision and deploy the app resources to.

1. Select the Azure location to use for the resources.

1. When prompted whether to create a new Git repository in the directory, enter <kbd>y</kbd> and press <kbd>Enter</kbd>.

    > [!NOTE]
    > Creating a GitHub repository required you being logged into GitHub. There are a few selections that vary based on your preferences. After logging in, you will be prompted to create a new repository in the current directory.

1. Select **Create a new private GitHub repository** to configure the git remote.

1. Enter a name of your choice for the new GitHub repository or press enter to use the default name. `azd` creates a new repository in GitHub and configures it with the necessary secrets required to authenticate to Azure.

    :::image type="content" loc-scope="other" source="media/pipeline-configuration.png" alt-text="A screenshot showing the pipeline configuration steps.":::

1. Enter <kbd>y</kbd> to proceed when `azd` prompts you to commit and push your local changes to start the configured pipeline.

## Explore the GitHub Actions workflow and deployment

1. Navigate to your new GitHub repository using the link output by `azd`.

1. Select the **Actions** tab to view the repository workflows. You should see the new workflow either running or already completed. Select the workflow to view the job steps and details in the logs of the run. For example, you can expand steps such as **Install .NET Aspire Workload** or **Deploy application** to see the details of the completed action.

    :::image type="content" loc-scope="github" source="media/github-action.png" alt-text="A screenshot showing the GitHub Action workflow steps.":::

1. Select **Deploy Application** to expand the logs for that step. You should see two endpoint urls printed out for the `apiservice` and `webfrontend`. Select either of these links to open them in another browser tab and explore the deployed application.

    :::image type="content" loc-scope="github" source="media/deployment-links.png" alt-text="A screenshot showing the deployed app links.":::

Congratulations! You successfully deployed a .NET Aspire project using the Azure Developer CLI and GitHub Actions.

## Configure working directory for multi-project solutions

When you add GitHub Actions to an existing multi-project .NET Aspire solution where the AppHost project isn't in the root directory, you might need to configure the `working-directory` parameter for certain workflow steps. This section explains when and how to make these adjustments.

### When working-directory configuration is needed

The `azd pipeline config` command generates a GitHub Actions workflow that assumes your .NET Aspire AppHost project is in the root directory of your repository. However, in many real-world scenarios, especially when [adding .NET Aspire to existing applications](../../get-started/add-aspire-existing-app.md), the AppHost project might be in a subdirectory.

For example, if your repository structure looks like this:

```Directory
└───📂 MyAspireApp
    ├───📂 MyAspireApp.ApiService
    ├───📂 MyAspireApp.AppHost
    │    ├─── MyAspireApp.AppHost.csproj
    │    └─── Program.cs
    ├───📂 MyAspireApp.Web
    └─── MyAspireApp.sln
```

The generated workflow steps for **Provision Infrastructure** and **Deploy Application** need to run from the `MyAspireApp.AppHost` directory, not from the repository root.

### Updating the GitHub Actions workflow

After running `azd pipeline config`, examine the generated workflow file in `.github/workflows/azure-dev.yml`. Look for steps that run `azd` commands and add the `working-directory` parameter as needed.

Here's an example of the original generated steps:

```yaml
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

Update these steps to include the `working-directory` parameter:

```yaml
- name: Provision Infrastructure
  run: azd provision --no-prompt
  working-directory: ./MyAspireApp.AppHost
  env:
    AZURE_ENV_NAME: ${{ vars.AZURE_ENV_NAME }}
    AZURE_LOCATION: ${{ vars.AZURE_LOCATION }}
    AZURE_SUBSCRIPTION_ID: ${{ vars.AZURE_SUBSCRIPTION_ID }}

- name: Deploy Application
  run: azd deploy --no-prompt
  working-directory: ./MyAspireApp.AppHost
  env:
    AZURE_ENV_NAME: ${{ vars.AZURE_ENV_NAME }}
    AZURE_LOCATION: ${{ vars.AZURE_LOCATION }}
    AZURE_SUBSCRIPTION_ID: ${{ vars.AZURE_SUBSCRIPTION_ID }}
```

### Finding the correct working directory

The working directory should point to the folder containing your .NET Aspire AppHost project (the project that contains the _azure.yaml_ file generated by `azd init`). You can identify this directory by:

1. Look for the project with the `Aspire.AppHost` package reference in its `.csproj` file.
1. Find the directory containing the _azure.yaml_ file.
1. Locate the project referenced in your solution that orchestrates other services.

> [!NOTE]
> Some `azd` commands, such as `azd init` during pipeline setup, might also need the `working-directory` parameter if they're not running from the AppHost project directory.

:::zone-end

:::zone pivot="azure-pipelines"

## Create the Azure DevOps repository and pipeline

> [!IMPORTANT]
> As mentioned in the prerequisites, you'll need to [create an Azure DevOps organization](/azure/devops/organizations/accounts/create-organization) or select an existing organization to complete the steps ahead. You will also need to [create a Personal Access Token (PAT)](/azure/devops/organizations/accounts/use-personal-access-tokens-to-authenticate#create-a-pat) with the permissions listed in the prerequisites.

The Azure Developer CLI enables you to automatically create pipelines with the correct configurations and permissions to provision and deploy resources to Azure. `azd` can also create an Azure Pipelines repository for your app if it doesn't exist already.

1. Run the `azd pipeline config` command to configure your deployment pipeline and securely connect it to Azure. Include the `--provider azdo` option to use Azure Pipelines instead of the default GitHub Actions configuration.

    ```azdeveloper
    azd pipeline config --provider azdo
    ```

    > [!IMPORTANT]
    > Before running `azd pipeline config`, ensure you have successfully run `azd init` to initialize your project. If you encounter errors like "no project exists" during pipeline execution, see the [troubleshooting section](#troubleshoot-azure-devops-pipeline-deployment) for solutions.

1. Select the subscription to provision and deploy the app resources to.

1. Select the Azure location to use for the resources.

1. Paste the Personal Access Token you created earlier.

1. Enter the Azure DevOps Organization name you created or selected.

1. When prompted to create a new repository in the current directory, enter <kbd>y</kbd> and press <kbd>Enter</kbd>.

1. When prompted to configure the git remote, select **Create a new Azure DevOps Project**.

1. Enter a unique name of your choice for the new repository, such as `aspireazd`. `azd` creates a new repository in Azure Repos and configures it with the necessary secrets required to authenticate to Azure.

    :::image type="content" loc-scope="other" source="media/pipeline-configuration.png" lightbox="media/pipeline-configuration.png" alt-text="A screenshot showing the pipeline configuration steps.":::

1. Enter <kbd>y</kbd> to proceed when `azd` prompts you to commit and push your local changes to start the configured pipeline.

## Explore the pipeline and deployed app

1. Navigate to your new Azure Pipeline using the status link output by `azd`.

    :::image type="content" loc-scope="azure" source="media/azure-pipeline-run.png" lightbox="media/azure-pipeline-run.png" alt-text="A screenshot showing the successful Azure Pipelines run.":::

1. Select the completed pipeline run to view the summary.

    :::image type="content" source="media/azure-pipeline-summary.png" alt-text="A screenshot showing the summary view of the Azure Pipelines run.":::

1. Select the job link at the bottom of the view to navigate to the job details.

    :::image type="content" loc-scope="azure" source="media/azure-pipeline-run-details.png" lightbox="media/azure-pipeline-run-details.png" alt-text="A screenshot showing the detailed view of the Azure Pipelines run." :::

1. The job details page shows the status of all the individual stages. Select **Provision Infrastructure** to view the logs for that stage, which detail all of the provisioning steps completed by `azd`. At the bottom of the logs take note of the final status message and link to the provisioned Azure resource group.

1. Select the link at the bottom of the provisioning output logs to navigate to the new Azure resource group.

    :::image type="content" loc-scope="azure" source="media/azure-pipeline-resource-group.png" lightbox="media/azure-pipeline-resource-group.png" alt-text="A screenshot showing the deployed Azure resources.":::

    > [!NOTE]
    > You can also navigate directly to your new resource group by searching for it in the Azure Portal. Your resource group name will be the environment name you provided to `azd` prefixed with `rg-`.

1. Select the **webfrontend** container app, which hosts the public facing portion of your site.

1. On the **webfrontend** details page, select the **Application Url** link to open your site in the browser.

> [!IMPORTANT]
> If you encounter a `403 Forbidden` error when viewing your site in the browser, make sure the ingress settings are configured correctly. On the **webfrontend** app page in the Azure Portal, navigate to **Ingress** on the left navigation. Make sure **Ingress traffic** is set to **Accepting traffic from anywhere** and save your changes.

Congratulations! You successfully deployed a .NET Aspire project using the Azure Developer CLI and Azure Pipelines.

## Troubleshoot Azure DevOps pipeline deployment

This section covers common issues you might encounter when deploying .NET Aspire projects using Azure DevOps pipelines.

### ERROR: no project exists; to create a new project, run azd init

**Problem**: During the provisioning step of your Azure DevOps pipeline, you encounter the error message:

```output
ERROR: no project exists; to create a new project, run azd init
```

**Cause**: This error occurs because the `azd init` command generates files (`azure.yaml` and the `.azure` folder) that are typically not committed to your repository. When the pipeline runs in a clean environment, these files don't exist, causing `azd` commands to fail.

**Solution**: There are several approaches to resolve this issue:

#### Option 1: Run azd init in your pipeline (Recommended)

Add an `azd init` step to your Azure DevOps pipeline before the provisioning step. You can use the `--from-code` and `--no-prompt` flags to run the command non-interactively:

```yaml
- task: AzureCLI@2
  displayName: 'Initialize Azure Developer CLI'
  inputs:
    azureSubscription: '$(AZURE_SERVICE_CONNECTION)'
    scriptType: 'bash'
    scriptLocation: 'inlineScript'
    inlineScript: |
      azd init --from-code --no-prompt
      azd env new $(AZURE_ENV_NAME) --location $(AZURE_LOCATION) --subscription $(AZURE_SUBSCRIPTION_ID)
```

> [!NOTE]
> If you encounter prompts even with `--no-prompt`, try running `azd init` and `azd env new` as separate steps, or use environment variables to provide answers to any prompts. The `--from-code` flag tells azd to use the existing code in the current directory rather than creating a new project from a template.

Make sure to define the following variables in your pipeline:

- `AZURE_ENV_NAME`: Your environment name (for example, `dev` or `prod`).
- `AZURE_LOCATION`: Your Azure region (for example, `eastus2`).
- `AZURE_SUBSCRIPTION_ID`: Your Azure subscription ID.

#### Option 2: Commit required files to your repository

If you prefer to commit the generated files to your repository:

1. Run `azd init` locally in your project directory.
1. Add the generated `azure.yaml` file to your repository.
1. Optionally, add the `.azure` folder to your repository if you want to preserve environment-specific settings.

> [!NOTE]
> The `.azure` folder contains environment-specific configuration that might include sensitive information. Review the contents carefully before committing to your repository.

#### Option 3: Use azd pipeline config with proper initialization

Ensure that you run `azd pipeline config --provider azdo` after successfully running `azd init` locally. This command should set up the pipeline with the correct configuration that handles the initialization automatically.

If you continue to experience issues, verify that:

- Your project structure matches what `azd` expects for .NET Aspire projects.
- You're running the commands from the correct directory (typically where your `.sln` file is located).
- Your Azure DevOps service connection has the necessary permissions for provisioning resources.

:::zone-end

[!INCLUDE [clean-up-resources](../../includes/clean-up-resources.md)]
