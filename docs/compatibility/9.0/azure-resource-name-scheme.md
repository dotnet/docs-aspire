---
title: "Better Azure resource name scheme"
description: This document outlines the breaking changes introduced in .NET Aspire 9.0 GA regarding the Azure resource naming scheme.
ms.date: 10/28/2024
---

# Better Azure resource name scheme

.NET Aspire 9.0 GA introduces a new Azure resource naming scheme that is more robust and flexible, but it is a breaking change from the previous version.

## Version introduced

.NET Aspire 9.0 GA

## Previous behavior

In .NET Aspire 8.x, an early/alpha version of Azure.Provisioning was used. This version employed a naming scheme for Azure resources that attempted to be the least common denominator of all resources.

```csharp
protected string GetGloballyUniqueName(string resourceName)
    => $"toLower(take('{resourceName}${{uniqueString(resourceGroup().id)}}', 24))";