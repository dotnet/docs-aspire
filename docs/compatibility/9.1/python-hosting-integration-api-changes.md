---
title: Python hosting integration parameter name changes
description: Update parameter names in Python hosting integration preview for Aspire to refer to apps instead of projects.
ms.date: 02/13/2025
ai-usage: ai-assisted
ms.custom: https://github.com/dotnet/docs-aspire/issues/2559
---

# Python hosting integration parameter name changes

In .NET Aspire 9.1, the Python hosting integration preview updates parameter names to refer to Python apps instead of projects. This change ensures consistent naming across the API.

## Version introduced

.NET Aspire 9.1

## Previous behavior

Some parameter names on methods and constructors in the Python hosting integration preview for .NET Aspire referred to the Python app as a `project` rather than an `app`.

## New behavior

Parameter names on methods and constructors in the Python hosting integration preview for .NET Aspire now refer to the Python app as an `app` instead of `project` where applicable.

## Type of breaking change

This change is a [source incompatible](../categories.md#source-incompatible).

## Reason for change

The change ensures consistent naming across the API.

## Recommended action

Update application source as appropriate to use the new parameter names.

## Affected APIs

- <xref:Aspire.Hosting.PythonAppResourceBuilderExtensions.AddPythonApp*>
- <xref:Aspire.Hosting.Python.PythonAppResource>
