---
title: Breaking changes in .NET Aspire 9.4
titleSuffix: ""
description: Navigate to the breaking changes in .NET Aspire 9.4.
ms.date: 08/14/2025
---

# Breaking changes in .NET Aspire 9.4

If you're migrating an app to .NET Aspire 9.4, the breaking changes listed here might affect you.

[!INCLUDE [binary-source-behavioral](../includes/binary-source-behavioral.md)]

> [!NOTE]
> This article is a work in progress. It's not a complete list of breaking changes in .NET Aspire 9.4.

## Breaking changes

| Title | Type of change | Introduced version |
|--|--|--|
| [AddAzureOpenAI defaults to CognitiveServicesOpenAIUser role](add-azure-openai-default-changes.md) | Behavioral change | 9.4 |
| [Azure Cosmos DB now provisions serverless accounts by default](cosmosdb-serverless-defaults.md) | Behavioral change | 9.4 |
| [Azure Storage APIs renamed and refactored](azure-storage-apis-renamed.md) | Binary incompatible, source incompatible | 9.4 |
| [BicepSecretOutputReference and GetSecretOutput are now obsolete](getsecretoutput-deprecated.md) | Binary incompatible, source incompatible | 9.4 |
| [Deprecating various known parameters in AzureBicepResource](azure-bicep-parameters-deprecated.md) | Source incompatible, behavioral change | 9.4 |
| [Hybrid compute mode between azd and Aspire apps dropped](hybrid-compute-support-dropped.md) | Behavioral change | 9.4 |
| [Local auth is disabled by default on Azure resources](local-auth-disabled-for-azure-resources.md) | Behavioral change | 9.4 |
