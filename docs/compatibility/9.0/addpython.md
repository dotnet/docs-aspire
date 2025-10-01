---
title: "Python resources and APIs changed"
description: Learn about the breaking change in Aspire 9.0 where Python resources and APIs updated.
ms.date: 10/24/2024
ai-usage: ai-assisted
---

# Python resources and APIs changed

When adding Python resources to your Aspire AppHost, use the `AddPythonApp` method instead of the `AddPythonProject` method. The `AddPythonProject` method is now obsolete. Additionally, the `PythonProjectResource` class is now obsolete. Use the `PythonAppResource` class instead.

## Version introduced

Aspire 9.0 RC1

## Previous behavior

The `AddPythonProject` method added a new `PythonProjectResource` to the AppHost.

## New behavior

The `AddPythonApp` method adds a new `PythonAppResource` to the AppHost.

## Type of breaking change

This change is a [source compatibility](../categories.md#source-compatibility).

## Reason for change

This changes removes the concept of a Python project from the AppHost and instead uses the concept of a Python app. Project is an overloaded term that's reserved for .NET projects. The new term `PythonAppResource` is more accurate.

## Recommended action

Replace calls to `AddPythonProject` with calls to `AddPythonApp` and also replace references to `PythonProjectResource` with references to `PythonAppResource`.

## Affected APIs

- `Aspire.Hosting.PythonProjectResourceBuilderExtensions.AddPythonProject`
- `Aspire.Hosting.Python.PythonProjectResource`
