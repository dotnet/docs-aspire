---
title: Compiler Error ASPIREPIPELINES001
description: Learn more about compiler Error ASPIREPIPELINES001. Pipeline infrastructure APIs are for evaluation purposes only and are subject to change or removal in future updates.
ms.date: 10/27/2025
f1_keywords:
  - "ASPIREPIPELINES001"
helpviewer_keywords:
  - "ASPIREPIPELINES001"
---

# Compiler Error ASPIREPIPELINES001

**Version introduced:** 9.2

> Pipeline infrastructure APIs are for evaluation purposes only and are subject to change or removal in future updates. Suppress this diagnostic to proceed.

Aspire introduced pipeline infrastructure APIs starting in version 9.2. These APIs provide the foundation for building custom deployment pipelines and include interfaces and types for reporting publishing activities, managing pipeline steps and tasks, and coordinating the publishing process.

The following types are marked with this diagnostic:

- `IPipelineActivityReporter` - Interface for reporting publishing activities
- `IReportingStep` - Represents a publishing step that can contain multiple tasks
- `IReportingTask` - Represents a publishing task that belongs to a step
- `CompletionState` - Enum representing completion state of publishing activities
- `PublishingContext` - Context for publishing operations
- `PublishingCallbackAnnotation` - Annotation for publishing callbacks
- All types in the `Aspire.Hosting.Pipelines` namespace

These APIs are considered experimental and are expected to change in future releases.

## To correct this Error

Suppress the Error with either of the following methods:

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
