---
title: .NET Aspire and Devcontainers in Visual Studio Code
description: Learn how to use .NET Aspire with Devcontainers in Visual Studio Code.
ms.date: 02/19/2025
zone_pivot_groups: dev-environment
uid: dotnet/aspire/devcontainers
---

# .NET Aspire and Visual Studio Code Devcontainers

The Devcontainers extension in Visual Studio Code provides a way for development teams to develop within a containerized environment where all dependencies are preconfigured. With .NET Aspire 9.1 we have added logic to better support working with .NET Aspire within a Devcontainer environment by automatically configuring port forwarding.

Prior to .NET Aspire 9.1 it was still possible to use .NET Aspire within a Devcontainer, however more manual configuration was required.

## Devcontainers vs. GitHub Codespaces

Using Devcontainers in Visual Studio Code is very similar to using GitHub Codespaces. In addition to supporting Devcontainers in Visual Studio Code, .NET Aspire 9.1 also improves support for using GitHub Codespaces. While the experience is similar between each approach there are some differences and so we have prepared separate documentation. If you want to use .NET Aspire with GitHub Codespaces see [.NET Aspire and GitHub Codespaces](codespaces.md).