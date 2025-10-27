---
title: Compiler Error ASPIREPIPELINES001
description: Learn more about compiler Error ASPIREPIPELINES001. Pipeline infrastructure APIs are for evaluation purposes only and are subject to change or removal in future updates.
ms.date: 10/27/2025
f1_keywords:
  - "ASPIREPIPELINES001"
helpviewer_keywords:
  - "ASPIREPIPELINES001"
ai-usage: ai-generated
---

# Compiler Error ASPIREPIPELINES001

**Version introduced:** 9.2

> Pipeline infrastructure APIs are for evaluation purposes only and are subject to change or removal in future updates. Suppress this diagnostic to proceed.

Aspire introduced pipeline infrastructure APIs starting in version 9.2. These APIs provide core functionality for building deployment pipelines, including interfaces and types for pipeline activity reporting, step management, and publishing contexts. Pipeline infrastructure enables you to create custom deployment workflows and track their execution.

Pipeline infrastructure APIs are considered experimental and are expected to change in future updates.

## APIs affected

This diagnostic applies to the following pipeline infrastructure APIs:

- `IPipelineActivityReporter` - Interface for reporting pipeline activities
- `IReportingStep` - Interface for managing pipeline steps
- `IReportingTask` - Interface for managing tasks within a step
- `CompletionState` - Enumeration for tracking completion status
- `PublishingContext` - Context for publishing operations
- `PublishingCallbackAnnotation` - Annotation for publishing callbacks
- Related extension methods and implementations

## To correct this error

Suppress the error with one of the following methods:

- Set the severity of the rule in the _.editorconfig_ file.

  ```ini
  [*.{cs,vb}]
  dotnet_diagnostic.ASPIREPIPELINES001.severity = none
  ```

  For more information about editor config files, see [Configuration files for code analysis rules](/dotnet/fundamentals/code-analysis/configuration-files).

- Add the following `PropertyGroup` to your project file:

  ```xml
  <PropertyGroup>
      <NoWarn>$(NoWarn);ASPIREPIPELINES001</NoWarn>
  </PropertyGroup>
  ```

- Suppress in code with the `#pragma warning disable ASPIREPIPELINES001` directive.
