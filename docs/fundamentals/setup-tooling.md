---
title: .NET Aspire tooling
description: Learn about essential tooling concepts for .NET Aspire.
ms.date: 07/26/2024
zone_pivot_groups: dev-environment
---

# .NET Aspire setup and tooling

.NET Aspire includes tooling to help you create and configure cloud-native apps. The tooling includes useful starter project templates and other features to streamline getting started with .NET Aspire for Visual Studio, Visual Studio Code, and CLI workflows. In the sections ahead, you'll learn how to work with .NET Aspire tooling and explore the following tasks:

> [!div class="checklist"]
>
> - Install .NET Aspire and its dependencies
> - Create starter project templates using Visual Studio, Visual Studio Code, or the .NET CLI
> - Install .NET Aspire integrations
> - Work with the .NET Aspire dashboard

## Install .NET Aspire

To work with .NET Aspire, you'll need the following installed locally:

- [.NET 8.0](https://dotnet.microsoft.com/download/dotnet/8.0).
- .NET Aspire workload (installed either Visual Studio or the .NET CLI).
- An OCI compliant container runtime, such as:
  - [Docker Desktop](https://www.docker.com/products/docker-desktop) or [Podman](https://podman.io/). For more information, see [Container runtime](#container-runtime).
- An Integrated Developer Environment (IDE) or code editor, such as:
  - [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) version 17.10 or higher (Optional)
  - [Visual Studio Code](https://code.visualstudio.com/) (Optional)
    - [C# Dev Kit: Extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) (Optional)

The .NET Aspire workload installs internal dependencies and makes available other tooling, such as project templates and Visual Studio features. There are two ways to install the .NET Aspire workload. If you prefer to use Visual Studio Code, follow the .NET CLI instructions:

:::zone pivot="visual-studio"

Visual Studio 2022 17.10 or higher includes the latest **.NET Aspire** workload by default. To verify that you have the .NET Aspire workload installed, run the following command:

```dotnetcli
dotnet workload list
```

If you have an earlier version of Visual Studio 2022, you can either upgrade to Visual Studio 2022 17.10 or you can install the .NET Aspire workload using the following steps:

To install the .NET Aspire workload in Visual Studio 2022, use the Visual Studio installer.

1. Open the Visual Studio Installer.
1. Select **Modify** next to Visual Studio 2022.
1. Select the **ASP.NET and web development** workload.
1. On the **Installation details** panel, select **.NET Aspire SDK (Preview)**.
1. Select **Modify** to install the .NET Aspire integration.

   :::image type="content" loc-scope="visual-studio" source="../media/install-aspire-workload-visual-studio.png" lightbox="../media/install-aspire-workload-visual-studio.png" alt-text="A screenshot showing how to install the .NET Aspire workload with the Visual Studio installer.":::

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

:::zone-end
:::zone pivot="vscode,dotnet-cli"

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
$env:DOTNET_ASPIRE_CONTAINER_RUNTIME = "podman"
```

For more information, see [Install Podman on Windows](https://podman.io/docs/installation#installing-on-mac--windows).

---

## .NET Aspire project templates

The .NET Aspire workload makes available .NET Aspire project templates. These project templates allow you to create new apps pre-configured with the .NET Aspire project structure and default settings. These projects also provide a unified debugging experience across the different resources of your app.

There are currently four project templates available:

- **.NET Aspire Empty App**: A minimal .NET Aspire project that includes the following:

  - **AspireSample.AppHost**: An orchestrator project designed to connect and configure the different projects and services of your app.

  - **AspireSample.ServiceDefaults**: A .NET Aspire shared project to manage configurations that are reused across the projects in your solution related to [resilience](/dotnet/core/resilience/http-resilience), [service discovery](../service-discovery/overview.md), and [telemetry](./telemetry.md).

- **.NET Aspire Starter Application**: In addition to the **.AppHost** and **.ServiceDefaults** projects, the .NET Aspire Starter Application also includes the following—assuming the solution is named _AspireSample_:

  - **AspireSample.ApiService**: An ASP.NET Core Minimal API project is used to provide data to the frontend. This project depends on the shared **AspireSample.ServiceDefaults** project.

  - **AspireSample.Web**: An ASP.NET Core Blazor App project with default .NET Aspire service configurations, this project depends on the **AspireSample.ServiceDefaults** project.

  - **AspireSample.Test**: Either an MSTest, NUnit, or xUnit test project with project references to the **AspireSample.AppHost** and an example _WebTests.cs_ file demonstrating an integration test.

- **.NET Aspire App Host**: A standalone **.AppHost** project that can be used to orchestrate and manage the different projects and services of your app.

- **.NET Aspire Test projects**: These project templates are used to create test projects for your .NET Aspire app, and they're intended to represent functional and integration tests. The test projects include the following templates:

  - **MSTest**: A project that contains MSTest integration of a .NET Aspire AppHost project.

  - **NUnit**: A project that contains NUnit integration of a .NET Aspire AppHost project.

  - **xUnit**: A project that contains xUnit.net integration of a .NET Aspire AppHost project.

- **.NET Aspire Service Defaults**: A standalone **.ServiceDefaults** project that can be used to manage configurations that are reused across the projects in your solution related to [resilience](/dotnet/core/resilience/http-resilience), [service discovery](../service-discovery/overview.md), and [telemetry](./telemetry.md).

  > [!IMPORTANT]
  > The service defaults project template takes a `FrameworkReference` dependency on `Microsoft.AspNetCore.App`. This may not be ideal for some project types. For more information, see [.NET Aspire service defaults](service-defaults.md).

Use Visual Studio, Visual Studio Code, or the .NET CLI to create new apps using these project templates. Explore additional .NET Aspire project templates in the [.NET Aspire samples](https://github.com/dotnet/aspire-samples) repository.

:::zone pivot="visual-studio"

To create a .NET Aspire project using Visual Studio, search for *Aspire* in the Visual Studio new project window and select your desired template.

:::image type="content" loc-scope="visual-studio" source="../media/aspire-templates.png" lightbox="../media/aspire-templates.png" alt-text="The .NET Aspire project templates in Visual Studio.":::

:::zone-end
:::zone pivot="vscode,dotnet-cli"

To see which .NET Aspire project templates are available, use the [dotnet new list](/dotnet/core/tools/dotnet-new-list) command, passing in the search term `aspire`:

```dotnetcli
dotnet new list aspire
```

When the .NET Aspire workload is installed, you'll see the following .NET Aspire templates:

```Output
These templates matched your input: 'aspire'

Template Name                      Short Name              Language  Tags
---------------------------------  ----------------------  --------  -------------------------------------------------------
.NET Aspire App Host               aspire-apphost          [C#]      Common/.NET Aspire/Cloud
.NET Aspire Empty App              aspire                  [C#]      Common/.NET Aspire/Cloud/Web/Web API/API/Service
.NET Aspire Service Defaults       aspire-servicedefaults  [C#]      Common/.NET Aspire/Cloud/Web/Web API/API/Service
.NET Aspire Starter App            aspire-starter          [C#]      Common/.NET Aspire/Blazor/Web/Web API/API/Service/Cloud
.NET Aspire Test Project (MSTest)  aspire-mstest           [C#]      Common/.NET Aspire/Cloud/Web/Web API/API/Service/Test
.NET Aspire Test Project (NUnit)   aspire-nunit            [C#]      Common/.NET Aspire/Cloud/Web/Web API/API/Service/Test
.NET Aspire Test Project (xUnit)   aspire-xunit            [C#]      Common/.NET Aspire/Cloud/Web/Web API/API/Service/Test
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

:::zone-end

## .NET Aspire dashboard

.NET Aspire templates that expose the app host project also include a useful [dashboard](dashboard/overview.md) that can be used to monitor and inspect various aspects of your app, such as logs, traces, and environment configurations. This dashboard is designed to improve the local development experience and provides an overview of the overall state and structure of your app.

The .NET Aspire dashboard is only visible while the app is running and starts automatically when you start the **.AppHost** project. Visual Studio launches both your app and the .NET Aspire dashboard for you automatically in your browser. If you start the app using the .NET CLI, copy and paste the dashboard URL from the output into your browser, or hold <kbd>Ctrl</kbd> and select the link (if your terminal supports hyperlinks).

:::image type="content" source="dashboard/media/explore/dotnet-run-login-url.png" lightbox="dashboard/media/explore/dotnet-run-login-url.png" alt-text="A screenshot showing how to launch the dashboard using the CLI.":::

The left navigation provides links to the different parts of the dashboard, each of which you'll explore in the following sections.

:::image type="content" source="../get-started/media/aspire-dashboard.png" lightbox="../get-started/media/aspire-dashboard.png" alt-text="A screenshot of the .NET Aspire dashboard Projects page.":::

:::zone pivot="visual-studio"

## Visual Studio tooling

Visual Studio provides additional features for working with .NET Aspire integrations and the App Host orchestrator project. Not all of these features are currently available in Visual Studio Code or through the CLI.

### Add a integration package

You add .NET Aspire integrations to your app like any other NuGet package using Visual Studio. However, Visual Studio also provides UI options to add .NET Aspire integrations directly.

1. In Visual Studio, right click on the project you want to add an .NET Aspire integration to and select **Add** > **.NET Aspire package...**.

    :::image type="content" loc-scope="visual-studio" source="../media/visual-studio-add-aspire-package.png" lightbox="../media/visual-studio-add-aspire-package.png" alt-text="The Visual Studio context menu displaying the Add .NET Aspire Component option.":::

1. The package manager will open with search results pre-configured (populating filter criteria) for .NET Aspire integrations, allowing you to easily browse and select the desired integration.

    :::image type="content" loc-scope="visual-studio" source="../media/visual-studio-add-aspire-comp-nuget.png" lightbox="../media/visual-studio-add-aspire-comp-nuget.png" alt-text="The Visual Studio context menu displaying the Add .NET Aspire integration options.":::

For more information on .NET Aspire integrations, see [.NET Aspire integrations overview](integrations-overview.md).

### Add hosting packages

.NET Aspire hosting packages are used to configure various resources and dependencies an app may depend on or consume. Hosting packages are differentiated from other integration packages in that they are added to the **.AppHost** project. To add a hosting package to your app, follow these steps:

1. In Visual Studio, right click on the **.AppHost** project and select **Add** > **.NET Aspire package...**.

    :::image type="content" loc-scope="visual-studio" source="../media/visual-studio-add-aspire-hosting-package.png" lightbox="../media/visual-studio-add-aspire-hosting-package.png" alt-text="The Visual Studio context menu displaying the Add .NET Aspire Hosting Resource option.":::

1. The package manager will open with search results pre-configured (populating filter criteria) for .NET Aspire hosting packages, allowing you to easily browse and select the desired package.

    :::image type="content" loc-scope="visual-studio" source="../media/visual-studio-add-aspire-hosting-nuget.png" lightbox="../media/visual-studio-add-aspire-hosting-nuget.png" alt-text="The Visual Studio context menu displaying the Add .NET Aspire resource options.":::

### Add orchestration projects

You can add .NET Aspire orchestration projects to an existing app using the following steps:

1. In Visual Studio, right click on an existing project and select **Add** > **.NET Aspire Orchestrator Support..**.

    :::image type="content" loc-scope="visual-studio" source="../media/visual-studio-add-aspire-orchestrator.png" lightbox="../media/visual-studio-add-aspire-orchestrator.png" alt-text="The Visual Studio context menu displaying the Add .NET Aspire Orchestrator Support option.":::

1. A dialog window will open with a summary of the **.AppHost** and **.ServiceDefaults** projects that will be added to your solution.

    :::image type="content" loc-scope="visual-studio" source="../media/add-orchestrator-app.png" alt-text="A screenshot showing the Visual Studio add .NET Aspire orchestration summary.":::

1. Select **OK** and the following changes will be applied:

    - The **.AppHost** and **.ServiceDefault** orchestration projects will be added to your solution.
    - A call to `builder.AddServiceDefaults` will be added to the _:::no-loc text="Program.cs":::_ file of your original project.
    - A reference to your original project will be added to the _:::no-loc text="Program.cs":::_ file of the **.AppHost** project.

For more information on .NET Aspire orchestration, see [.NET Aspire orchestration overview](app-host-overview.md).

### Enlist in orchestration

Visual Studio provides the option to **Enlist in Aspire orchestration** during the new project workflow. Select this option to have Visual Studio create **.AppHost** and **.ServiceDefault** projects alongside your selected project template.

:::image type="content" loc-scope="visual-studio" source="../media/aspire-enlist-orchestration.png" lightbox="../media/aspire-enlist-orchestration.png" alt-text="A screenshot showing how to enlist in .NET Aspire orchestration.":::

### Create test project

When you're using Visual Studio, and you select the **.NET Aspire Start Application** template, you have the option to include a test project. This test project is an xUnit project that includes a sample test that you can use as a starting point for your tests.

:::image type="content" source="media/setup-tooling/create-test-projects-template.png" lightbox="media/setup-tooling/create-test-projects-template.png" alt-text="A screenshot of Visual Studio displaying the option to create a test project.":::

For more information, see [Testing .NET Aspire projects](testing.md).

:::zone-end
:::zone pivot="vscode"

## Visual Studio Code tooling

You can use Visual Studio Code, in conjunction with the [C# Dev Kit extension](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit), to create and develop .NET Aspire projects. To create a new .NET Aspire project in Visual Studio Code, select the **Create .NET Project** button in the **Explorer** view, then select one of the .NET Aspire templates:

:::image type="content" source="media/setup-tooling/vscode-create-project.png" lightbox="media/setup-tooling/vscode-create-project.png" alt-text="A screenshot showing how to create a new .NET Aspire project in Visual Studio Code.":::

Once you've created a new .NET Aspire project, you run and debug the app, stepping through breakpoints, and inspecting variables using the Visual Studio Code debugger:

:::image type="content" source="media/setup-tooling/vscode-debugging.png" lightbox="media/setup-tooling/vscode-debugging.png" alt-text="A screenshot showing how to debug a .NET Aspire project in Visual Studio Code.":::

:::zone-end

## See also

- [Unable to install .NET Aspire workload](../troubleshooting/unable-to-install-workload.md)
- [Use Dev Proxy with .NET Aspire project](/microsoft-cloud/dev/dev-proxy/how-to/use-dev-proxy-with-dotnet-aspire)
