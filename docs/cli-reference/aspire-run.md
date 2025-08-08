---
title: aspire run command
description: Learn about the aspire run command and its usage. This command runs an Aspire app host.
ms.date: 07/11/2025
---
# aspire run command

**This article applies to:** ✔️ Aspire CLI 9.4.0 and later versions.

## Name

`aspire run` - Run an Aspire AppHost in development mode.

## Synopsis

```Command
aspire run [options] [[--] <additional arguments>...]
```

## Description

The `aspire run` command runs the AppHost project in development mode, which configures the Aspire environment, builds and starts resources defined by the AppHost, launches the web dashboard, and prints a list of endpoints.

[!INCLUDE [project-search-logic-description](includes/project-search-logic-description.md)]

The command performs the following steps to run an Aspire AppHost:

- Creates or modifies the `.aspire/settings.json` config file in the current directory, and sets the `appHostPath` config value to the path of the AppHost project file.
- Installs or verifies that Aspire's local hosting certificates are installed and trusted.
- Builds the AppHost project and its resources.
- Starts the AppHost and its resources.
- Starts the dashboard.

The following snippet is an example of the output displayed by the `aspire run` command:

```Aspire CLI
Dashboard:  https://localhost:17178/login?t=17f974bf68e390b0d4548af8d7e38b65

    Logs:  /home/vscode/.aspire/cli/logs/apphost-1295-2025-07-14-18-16-13.log
```

## Options

The following options are available:

- **`--`**

  Delimits arguments to `aspire run` from arguments for the AppHost being run. All arguments after this delimiter are passed to the AppHost run.

- [!INCLUDE [option-project](includes/option-project.md)]

- **`-w, --watch`**

  Start project resources in watch mode.

- [!INCLUDE [option-help](includes/option-help.md)]

- [!INCLUDE [option-debug](includes/option-debug.md)]

- [!INCLUDE [option-wait](includes/option-wait.md)]

## Examples

- Search the current directory structure for AppHost projects to build and run:

  ```Command
  aspire run
  ```

- Run a specific AppHost project:

  ```Command
  aspire run --project './projects/apphost/orchestration.AppHost.csproj'
  ```

- Run a specific AppHost project with arguments:

  ```Command
  aspire run --project './projects/apphost/orchestration.AppHost.csproj' -- -fast
  ```
