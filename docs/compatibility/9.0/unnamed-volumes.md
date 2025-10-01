---
title: Updates to implicitly named volumes to avoid collisions
description: This article describes the breaking change related to the implicit naming of volumes created by the WithDataVolume() method in Aspire.
ms.date: 10/28/2024
ai-usage: ai-assisted
---

# Updates to implicitly named volumes to avoid collisions

The `WithDataVolume` methods create container volumes with names implicitly based on the AppHost project name and resource name. This can lead to collisions when multiple AppHost projects share the same name.

## Version introduced

Aspire 9.0 GA

## Previous behavior

The implicit volume name is based on the AppHost project name and resource name. For example, if the AppHost project is named "TestShop.AppHost" and the resource is named "postgres", the implicit volume name will be "TestShop.AppHost-postgres-data".

## New behavior

The implicit volume naming logic now includes a hash of the AppHost project path or another deterministic, stable value derived from the AppHost project. This prevents collisions of implicit volume names across different solutions.

## Type of breaking change

This change is a [behavioral change](../categories.md#behavioral-change).

## Recommended action

Developers should review their usage of the `WithDataVolume` method and ensure that any custom volume names are unique to avoid collisions. If relying on implicit naming, verify that the new naming logic does not introduce any issues.

## Affected APIs

- `WithDataVolume`
