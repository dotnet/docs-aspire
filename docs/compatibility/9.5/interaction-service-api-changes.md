---
title: "Breaking change - Interaction service API changes"
description: "Learn about the breaking change in .NET Aspire 9.5 where the interaction service API requires input names, makes labels optional, and enables name-based access to results."
ms.date: 09/23/2025
ai-usage: ai-assisted
ms.custom: https://github.com/dotnet/docs-aspire/issues/4270
---

# Interaction service API changes

In .NET Aspire 9.5, the experimental interaction service API changes input requirements and result access. Input names are required, labels are optional (defaulting to the name), and results allow name-based lookup of inputs.

## Version introduced

.NET Aspire 9.5

## Previous behavior

In .NET Aspire 9.4, inputs didn't require a name. Labels were required and commonly used to identify inputs. Results didn't support looking up inputs by name, so you typically accessed values by position or by iterating.

## New behavior

- The input name is required.
- The input label isn't required. If not specified, it defaults to the name.
- You can access inputs from results by name.

## Type of breaking change

This change can affect [binary compatibility](../categories.md#binary-compatibility) and [source compatibility](../categories.md#source-compatibility). It's also a [behavioral change](../categories.md#behavioral-change).

## Reason for change

To make it easier and more reliable to access, input results.

## Recommended action

- Provide a name for each interaction input.
- Remove any reliance on labels being required; specify a label only when you want it to differ from the name.
- Update code that reads results to use name-based access when appropriate.
- Recompile and test to ensure your app still works as expected.

## Affected APIs

None.
