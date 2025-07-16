---
title: aspire command
description: Learn about the aspire command (the generic driver for the Aspire CLI) and its usage.
ms.date: 07/11/2025
---
# aspire command

**This article applies to:** ✔️ Aspire CLI 9.4.0 and later versions.

## Name

`aspire` - The generic driver for the Aspire CLI.

## Synopsis

To get information about the available commands and the environment:

```dotnetcli
aspire [command] [options]
```

## Description

The `aspire` command provides commands for working with Aspire projects. For example, `aspire run` runs an Aspire AppHost project.

## Options

The following options are available when `aspire` is used by itself, without specifying a command. For example, `aspire --version`.

- [!INCLUDE [option-help](includes/option-help.md)]

- [!INCLUDE [option-version](includes/option-version.md)]

- [!INCLUDE [option-debug](includes/option-debug.md)]

- [!INCLUDE [option-wait](includes/option-wait.md)]

## Commands

| Command                               | Status  | Function                                                                 |
|---------------------------------------|---------|--------------------------------------------------------------------------|
| [`aspire add`](aspire-add.md)         | Stable  | Add an integration to the Aspire project.                                |
| [`aspire new`](aspire-new.md)         | Stable  | Create an Aspire sample project from a template.                         |
| [`aspire run`](aspire-run.md)         | Stable  | Run an Aspire apphost in development mode.                               |
| [`aspire exec`](aspire-exec.md)       | Stable  | Similar to the `aspire run` command, but passes commands to the apphost. |
| [`aspire deploy`](aspire-deploy.md)   | Preview | Deploys the artifacts created by `aspire publish`.                       |
| [`aspire publish`](aspire-publish.md) | Preview | Generates deployment artifacts for an Aspire apphost project.            |

<!-- These commands aren't used yet

| [`aspire config`](#configure-aspire-environment)         | Stable  | Configures the Aspire environment.                                        |
| `aspire init`                                            | Future  | ... |

-->

## Examples

- Create an Aspire solution from the template:

  ```Command
  aspire new aspire-starter
  ```

- Run an Aspire AppHost project:

  ```Command
  aspire run
  ```
