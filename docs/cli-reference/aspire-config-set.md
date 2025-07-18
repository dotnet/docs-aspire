---
title: aspire config set command
description: Learn about the aspire config set command and its usage. This command sets an Aspire CLI config value by key name.
ms.date: 07/11/2025
---
# aspire config set command

**This article applies to:** ✔️ Aspire CLI 9.4.0 and later versions

## Name

`aspire config set` - Set a configuration value.

## Synopsis

```dotnetcli
aspire config set <key> <value> [options]
```

## Description

The `aspire config set` command sets a config value by key name.

[!INCLUDE [config-file-description](includes/config-file-description.md)]

## Arguments

The following arguments are available:

- **`key`**

  The configuration key to set.

- **`value`**

  The configuration value to set.

## Options

The following options are available:

- **`-g, --global`**

  Set the configuration value globally in `$HOME/.aspire/settings.json` instead of the local settings file.

- [!INCLUDE [option-help](includes/option-help.md)]

- [!INCLUDE [option-debug](includes/option-debug.md)]

- [!INCLUDE [option-wait](includes/option-wait.md)]
