---
title: .NET Aspire tooling
description: Learn about essential tooling concepts for .NET Aspire.
ms.date: 07/22/2025
zone_pivot_groups: dev-environment
uid: dotnet/aspire/setup-tooling
ms.custom: sfi-image-nochange
---

# .NET Aspire setup and tooling

.NET Aspire includes tooling to help you create and configure cloud-native apps. The tooling includes useful starter project templates and other features to streamline getting started with .NET Aspire for Visual Studio, Visual Studio Code, and CLI workflows. In the sections ahead, you learn how to work with .NET Aspire tooling and explore the following tasks:

> [!div class="checklist"]
>
> - Install .NET Aspire and its dependencies
> - Create starter project templates using Visual Studio, Visual Studio Code, or the .NET CLI
> - Install .NET Aspire integrations
> - Work with the .NET Aspire dashboard

## Install .NET Aspire prerequisites

To work with .NET Aspire, you need the following installed locally:

- [.NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) or [.NET 9.0](https://dotnet.microsoft.com/download/dotnet/9.0).
  - Starting with .NET Aspire 9.4, [.NET 10 Preview 5 or later](https://dotnet.microsoft.com/download/dotnet/10.0) is supported.
- An OCI compliant container runtime, such as:
  - [Docker Desktop](https://www.docker.com/products/docker-desktop)
  - [Podman](https://podman.io/)
    - _For more information, see [Container runtime](#container-runtime)_.
- An Integrated Developer Environment (IDE) or code editor, such as:
  - [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) version 17.9 or higher (Optional)
  - [Visual Studio Code](https://code.visualstudio.com/) (Optional)
    - [C# Dev Kit: Extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) (Optional)
  - [JetBrains Rider with .NET Aspire plugin](https://blog.jetbrains.com/dotnet/2024/02/19/jetbrains-rider-and-the-net-aspire-plugin/) (Optional)

> [!TIP]
> Alternatively, you can develop .NET Aspire solutions using [GitHub Codespaces](../get-started/github-codespaces.md) or [Dev Containers](../get-started/dev-containers.md).

:::zone pivot="visual-studio"

Visual Studio 2022 17.9 or higher includes the latest [.NET Aspire SDK](dotnet-aspire-sdk.md) by default when you install the Web & Cloud workload. If you have an earlier version of Visual Studio 2022, you can either upgrade to Visual Studio 2022 17.9 or you can install the .NET Aspire SDK using the following steps:

To install the .NET Aspire workload in Visual Studio 2022, use the Visual Studio installer.

1. Open the Visual Studio Installer.
1. Select **Modify** next to Visual Studio 2022.
1. Select the **ASP.NET and web development** workload.
1. On the **Installation details** panel, select **.NET Aspire SDK**.
1. Select **Modify** to install the .NET Aspire integration.

   :::image type="content" loc-scope="visual-studio" source="media/setup-tooling/web-workload-with-aspire.png" lightbox="media/setup-tooling/web-workload-with-aspire.png" alt-text="A screenshot showing how to install the .NET Aspire workload with the Visual Studio installer.":::

:::zone-end
:::zone pivot="vscode,dotnet-cli"

<!-- Visual Studio Code and .NET CLI instructions

  Intentionally left blank, as you don't need to do anything extra.

-->

:::zone-end

## .NET Aspire templates

.NET Aspire provides a set of solution and project templates. These templates are available in your favorite .NET developer integrated environment. You can use these templates to create full .NET Aspire solutions, or add individual projects to existing .NET Aspire solutions.

### Install the .NET Aspire templates

[!INCLUDE [Install templates](includes/install-templates.md)]

### List the .NET Aspire templates

:::zone pivot="visual-studio"

The .NET Aspire templates are installed automatically when you install Visual Studio 17.9 or later. To see what .NET Aspire templates are available, select **File** > **New** > **Project** in Visual Studio, and search for "Aspire" in the search bar (<kbd>Alt</kbd>+<kbd>S</kbd>). You see a list of available .NET Aspire project templates:

:::image type="content" source="media/vs-create-dotnet-aspire-proj.png" alt-text="Visual Studio: Create new project and search for 'Aspire'." lightbox="media/vs-create-dotnet-aspire-proj.png":::

:::zone-end
:::zone pivot="vscode"

To view the available templates in Visual Studio Code with the C# DevKit installed, select the **Create .NET Project** button when no folder is opened in the **Explorer** view:

:::image type="content" source="media/vscode-create-dotnet-proj.png" alt-text="Visual Studio Code: Create .NET Project button." lightbox="media/vscode-create-dotnet-proj.png":::

Then, search for "Aspire" in the search bar to see the available .NET Aspire project templates:

:::image type="content" source="media/vscode-create-dotnet-aspire-proj.png" alt-text="Visual Studio Code: Create new project and search for 'Aspire'." lightbox="media/vscode-create-dotnet-aspire-proj.png":::

:::zone-end
:::zone pivot="dotnet-cli"

To verify that the .NET Aspire templates are installed, use the [dotnet new list](/dotnet/core/tools/dotnet-new-list) command, passing in the `aspire` template name:

```dotnetcli
dotnet new list aspire
```

Your console output should look like the following:

[!INCLUDE [dotnet-new-list-aspire-output](includes/dotnet-new-list-aspire-output.md)]

:::zone-end

For more information, see [.NET Aspire templates](aspire-sdk-templates.md).

## Container runtime

.NET Aspire can run containers using several OCI-compatible runtimes, including Docker Desktop and Podman. While some users have reported success using [Rancher Desktop](https://rancherdesktop.io/)—particularly when configured to use the Docker CLI—this isn't an officially supported or regularly tested scenario. It might be possible to use Rancher Desktop with the default installation, but it's not an officially supported or validated approach. If you encounter issues with Rancher Desktop, please let us know, but be aware that fixes might not be prioritized.

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) is the most popular container runtime among .NET Aspire developers, offering a familiar and widely supported environment for building and running containers.
- [Podman](https://podman.io/docs/installation) is an open-source, daemonless alternative to Docker. It supports building and running Open Container Initiative (OCI) containers, making it a flexible choice for developers who prefer a lightweight solution.

If your host environment has a Docker and Podman installed, .NET Aspire defaults to using Docker. You can instruct .NET Aspire to use Podman instead, by setting the `ASPIRE_CONTAINER_RUNTIME` environment variable to `podman`:

## [Linux](#tab/linux)

```bash
export ASPIRE_CONTAINER_RUNTIME=podman
```

For more information, see [Install Podman on Linux](https://podman.io/docs/installation#installing-on-linux).

## [Windows](#tab/windows)

```powershell
[System.Environment]::SetEnvironmentVariable("ASPIRE_CONTAINER_RUNTIME", "podman", "User")
```

For more information, see [Install Podman on Windows](https://podman.io/docs/installation#installing-on-mac--windows).

---

### WSL (Windows Subsystem for Linux) considerations

When using Podman with WSL, ensure that the `podman` executable is available in your `PATH` and not just defined as a shell alias. .NET Aspire resolves container runtimes by searching for the executable in the system PATH, and shell aliases aren't recognized during this process.

**Common issues and solutions:**

- **Podman installed in a separate WSL distribution**: If Podman is installed in a different WSL distribution than your .NET Aspire application, the `podman` command might not be available in your current distribution's PATH.

  **Solution**: Install Podman directly in the WSL distribution where you're running your .NET Aspire application, or create a symbolic link to the Podman executable in a directory that's in your PATH (such as `/usr/local/bin`).

- **Using shell aliases**: If you have a shell alias like `alias podman='podman-remote-static-linux_amd64'` in your `~/.bash_aliases` or similar file, .NET Aspire won't be able to find the container runtime.

  **Solution**: Instead of using an alias, create a symbolic link or add the directory containing the Podman executable to your PATH:

  ```bash
  # Option 1: Create a symbolic link
  sudo ln -s /path/to/podman-remote-static-linux_amd64 /usr/local/bin/podman
  
  # Option 2: Add to PATH in your shell profile
  echo 'export PATH="/path/to/podman/directory:$PATH"' >> ~/.bashrc
  source ~/.bashrc
  ```

**Verify your setup**: You can verify that Podman is correctly configured by running:

```bash
which podman
podman --version
```

Both commands should succeed and return valid results before running your .NET Aspire application.

> [!TIP]
> If you encounter issues with Podman in WSL environments, see [Container runtime 'podman' could not be found in WSL](../troubleshooting/podman-wsl-not-found.md) for specific troubleshooting guidance.

## .NET Aspire dashboard

.NET Aspire templates that expose the [AppHost](app-host-overview.md) project also include a useful developer [dashboard](dashboard/overview.md) that's used to monitor and inspect various aspects of your app, such as logs, traces, and environment configurations. This dashboard is designed to improve the local development experience and provides an overview of the overall state and structure of your app.

The .NET Aspire dashboard is only visible while the app is running and starts automatically when you start the _*.AppHost_ project. Visual Studio and Visual Studio Code launch both your app and the .NET Aspire dashboard for you automatically in your browser. If you start the app using the .NET CLI, copy and paste the dashboard URL from the output into your browser, or hold <kbd>Ctrl</kbd> and select the link (if your terminal supports hyperlinks).

:::image type="content" source="dashboard/media/explore/dotnet-run-login-url.png" lightbox="dashboard/media/explore/dotnet-run-login-url.png" alt-text="A screenshot showing how to launch the dashboard using the CLI.":::

The left navigation provides links to the different parts of the dashboard, each of which you explore in the following sections.

:::image type="content" source="../get-started/media/aspire-dashboard.png" lightbox="../get-started/media/aspire-dashboard.png" alt-text="A screenshot of the .NET Aspire dashboard Projects page.":::

The .NET Aspire dashboard is also available in a standalone mode. For more information, see [Standalone .NET Aspire dashboard](dashboard/standalone.md).

:::zone pivot="visual-studio"

## Visual Studio tooling

Visual Studio provides extra features for working with .NET Aspire integrations and the AppHost orchestrator project. Not all of these features are currently available in Visual Studio Code or through the CLI.

### Add an integration package

You add .NET Aspire integrations to your app like any other NuGet package using Visual Studio. However, Visual Studio also provides UI options to add .NET Aspire integrations directly.

1. In Visual Studio, right select on the project you want to add an .NET Aspire integration to and select **Add** > **.NET Aspire package...**.

    :::image type="content" loc-scope="visual-studio" source="../media/visual-studio-add-aspire-package.png" lightbox="../media/visual-studio-add-aspire-package.png" alt-text="The Visual Studio context menu displaying the Add .NET Aspire Component option.":::

1. The package manager opens with search results preconfigured (populating filter criteria) for .NET Aspire integrations, allowing you to easily browse and select the desired integration.

    :::image type="content" loc-scope="visual-studio" source="../media/visual-studio-add-aspire-comp-nuget.png" lightbox="../media/visual-studio-add-aspire-comp-nuget.png" alt-text="The Visual Studio context menu displaying the Add .NET Aspire integration options.":::

For more information on .NET Aspire integrations, see [.NET Aspire integrations overview](integrations-overview.md).

### Add hosting packages

.NET Aspire hosting packages are used to configure various resources and dependencies an app might depend on or consume. Hosting packages are differentiated from other integration packages in that they're added to the _*.AppHost_ project. To add a hosting package to your app, follow these steps:

1. In Visual Studio, right select on the _*.AppHost_ project and select **Add** > **.NET Aspire package...**.

    :::image type="content" loc-scope="visual-studio" source="../media/visual-studio-add-aspire-hosting-package.png" lightbox="../media/visual-studio-add-aspire-hosting-package.png" alt-text="The Visual Studio context menu displaying the Add .NET Aspire Hosting Resource option.":::

1. The package manager opens with search results preconfigured (populating filter criteria) for .NET Aspire hosting packages, allowing you to easily browse and select the desired package.

    :::image type="content" loc-scope="visual-studio" source="../media/visual-studio-add-aspire-hosting-nuget.png" lightbox="../media/visual-studio-add-aspire-hosting-nuget.png" alt-text="The Visual Studio context menu displaying the Add .NET Aspire resource options.":::

### Add orchestration projects

You can add .NET Aspire orchestration projects to an existing app using the following steps:

1. In Visual Studio, right select on an existing project and select **Add** > **.NET Aspire Orchestrator Support..**.

    :::image type="content" loc-scope="visual-studio" source="../media/visual-studio-add-aspire-orchestrator.png" lightbox="../media/visual-studio-add-aspire-orchestrator.png" alt-text="The Visual Studio context menu displaying the Add .NET Aspire Orchestrator Support option.":::

1. A dialog window opens with a summary of the _*.AppHost_ and _*.ServiceDefaults_ projects that are added to your solution.

    :::image type="content" loc-scope="visual-studio" source="../media/add-orchestrator-app.png" alt-text="A screenshot showing the Visual Studio add .NET Aspire orchestration summary.":::

1. Select **OK** and the following changes are applied:

    - The _*.AppHost_ and _*.ServiceDefaults_ orchestration projects are added to your solution.
    - A call to `builder.AddServiceDefaults` will be added to the _:::no-loc text="Program.cs":::_ file of your original project.
    - A reference to your original project will be added to the _:::no-loc text="Program.cs":::_ file of the _*.AppHost_ project.

For more information on .NET Aspire orchestration, see [.NET Aspire orchestration overview](app-host-overview.md).

### Enlist in orchestration

Visual Studio provides the option to **Enlist in Aspire orchestration** during the new project workflow. Select this option to have Visual Studio create _*.AppHost_ and _*.ServiceDefaults_ projects alongside your selected project template.

:::image type="content" loc-scope="visual-studio" source="../media/aspire-enlist-orchestration.png" lightbox="../media/aspire-enlist-orchestration.png" alt-text="A screenshot showing how to enlist in .NET Aspire orchestration.":::

### Create test project

When you're using Visual Studio, and you select the **.NET Aspire Start Application** template, you have the option to include a test project. This test project is an xUnit project that includes a sample test that you can use as a starting point for your tests.

:::image type="content" source="media/setup-tooling/create-test-projects-template.png" lightbox="media/setup-tooling/create-test-projects-template.png" alt-text="A screenshot of Visual Studio displaying the option to create a test project.":::

For more information, see [Write your first .NET Aspire test](../testing/write-your-first-test.md).

:::zone-end
:::zone pivot="vscode"

## Visual Studio Code tooling

You can use Visual Studio Code, with the [C# Dev Kit extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit), to create and develop .NET Aspire projects. To create a new .NET Aspire project in Visual Studio Code, select the **Create .NET Project** button in the **Explorer** view, then select one of the .NET Aspire templates:

:::image type="content" source="media/vscode-create-dotnet-aspire-proj.png" lightbox="media/vscode-create-dotnet-aspire-proj.png" alt-text="A screenshot showing how to create a new .NET Aspire project in Visual Studio Code.":::

Once you create a new .NET Aspire project, you run and debug the app, stepping through breakpoints, and inspecting variables using the Visual Studio Code debugger:

:::image type="content" source="media/setup-tooling/vscode-debugging.png" lightbox="media/setup-tooling/vscode-debugging.png" alt-text="A screenshot showing how to debug a .NET Aspire project in Visual Studio Code.":::

:::zone-end

## 🖥️ Aspire CLI

🧪 The Aspire CLI is **still in preview** and under active development. Expect more features and polish in future releases.

[!INCLUDE [install-aspire-cli](../includes/install-aspire-cli.md)]

## See also

- [Unable to install .NET Aspire workload](../troubleshooting/unable-to-install-workload.md)
- [Use Dev Proxy with .NET Aspire project](/microsoft-cloud/dev/dev-proxy/how-to/use-dev-proxy-with-dotnet-aspire)
