---
title: "Make unnamed volume names more unique"
description: "This document describes the breaking change related to the naming scheme for unnamed volumes in Aspire."
ms.date: 10/28/2024
ai-usage: ai-assisted
---

# Make unnamed volume names more unique

This change updates the naming scheme for unnamed volumes to use the first 10 characters of the SHA256 of the AppHost's physical path as the volume name prefix. This ensures more unique volume names.

## Version introduced

Aspire 9.0 GA

## Previous behavior

Previously, unnamed volumes did not follow a specific scheme for uniqueness, which could lead to conflicts.

## New behavior

The new behavior uses the first 10 characters of the SHA256 of the AppHost's physical path as the volume name prefix, ensuring more unique volume names.

## Type of breaking change

This change is a [behavioral change](../categories.md#behavioral-change).

## Recommended action

Manually specify the volume name if the new naming scheme causes issues in your environment.

## Affected APIs

None.
