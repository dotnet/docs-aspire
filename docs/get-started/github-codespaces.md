---
title: .NET Aspire and GitHub Codespaces
description: Learn how to use .NET Aspire with GitHub Codespaces.
ms.date: 02/21/2025
---

# .NET Aspire and GitHub Codespaces

[GitHub Codespaces](https://github.com/features/codespaces) offers a cloud-hosted development environment based on Visual Studio Code. It can be accessed directly from a web browser or through Visual Studio Code locally, where Visual Studio Code acts as a client connecting to a cloud-hosted backend. With .NET Aspire 9.1, comes logic to better support GitHub Codespaces including:

- Automatically configure port forwarding with the correct protocol.
- Automatically translate URLs in the .NET Aspire dashboard.

Before .NET Aspire 9.1 it was still possible to use .NET Aspire within a GitHub Codespace, however more manual configuration was required.

## GitHub Codespaces vs. Dev Containers

GitHub Codespaces builds upon Visual Studio Code and the [Dev Containers specification](https://containers.dev/implementors/spec/). In addition to supporting GitHub Codespaces, .NET Aspire 9.1 enhances support for using Visual Studio Code and locally hosted Dev Containers. While the experiences are similar, there are some differences. For more information, see [.NET Aspire with Dev Containers in Visual Studio Code](dev-ontainers.md).

## Quick start using template repository

To configure GitHub Codespaces for .NET Aspire, use the _.devcontainer/devcontainer.json_ file in your repository. The simplest way to get started is by creating a new repository from our [template repository](https://github.com/dotnet/aspire-devcontainer). Consider the following steps:

1. [Create a new repository](https://github.com/new?template_name=aspire-devcontainer&template_owner=dotnet) using our template.

    :::image source="media/new-repository-from-template.png" lightbox="media/new-repository-from-template.png" alt-text="Create new repository.":::

    Once you provide the details and select **Create repository**, the repository is created and shown in GitHub.

1. From the new repository, select on the Code button and select the Codespaces tab and then select **Create codespace on main**.

    :::image source="media/create-codespace-from-repository.png" lightbox="media/create-codespace-from-repository.png" alt-text="Create codespace":::

    After you select **Create codespace on main**, you navigate to a web-based version of Visual Studio Code. Before you use the Codespace, the containerized development environment needs to be prepared. This process happens automatically on the server and you can review progress by selecting the **Building codespace** link on the notification in the bottom right of the browser window.

    :::image source="media/building-codespace-image.png" lightbox="media/building-codespace-image.png" alt-text="Building codespace":::

    When the container image has finished being built the **Terminal** prompt appears which signals that the environment is ready to be interacted with.

    :::image source="media/codespace-terminal.png" lightbox="media/codespace-terminal.png" alt-text="Codespace terminal prompt":::

    At this point, the .NET Aspire templates have been installed and the ASP.NET Core developer certificate has been added and accepted.

1. Create a new .NET Aspire project using the starter template.

    ```dotnetcli
    dotnet new aspire-starter --name HelloAspire
    ```

    This results in many files and folders being created in the repository, which are visible in the **Explorer** panel on the left side of the window.

    :::image source="media/codespaces-explorer-panel.png" lightbox="media/codespaces-explorer-panel.png" alt-text="Codespaces Explorer panel":::

1. Launch the app host via the _HelloAspire.AppHost/Program.cs_ file, by selecting the **Run project** button near the top-right corner of the **Tab bar**.

    :::image source="media/codespace-launch-apphost.png" lightbox="media/codespace-launch-apphost.png" alt-text="Launch app host in Codespace":::

    After a few moments the **Debug Console** panel is displayed, and it includes a link to the .NET Aspire dashboard exposed on a GitHub Codespaces endpoint with the authentication token.

    :::image source="media/codespaces-debug-console.png" lightbox="media/codespaces-debug-console.png" alt-text="Codespaces debug console":::

1. Open the .NET Aspire dashboard by selecting the dashboard URL in the **Debug Console**. This opens the .NET Aspire dashboard in a separate tab within your browser.

    You notice on the dashboard that all HTTP/HTTPS endpoints defined on resources have had their typical `localhost` address translated to a unique fully qualified subdomain on the `app.github.dev` domain.

    :::image source="media/codespaces-translated-urls.png" lightbox="media/codespaces-translated-urls.png" alt-text="Codespaces translated URLs":::

    Traffic to each of these endpoints is automatically forwarded to the underlying process or container running within the Codespace. This includes development time tools such as PgAdmin and Redis Insight.

    > [!NOTE]
    > In addition to the authentication token embedded within the URL of the dashboard link of the **Debug Console**, endpoints also require authentication via your GitHub identity to avoid port forwarded endpoints being accessible to everyone. For more information on port forwarding in GitHub Codespaces, see [Forwarding ports in your codespace](https://docs.github.com/codespaces/developing-in-a-codespace/forwarding-ports-in-your-codespace?tool=webui).

1. Commit changes to the GitHub repository.

    GitHub Codespaces doesn't automatically commit your changes to the branch you're working on in GitHub. You have to use the **Source Control** panel to stage and commit the changes and push them back to the repository.

    Working in a GitHub Codespace is similar to working with Visual Studio Code on your own machine. You can checkout different branches and push changes just like you normally would. In addition, you can easily spin up multiple Codespaces simultaneously if you want to quickly work on another branch without disrupting your existing debug session. For more information, see [Developing in a codespace](https://docs.github.com/codespaces/developing-in-a-codespace/developing-in-a-codespace?tool=webui).

1. Clean up your Codespace.

    GitHub Codespaces are temporary development environments and while you might use one for an extended period of time, they should be considered a disposable resource that you recreate as needed (with all of the customization/setup contained within the _devcontainer.json_ and associated configuration files).

    To delete your GitHub Codespace, visit the GitHub Codespaces page. This shows you a list of all of your Codespaces. From here you can perform management operations on each Codespace, including deleting them.

    GitHub charges for the use of Codespaces. For more information, see [Managing the cost of GitHub Codespaces in your organization](https://docs.github.com/codespaces/managing-codespaces-for-your-organization/choosing-who-owns-and-pays-for-codespaces-in-your-organization).

    > [!NOTE]
    > .NET Aspire supports the use of Dev Containers in Visual Studio Code independent of GitHub Codespaces. For more information on how to use Dev Containers locally, see [.NET Aspire and Dev Containers in Visual Studio Code](dev-containers.md).

## Manually configuring _devcontainer.json_

The preceding walkthrough demonstrates the streamlined process of creating a GitHub Codespace using the .NET Aspire Devcontainer template. If you already have an existing repository and wish to utilize Devcontainer functionality with .NET Aspire, add a _devcontainer.json_ file to the _.devcontainer_ folder within your repository:

```Directory
└───📂 .devcontainer
     └─── devcontainer.json
```

The [template repository](https://github.com/dotnet/aspire-devcontainer) contains a copy of the _devcontainer.json_ file that you can use as a starting point, which should be sufficient for .NET Aspire. The following JSON represents the latest version of the _.devcontainer/devcontainer.json_ file from the template:

<!-- 
When https://github.com/dotnet/aspire-devcontainer is public, add the following JSON to the openpublishing.publish.config.json file:

```json
    {
      "path_to_root": "aspire-devcontainer",
      "url": "https://github.com/dotnet/aspire-devcontainer",
      "branch": "main",
      "branch_mapping": {}
    },
```

And use this instead of the hardcoded JSON below:

:::code language="json" source="~/aspire-devcontainer/.devcontainer/devcontainer.json":::

-->

```json
// For format details, see https://aka.ms/devcontainer.json. For config options, see the
// README at: https://github.com/devcontainers/templates/tree/main/src/dotnet
{
    "name": ".NET Aspire",
    // Or use a Dockerfile or Docker Compose file. More info: https://containers.dev/guide/dockerfile
    "image": "mcr.microsoft.com/devcontainers/dotnet:9.0-bookworm",
    "features": {
        "ghcr.io/devcontainers/features/azure-cli:1": {},
        "ghcr.io/devcontainers/features/docker-in-docker:2": {},
        "ghcr.io/devcontainers/features/powershell:1": {},
        "ghcr.io/azure/azure-dev/azd:0": {}
    },

    "hostRequirements": {
        "cpus": 8,
        "memory": "32gb",
        "storage": "64gb"
    },

    // Use 'forwardPorts' to make a list of ports inside the container available locally.
    // "forwardPorts": [5000, 5001],
    // "portsAttributes": {
    //        "5001": {
    //            "protocol": "https"
    //        }
    // }

    // Use 'postCreateCommand' to run commands after the container is created.
    // "postCreateCommand": "dotnet restore",
    "onCreateCommand": "dotnet new install Aspire.ProjectTemplates::9.0.0 --force",
    "postStartCommand": "dotnet dev-certs https --trust",
    "customizations": {
        "vscode": {
            "extensions": [
                "ms-dotnettools.csdevkit",
                "ms-azuretools.vscode-bicep",
                "ms-azuretools.azure-dev",
                "GitHub.copilot-chat",
                "GitHub.copilot"
            ]
        }
    }
    // Configure tool-specific properties.
    // "customizations": {},

    // Uncomment to connect as root instead. More info: https://aka.ms/dev-containers-non-root.
    // "remoteUser": "root"
}
```

## Speed up Codespace creation

Creating a GitHub Codespace can take some time as it prepares the underlying container image. To expedite this process, you can utilize _prebuilds_ to significantly reduce the creation time to approximately 30-60 seconds (exact timing might vary). For more information on GitHub Codespaces prebuilds, see [GitHub Codespaces prebuilds](https://docs.github.com/codespaces/prebuilding-your-codespaces/about-github-codespaces-prebuilds).
