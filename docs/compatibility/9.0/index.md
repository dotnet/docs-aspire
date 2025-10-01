---
title: Breaking changes in Aspire 9
titleSuffix: ""
description: Navigate to the breaking changes in Aspire 9.
ms.date: 11/04/2024
---

# Breaking changes in Aspire 9

If you're migrating an app to .NET 9, the breaking changes listed here might affect you.

[!INCLUDE [binary-source-behavioral](../includes/binary-source-behavioral.md)]

> [!NOTE]
> This article is a work in progress. It's not a complete list of breaking changes in Aspire 9.

## Breaking changes

| Title | Type of change | Introduced version |
|--|--|--|
| [Remove default values from AzureOpenAIDeployment ctor](azureopenai-ctor.md) | Binary incompatible | Aspire 9.0 RC1 |
| [Python resources and APIs changed](addpython.md) | Source incompatible | Aspire 9.0 RC1 |
| [Updates to implicitly named volumes to avoid collisions](unnamed-volumes.md) | Source incompatible | Aspire 9.0 RC1 |
| [Make unnamed volumes more unique](make-unnamed-volumes-unique.md) | Source incompatible | Aspire 9.0 RC1 |
| [New `Azure.Provisioning` version](azure-provisioning.md) | Source incompatible | Aspire 9.0 RC1 |
| [Allow customization of Azure `ProvisioningContext`](provisioning-context.md) | Source incompatible | Aspire 9.0 RC1 |
| [Changes to `Azure.Hosting` APIs](azure-hosting.md) | Source incompatible | Aspire 9.0 RC1 |
| [Improved Azure resource name scheme](azure-resource-name-scheme.md) | Source incompatible | Aspire 9.0 RC1 |
| [Ollama integration updates](ollama-integration-updates.md) | Binary incompatible, behavioral change | Aspire 9.0 GA |
