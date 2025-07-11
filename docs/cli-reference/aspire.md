---
title: aspire command
description: Learn about the aspire command (the generic driver for the Aspire CLI) and its usage.
ms.date: 06/26/2025
---
# dotnet command

**This article applies to:** ✔️ Aspire CLI 9.3.1 and later versions

## Name

`aspire` - The generic driver for the Aspire CLI.

## Synopsis

To get information about the available commands and the environment:

```dotnetcli
aspire [command] [options]
```

## Description

The `aspire` command provides commands for working with Aspire projects. For example `aspire run` runs an Aspire AppHost project.

## Options

The following options are available when `aspire` is used by itself, without specifying a command. For example, `aspire --version`.

- **`-?, -h, --help`**

  [!INCLUDE [option-help](includes/option-help.md)]

- [!INCLUDE [option-version](includes/option-version.md)]

- **`-d, --debug`**

  Enable debug logging to the console, which prints out detailed information about what .NET Aspire CLI is doing when a command is run.

- **`--wait-for-debugger`**

  Wait for a debugger to attach before running a command.

## Commands

| Command                               | Status  | Function                                                                 |
|---------------------------------------|---------|--------------------------------------------------------------------------|
| [`aspire add`](aspire-add.md)         | Stable  | Add an integration to the Aspire project.                                |
| [`aspire new`](aspire-new.md)         | Stable  | Create an Aspire sample project from a template.                         |
| [`aspire run`](aspire-run.md)         | Stable  | Run an Aspire apphost in development mode.                               |
| [`aspire exec`](aspire-exec.md)       | Preview | Similar to the `aspire run` command, but passes commands to the apphost. |
| [`aspire deploy`](aspire-deploy.md)   | Preview | Deploys the artifacts created by `aspire publish`.                       |
| [`aspire publish`](aspire-publish.md) | Preview | Generates deployment artifacts for an Aspire apphost project.            |

## Examples

Create an Aspire solution from the template:

```Command
aspire new aspire-starter
```

Run an Aspire app host project:

```Command
aspire run
```

## See also

- [Environment variables used by .NET SDK, .NET CLI, and .NET runtime](dotnet-environment-variables.md)
- [Runtime Configuration Files](https://github.com/dotnet/sdk/blob/main/documentation/specs/runtime-configuration-file.md)
- [.NET runtime configuration settings](../runtime-config/index.md)