---
title: aspire-install script reference
description: Learn about the aspire-install scripts to install the .NET Aspire CLI. Use the .NET Aspire CLI to create, run, and manage .NET Aspire projects.
author: adegeo
ms.author: adegeo
ms.date: 08/28/2025
ai-usage: ai-assisted
#customer intent: As a developer, I need the aspire-install script reference so that I know what options it provides when installing the Aspire CLI.
---

# aspire-install script reference

This article contains the syntax of the aspire install PowerShell and Bash scripts. To download the script, see [Install as a native executable](install.md#install-as-a-native-executable).

## Name

`aspire-install.ps1` | `aspire-install.sh` - Scripts used to install the Aspire CLI.

## Synopsis

Windows:

```powershell
aspire-install.ps1 [-InstallPath <DIRECTORY>] [-Version <VERSION>] [-Quality <QUALITY>]
                   [-OS <OPERATING_SYSTEM>] [-Architecture <ARCHITECTURE>] [-KeepArchive] [-Help]"
```

Linux/macOS:

```bash
aspire-install.sh [--install-path <DIRECTORY>] [--version <VERSION>] [--quality <QUALITY>]
                  [--os <OPERATING_SYSTEM>] [--arch <ARCHITECTURE>] [--keep-archive] [--verbose] [--help]
```

## Description

The `aspire-install` scripts perform a nonadmin installation of the Aspire CLI.

> [!IMPORTANT]
> The Aspire CLI is **still in preview** and under active development.

### Script behavior

Both scripts behave the same way. They download the Aspire CLI for the target platform, install it to the specified location, and add it to your PATH environment variable.

By default, the installation scripts download and install the latest stable release of the Aspire CLI. Specify a different version with the `-Version|--version` argument or a different quality level with the `-Quality|--quality` argument.

Unless changed with the `-InstallPath|--install-path` argument, the script installs to a user-specific location: `%USERPROFILE%\.aspire\bin` on Windows or `$HOME/.aspire/bin` on Linux/macOS.

## Options

- **`-InstallPath|--install-path <DIRECTORY>`**

  Specifies the installation path. The directory is created if it doesn't exist. The default value is *%USERPROFILE%\.aspire\bin* on Windows and *$HOME/.aspire/bin* on Linux/macOS. The Aspire CLI executable is placed directly in this directory.

- **`-Version|--version <VERSION>`**

  Represents a specific version to install. For example: `9.4`. If not specified, the latest stable version is installed.

- **`-Quality|--quality <QUALITY>`**

  Downloads the specified quality build. The possible values are:

  - `release`: The latest stable release (default).
  - `staging`: Builds from the current release branch (prerelease builds).
  - `dev`: Latest builds from the `main` branch (development builds).

  The different quality values signal different stages of the release process:

  - `release`: The final stable releases of the Aspire CLI. Intended for production use.
  - `staging`: Prerelease builds intended for testing upcoming releases. Not recommended for production use.
  - `dev`: The latest builds from the main development branch. They're built frequently and aren't fully tested. Not recommended for production use but can be used to test specific features or fixes immediately after they're merged.

- **`-OS|--os <OPERATING_SYSTEM>`**

  Specifies the operating system for which the CLI is being installed. Possible values are: `win`, `linux`, `linux-musl`, `osx`.

  The parameter is optional and should only be used when it's required to override the operating system that is detected by the script.

- **`-Architecture|--arch <ARCHITECTURE>`**

  Architecture of the Aspire CLI binaries to install. Possible values are `x64`, `x86`, `arm64`. The default behavior is to autodetect the current system architecture.

- **`-KeepArchive|--keep-archive`**

  If you set this option, the script keeps the downloaded archive files and temporary directory after installation. By default, the script deletes the archive after extraction and cleans up the temporary directory.

- **`--verbose`**

  Displays diagnostics information. (Linux/macOS only)

- **`-Help|--help`**

  Prints help information for the script.

## Examples

- Install the latest stable version to the default location:

  Windows:

  ```powershell
  .\aspire-install.ps1
  ```

  macOS/Linux:

  ```bash
  ./aspire-install.sh
  ```

- Install to a custom directory:

  Windows:

  ```powershell
  .\aspire-install.ps1 -InstallPath 'C:\Tools\Aspire'
  ```

  macOS/Linux:

  ```bash
  ./aspire-install.sh --install-path "/usr/local/bin"
  ```

- Install a specific version:

  Windows:

  ```powershell
  .\aspire-install.ps1 -Version '9.4'
  ```

  macOS/Linux:

  ```bash
  ./aspire-install.sh --version "9.4"
  ```

- Install development builds:

  Windows:

  ```powershell
  .\aspire-install.ps1 -Quality 'dev'
  ```

  macOS/Linux:

  ```bash
  ./aspire-install.sh --quality "dev"
  ```

- Install with verbose output (Linux/macOS only):

  ```bash
  ./aspire-install.sh --verbose
  ```

## Uninstall

There's no uninstall script. Delete the installation directory, such as `%USERPROFILE%\.aspire\bin` on Windows or `$HOME/.aspire/bin` on Linux/macOS.

## See also

- [Install .NET Aspire CLI](install.md)
