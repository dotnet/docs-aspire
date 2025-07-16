---
title: aspire run command
description: Learn about the aspire run command and its usage. This command runs an Aspire app host.
ms.date: 07/11/2025
---
# aspire run command

**This article applies to:** ✔️ Aspire CLI 9.4.0 and later versions.

## Name

`aspire run` - Run an Aspire app host in development mode.

## Synopsis

```dotnetcli
aspire run [options] [[--] <additional arguments>...]
```

## Description

The `aspire run` command runs the apphost project in development mode, which configures the Aspire environment, builds the apphost, launches the web dashboard, and prints a list of endpoints.

[!INCLUDE [project-search-logic-description](includes/project-search-logic-description.md)]

The command performs the following steps to run an apphost project:

- Writes the apphost project path to the `.aspire/settings.json` config file in the current directory.
- Installs or verifies that Aspires local hosting certificates are installed and trusted.
- Builds the apphost project.
- Starts the apphost and any services defined in the apphost.
- Starts the dashboard.

The following snippet is an example of the output displayed by the `aspire run` command:

```Aspire CLI
Dashboard:  https://localhost:17178/login?t=17f974bf68e390b0d4548af8d7e38b65                                         
            https://literate-eureka-55x5r74pwxv2vx7p-17178.app.github.dev/login?t=17f974bf68e390b0d4548af8d7e38b65   
                                                                                                                    
    Logs:  /home/vscode/.aspire/cli/logs/apphost-1295-2025-07-14-18-16-13.log                                       
                                                              
Endpoints:  webfrontend has endpoint https://localhost:7294   
            webfrontend has endpoint http://localhost:5131   
            apiservice has endpoint https://localhost:7531   
            apiservice has endpoint http://localhost:5573   
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
