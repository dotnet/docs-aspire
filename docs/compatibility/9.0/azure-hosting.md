---
title: Changes to `Azure.Hosting` APIs
description: "This document outlines the breaking changes in the Azure Hosting APIs for Aspire 9.0 GA."
ms.date: 10/28/2024
ai-usage: ai-assisted
---

# Changes to `Azure.Hosting` APIs

This document outlines the breaking changes in the `Azure.Hosting` APIs for Aspire 9.0 GA. The changes include the removal of experimental callbacks, renaming of several classes and methods, and a holistic review of the Azure Hosting APIs.

## Version introduced

Aspire 9.0 GA

## Previous behavior

- Experimental callbacks were available for customization.
- The class `ResourceModuleConstruct` existed.
- The class `AzureConstructResource` existed.
- The method `ConfigureConstruct` was used for configuration.

## New behavior

- Experimental callbacks are removed. Callers now use `builder.AddAzureStorage("storage").ConfigureConstruct(c => /* ... */)` for customization.
- `ResourceModuleConstruct` is renamed to `AzureResourceInfrastructure`.
- `AzureConstructResource` is renamed to `AzureProvisioningResource`.
- `ConfigureConstruct` is renamed to `ConfigureInfrastructure`.

## Type of breaking change

This change is a [behavioral change](../categories.md#behavioral-change).

## Recommended action

Users should update their code to use the new class and method names. Specifically:

- Replace any usage of experimental callbacks with `builder.AddAzureStorage("storage").ConfigureConstruct(c => /* ... */)`.
- Rename instances of `ResourceModuleConstruct` to `AzureResourceInfrastructure`.
- Rename instances of `AzureConstructResource` to `AzureProvisioningResource`.
- Rename instances of `ConfigureConstruct` to `ConfigureInfrastructure`.

## Affected APIs

- `ResourceModuleConstruct`
- `AzureConstructResource`
- `ConfigureConstruct`
- Experimental callbacks in `AzureStorageExtensions`
