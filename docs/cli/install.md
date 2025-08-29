---
title: Install .NET Aspire CLI
description: Learn how to install .NET Aspire CLI, which is a .NET global tool. Use the .NET Aspire CLI to create, run, and manage .NET Aspire projects.
author: adegeo
ms.author: adegeo
ms.topic: install-set-up-deploy
ms.date: 07/24/2025

#customer intent: As a developer, I want to install the .NET Aspire CLI so that I can create, run, and manage .NET Aspire projects.

---

# Install .NET Aspire CLI

This article teaches you how to install the Aspire CLI, which is a CLI tool used to manage your .NET Aspire projects.

## Prerequisites

- [.NET SDK 9.0](https://dotnet.microsoft.com/download/dotnet/9.0).

## Install as a native executable

The compiled version of the Aspire CLI can be installed using the Aspire CLI installation script. The script is available for PowerShell and Bash.

01. Open a terminal.
01. Download the script and save it as a file:

    ```powershell
    Invoke-RestMethod https://aspire.dev/install.ps1 -OutFile aspire-install.ps1
    ```

    ```bash
    curl -sSL https://aspire.dev/install.sh -o aspire-install.sh
    ```

01. Run the script to install the stable release build of Aspire.

    You should see output similar to the following snippet:

    ```Output
    Downloading from: https://aka.ms/dotnet/9/aspire/ga/daily/aspire-cli-win-x64.zip
    Aspire CLI successfully installed to: C:\Users\name\.aspire\bin\aspire.exe
    Added C:\name\adegeo\.aspire\bin to PATH for current session
    Added C:\name\adegeo\.aspire\bin to user PATH environment variable
    
    The aspire cli is now available for use in this and new sessions.
    ```

For more information about the install script, see [aspire-install script reference](install-script-reference.md).

## Install as a .NET global tool

Use the `dotnet tool` command to install the Aspire CLI global tool. The name of the global tool is [Aspire.Cli](https://www.nuget.org/packages/Aspire.CLI).

01. Open a terminal.
01. Run the following command to install Aspire CLI:

    ```dotnetcli
    dotnet tool install -g Aspire.Cli --prerelease
    ```

## Validation

To validate that the global tool is installed, use the `--version` option to query Aspire CLI for a version number:

```Aspire
aspire --version
```

If that command works, you're presented with the version of the Aspire CLI tool:

```Aspire
9.4.0
```

## See also

- [aspire-install script reference](install-script-reference.md).
