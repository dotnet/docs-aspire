---
title: Aspire Visual Studio Code extension
description: Learn how to use the Aspire Visual Studio Code extension to create, configure, run, and deploy Aspire solutions.
ms.date: 10/20/2025
ms.topic: overview
uid: dotnet/aspire/vscode-extension
ai-usage: ai-assisted
---

# Aspire Visual Studio Code extension

The Aspire Visual Studio Code extension provides a set of commands and tools to streamline your work with Aspire within Visual Studio Code. The extension includes commands to create projects, add integrations, configure solutions, and manage deployments. The extension requires the Aspire CLI and provides similar functionality on the Visual Studio Code command palette.

[!INCLUDE [aspire-prereqs](../includes/aspire-prereqs.md)]

In addition, before you can use the Aspire Visual Studio Code extension, you must have the [Aspire CLI](../cli/install.md) installed and available on your PATH.

### Optional extensions

Some features require additional VS Code extensions:

| Feature | Required Extension | Notes |
|---------|-------------------|-------|
| Debug C# projects | [C# Dev Kit](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit) or [C# for Visual Studio Code](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp) | The C# extension is required for debugging .NET projects. Apphosts are built in VS Code if C# Dev Kit is available. |
| Debug Python projects | [Python extension](https://marketplace.visualstudio.com/items?itemName=ms-python.python) | Required for debugging Python projects hosted in Aspire |

## Install the Aspire extension

To install the Aspire Visual Studio Code extension:

1. Open Visual Studio Code.
1. Open the Extensions view by selecting **View** > **Extensions** or pressing <kbd>Ctrl</kbd>+<kbd>Shift</kbd>+<kbd>X</kbd> (Windows/Linux) or <kbd>Cmd</kbd>+<kbd>Shift</kbd>+<kbd>X</kbd> (macOS).
1. Search for "Aspire" in the Extensions marketplace.
1. Select the **Aspire** extension published by **microsoft-aspire**.
1. Select **Install**.

Alternatively, you can install the extension directly from the [Visual Studio Code Marketplace](https://marketplace.visualstudio.com/items?itemName=microsoft-aspire.aspire-vscode).

## Access extension commands

All Aspire extension commands are available from the Visual Studio Code Command Palette:

1. Open the Command Palette by pressing <kbd>Ctrl</kbd>+<kbd>Shift</kbd>+<kbd>P</kbd> (Windows/Linux) or <kbd>Cmd</kbd>+<kbd>Shift</kbd>+<kbd>P</kbd> (macOS).
1. Type "Aspire" to filter and display all available Aspire commands.
1. Select the desired command from the list.

All commands are grouped under the **Aspire** category in the Command Palette for easy discovery.

## Available commands

The Aspire Visual Studio Code extension provides the following commands:

| Command | Description | Status |
|---------|-------------|--------|
| **Aspire: New Aspire project** | Create a new Aspire apphost or starter app from a template | Available |
| **Aspire: Add an integration** | Add a hosting integration package (`Aspire.Hosting.*`) to the Aspire apphost | Available |
| **Aspire: Configure launch.json** | Add the default Aspire debugger launch configuration to your workspace's `launch.json` file | Available |
| **Aspire: Manage configuration settings** | Manage Aspire configuration settings including feature flags | Available |
| **Aspire: Open Aspire terminal** | Open an Aspire terminal for working with Aspire projects using CLI commands | Available |
| **Aspire: Publish deployment artifacts** | Generate deployment artifacts for an Aspire apphost | Preview |
| **Aspire: Deploy app** | Deploy the contents of an Aspire apphost to its defined deployment targets | Preview |
| **Aspire: Update integrations** | Update hosting integrations and Aspire SDK in the apphost | Preview |

## Create a new Aspire solution

To create a new Aspire solution using the extension:

1. Open the Command Palette (<kbd>Ctrl</kbd>+<kbd>Shift</kbd>+<kbd>P</kbd> or <kbd>Cmd</kbd>+<kbd>Shift</kbd>+<kbd>P</kbd>).
1. Run the **Aspire: New Aspire project** command.
1. Select the desired template:
   - **Aspire Empty App**: Creates a minimal Aspire project with an AppHost and ServiceDefaults project.
   - **Aspire Starter App**: Creates a full solution with a sample UI and backing API included.
1. Specify the project name and location.
1. The extension creates the project and opens it in Visual Studio Code.

## Add an integration to the Aspire solution

Aspire integrations provide pre-configured connections to various cloud services and dependencies. To add an integration:

1. Open the Command Palette.
1. Run the **Aspire: Add an integration** command.
1. Browse or search for the desired integration package.
1. Select the integration to add it to your AppHost project.

The extension adds the appropriate NuGet package reference to your AppHost project.

## Configure an Aspire solution

### Configure launch.json for debugging

To run and debug your Aspire application in Visual Studio Code, you need to configure the `launch.json` file:

1. Open the Command Palette.
1. Run the **Aspire: Configure launch.json** command.
1. The extension adds a default Aspire debugger configuration to your workspace's `launch.json` file.

The default configuration looks like this:

```json
{
    "type": "aspire",
    "request": "launch",
    "name": "Aspire: Launch AppHost",
    "program": "${workspaceFolder}"
}
```

You can customize the `program` field to point to a specific AppHost project file:

```json
{
    "type": "aspire",
    "request": "launch",
    "name": "Aspire: Launch MyAppHost",
    "program": "${workspaceFolder}/MyAppHost/MyAppHost.csproj"
}
```

### Customize debugger attributes for resources

The `debuggers` property in the launch configuration allows you to specify common debug configuration properties for different types of Aspire services:

| Language | Debugger entry |
|----------|---------------|
| C# | `project` |
| Python | `python` |
| AppHost | `apphost` |

For example, to customize debugging properties for C# projects and the AppHost:

```json
{
    "type": "aspire",
    "request": "launch",
    "name": "Aspire: Launch MyAppHost",
    "program": "${workspaceFolder}/MyAppHost/MyAppHost.csproj",
    "debuggers": {
        "project": {
            "console": "integratedTerminal",
            "logging": {
                "moduleLoad": false
            }
        },
        "apphost": {
            "stopAtEntry": true
        }
    }
}
```

### Manage configuration settings

Use the **Aspire: Manage configuration settings** command to manage Aspire CLI configuration settings and feature flags. This command provides an interface to toggle features on or off and adjust CLI behavior.

## Run an Aspire solution in development mode

To run your Aspire application in development mode:

1. Ensure you have configured `launch.json` as described in the [Configure launch.json for debugging](#configure-launchjson-for-debugging) section.
1. Open the Run and Debug view by selecting **View** > **Run** or pressing <kbd>Ctrl</kbd>+<kbd>Shift</kbd>+<kbd>D</kbd> (Windows/Linux) or <kbd>Cmd</kbd>+<kbd>Shift</kbd>+<kbd>D</kbd> (macOS).
1. Select your Aspire launch configuration from the dropdown.
1. Select the green **Start Debugging** button or press <kbd>F5</kbd>.

The extension builds and starts the AppHost project, launches the Aspire dashboard in your browser, and enables debugging for all resources in your solution.

### Run or debug from the editor

When an AppHost project is detected in your workspace, you can also run or debug directly from the editor:

- Right-select on a file named `AppHost.cs` in the Explorer view and select **Aspire: Run AppHost** or **Aspire: Debug AppHost**.
- Use the run buttons in the editor title bar when viewing an AppHost file.

## Publish deployment artifacts

> [!IMPORTANT]
> This feature is in **Preview**.

The **Aspire: Publish deployment artifacts** command generates deployment artifacts for your Aspire apphost. This command serializes resources to disk, allowing them to be consumed by deployment tools.

To publish deployment artifacts:

1. Open the Command Palette.
1. Run the **Aspire: Publish deployment artifacts** command.
1. Select the output location for the generated artifacts.

The command invokes registered publishing callback annotations to generate artifacts such as:

- Bicep files for Azure resources
- Docker Compose YAML files
- Kubernetes Helm charts

## Deploy an Aspire solution

> [!IMPORTANT]
> This feature is in **Preview**.

The **Aspire: Deploy app** command deploys the contents of an Aspire apphost to its defined deployment targets.

To deploy an Aspire solution:

1. Open the Command Palette.
1. Run the **Aspire: Deploy app** command.
1. Follow the prompts to select deployment targets and provide any required configuration.

The command publishes deployment artifacts and then invokes deployment callback annotations to deploy resources to the specified targets.

## Open Aspire terminal

The **Aspire: Open Aspire terminal** command opens a terminal window configured for working with Aspire projects. This terminal provides easy access to Aspire CLI commands and is preconfigured with the appropriate environment variables.

## Feedback and issues

To report issues or request features for the Aspire Visual Studio Code extension:

1. Visit the [Aspire GitHub repository](https://github.com/dotnet/aspire/issues).
1. Create a new issue and add the `area-extension` label.

## See also

- [Aspire setup and tooling](setup-tooling.md)
- [Aspire CLI Overview](../cli/overview.md)
- [Aspire templates](aspire-sdk-templates.md)
- [Aspire Visual Studio Code extension on the marketplace](https://marketplace.visualstudio.com/items?itemName=microsoft-aspire.aspire-vscode)
