---
title: aspire config get command
description: Learn about the aspire config get command and its usage. This command gets an Aspire CLI config value by key name.
ms.date: 07/11/2025
---
# aspire config get command

**This article applies to:** ✔️ Aspire CLI 9.4.0 and later versions

## Name

`aspire config get` - Get a configuration value.

## Synopsis

```dotnetcli
aspire config get <key> [options]
```

## Description

The `aspire config get` command retrieves a config value by key name.

[!INCLUDE [config-file-description](includes/config-file-description.md)]

If the config value doesn't exist in a local settings file, the config file is retrieved from the global settings file.

The command returns the following exit codes:

- `0`&mdash;The command succeeded.
- `10`&mdash;The supplied key doesn't exist in the config file, or the config file is missing.

## Arguments

The following arguments are available:

- **`key`**

  The configuration key to retrieve.

## Options

The following options are available:

- [!INCLUDE [option-help](includes/option-help.md)]

- [!INCLUDE [option-debug](includes/option-debug.md)]

- [!INCLUDE [option-wait](includes/option-wait.md)]
