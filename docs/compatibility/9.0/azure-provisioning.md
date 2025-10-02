---
title: "Integrate new version of Azure.Provisioning"
description: Documents the breaking changes introduced by integrating a new version of Azure.Provisioning in Aspire 9.0 GA.
ms.date: 10/28/2024
ai-usage: ai-assisted
---

# Integrate new version of `Azure.Provisioning`

This update introduces significant breaking changes to the `Azure.Provisioning` library, requiring adaptations in existing implementations.

## Version introduced

Aspire 9.0 GA

## Previous behavior

Previously, the `Azure.Provisioning` library allowed provisioning operations without explicit authentication, relying on default credentials.

## New behavior

The new version mandates explicit authentication for all provisioning operations, enhancing security and control.

## Type of breaking change

This change is a [behavioral change](../categories.md#behavioral-change).

## Recommended action

Users should update their code to include explicit authentication mechanisms when performing provisioning operations.

## Affected APIs

- `Azure.Provisioning.ProvisionAsync`
- `Azure.Provisioning.DeprovisionAsync`
- `Azure.Provisioning.GetProvisioningStatus`
