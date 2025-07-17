---
title: aspire config command
description: Learn about the aspire config command and its usage. This command driver is used to manage the Aspire CLI config settings.
ms.date: 07/11/2025
---
# aspire config command

**This article applies to:** ✔️ Aspire CLI 9.4.0 and later versions

## Name

`aspire config` - Manage configuration settings.

## Synopsis

```dotnetcli
aspire config [command] [options]
```

## Description

The `aspire config` command driver provides commands to manage the Aspire CLI config settings.

## Options

The following options are available:

- [!INCLUDE [option-help](includes/option-help.md)]

- [!INCLUDE [option-debug](includes/option-debug.md)]

- [!INCLUDE [option-wait](includes/option-wait.md)]

## Commands

The following commands are available:

| Command                                            | Status | Function                       |
|----------------------------------------------------|--------|--------------------------------|
| [`aspire config get <key>`](aspire-add.md)         | Stable | Get a configuration value.     |
| [`aspire config set <key> <value>`](aspire-new.md) | Stable | Set a configuration value.     |
| [`aspire config list`](aspire-run.md)              | Stable | List all configuration values. |
| [`aspire config delete <key>`](aspire-exec.md)     | Stable | Delete a configuration value.  |
