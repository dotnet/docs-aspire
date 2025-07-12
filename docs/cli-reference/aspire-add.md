---
title: aspire add command
description: Learn about the aspire add command and its usage. This command adds an integration package to an Aspire apphost project.
ms.date: 07/11/2025
---
# aspire add command

**This article applies to:** ✔️ Aspire CLI 9.3.1 and later versions

## Name

`aspire add` - Add an integration to the Aspire project.

## Synopsis

```dotnetcli
aspire add [<integration>] [options]
```

## Description

The `aspire add` command searches NuGet for an integration package and adds it to the apphost project.

[!INCLUDE [project-search-logic-description](includes/project-search-logic-description.md)]

## Arguments

The following arguments are available:

- **`integration`**

  The name of the integration to add (for example: redis, postgres).

  If a partial name or invalid name is provided, the CLI searches NuGet for approximate matches and prints them in the terminal for the user to select. If no results are found, all packages are listed.

## Options

The following options are available:

- [!INCLUDE [option-project](includes/option-project.md)]

- **`-v, --version`**

  The version of the integration to add.

- **`-s, --source`**

  The NuGet source to use for the integration.

- [!INCLUDE [option-help](includes/option-help.md)]

- [!INCLUDE [option-debug](includes/option-debug.md)]

- [!INCLUDE [option-wait](includes/option-wait.md)]

## Examples

- Finds an apphost project and lists all Aspire integration packages from NuGet:

  ```Command
  aspire add
  ```

- Finds an apphost project and adds the **kafka** (Aspire.Hosting.Kafka) integration package:

  ```Command
  aspire add kafka --version 9.3.2
  ```
