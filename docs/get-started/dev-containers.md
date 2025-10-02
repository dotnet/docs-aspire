---
title: Dev Containers in Visual Studio Code
description: Learn how to use Aspire with Dev Containers in Visual Studio Code.
ms.date: 02/25/2025
---

# Aspire and Visual Studio Code Dev Containers

The [Dev Containers Visual Studio Code extension](https://marketplace.visualstudio.com/items?itemName=ms-vscode-remote.remote-containers) provides a way for development teams to develop within a containerized environment where all dependencies are preconfigured. With Aspire 9.1, there's added logic to better support working with Aspire within a Dev Container environment by automatically configuring port forwarding.

Before Aspire 9.1, it possible to use Aspire within a Dev Container, however more manual configuration was required.

## Dev Containers vs. GitHub Codespaces

Using Dev Containers in Visual Studio Code is similar to using GitHub Codespaces. With the release of Aspire 9.1, support for both Dev Containers in Visual Studio Code and GitHub Codespaces was enhanced. Although the experiences are similar, there are some differences. For more information on using Aspire with GitHub Codespaces, see [Aspire and GitHub Codespaces](github-codespaces.md).

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

1. Open a new terminal window in Visual Studio Code (<kbd>Ctrl</kbd>+<kbd>Shift</kbd>+<kbd>\`</kbd>) and create a new Aspire project using the `dotnet` command-line.

    ```dotnetcli
    dotnet new aspire-starter -n HelloAspire
    ```

    After a few moments, the project will be created and initial dependencies restored.

1. Open the _ProjectName.AppHost/AppHost.cs_ file in the editor and select the run button on the top right corner of the editor window.

    :::image source="media/vscode-run-button.png" lightbox="media/vscode-run-button.png" alt-text="Run button in editor.":::

    Visual Studio Code builds and starts the Aspire AppHost and automatically opens the Aspire Dashboard. Because the endpoints hosted in the container are using a self-signed certificate the first time, you access an endpoint for a specific Dev Container you're presented with a certificate error.

    :::image source="media/browser-certificate-error.png" lightbox="media/browser-certificate-error.png" alt-text="Browser certificate error.":::

    The certificate error is expected. Once you've confirmed that the URL being requested corresponds to the dashboard in the Dev Container you can ignore this warning.

    :::image source="media/aspire-dashboard-in-devcontainer.png" lightbox="media/aspire-dashboard-in-devcontainer.png" alt-text="Aspire dashboard running in Dev Container.":::

    Aspire automatically configures forwarded ports so that when you select on the endpoints in the Aspire dashboard they're tunneled to processes and nested containers within the Dev Container.

1. Commit changes to the GitHub repository

    After successfully creating the Aspire project and verifying that it launches and you can access the dashboard, it's a good idea to commit the changes to the repository.

## Manually configuring _devcontainer.json_

The preceding walkthrough demonstrates the streamlined process of creating a Dev Container using the Aspire Dev Container template. If you already have an existing repository and wish to utilize Dev Container functionality with Aspire, add a _devcontainer.json_ file to the _.devcontainer_ folder within your repository:

```Directory
└───📂 .devcontainer
     └─── devcontainer.json
```

The [template repository](https://github.com/dotnet/aspire-devcontainer) contains a copy of the _devcontainer.json_ file that you can use as a starting point, which should be sufficient for Aspire. The following JSON represents the latest version of the _.devcontainer/devcontainer.json_ file from the template:

:::code language="json" source="~/aspire-devcontainer/.devcontainer/devcontainer.json":::

## Dev Container scenarios

The basic Aspire Dev Container template works well for simple scenarios, but you might need additional configuration depending on your specific requirements. The following sections provide examples for various common scenarios.

### Stateless .NET apps only

For simple Aspire projects that only use .NET project resources without external containers or complex orchestration, you can use a minimal Dev Container configuration:

```json
{
  "name": "Aspire - Simple",
  "image": "mcr.microsoft.com/devcontainers/dotnet:9.0-bookworm",
  "onCreateCommand": "dotnet new install Aspire.ProjectTemplates --force",
  "postStartCommand": "dotnet dev-certs https --trust",
  "customizations": {
    "vscode": {
      "extensions": [
        "ms-dotnettools.csdevkit"
      ]
    }
  }
}
```

This minimal configuration is suitable for Aspire apps that orchestrate only .NET services without external dependencies.

### Adding Node.js resources

If your Aspire app includes Node.js resources, add the Node.js feature to your Dev Container:

```json
{
  "name": "Aspire with Node.js",
  "image": "mcr.microsoft.com/devcontainers/dotnet:9.0-bookworm",
  "features": {
    "ghcr.io/devcontainers/features/node:1": {
      "version": "lts"
    }
  },
  "onCreateCommand": "dotnet new install Aspire.ProjectTemplates --force",
  "postStartCommand": "dotnet dev-certs https --trust",
  "customizations": {
    "vscode": {
      "extensions": [
        "ms-dotnettools.csdevkit",
        "ms-vscode.vscode-typescript-next"
      ]
    }
  }
}
```

This configuration provides both .NET and Node.js development capabilities within the same container environment.

### Container orchestration with Docker-in-Docker

When your Aspire app orchestrates container resources, you need Docker-in-Docker (DinD) support. Here's a basic configuration:

```json
{
  "name": "Aspire with Containers",
  "image": "mcr.microsoft.com/devcontainers/dotnet:9.0-bookworm",
  "features": {
    "ghcr.io/devcontainers/features/docker-in-docker:2": {
      "version": "latest",
      "enableNonRootDocker": true,
      "moby": true
    }
  },
  "hostRequirements": {
    "cpus": 4,
    "memory": "16gb",
    "storage": "32gb"
  },
  "onCreateCommand": "dotnet new install Aspire.ProjectTemplates --force",
  "postStartCommand": "dotnet dev-certs https --trust",
  "customizations": {
    "vscode": {
      "extensions": [
        "ms-dotnettools.csdevkit",
        "ms-azuretools.vscode-docker"
      ]
    }
  }
}
```

#### Advanced container networking

If you encounter networking issues between containers or need IPv6 support, you can add additional network configuration:

```json
{
  "name": "Aspire with Advanced Networking",
  "image": "mcr.microsoft.com/devcontainers/dotnet:9.0-bookworm",
  "features": {
    "ghcr.io/devcontainers/features/docker-in-docker:2": {
      "version": "latest",
      "enableNonRootDocker": true,
      "moby": true
    }
  },
  "runArgs": [
    "--sysctl",
    "net.ipv6.conf.all.disable_ipv6=0",
    "--sysctl",
    "net.ipv6.conf.default.forwarding=1",
    "--sysctl",
    "net.ipv6.conf.all.forwarding=1"
  ],
  "hostRequirements": {
    "cpus": 8,
    "memory": "32gb",
    "storage": "64gb"
  },
  "onCreateCommand": "dotnet new install Aspire.ProjectTemplates --force",
  "postStartCommand": "dotnet dev-certs https --trust",
  "customizations": {
    "vscode": {
      "extensions": [
        "ms-dotnettools.csdevkit",
        "ms-azuretools.vscode-docker"
      ]
    }
  }
}
```

> [!IMPORTANT]
> **Docker-in-Docker considerations:**
>
> - Docker-in-Docker requires higher resource allocation including increased CPU, memory, and storage.
> - The advanced networking configuration above includes IPv6 forwarding settings that may be needed for complex container-to-container communication scenarios.
> - This configuration works with Docker Desktop but may have limitations with Rancher Desktop.
> - Network connectivity between containers might require additional configuration depending on your specific use case.

### Dapr integration examples

For Aspire apps that integrate with Dapr, you can set up Dapr components in your Dev Container. For more information, see [Aspire Dapr integration](../community-toolkit/dapr.md).

#### Basic Dapr setup

```json
{
  "name": "Aspire with Dapr",
  "image": "mcr.microsoft.com/devcontainers/dotnet:9.0-bookworm",
  "features": {
    "ghcr.io/devcontainers/features/docker-in-docker:2": {
      "enableNonRootDocker": true
    },
    "ghcr.io/dapr/cli/dapr-cli:0": {}
  },
  "onCreateCommand": "dotnet new install Aspire.ProjectTemplates --force",
  "postCreateCommand": "dotnet dev-certs https --trust && dapr init",
  "customizations": {
    "vscode": {
      "extensions": [
        "ms-dotnettools.csdevkit",
        "ms-azuretools.vscode-dapr"
      ]
    }
  }
}
```

#### Dapr with external backends

For more complex Dapr scenarios that use external backends (Redis, PostgreSQL), you can use Docker Compose:

```json
{
  "name": "Aspire with Dapr and Backends",
  "image": "mcr.microsoft.com/devcontainers/dotnet:9.0-bookworm",
  "features": {
    "ghcr.io/devcontainers/features/docker-in-docker:2": {
      "enableNonRootDocker": true
    },
    "ghcr.io/dapr/cli/dapr-cli:0": {}
  },
  "runArgs": [
    "--sysctl",
    "net.ipv6.conf.all.disable_ipv6=0"
  ],
  "onCreateCommand": "dotnet new install Aspire.ProjectTemplates --force",
  "postCreateCommand": [
    "dotnet dev-certs https --trust",
    "docker compose up -d",
    "dapr init"
  ],
  "customizations": {
    "vscode": {
      "extensions": [
        "ms-dotnettools.csdevkit",
        "ms-azuretools.vscode-dapr",
        "ms-azuretools.vscode-docker"
      ]
    }
  }
}
```

## Common considerations

When using Dev Containers with Aspire, keep the following considerations in mind:

**Resource requirements**

- **Basic .NET apps**: Standard Dev Container resources are sufficient for simple scenarios.
- **Container orchestration**: A minimum of 8 CPUs, 32GB memory, and 64GB storage is recommended.
- **Complex scenarios with Dapr/Kubernetes**: Higher resource allocation is recommended for optimal performance.

**Networking**

- IPv6 configuration may be required for container-to-container communication.
- Port forwarding is automatically handled by Aspire 9.1 and later versions.
- External service connectivity depends on your container runtime configuration.

**Performance**

- Docker-in-Docker scenarios incur performance overhead compared to native Docker.
- Consider using Docker outside of Docker (DooD) for production workflows.
- Local development and deployment scenarios may require different configurations.

**Security**

- Dev Containers run with elevated privileges when using Docker-in-Docker.
- SSL certificate trust is handled automatically in most scenarios.
- Consider security implications when exposing ports in cloud environments.

## See also

- [Aspire and GitHub Codespaces](github-codespaces.md)
- [Aspire Dapr integration](../community-toolkit/dapr.md)
- [Add Dockerfiles to your .NET app model](../app-host/withdockerfile.md)
- [Dev Containers specification](https://containers.dev/implementors/spec/)
