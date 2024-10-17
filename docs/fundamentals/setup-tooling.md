---
title: .NET Aspire tooling
description: Learn about essential tooling concepts for .NET Aspire.
ms.date: 10/17/2024
zone_pivot_groups: dev-environment
uid: dotnet/aspire/setup-tooling
---

# .NET Aspire setup and tooling

.NET Aspire includes tooling to help you create and configure cloud-native apps. The tooling includes useful starter project templates and other features to streamline getting started with .NET Aspire for Visual Studio, Visual Studio Code, and CLI workflows. In the sections ahead, you learn how to work with .NET Aspire tooling and explore the following tasks:

> [!div class="checklist"]
>
> - Install .NET Aspire and its dependencies
> - Create starter project templates using Visual Studio, Visual Studio Code, or the .NET CLI
> - Install .NET Aspire integrations
> - Work with the .NET Aspire dashboard

## Install .NET Aspire

To work with .NET Aspire, you need the following installed locally:

- [.NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) or [.NET 9.0](https://dotnet.microsoft.com/download/dotnet/9.0).
- [.NET Aspire SDK](dotnet-aspire-sdk.md)
- An OCI compliant container runtime, such as:
  - [Docker Desktop](https://www.docker.com/products/docker-desktop) or [Podman](https://podman.io/). For more information, see [Container runtime](#container-runtime).
- An Integrated Developer Environment (IDE) or code editor, such as:
  - [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) version 17.12 or higher (Optional)
  - [Visual Studio Code](https://code.visualstudio.com/) (Optional)
    - [C# Dev Kit: Extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) (Optional)
  - [JetBrains Rider with .NET Aspire Plugin](https://blog.jetbrains.com/dotnet/2024/02/19/jetbrains-rider-and-the-net-aspire-plugin/)

:::zone pivot="visual-studio"

Visual Studio 2022 17.12 or higher includes the latest [.NET Aspire SDK](dotnet-aspire-sdk.md) by default when you install the Web & Cloud workload. To verify that you have the .NET Aspire SDK installed, ...

If you have an earlier version of Visual Studio 2022, you can either upgrade to Visual Studio 2022 17.12 or you can install the .NET Aspire SDK using the following steps:

To install the .NET Aspire workload in Visual Studio 2022, use the Visual Studio installer.

1. Open the Visual Studio Installer.
1. Select **Modify** next to Visual Studio 2022.
1. Select the **ASP.NET and web development** workload.
1. On the **Installation details** panel, select **.NET Aspire SDK**.
1. Select **Modify** to install the .NET Aspire integration.

   :::image type="content" loc-scope="visual-studio" source="media/setup-tooling/web-workload-with-aspire.png" lightbox="media/setup-tooling/web-workload-with-aspire.png" alt-text="A screenshot showing how to install the .NET Aspire workload with the Visual Studio installer.":::

:::zone-end
:::zone pivot="vscode,dotnet-cli"

To ensure that you install the latest version of the .NET Aspire SDK, ...

:::zone-end

## Container runtime

.NET Aspire projects are designed to run in containers. You can use either Docker Desktop or Podman as your container runtime. [Docker Desktop](https://www.docker.com/products/docker-desktop/) is the most common container runtime. [Podman](https://podman.io/docs/installation) is an open-source daemonless alternative to Docker, that can build and run Open Container Initiative (OCI) containers. If your host environment has both Docker and Podman installed, .NET Aspire defaults to using Docker. You can instruct .NET Aspire to use Podman instead, by setting the `DOTNET_ASPIRE_CONTAINER_RUNTIME` environment variable to `podman`:

## [Linux](#tab/linux)

```bash
export DOTNET_ASPIRE_CONTAINER_RUNTIME=podman
```

For more information, see [Install Podman on Linux](https://podman.io/docs/installation#installing-on-linux).

## [Windows](#tab/windows)

```powershell
[System.Environment]::SetEnvironmentVariable('DOTNET_ASPIRE_CONTAINER_RUNTIME', 'podman')
```

For more information, see [Install Podman on Windows](https://podman.io/docs/installation#installing-on-mac--windows).

---

## .NET Aspire templates

.NET Aspire provides a set of solution and project templates. These templates are available in your favorite .NET developer integrated environment. You can use these templates to create full .NET Aspire solutions, or add individual projects to existing .NET Aspire solutions. For more information, see [.NET Aspire templates](aspire-sdk-templates.md).

## .NET Aspire dashboard

.NET Aspire templates that expose the [app host](app-host-overview.md) project also include a useful developer [dashboard](dashboard/overview.md) that's used to monitor and inspect various aspects of your app, such as logs, traces, and environment configurations. This dashboard is designed to improve the local development experience and provides an overview of the overall state and structure of your app.

The .NET Aspire dashboard is only visible while the app is running and starts automatically when you start the _*.AppHost_ project. Visual Studio and Visual Studio Code launch both your app and the .NET Aspire dashboard for you automatically in your browser. If you start the app using the .NET CLI, copy and paste the dashboard URL from the output into your browser, or hold <kbd>Ctrl</kbd> and select the link (if your terminal supports hyperlinks).

:::image type="content" source="dashboard/media/explore/dotnet-run-login-url.png" lightbox="dashboard/media/explore/dotnet-run-login-url.png" alt-text="A screenshot showing how to launch the dashboard using the CLI.":::

The left navigation provides links to the different parts of the dashboard, each of which you explore in the following sections.

:::image type="content" source="../get-started/media/aspire-dashboard.png" lightbox="../get-started/media/aspire-dashboard.png" alt-text="A screenshot of the .NET Aspire dashboard Projects page.":::

The .NET Aspire dashboard is also available in a standalone mode. For more information, see [Standalone .NET Aspire dashboard](dashboard/standalone.md).

:::zone pivot="visual-studio"

## Visual Studio tooling

Visual Studio provides additional features for working with .NET Aspire integrations and the App Host orchestrator project. Not all of these features are currently available in Visual Studio Code or through the CLI.

### Add an integration package

You add .NET Aspire integrations to your app like any other NuGet package using Visual Studio. However, Visual Studio also provides UI options to add .NET Aspire integrations directly.

1. In Visual Studio, right select on the project you want to add an .NET Aspire integration to and select **Add** > **.NET Aspire package...**.

    :::image type="content" loc-scope="visual-studio" source="../media/visual-studio-add-aspire-package.png" lightbox="../media/visual-studio-add-aspire-package.png" alt-text="The Visual Studio context menu displaying the Add .NET Aspire Component option.":::

1. The package manager opens with search results preconfigured (populating filter criteria) for .NET Aspire integrations, allowing you to easily browse and select the desired integration.

    :::image type="content" loc-scope="visual-studio" source="../media/visual-studio-add-aspire-comp-nuget.png" lightbox="../media/visual-studio-add-aspire-comp-nuget.png" alt-text="The Visual Studio context menu displaying the Add .NET Aspire integration options.":::

For more information on .NET Aspire integrations, see [.NET Aspire integrations overview](integrations-overview.md).

### Add hosting packages

.NET Aspire hosting packages are used to configure various resources and dependencies an app may depend on or consume. Hosting packages are differentiated from other integration packages in that they're added to the _*.AppHost_ project. To add a hosting package to your app, follow these steps:

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

For more information, see [Testing .NET Aspire projects](testing.md).

:::zone-end
:::zone pivot="vscode"

## Visual Studio Code tooling

You can use Visual Studio Code, with the [C# Dev Kit extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit), to create and develop .NET Aspire projects. To create a new .NET Aspire project in Visual Studio Code, select the **Create .NET Project** button in the **Explorer** view, then select one of the .NET Aspire templates:

:::image type="content" source="media/setup-tooling/vscode-create-project.png" lightbox="media/setup-tooling/vscode-create-project.png" alt-text="A screenshot showing how to create a new .NET Aspire project in Visual Studio Code.":::

Once you create a new .NET Aspire project, you run and debug the app, stepping through breakpoints, and inspecting variables using the Visual Studio Code debugger:

:::image type="content" source="media/setup-tooling/vscode-debugging.png" lightbox="media/setup-tooling/vscode-debugging.png" alt-text="A screenshot showing how to debug a .NET Aspire project in Visual Studio Code.":::

:::zone-end

## See also

- [Unable to install .NET Aspire workload](../troubleshooting/unable-to-install-workload.md)
- [Use Dev Proxy with .NET Aspire project](/microsoft-cloud/dev/dev-proxy/how-to/use-dev-proxy-with-dotnet-aspire)
