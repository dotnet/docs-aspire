---
title: Install .NET Aspire CLI
description: Learn how to install .NET Aspire CLI, which is a .NET global tool. Use the .NET Aspire CLI to create, run, and manage .NET Aspire projects.
author: adegeo
ms.author: adegeo
ms.topic: install-set-up-deploy
ms.date: 06/26/2025

#customer intent: As a developer, I want to install the .NET Aspire CLI so that I can create, run, and manage .NET Aspire projects.

---

# Install .NET Aspire CLI

This article teaches you how to install .NET Aspire CLI, which is a .NET global tool used to manage your .NET Aspire projects.

## Prerequisites

- [.NET SDK 8.0](https://dotnet.microsoft.com/download/dotnet/8.0) or [.NET SDK 9.0](https://dotnet.microsoft.com/download/dotnet/9.0).

## Install the global tool

Use the `dotnet tool` command to install .NET Aspire CLI global tool. The name of the global tool is `Aspire.Cli`.

1. Open a terminal.
1. Run `dotnet tool install -g Aspire.Cli --prerelease` to install the global tool.

## Validation

To validate that the global tool is installed, use the `--version` parameter to query Aspire CLI for a version number:

```
aspire --version
```

If that command works, you're presented with the version of the .NET Aspire CLI tool:

```
9.3.1-preview.1.25305.6+5bc26c78ff8c7be825d0ae33633a1ae9f1d64a67
```
