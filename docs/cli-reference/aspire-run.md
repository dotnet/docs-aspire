---
title: aspire run command
description: Learn about the aspire run command and its usage. This command runs an Aspire app host.
ms.date: 07/11/2025
---
# aspire run command

**This article applies to:** ✔️ Aspire CLI 9.3.1 and later versions

## Name

`aspire run` - Run an Aspire app host in development mode.

## Synopsis

```dotnetcli
aspire run [options] [[--] <additional arguments>...]]
```

## Description

The `aspire run` command runs the apphost project in development mode, which configures the Aspire environment, builds the apphost, launches the web dashboard, and displays a terminal-based dashboard. This is similar to running the apphost project in your IDE of choice, however, a terminal-based version dashboard is also visible.

[!INCLUDE [project-search-logic-description](includes/project-search-logic-description.md)]

The command performs the following steps to run an apphost project:

- Writes the apphost project path to the `.aspire/settings.json` config file in the current directory.
- Installs or verifies that Aspires local hosting certificates are installed and trusted.
- Builds the apphost project.
- Starts the apphost.
- Starts the dashboard.
- Displays a terminal-based dashboard.

<!-- Add asciinema here instead of the terminal dashboard -->

The following snippet is an example of the terminal dashboard:

```Aspire CLI
Dashboard:
📈  Direct: https://localhost:17077/login?t=64ebc6d760ab2c48df93607fd431cf0b

╭─────────────┬─────────┬─────────┬────────────────────────╮
│ Resource    │ Type    │ State   │ Endpoint(s)            │
├─────────────┼─────────┼─────────┼────────────────────────┤
│ apiservice  │ Project │ Running │ https://localhost:7422 │
│             │         │         │ http://localhost:5327  │
│ webfrontend │ Project │ Running │ https://localhost:7241 │
│             │         │         │ http://localhost:5185  │
╰─────────────┴─────────┴─────────┴────────────────────────╯
Press CTRL-C to stop the apphost and exit.
```

## Options

The following options are available:

- **`--`**

  Delimits arguments to aspire run from arguments for the apphost being run. All arguments after this delimiter are passed to the apphost run.

- [!INCLUDE [option-project](includes/option-project.md)]

- **`-w, --watch`**

  Start project resources in watch mode.

- [!INCLUDE [option-help](includes/option-help.md)]

- [!INCLUDE [option-debug](includes/option-debug.md)]

- [!INCLUDE [option-wait](includes/option-wait.md)]

## Examples

- Search the current directory structure for apphost projects to build and run:

  ```Command
  aspire run
  ```

- Run a specific apphost project:

  ```Command
  aspire run --project './projects/apphost/orchestration.AppHost.csproj'
  ```

- Run a specific apphost project with arguments:

  ```Command
  aspire run --project './projects/apphost/orchestration.AppHost.csproj' -- -fast
  ```
