---
title: Dev Containers in Visual Studio Code
description: Learn how to use .NET Aspire with Dev Containers in Visual Studio Code.
ms.date: 02/25/2025
---

# .NET Aspire and Visual Studio Code Dev Containers

The [Dev Containers Visual Studio Code extension](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-containers) provides a way for development teams to develop within a containerized environment where all dependencies are preconfigured. With .NET Aspire 9.1, there's added logic to better support working with .NET Aspire within a Dev Container environment by automatically configuring port forwarding.

Before .NET Aspire 9.1, it possible to use .NET Aspire within a Dev Container, however more manual configuration was required.

## Dev Containers vs. GitHub Codespaces

Using Dev Containers in Visual Studio Code is similar to using GitHub Codespaces. With the release of .NET Aspire 9.1, support for both Dev Containers in Visual Studio Code and GitHub Codespaces was enhanced. Although the experiences are similar, there are some differences. For more information on using .NET Aspire with GitHub Codespaces, see [.NET Aspire and GitHub Codespaces](github-codespaces.md).

## Quick start using template repository

To configure Dev Containers in Visual Studio Code, use the _.devcontainer/devcontainer.json file in your repository. The simplest way to get started is by creating a new repository from our [template repository](https://github.com/dotnet/aspire-devcontainer). Consider the following steps:

1. [Create a new repository](https://github.com/new?template_name=aspire-devcontainer&template_owner=dotnet) using our template.

    :::image source="media/new-repository-from-template.png" lightbox="media/new-repository-from-template.png" alt-text="Create new repository.":::

    Once you provide the details and select **Create repository**, the repository is created and shown in GitHub.

1. Clone the repository to your local developer workstation using the following command:

    ```dotnetcli
    git clone https://github.com/<org>/<username>/<repository>
    ```

1. Open the repository in Visual Studio Code. After a few moments Visual Studio Code detects the _.devcontainer/devcontainer.json_ file and prompt to open the repository inside a container. Select whichever option is most appropriate for your workflow.

    :::image source="media/reopen-in-container.png" lightbox="media/reopen-in-container.png" alt-text="Prompt to open repository inside a container.":::

    After a few moments, the list of files become visible and the local build of the dev container will be completed.

    :::image source="media/devcontainer-build-completed.png" lightbox="media/devcontainer-build-completed.png" alt-text="Dev Container build completed.":::

1. Open a new terminal window in Visual Studio Code (<kbd>Ctrl</kbd>+<kbd>Shift</kbd>+<kbd>\`</kbd>) and create a new .NET Aspire project using the `dotnet` command-line.

    ```dotnetcli
    dotnet new aspire-starter -n HelloAspire
    ```

    After a few moments, the project will be created and initial dependencies restored.

1. Open the _ProjectName.AppHost/Program.cs_ file in the editor and select the run button on the top right corner of the editor window.

    :::image source="media/vscode-run-button.png" lightbox="media/vscode-run-button.png" alt-text="Run button in editor.":::

    Visual Studio Code builds and starts the .NET Aspire app host and automatically opens the .NET Aspire Dashboard. Because the endpoints hosted in the container are using a self-signed certificate the first time, you access an endpoint for a specific Dev Container you're presented with a certificate error.

    :::image source="media/browser-certificate-error.png" lightbox="media/browser-certificate-error.png" alt-text="Browser certificate error.":::

    The certificate error is expected. Once you've confirmed that the URL being requested corresponds to the dashboard in the Dev Container you can ignore this warning.

    :::image source="media/aspire-dashboard-in-devcontainer.png" lightbox="media/aspire-dashboard-in-devcontainer.png" alt-text=".NET Aspire dashboard running in Dev Container.":::

    .NET Aspire automatically configures forwarded ports so that when you select on the endpoints in the .NET Aspire dashboard they're tunneled to processes and nested containers within the Dev Container.

1. Commit changes to the GitHub repository

    After successfully creating the .NET Aspire project and verifying that it launches and you can access the dashboard, it's a good idea to commit the changes to the repository.

## Manually configuring _devcontainer.json_

The preceding walkthrough demonstrates the streamlined process of creating a Dev Container using the .NET Aspire Dev Container template. If you already have an existing repository and wish to utilize Dev Container functionality with .NET Aspire, add a _devcontainer.json_ file to the _.devcontainer_ folder within your repository:

```Directory
└───📂 .devcontainer
     └─── devcontainer.json
```

The [template repository](https://github.com/dotnet/aspire-devcontainer) contains a copy of the _devcontainer.json_ file that you can use as a starting point, which should be sufficient for .NET Aspire. The following JSON represents the latest version of the _.devcontainer/devcontainer.json_ file from the template:

:::code language="json" source="~/aspire-devcontainer/.devcontainer/devcontainer.json":::
