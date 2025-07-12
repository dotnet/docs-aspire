---
title: aspire new command
description: Learn about the aspire new command and its usage. This command creates new Aspire projects or solutions.
ms.date: 07/11/2025
---
# aspire new command

**This article applies to:** ✔️ Aspire CLI 9.3.1 and later versions

## Name

`aspire new` - Create a new Aspire project or solution.

## Synopsis

```dotnetcli
aspire new [command] [options]
```

## Description

The `aspire new` command is the driver for creating Aspire projects, apps, or solutions, based on the Aspire templates. Each command specifies the template to use, and the options for the driver specify the options for the template.

This command defaults to **interactive** mode. By issuing the command without any options, the command prompts you for the project template and version, name, and output folder. input to fill out the options. When the `--name`, `--output`, and `--version` options are provided, the command runs **non-interactive** and generates files based on the command template.

## Options

The following options are available:

- **`-n, --name`**

  The name of the project to create.

- **`-o, --output`**

  The output path for the project.

- **`-s, --source`**

  The NuGet source to use for the project templates.

- **`-v, --version`**

  The version of the project templates to use.

- [!INCLUDE [option-help](includes/option-help.md)]

- [!INCLUDE [option-debug](includes/option-debug.md)]

- [!INCLUDE [option-wait](includes/option-wait.md)]

## Commands

Each command represents a template. Pass the `--help` parameter to the template command to print the options available to the template.

| Command                  | Template                     |
|--------------------------|------------------------------|
| `aspire`                 | Aspire Empty App             |
| `aspire-apphost`         | Aspire App Host              |
| `aspire-mstest`          | Aspire Test Project (MSTest) |
| `aspire-nunit`           | Aspire Test Project (NUnit)  |
| `aspire-servicedefaults` | Aspire Service Defaults      |
| `aspire-starter`         | Aspire Starter App           |
| `aspire-xunit`           | Aspire Test Project (xUnit)  |

## Examples

- Create an Aspire solution from the template. Because the template was selected (`aspire-starter`), you're prompted for the name, output folder, and template version.

  ```Command
  aspire new aspire-starter
  ```

- Create an AppHost project named `aspireapp` from the **9.3.1** templates and place the output in a folder named `aspire1`.

  ```Command
  aspire new aspire-apphost --version 9.3.1 --name aspireapp1 --output ./aspire1
  ```
