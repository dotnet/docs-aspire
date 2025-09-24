---
title: aspire config delete command
description: Learn about the aspire config delete command and its usage. This command deletes an Aspire CLI config value by key name.
ms.date: 07/11/2025
---
# aspire config delete command

**This article applies to:** ✔️ Aspire CLI 9.4.0 and later versions

## Name

`aspire config delete` - Delete a configuration value.

## Synopsis

```Command
aspire config delete <key> [options]
```

## Description

The `aspire config delete` command deletes a config value by key name.

[!INCLUDE [config-file-description](includes/config-file-description.md)]

The command returns the following exit codes:

- `0`&mdash;The command succeeded.
- `1`&mdash;The supplied key doesn't exist in the config file, or the config file is missing.

## Arguments

The following arguments are available:

- **`key`**

  The configuration key to delete.

## Options

The following options are available:

- **`-g, --global`**

  Delete the configuration value from the global `$HOME/.aspire/settings.json` instead of the local settings file.

- [!INCLUDE [option-help](includes/option-help.md)]

- [!INCLUDE [option-debug](includes/option-debug.md)]

- [!INCLUDE [option-wait](includes/option-wait.md)]

## Available settings

The following config settings are available:

[!INCLUDE [config-settings-table](includes/config-settings-table.md)]
