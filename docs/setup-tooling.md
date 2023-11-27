---
title: .NET Aspire tooling
description: Learn about essential tooling concepts for .NET Aspire.
ms.date: 11/11/2023
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

- [.NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0)
- [.NET Aspire workload](/dotnet/core/tools/dotnet-workload-install)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- Integrated Developer Environment (IDE) or code editor, such as:
  - [Visual Studio 2022 Preview](https://visualstudio.microsoft.com/vs/preview/) version 17.9 or higher (Optional)
  - [Visual Studio Code](https://code.visualstudio.com/) (Optional)

The .NET Aspire workload installs internal dependencies and makes available other tooling, such as project templates and Visual Studio features. There are two ways to install the .NET Aspire workload. If you prefer to use Visual Studio Code, following the .NET CLI instructions:

# [Visual Studio](#tab/visual-studio)

To install the .NET Aspire workload in Visual Studio 2022 Preview, use the Visual Studio installer.

1. Open the Visual Studio Installer.
1. Select **Modify** next to Visual Studio 2022 Preview.
1. Select the **ASP.NET and web development** workload.
1. On the **Installation details** panel, select **.NET Aspire SDK (Preview)**.
1. Select **Modify** to install the .NET Aspire component.

:::image type="content" source="media/install-aspire-workload-visual-studio.png" lightbox="media/install-aspire-workload-visual-studio.png" alt-text="A screenshot showing how to install the .NET Aspire workload with the Visual Studio installer.":::

# [.NET CLI](#tab/dotnet-cli)

To install the .NET Aspire workload from the .NET CLI, use the [dotnet workload install](/dotnet/core/tools/dotnet-workload-install) command:

```dotnetcli
dotnet workload install aspire
```

---

## .NET Aspire project templates

The .NET Aspire workload makes available .NET Aspire project templates. These project templates allow you to create new apps pre-configured with the .NET Aspire project structure and default settings. These projects also provide a unified debugging experience across the different resources of your app.

There are currently two project templates available:

- **.NET Aspire Application**: A minimal .NET Aspire app that includes the following:

  - **AspireSample.AppHost**: An orchestrator project designed to connect and configure the different projects and services of your app.

  - **AspireSample.ServiceDefaults**: A .NET Aspire shared project to manage configurations that are reused across the projects in your solution related to [resilience](/dotnet/core/resilience/http-resilience), [service discovery](./service-discovery/overview.md), and [telemetry](./telemetry.md).

- **.NET Aspire Starter Application**: In addition to the **.AppHost** and **.ServiceDefaults** projects, the .NET Aspire Starter Application also includes the following:

  - **AspireSample.ApiService**: An ASP.NET Core Minimal API project is used to provide data to the frontend. This project depends on the shared **AspireSample.ServiceDefaults** project.

  - **AspireSample.Web**: An ASP.NET Core Blazor App project with default .NET Aspire service configurations, this project depends on the **AspireSample.ServiceDefaults** project.

Use Visual Studio or the .NET CLI to create new apps using these project templates. Explore additional .NET Aspire project templates in the [.NET Aspire samples](https://github.com/dotnet/aspire-samples) repository.

# [Visual Studio](#tab/visual-studio)

To create a .NET Aspire project using Visual Studio, search for *Aspire* in the Visual Studio new project window and select your desired template.

:::image type="content" source="media/aspire-templates.png" lightbox="media/aspire-templates.png" alt-text="The .NET Aspire project templates in VIsual Studio.":::

# [.NET CLI](#tab/dotnet-cli)

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

.NET Aspire project templates also include a useful [dashboard](dashboard.md) that can be used to monitor and inspect various aspects of your app, such as logs, traces, and environment configurations. This dashboard is designed to improve the local development experience and provides an overview of the overall state and structure of your app.

The .NET Aspire dashboard is only visible while the app is running and starts automatically when you start the **.AppHost** project. Visual Studio launches both your app and the .NET Aspire dashboard for you automatically in your browser. If you start the app using the .NET CLI, copy and paste the dashboard URL from the output into your browser.

:::image type="content" source="media/run-dashboard-cli.png" alt-text="A screenshot showing how to launch the dashboard using the CLI.":::

The left navigation provides links to the different parts of the dashboard, each of which you'll explore in the following sections.

:::image type="content" source="media/dashboard/projects.png" lightbox="media/dashboard/projects.png" alt-text="A screenshot of the .NET Aspire dashboard Projects page.":::

## Visual Studio tooling

Visual Studio provides additional features for working with .NET Aspire components and the App Host orchestrator project. These features are currently not available in Visual Studio Code or through the CLI.

### Add a component

You can add .NET Aspire components to your app like any other NuGet package using Visual Studio. However, Visual Studio also provides UI options to add .NET Aspire components directly.

1. In Visual Studio, right click on the project you want to add an .NET Aspire component to and select **Add** > **.NET Aspire Component...**.
1. The package manager will open with search results pre-configured for .NET Aspire components, allowing you to easily browse and select the desired component.

### Add orchestration projects

You can add .NET Aspire orchestration projects to an existing app using the following steps:

1. In Visual Studio, right click on an existing project and select **Add** > **.NET Aspire Orchestrator Support..**.
1. A dialog window will open with a summary of the **.AppHost** and **.ServiceDefaults** projects that will be added to your solution.

    :::image type="content" source="media/add-orchestrator-app.png" alt-text="A screenshot showing the Visual Studio add .NET Aspire orchestration summary.":::

1. Select **OK** and the following changes will be applied:

    - The **.AppHost** and **.ServiceDefault** orchestration projects will be added to your solution.
    - A call to `builder.AddServiceDefaults` will be added to the _Program.cs_ file of your original project.
    - A reference to your original project will be added to the _Program.cs_ file of the **.AppHost** project.

### Enlist in orchestration

Visual Studio provides the option to **Enlist in Aspire orchestration** during the new project workflow. Select this option to have Visual Studio create **.AppHost** and **.ServiceDefault** projects alongside your selected project template.

:::image type="content" source="media/aspire-enlist-orchestration.png" lightbox="media/aspire-enlist-orchestration.png" alt-text="A screenshot showing how to enlist in .NET Aspire orchestration.":::
