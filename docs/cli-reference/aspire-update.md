---
title: aspire update command
description: Learn about the aspire update command and its usage. This command updates Aspire integration package versions in an AppHost-based solution to the latest versions on a selected channel.
ms.date: 09/22/2025
---

# aspire update command (Preview)

**This article applies to:** ✔️ Aspire CLI 9.5.0 and later versions.

## Name

`aspire update` - Update Aspire integration packages to the latest versions for a chosen channel.

[!INCLUDE [mode-preview](includes/mode-preview.md)]

## Synopsis

```Command
aspire update [options]
```

## Description

The `aspire update` command scans an AppHost-based solution and updates Aspire-related NuGet packages (core and integration packages) to the latest versions published on a user-selected channel (for example: `stable`, `preview`, or `daily`). The command:

- Locates the AppHost project automatically (prompts if multiple or none located).
- Prompts you to choose a channel sourced from configured NuGet feeds.
- Detects outdated Aspire packages and computes safe upgrade targets.
- Updates package versions either inline (`<PackageReference Version="..." />`) or via Central Package Management (`Directory.Packages.props`).
- Updates `NuGet.config` file if required to make sure that package sources and package source mappings are appropriate for the channel selected.
- Skips packages already at the latest channel-compatible version.

Typical reasons to run this command include adopting the latest stable servicing fixes, trying preview features, or experimenting with daily builds. After updating, build or run your solution (for example with `aspire run`) to validate changes.

## Options

The following options are available:

- [!INCLUDE [option-project](includes/option-project.md)]
- [!INCLUDE [option-help](includes/option-help.md)]
- [!INCLUDE [option-debug](includes/option-debug.md)]
- [!INCLUDE [option-wait](includes/option-wait.md)]

## Examples

- Update Aspire packages in the current solution (interactive channel selection):

  ```Command
  aspire update
  ```

- Update packages for a specific AppHost project path:

  ```Command
  aspire update --project './src/MyApp.AppHost/MyApp.AppHost.csproj'
  ```

- Example interactive session (conceptual):

  ```Command
  $ aspire update
  Detected AppHost: MyApp.AppHost.csproj
  Select channel:
    default
    stable
  > daily
  Updating Aspire.Hosting 9.4.2 -> 9.5.0-daily.20240923.1
  Updating Aspire.Azure.Cosmos 9.4.2 -> 9.5.0-daily.20240923.1
  Done.
  ```

> [!NOTE]
> Commit your repository before updating so you can easily revert if a preview or daily build intro
