---
title: .NET Aspire diagnostics overview
description: Learn about the diagnostics tools and features available in .NET Aspire.
ms.topic: overview
ms.date: 03/25/2024
---

# .NET Aspire diagnostics overview

Several APIs of .NET Aspire are decorated with the <xref:System.Diagnostics.CodeAnalysis.ExperimentalAttribute>. This attribute indicates that the API is experimental and may be removed or changed in future versions of .NET Aspire. The attribute is used to identify APIs that aren't yet stable and may not be suitable for production use.

## ASPIRE0001

.NET Aspire provides various overloads for Cloud Development Kit (CDK) resource types. The overloads are used to create resources with different configurations. The overloads are experimental and may be removed or changed in future versions of .NET Aspire.

To suppress this diagnostic with the `SuppressMessageAttribute`, add the following code to your project:

```csharp
using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("ASPIRE0001", "Justification")]
```

Alternatively, you can suppress this diagnostic with preprocessor directive by adding the following code to your project:

```csharp
#pragma warning disable ASPIRE0001
        // API that is causing the warning.
#pragma warning restore ASPIRE0001
```
