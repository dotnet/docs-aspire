---
title: aspire config command
description: Learn about the aspire config command and its usage. This command driver is used to manage the Aspire CLI config settings.
ms.date: 07/24/2025
---
# aspire config command

**This article applies to:** ✔️ Aspire CLI 9.4.0 and later versions

## Name

`aspire config` - Manage configuration settings.

## Synopsis

```Command
aspire config [command] [options]
```

## Description

The `aspire config` command driver provides commands to manage the Aspire CLI config settings.

[!INCLUDE [config-file-description](includes/config-file-description.md)]

## Options

The following options are available:

- [!INCLUDE [option-help](includes/option-help.md)]

- [!INCLUDE [option-debug](includes/option-debug.md)]

- [!INCLUDE [option-wait](includes/option-wait.md)]

## Commands

The following commands are available:

| Command                                                   | Status | Function                       |
|-----------------------------------------------------------|--------|--------------------------------|
| [`aspire config list`](aspire-config-list.md)             | Stable | List all configuration values. |
| [`aspire config get <key>`](aspire-config-get.md)         | Stable | Get a configuration value.     |
| [`aspire config set <key> <value>`](aspire-config-set.md) | Stable | Set a configuration value.     |
| [`aspire config delete <key>`](aspire-config-delete.md)   | Stable | Delete a configuration value.  |

## Settings

The following config settings are available:

[!INCLUDE [config-settings-table](includes/config-settings-table.md)]
