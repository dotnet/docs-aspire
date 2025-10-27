---
title: Compiler Error ASPIREPIPELINES002
description: Learn more about compiler Error ASPIREPIPELINES002. Deployment state management APIs are for evaluation purposes only and are subject to change or removal in future updates.
ms.date: 10/27/2025
f1_keywords:
  - "ASPIREPIPELINES002"
helpviewer_keywords:
  - "ASPIREPIPELINES002"
---

# Compiler Error ASPIREPIPELINES002

**Version introduced:** 9.2

> Deployment state management APIs are for evaluation purposes only and are subject to change or removal in future updates. Suppress this diagnostic to proceed.

Aspire introduced deployment state management APIs starting in version 9.2. These APIs enable managing deployment state across publishing operations, allowing you to cache deployment information and control deployment behavior.

The following types and members are marked with this diagnostic:

- `IDeploymentStateManager` - Interface for deployment state management
- Implementations of `IDeploymentStateManager` such as:
  - `DeploymentStateManagerBase`
  - `FileDeploymentStateManager`
  - `UserSecretsDeploymentStateManager`
- `PublishingOptions.Deploy` - Property to enable deployment after publishing
- `PublishingOptions.ClearCache` - Property to clear deployment cache
- `PublishingOptions.Step` - Property to specify a deployment step to run

These APIs are considered experimental and are expected to change in future releases.

## To correct this Error

Suppress the Error with either of the following methods:

- Set the severity of the rule in the _.editorconfig_ file.

  ```ini
  [*.{cs,vb}]
  dotnet_diagnostic.ASPIREPIPELINES002.severity = none
  ```

  For more information about editor config files, see [Configuration files for code analysis rules](/dotnet/fundamentals/code-analysis/configuration-files).

- Add the following `PropertyGroup` to your project file:

  ```xml
  <PropertyGroup>
      <NoWarn>$(NoWarn);ASPIREPIPELINES002</NoWarn>
  </PropertyGroup>
  ```

- Suppress in code with the `#pragma warning disable ASPIREPIPELINES002` directive.
