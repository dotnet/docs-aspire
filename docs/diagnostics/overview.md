---
title: .NET Aspire diagnostics overview
description: Learn about the diagnostics tools and features available in .NET Aspire.
ms.topic: overview
ms.date: 04/11/2025
---

# .NET Aspire diagnostics overview

Several APIs of .NET Aspire are decorated with the <xref:System.Diagnostics.CodeAnalysis.ExperimentalAttribute>. This attribute indicates that the API is experimental and might be removed or changed in future versions of .NET Aspire. The attribute is used to identify APIs that aren't yet stable and might not be suitable for production use.

## AZPROVISION001

<span id="AZPROVISION001"></span>

.NET Aspire provides various overloads for [Azure provisioning](../azure/integrations-overview.md#azureprovisioning-customization) resource types (from the `Azure.Provisioning` package). The overloads are used to create resources with different configurations. The overloads are experimental and might be removed or changed in future versions of .NET Aspire.

To suppress the `AZPROVISION001` diagnostic with the <xref:System.Diagnostics.CodeAnalysis.SuppressMessageAttribute>, add the following code to your project:

```csharp
using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("AZPROVISION001", "Justification")]
```

Alternatively, to suppress the `AZPROVISION001` diagnostic with preprocessor directives, apply the following code to your project:

```csharp
#pragma warning disable AZPROVISION001
// API that is causing the warning.
#pragma warning restore AZPROVISION001
```

## ASPIREACADOMAINS001

<span id="ASPIREACADOMAINS001"></span>

.NET Aspire 9.0 introduces the ability to customize container app resources using the `PublishAsAzureContainerApp(...)` extension method. When you use this method, the Azure Developer CLI (`azd`) can no longer preserve custom domains. Instead use the `ConfigureCustomDomain` method to configure a custom domain within the .NET Aspire app host. The `ConfigureCustomDomain(...)` extension method is experimental. To suppress the compiler error/warning, use the following code:

To suppress the `ASPIREACADOMAINS001` diagnostic with the <xref:System.Diagnostics.CodeAnalysis.SuppressMessageAttribute>, add the following code to your project:

```csharp
using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("ASPIREACADOMAINS001", "Justification")]
```

Alternatively, to suppress the `ASPIREACADOMAINS001` diagnostic with preprocessor directives, apply the following code to your project:

```csharp
#pragma warning disable ASPIREACADOMAINS001
// API that is causing the warning.
#pragma warning restore ASPIREACADOMAINS001
```

## ASPIREHOSTINGPYTHON001

<span id="ASPIREHOSTINGPYTHON001"></span>

.NET Aspire provides a way to add Python executables or applications to the .NET Aspire app host. Since the shape of this API is expected to change in the future, it's experimental (<xref:System.Diagnostics.CodeAnalysis.ExperimentalAttribute>). To suppress the compiler error/warning, use the following code:

To suppress the `ASPIREHOSTINGPYTHON001` diagnostic with the <xref:System.Diagnostics.CodeAnalysis.SuppressMessageAttribute>, add the following code to your project file:

```xml
<PropertyGroup>
  <NoWarn>$(NoWarn);ASPIREHOSTINGPYTHON001</NoWarn>
<PropertyGroup>
```

Alternatively, to suppress the `ASPIREHOSTINGPYTHON001` diagnostic with preprocessor directives, apply the following code to your project:

```csharp
#pragma warning disable ASPIREHOSTINGPYTHON001
// API that is causing the warning.
#pragma warning restore ASPIREHOSTINGPYTHON001
```

## ASPIRECOSMOSDB001

<span id="ASPIRECOSMOSDB001"></span>

.NET Aspire provides a way to use the Cosmos DB Linux-based (preview) emulator. Since this emulator is in preview and the shape of this API is expected to change in the future, it's experimental (<xref:System.Diagnostics.CodeAnalysis.ExperimentalAttribute>). To suppress the compiler error/warning, use the following code:

To suppress the `ASPIRECOSMOSDB001` diagnostic with the <xref:System.Diagnostics.CodeAnalysis.SuppressMessageAttribute>, add the following code to your project file:

```xml
<PropertyGroup>
  <NoWarn>$(NoWarn);ASPIRECOSMOSDB001</NoWarn>
<PropertyGroup>
```

Alternatively, to suppress the `ASPIRECOSMOSDB001` diagnostic with preprocessor directives, apply the following code to your project:

```csharp
#pragma warning disable ASPIRECOSMOSDB001
// API that is causing the warning.
#pragma warning restore ASPIRECOSMOSDB001
```

## ASPIREPUBLISHERS001

<span id="ASPIREPUBLISHERS001"></span>

.NET Aspire introduced the concept of _Publishers_ starting in version 9.2. Publishers play a pivotal role in the deployment process, enabling the transformation of your distributed app into deployable assets. This alleviates the intermediate step of producing the publishing [manifest](../deployment/manifest-format.md) for tools to act on, instead empowering the developer to express their intent directly in C#. Publishers are currently in preview and the APIs are experimental (<xref:System.Diagnostics.CodeAnalysis.ExperimentalAttribute>). To suppress the compiler error/warning, use the following code:




## ASPIREAZURE001

<span id="ASPIREAZURE001"></span>

.NET Aspire introduced the concept of _Publishers_ starting in version 9.2. Publishers play a pivotal role in the deployment process, enabling the transformation of your distributed app into deployable assets. This alleviates the intermediate step of producing the publishing [manifest](../deployment/manifest-format.md) for tools to act on, instead empowering the developer to express their intent directly in C#. Publishers are currently in preview and the APIs are experimental (<xref:System.Diagnostics.CodeAnalysis.ExperimentalAttribute>). To suppress the compiler error/warning, use the following code:

To suppress the `ASPIREAZURE001` diagnostic with the <xref:System.Diagnostics.CodeAnalysis.SuppressMessageAttribute>, add the following code to your project file:

```xml
<PropertyGroup>
  <NoWarn>$(NoWarn);ASPIREAZURE001</NoWarn>
</PropertyGroup>
```

Alternatively, to suppress the `ASPIREAZURE001` diagnostic with preprocessor directives, apply the following code to your project:

```csharp
#pragma warning disable ASPIREAZURE001
// API that is causing the warning.
#pragma warning restore ASPIREAZURE001
```

The following APIs are experimental and might be removed or changed in future versions of .NET Aspire:

- <xref:Aspire.Hosting.AzurePublisherExtensions.AddAzurePublisher*>
- <xref:Aspire.Hosting.Azure.AzurePublisherOptions>

- <xref:Aspire.Hosting.DockerComposePublisherExtensions.AddDockerComposePublisher*>
- <xref:Aspire.Hosting.KubernetesPublisherExtensions.AddKubernetesPublisher*>

## ASPIREPROXYENDPOINTS001
