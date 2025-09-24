---
title: Configuration settings
description: Learn about 
ms.date: 09/24/2025
ms.topic: overview
---

# What is .NET Aspire configuration?

The .NET Aspire CLI uses a configuration file to control its behavior. You can configure feature flags to enable or disable CLI features, and specify which AppHost project the CLI should use by default for a .NET Aspire solution.

The settings file is stored in a `.aspire` folder and is named `settings.json`. Settings files can be stored locally or globally.

The following snippet is an example `.aspire/settings.json` file:

```json
{
  "appHostPath": "../AspireShop/AspireShop.AppHost/AspireShop.AppHost.csproj",
  "features": {
    "deployCommandEnabled": "true"
  }
}
```

## Config file locations

A global .NET Aspire CLI settings file is stored at `$HOME/.aspire/settings.json`, and is used as the default settings for the CLI. A local settings file overwrites the settings from the global file. Local settings files are stored at `./.aspire/settings.json`.

## Generating a config file

The CLI automatically generates a local settings file when you run a command that requires interaction with the AppHost, or if you set a config option. For example, `aspire run` searches for an AppHost project, and when found, generates the `./.aspire/settings.json` file with the `appHostPath` setting set to the project it found.

> [!IMPORTANT]
> `appHostPath` can be set globally, but the CLI ignores it and only reads it in the local settings file.

## Settings

The .NET Aspire CLI supports two categories of configuration settings:

- **Feature flags**\
These settings enable or disable specific CLI features. All feature flag setting names start with `feature.`

- **CLI behavior**\
These settings control how the CLI operates. Currently, the only CLI behavior setting is `appHostPath`, which specifies the location of the AppHost project.

The following table lists the settings that can be set in the config file:

[!INCLUDE [config-settings-table](../cli-reference/includes/config-settings-table.md)]

## CLI commands

| Command                                                                    | Status | Function                                               |
|----------------------------------------------------------------------------|--------|--------------------------------------------------------|
| [`aspire config`](../cli-reference/aspire-config-list.md)                  | Stable | Command driver for managing .NET Aspire configuration. |
| [`aspire config list`](../cli-reference/aspire-config-list.md)             | Stable | List all configuration values.                         |
| [`aspire config get <key>`](../cli-reference/aspire-config-get.md)         | Stable | Get a configuration value.                             |
| [`aspire config set <key> <value>`](../cli-reference/aspire-config-set.md) | Stable | Set a configuration value.                             |
| [`aspire config delete <key>`](../cli-reference/aspire-config-delete.md)   | Stable | Delete a configuration value.                          |
