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

## Install the Aspire extension

To install the Aspire VS Code extension:

1. Open VS Code.
1. Open the Extensions view by selecting **View** > **Extensions** or pressing <kbd>Ctrl</kbd>+<kbd>Shift</kbd>+<kbd>X</kbd> (Windows/Linux) or <kbd>Cmd</kbd>+<kbd>Shift</kbd>+<kbd>X</kbd> (macOS).
1. Search for "Aspire" in the Extensions marketplace.
1. Select the **Aspire** extension published by **Microsoft**.
1. Select **Install**.

    :::image type="content" source="media/install-aspire-code-extension.png" lightbox="media/install-aspire-code-extension.png" alt-text="A screenshot of VS Code showing how to install the Aspire extension.":::

Alternatively, you can install the extension directly from the [VS Code Marketplace](https://marketplace.visualstudio.com/items?itemName=microsoft-aspire.aspire-vscode).

## Access extension commands

All Aspire extension commands are available from the VS Code Command Palette:

1. Open the Command Palette by pressing <kbd>Ctrl</kbd>+<kbd>Shift</kbd>+<kbd>P</kbd> (Windows/Linux) or <kbd>Cmd</kbd>+<kbd>Shift</kbd>+<kbd>P</kbd> (macOS).
1. Type "Aspire" to filter and display all available Aspire commands.
1. Select the desired command from the list.

All commands are grouped under the **Aspire** category in the Command Palette for easy discovery.

## Available commands

The Aspire VS Code extension provides the following commands:

| Command | Description | Availability |
|---------|-------------|--------------|
| Aspire: New Aspire project | Create a new Aspire apphost or starter app from a template. | Available |
| Aspire: Add an integration | Add a hosting integration (`Aspire.Hosting.*`) to the Aspire apphost. | Available |
| Aspire: Configure launch.json | Adds the default Aspire debugger launch configuration to your workspace's `launch.json`, which will detect and run the apphost in the workspace. | Available |
| Aspire: Manage configuration settings | Manage configuration settings including feature flags. | Available |
| Aspire: Open Aspire terminal | Open an Aspire VS Code terminal for working with Aspire projects. | Available |
| Aspire: Publish deployment artifacts | Generates deployment artifacts for an Aspire apphost. | Preview |
| Aspire: Deploy app | Deploy the contents of an Aspire apphost to its defined deployment targets. | Preview |
| Aspire: Update integrations | Update hosting integrations and Aspire SDK in the apphost. | Preview |

## Create a new Aspire app

To create a new Aspire app using the extension:

1. Open the Command Palette (<kbd>Ctrl</kbd>+<kbd>Shift</kbd>+<kbd>P</kbd> or <kbd>Cmd</kbd>+<kbd>Shift</kbd>+<kbd>P</kbd>).
1. Run the **Aspire: New Aspire project** command.
1. Select the desired template:
1. Specify the project name and location.

    :::image type="content" source="media/code-extension-create-aspire-project.png" lightbox="media/code-extension-create-aspire-project.png" alt-text="A screenshot of VS Code showing how to create a new Aspire solution by using the Aspire extension.":::

The extension creates the project and opens it in VS Code.

## Add an integration to the Aspire solution

Aspire integrations provide pre-configured connections to various cloud services and dependencies. To add an integration:

1. Open the Command Palette.
1. Run the **Aspire: Add an integration** command.
1. Browse or search for the desired integration package.
1. Select the integration to add it to your AppHost project.

The extension adds the appropriate NuGet package reference to your AppHost project.

> [!NOTE]
> The **Add an integration** command adds a hosting integration to the AppHost project. It doesn't add the corresponding client integration to any consuming projects.

## Configure an Aspire solution

The Aspire extension includes several commands that configure the behavior of Aspire and the Aspire CLI during development:

### Configure launch.json for debugging

To run and debug your Aspire application in VS Code, you need to configure the `launch.json` file:

1. Open the Command Palette.
1. Run the **Aspire: Configure launch.json** command.
1. The extension adds a default Aspire debugger configuration to your workspace's `launch.json` file.

The default configuration looks like this:

```json
{
    "type": "aspire",
    "request": "launch",
    "name": "Aspire: Launch Default AppHost",
    "program": "${workspaceFolder}"
}
```

You can customize the `program` field to point to a specific AppHost project file. For example:

```json
{
    "type": "aspire",
    "request": "launch",
    "name": "Aspire: Launch MyAppHost",
    "program": "${workspaceFolder}/MyAppHost/MyAppHost.csproj"
}
```

### Manage configuration settings

The **Aspire: Manage configuration settings** command executes `aspire config` request options and displays the results in the VS Code terminal. Use `get` and `set` commands to configure the Aspire CLI. Use the `list` command to show current configuration values.

## Run an Aspire solution in development mode

To run your Aspire application in development mode:

1. Ensure you have configured `launch.json` as described in the [Configure launch.json for debugging](#configure-launchjson-for-debugging) section.
1. Open the Run and Debug view by selecting **View** > **Run** or pressing <kbd>Ctrl</kbd>+<kbd>Shift</kbd>+<kbd>D</kbd> (Windows/Linux) or <kbd>Cmd</kbd>+<kbd>Shift</kbd>+<kbd>D</kbd> (macOS).
1. Select your Aspire launch configuration from the dropdown.
1. Select the green **Start Debugging** button or press <kbd>F5</kbd>.

The extension builds and starts the AppHost project, launches the Aspire dashboard in your browser, and enables debugging for all resources in your solution.

Alternatively, you can open an Aspire VS Code terminal using `Aspire: Open Aspire terminal` and run `aspire run --start-debug-session` to start a VS Code debug session.

### Run or debug from the editor

When an AppHost project is detected in your workspace, you can also run or debug it directly from the editor. Use the run buttons in the editor title bar when viewing an AppHost file.

## Publish deployment artifacts

> [!IMPORTANT]
> This feature is in **Preview**.

The **Aspire: Publish deployment artifacts** command generates deployment artifacts for your Aspire AppHost. This command serializes resources to disk, allowing them to be consumed by deployment tools.

To publish deployment artifacts:

1. Open the Command Palette.
1. Run the **Aspire: Publish deployment artifacts** command.
1. Select the output location for the generated artifacts.

The command invokes registered publishing callback annotations to generate artifacts such as:

- Bicep files for Azure resources.
- Docker Compose YAML files.
- Kubernetes Helm charts.

For more information about Aspire publishing, see [Aspire publishing and deployment overview](../deployment/overview.md).

## Deploy an Aspire solution

> [!IMPORTANT]
> This feature is in **Preview**.

The **Aspire: Deploy app host** command deploys the contents of an Aspire AppHost to its defined deployment targets.

To deploy an Aspire solution:

1. Open the Command Palette.
1. Run the **Aspire: Deploy app host** command.
1. Follow the prompts to select deployment targets and provide any required configuration.

The command publishes deployment artifacts and then invokes deployment callback annotations to deploy resources to the specified targets.

For more information about Aspire deployment, see [Aspire publishing and deployment overview](../deployment/overview.md).

## Open Aspire terminal

The **Aspire: Open Aspire terminal** command opens a terminal window configured for working with Aspire projects. This terminal provides easy access to Aspire CLI commands and is preconfigured with the appropriate environment variables.

## Feedback and issues

To report issues or request features for the Aspire VS Code extension:

1. Visit the [Aspire GitHub repository](https://github.com/dotnet/aspire/issues).
1. Create a new issue and add the `area-extension` label.

## See also

- [Aspire setup and tooling](setup-tooling.md)
- [Aspire CLI Overview](../cli/overview.md)
- [Aspire templates](aspire-sdk-templates.md)
- [Aspire VS Code extension on the marketplace](https://marketplace.visualstudio.com/items?itemName=microsoft-aspire.aspire-vscode)
