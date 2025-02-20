---
title: Devcontainers in Visual Studio Code
description: Learn how to use .NET Aspire with Devcontainers in Visual Studio Code.
ms.date: 02/19/2025
---

# .NET Aspire and Visual Studio Code Devcontainers

The Devcontainers extension in Visual Studio Code provides a way for development teams to develop within a containerized environment where all dependencies are preconfigured. With .NET Aspire 9.1 we have added logic to better support working with .NET Aspire within a Devcontainer environment by automatically configuring port forwarding.

Prior to .NET Aspire 9.1 it was still possible to use .NET Aspire within a Devcontainer, however more manual configuration was required.

## Devcontainers vs. GitHub Codespaces

Using Devcontainers in Visual Studio Code is quite similar to using GitHub Codespaces. With the release of .NET Aspire 9.1, support for both Devcontainers in Visual Studio Code and GitHub Codespaces has been enhanced. Although the experiences are similar, there are some differences. For more information on using .NET Aspire with GitHub Codespaces, see [.NET Aspire and GitHub Codespaces](codespaces.md).