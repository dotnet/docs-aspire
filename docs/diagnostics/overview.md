---
title: .NET Aspire diagnostics overview
description: Learn about the diagnostics tools and features available in .NET Aspire.
ms.topic: overview
ms.date: 10/21/2024
---

# .NET Aspire diagnostics overview

Several APIs of .NET Aspire are decorated with the <xref:System.Diagnostics.CodeAnalysis.ExperimentalAttribute>. This attribute indicates that the API is experimental and may be removed or changed in future versions of .NET Aspire. The attribute is used to identify APIs that aren't yet stable and may not be suitable for production use.

## AZPROVISION001

.NET Aspire provides various overloads for Azure Provisioning resource types (from the `Azure.Provisioning` package). The overloads are used to create resources with different configurations. The overloads are experimental and may be removed or changed in future versions of .NET Aspire.

To suppress this diagnostic with the `SuppressMessageAttribute`, add the following code to your project:

```csharp
using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("AZPROVISION001", "Justification")]
```

Alternatively, you can suppress this diagnostic with preprocessor directive by adding the following code to your project:

```csharp
#pragma warning disable AZPROVISION001
        // API that is causing the warning.
#pragma warning restore AZPROVISION001
```

## ASPIREACADOMAINS001

.NET Aspire 9.0 introduces the ability to customize container app resources using the `PublishAsAzureContainerApp(...)` extension method. When using this method the Azure Developer CLI (`azd`) can no longer preserve custom domains. Instead use the `ConfigureCustomDomain` method to configure a custom domain within the .NET Aspire app host. The `ConfigureCustomDomain(...)` extension method is experimental. To suppress the compiler error/warning use the following code:

To suppress this diagnostic with the `SuppressMessageAttribute`, add the following code to your project:

```csharp
using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("ASPIREACADOMAINS001", "Justification")]
```

Alternatively, you can suppress this diagnostic with preprocessor directive by adding the following code to your project:

```csharp
#pragma warning disable ASPIREACADOMAINS001
        // API that is causing the warning.
#pragma warning restore ASPIREACADOMAINS001
```
