---
title: "Remove default values from AzureOpenAIDeployment .ctor"
description: Learn about the breaking change in Aspire 9.0 where the AzureOpenAIDeployment constructor removed defaults.
ms.date: 10/24/2024
ai-usage: ai-assisted
---

# Remove default values from AzureOpenAIDeployment .ctor

The `AzureOpenAIDeployment` constructor no longer defines non-null default values for its parameters. This change allows for default values to be updated in the future without updating the API.

## Version introduced

Aspire 9.0 RC1

## Previous behavior

The `AzureOpenAIDeployment` constructor had default values for the `skuName` and `skuCapacity` parameters. This meant that if you didn't specify these values, the defaults were used.

- `skuName` defaulted to `"Standard"`.
- `skuCapacity` defaulted to `8`.

## New behavior

While these default values are still used, they're no longer part of the constructor signature.

## Type of breaking change

This change is a [binary compatibility](../categories.md#binary-compatibility)

## Reason for change

This change was made to allow future updates to the defaults without updating the API.

## Recommended action

No action is required.

## Affected APIs

- <xref:Aspire.Hosting.ApplicationModel.AzureOpenAIDeployment>
