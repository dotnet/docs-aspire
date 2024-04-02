---
title: .NET Aspire tooling
description: Learn about essential tooling concepts for .NET Aspire.
ms.date: 04/02/2024
---

# .NET Aspire setup and tooling

.NET Aspire includes tooling to help you create and configure cloud-native apps. The tooling includes useful starter project templates and other features to streamline getting started with .NET Aspire for both Visual Studio and CLI workflows. In the sections ahead, you'll learn how to work with .NET Aspire tooling and explore the following tasks:

> [!div class="checklist"]
>
> - Install .NET Aspire and its dependencies
> - Create starter project templates using Visual Studio or the .NET CLI
> - Install .NET Aspire components
> - Work with the .NET Aspire dashboard

## Install .NET Aspire

To work with .NET Aspire, you'll need the following installed locally:

- [.NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0).
- [.NET Aspire workload](/dotnet/core/tools/dotnet-workload-install).
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) or [Podman](https://podman.io/docs/installation) for container runtime support. For more information, see [Container runtime](#container-runtime).
- Integrated Developer Environment (IDE) or code editor, such as:
  - [Visual Studio 2022 Preview](https://visualstudio.microsoft.com/vs/preview/) version 17.10 or higher (Optional).
  - [Visual Studio Code](https://code.visualstudio.com/) (Optional).

The .NET Aspire workload installs internal dependencies and makes available other tooling, such as project templates and Visual Studio features. There are two ways to install the .NET Aspire workload. If you prefer to use Visual Studio Code, follow the .NET CLI instructions:

# [Visual Studio](#tab/visual-studio)

Visual Studio 2022 17.10 Preview 1 or higher includes the **.NET Aspire preview 4** workload by default. To verify that you have the .NET Aspire workload installed, run the following command:

```dotnetcli
dotnet workload list
```

If you have an earlier version of Visual Studio 2022 Preview, you can either upgrade to Visual Studio 2022 17.10 Preview 1 or you can install the .NET Aspire workload using the following steps:

To install the .NET Aspire workload in Visual Studio 2022 Preview, use the Visual Studio installer.

1. Open the Visual Studio Installer.
1. Select **Modify** next to Visual Studio 2022 Preview.
1. Select the **ASP.NET and web development** workload.
1. On the **Installation details** panel, select **.NET Aspire SDK (Preview)**.
1. Select **Modify** to install the .NET Aspire component.

   :::image type="content" source="../media/install-aspire-workload-visual-studio.png" lightbox="../media/install-aspire-workload-visual-studio.png" alt-text="A screenshot showing how to install the .NET Aspire workload with the Visual Studio installer.":::

1. To ensure that you install the latest version of the .NET Aspire workload, run the following [dotnet workload update](/dotnet/core/tools/dotnet-workload-update) command before you install .NET Aspire:

    ```dotnetcli
    dotnet workload update
    ```

1. To install the .NET Aspire workload from the .NET CLI, use the [dotnet workload install](/dotnet/core/tools/dotnet-workload-install) command:

    ```dotnetcli
    dotnet workload install aspire
    ```

1. To check your version of .NET Aspire, run this command:

    ```dotnetcli
    dotnet workload list
    ```

# [.NET CLI](#tab/dotnet-cli)

To ensure that you install the latest version of the .NET Aspire workload, run the following [dotnet workload update](/dotnet/core/tools/dotnet-workload-update) command before you install .NET Aspire:

```dotnetcli
dotnet workload update
```

To install the .NET Aspire workload from the .NET CLI, use the [dotnet workload install](/dotnet/core/tools/dotnet-workload-install) command:

```dotnetcli
dotnet workload install aspire
```

To check your version of .NET Aspire, run this command:

```dotnetcli
dotnet workload list
```

---

## Container runtime

.NET Aspire apps are designed to run in containers. You can use either Docker Desktop or Podman as your container runtime. [Docker Desktop](https://www.docker.com/products/docker-desktop/) is the most common container runtime. [Podman](https://podman.io/docs/installation) is an open-source daemonless alternative to Docker, that can build and run Open Container Initiative (OCI) containers. If you're host environment has both Docker and Podman installed, .NET Aspire defaults to using Docker. You can instruct .NET Aspire to use Podman instead, by setting the `DOTNET_ASPIRE_CONTAINER_RUNTIME` environment variable to `podman`:

## [Unix](#tab/unix)

```bash
export DOTNET_ASPIRE_CONTAINER_RUNTIME=podman
```

## [Windows](#tab/windows)

```powershell
$env:DOTNET_ASPIRE_CONTAINER_RUNTIME = "podman"
```

---

For more information, see:

- [Install Podman on Windows or macOS](https://podman.io/docs/installation#installing-on-mac--windows).
- [Install Podman on Linux](https://podman.io/docs/installation#installing-on-linux).

## .NET Aspire project templates

> [!NOTE]
> The .NET 9.0 SDK causes issues with the .NET Aspire workloads ability to display templates, and create projects from the templates. If you're using .NET 9.0, know that your ability to create new .NET Aspire apps is currently limited. For more information, see [.NET Aspire preview-3 cannot create app when .NET 9 is installed](https://github.com/dotnet/aspire/issues/1951) and [.NET Aspire preview-3 installed with Visual Studio 17.10 doesn't show templates](https://github.com/dotnet/aspire/issues/2186).

The .NET Aspire workload makes available .NET Aspire project templates. These project templates allow you to create new apps pre-configured with the .NET Aspire project structure and default settings. These projects also provide a unified debugging experience across the different resources of your app.

There are currently four project templates available:

- **.NET Aspire Application**: A minimal .NET Aspire app that includes the following:

  - **AspireSample.AppHost**: An orchestrator project designed to connect and configure the different projects and services of your app.

  - **AspireSample.ServiceDefaults**: A .NET Aspire shared project to manage configurations that are reused across the projects in your solution related to [resilience](/dotnet/core/resilience/http-resilience), [service discovery](../service-discovery/overview.md), and [telemetry](./telemetry.md).

- **.NET Aspire Starter Application**: In addition to the **.AppHost** and **.ServiceDefaults** projects, the .NET Aspire Starter Application also includes the following:

  - **AspireSample.ApiService**: An ASP.NET Core Minimal API project is used to provide data to the frontend. This project depends on the shared **AspireSample.ServiceDefaults** project.

  - **AspireSample.Web**: An ASP.NET Core Blazor App project with default .NET Aspire service configurations, this project depends on the **AspireSample.ServiceDefaults** project.

- **.NET Aspire App Host**: A standalone **.AppHost** project that can be used to orchestrate and manage the different projects and services of your app.

- **.NET Aspire Text Project (xUnit)**:

- **.NET Aspire Service Defaults**: A standalone **.ServiceDefaults** project that can be used to manage configurations that are reused across the projects in your solution related to [resilience](/dotnet/core/resilience/http-resilience), [service discovery](../service-discovery/overview.md), and [telemetry](./telemetry.md).

  > [!IMPORTANT]
  > The service defaults project template takes a `FrameworkReference` dependency on `Microsoft.AspNetCore.App`. This may not be ideal for some project types. For more information, see [.NET Aspire service defaults](service-defaults.md).

Use Visual Studio or the .NET CLI to create new apps using these project templates. Explore additional .NET Aspire project templates in the [.NET Aspire samples](https://github.com/dotnet/aspire-samples) repository.

# [Visual Studio](#tab/visual-studio)

To create a .NET Aspire project using Visual Studio, search for *Aspire* in the Visual Studio new project window and select your desired template.

:::image type="content" source="../media/aspire-templates.png" lightbox="../media/aspire-templates.png" alt-text="The .NET Aspire project templates in Visual Studio.":::

# [.NET CLI](#tab/dotnet-cli)

To see which .NET Aspire project templates are available, use the [dotnet new list](/dotnet/core/tools/dotnet-new-list) command, passing in the search term `aspire`:

```dotnetcli
dotnet new list aspire
```

When the .NET Aspire workload is installed, you'll see the following .NET Aspire templates:

```Output
These templates matched your input: 'aspire'

Template Name                     Short Name              Language  Tags
--------------------------------  ----------------------  --------  -------------------------------------------------------
.NET Aspire App Host              aspire-apphost          [C#]      Common/.NET Aspire/Cloud
.NET Aspire Application           aspire                  [C#]      Common/.NET Aspire/Cloud/Web/Web API/API/Service
.NET Aspire Service Defaults      aspire-servicedefaults  [C#]      Common/.NET Aspire/Cloud/Web/Web API/API/Service
.NET Aspire Starter Application   aspire-starter          [C#]      Common/.NET Aspire/Blazor/Web/Web API/API/Service/Cloud
.NET Aspire Test Project (xUnit)  aspire-xunit            [C#]      Common/.NET Aspire/Cloud/Web/Web API/API/Service/Test
```

To create a .NET Aspire project using the .NET CLI, use the `dotnet new` command and specify which template you would like to create.

To create a basic .NET Aspire project:

```dotnetcli
dotnet new aspire
```

To create a .NET Aspire project with a sample UI and API included:

```dotnetcli
dotnet new aspire-starter
```

---

## .NET Aspire dashboard

.NET Aspire templates that expose the app host project also include a useful [dashboard](dashboard.md) that can be used to monitor and inspect various aspects of your app, such as logs, traces, and environment configurations. This dashboard is designed to improve the local development experience and provides an overview of the overall state and structure of your app.

The .NET Aspire dashboard is only visible while the app is running and starts automatically when you start the **.AppHost** project. Visual Studio launches both your app and the .NET Aspire dashboard for you automatically in your browser. If you start the app using the .NET CLI, copy and paste the dashboard URL from the output into your browser, or hold <kbd>Ctrl</kbd> and select the link (if your terminal supports hyperlinks).

:::image type="content" source="../media/run-dashboard-cli.png" alt-text="A screenshot showing how to launch the dashboard using the CLI.":::

The left navigation provides links to the different parts of the dashboard, each of which you'll explore in the following sections.

:::image type="content" source="media/dashboard/projects.png" lightbox="media/dashboard/projects.png" alt-text="A screenshot of the .NET Aspire dashboard Projects page.":::

## Visual Studio tooling

Visual Studio provides additional features for working with .NET Aspire components and the App Host orchestrator project. These features are currently not available in Visual Studio Code or through the CLI.

### Add a component

You add .NET Aspire components to your app like any other NuGet package using Visual Studio. However, Visual Studio also provides UI options to add .NET Aspire components directly.

1. In Visual Studio, right click on the project you want to add an .NET Aspire component to and select **Add** > **.NET Aspire Component...**.

    :::image type="content" source="../media/visual-studio-add-aspire-component.png" lightbox="../media/visual-studio-add-aspire-component.png" alt-text="The Visual Studio context menu displaying the Add .NET Aspire Component option.":::

1. The package manager will open with search results pre-configured (populating filter criteria) for .NET Aspire components, allowing you to easily browse and select the desired component. The **Include prerelease** checkbox needs to be checked to see preview components.

    :::image type="content" source="../media/visual-studio-add-aspire-comp-nuget.png" lightbox="../media/visual-studio-add-aspire-comp-nuget.png" alt-text="The Visual Studio context menu displaying the Add .NET Aspire component options.":::

For more information on .NET Aspire components, see [.NET Aspire components overview](components-overview.md).

### Add hosting packages

.NET Aspire hosting packages are used to configure various resources and dependencies an app may depend on or consume. Hosting packages are differentiated from other component packages in that they are added to the **.AppHost** project. To add a hosting package to your app, follow these steps:

1. In Visual Studio, right click on the **.AppHost** project and select **Add** > **.NET Aspire package...**.

    :::image type="content" source="../media/visual-studio-add-aspire-hosting-package.png" lightbox="../media/visual-studio-add-aspire-hosting-package.png" alt-text="The Visual Studio context menu displaying the Add .NET Aspire Hosting Resource option.":::

1. The package manager will open with search results pre-configured (populating filter criteria) for .NET Aspire hosting packages, allowing you to easily browse and select the desired package. The **Include prerelease** checkbox needs to be checked to see preview packages.

    :::image type="content" source="../media/visual-studio-add-aspire-hosting-nuget.png" lightbox="../media/visual-studio-add-aspire-hosting-nuget.png" alt-text="The Visual Studio context menu displaying the Add .NET Aspire resource options.":::

### Add orchestration projects

You can add .NET Aspire orchestration projects to an existing app using the following steps:

1. In Visual Studio, right click on an existing project and select **Add** > **.NET Aspire Orchestrator Support..**.

    :::image type="content" source="../media/visual-studio-add-aspire-orchestrator.png" lightbox="../media/visual-studio-add-aspire-orchestrator.png" alt-text="The Visual Studio context menu displaying the Add .NET Aspire Orchestrator Support option.":::

1. A dialog window will open with a summary of the **.AppHost** and **.ServiceDefaults** projects that will be added to your solution.

    :::image type="content" source="../media/add-orchestrator-app.png" alt-text="A screenshot showing the Visual Studio add .NET Aspire orchestration summary.":::

1. Select **OK** and the following changes will be applied:

    - The **.AppHost** and **.ServiceDefault** orchestration projects will be added to your solution.
    - A call to `builder.AddServiceDefaults` will be added to the _Program.cs_ file of your original project.
    - A reference to your original project will be added to the _Program.cs_ file of the **.AppHost** project.

For more information on .NET Aspire orchestration, see [.NET Aspire orchestration overview](app-host-overview.md).

### Enlist in orchestration

Visual Studio provides the option to **Enlist in Aspire orchestration** during the new project workflow. Select this option to have Visual Studio create **.AppHost** and **.ServiceDefault** projects alongside your selected project template.

:::image type="content" source="../media/aspire-enlist-orchestration.png" lightbox="../media/aspire-enlist-orchestration.png" alt-text="A screenshot showing how to enlist in .NET Aspire orchestration.":::
