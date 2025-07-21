---
title: aspire exec command
description: Learn about the aspire exec command and its usage. This command builds and runs an Aspire AppHost project, then sends commands to a resource.
ms.date: 07/11/2025
---
# aspire exec command

**This article applies to:** ✔️ Aspire CLI 9.4.0 and later versions.

## Name

`aspire exec` - Run an Aspire app host to execute a command against the resource.

[!INCLUDE [mode-preview](includes/mode-preview.md)]

## Synopsis

```Command
aspire exec [options] [[--] <additional arguments>...]
```

## Description

The `aspire exec` command runs a command in the context of one of the resources defined in the AppHost.

You must specify either the `--resource` or the `--start-resource` option, and you must provide parameters with the `--` option.

[!INCLUDE [project-search-logic-description](includes/project-search-logic-description.md)]

## Options

The following options are available:

- **`--`**

  Delimits arguments to `aspire exec` from arguments for the resource. All arguments after this delimiter are passed to the resource.

- [!INCLUDE [option-project](includes/option-project.md)]

- **`-r, --resource`**

  The name of the target resource to execute the command against.

- **`-s, --start-resource`**

  The name of the target resource to start and execute the command against.

- [!INCLUDE [option-help](includes/option-help.md)]

- [!INCLUDE [option-debug](includes/option-debug.md)]

- [!INCLUDE [option-wait](includes/option-wait.md)]

## Examples

- Builds and runs the AppHost project, then sends the command `migrate` to the `database1` resource:

  ```Command
  aspire exec --resource database1 -- migrate
  ```
