---
title: aspire config list command
description: Learn about the aspire config list command and its usage. This command lists all Aspire CLI config values.
ms.date: 07/11/2025
---
# aspire config list command

**This article applies to:** ✔️ Aspire CLI 9.4.0 and later versions

## Name

`aspire config list` - List all configuration values.

## Synopsis

```Command
aspire config list [options]
```

## Description

The `aspire config list` command lists all config value by key name.

[!INCLUDE [config-file-description](includes/config-file-description.md)]

This command lists both the global and local settings. If a local setting overrides a global setting, only the local setting is displayed.

## Options

The following options are available:

- [!INCLUDE [option-help](includes/option-help.md)]

- [!INCLUDE [option-debug](includes/option-debug.md)]

- [!INCLUDE [option-wait](includes/option-wait.md)]
