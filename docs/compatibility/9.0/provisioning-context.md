---
title: "Allow for customization of the Azure ProvisioningContext + Better Azure resource name scheme"
description: This article describes the breaking changes introduced by allowing customization of the Azure ProvisioningContext and adopting a new Azure resource naming scheme.
ms.date: 10/28/2024
ai-usage: ai-assisted
---

# Allow for customization of the Azure ProvisioningContext + Better Azure resource name scheme

This change introduces `IOptions<AzureProvisioningOptions>` which contains a `ProvisioningContext`. Users can customize the `ProvisioningContext` by configuring the `AzureProvisioningOptions` like a typical `IOptions`. This allows the use of the new CDK default naming scheme. Users can opt into the old scheme by customizing the `ProvisioningContext`.

## Version introduced

Aspire 9.0 GA

## Previous behavior

Users could not customize the `ProvisioningContext` and had to use the default Azure resource naming scheme.

## New behavior

Users can now customize the `ProvisioningContext` by configuring the `AzureProvisioningOptions`. This allows the use of the new CDK default naming scheme, with an option to revert to the old scheme.

## Type of breaking change

This change is a [behavioral change](../categories.md#behavioral-change).

## Recommended action

Users should review their current provisioning configurations and update their code to customize the `ProvisioningContext` if they wish to use the new or old naming schemes.

## Affected APIs

- `IOptions<AzureProvisioningOptions>`
- `ProvisioningContext`
