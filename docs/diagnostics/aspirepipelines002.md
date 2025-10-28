---
title: Compiler Error ASPIREPIPELINES002
description: Learn more about compiler Error ASPIREPIPELINES002. Deployment state manager APIs are for evaluation purposes only and are subject to change or removal in future updates.
ms.date: 10/28/2025
f1_keywords:
  - "ASPIREPIPELINES002"
helpviewer_keywords:
  - "ASPIREPIPELINES002"
ai-usage: ai-generated
---

# Compiler Error ASPIREPIPELINES002

**Version introduced:** 13.0

> Deployment state manager APIs are for evaluation purposes only and are subject to change or removal in future updates. Suppress this diagnostic to proceed.

Aspire introduced deployment state manager APIs as part of the pipeline infrastructure starting in version 13.0. These APIs enable you to manage and persist deployment state information across pipeline executions. The deployment state manager provides functionality for storing, retrieving, and clearing deployment artifacts and metadata, which is essential for incremental deployments and tracking deployment history.

Deployment state manager APIs are considered experimental and are expected to change in future updates.

## APIs affected

This diagnostic applies to the following deployment state manager APIs:

- `IDeploymentStateManager` - Interface for managing deployment state
- Deployment state manager implementations
- `Deploy` property in `PublishingOptions`
- `ClearCache` property in `PublishingOptions`
- `Step` property in `PublishingOptions`
- `DeployingCallbackAnnotation` - Annotation for deploying callbacks
- Azure provisioning context providers
- Related extension methods and implementations

## To correct this error

Suppress the error with one of the following methods:

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
