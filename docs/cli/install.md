---
title: Install .NET Aspire CLI
description: Learn how to install .NET Aspire CLI, which is a .NET global tool. Use the .NET Aspire CLI to create, run, and manage .NET Aspire projects.
author: adegeo
ms.author: adegeo
ms.topic: install-set-up-deploy
ms.date: 07/22/2025

#customer intent: As a developer, I want to install the .NET Aspire CLI so that I can create, run, and manage .NET Aspire projects.

---

# Install .NET Aspire CLI

This article teaches you how to install the Aspire CLI, which is a CLI tool used to manage your .NET Aspire projects.

## Prerequisites

- [.NET SDK 9.0](https://dotnet.microsoft.com/download/dotnet/9.0).

## Install as a native executable

The compiled version of the Aspire CLI can be installed using the Aspire CLI installation script.

[!INCLUDE [install-aspire-cli](../includes/install-aspire-cli.md)]

## Install as a .NET global tool

Use the `dotnet tool` command to install the Aspire CLI global tool. The name of the global tool is [Aspire.Cli](https://www.nuget.org/packages/Aspire.CLI).

01. Open a terminal.
01. Run the following command to install Aspire CLI:

    ```dotnetcli
    dotnet tool install -g Aspire.Cli --prerelease
    ```

## Validation

To validate that the global tool is installed, use the `--version` option to query Aspire CLI for a version number:

```
aspire --version
```

If that command works, you're presented with the version of the Aspire CLI tool:

```
9.4.0
```
