---
title: "Breaking change - Hybrid compute mode between azd and Aspire apps dropped"
description: "Learn about the breaking change in Aspire 9.4 where hybrid compute mode support between azd and Aspire apps was dropped."
ms.date: 07/22/2025
ai-usage: ai-assisted
ms.custom: https://github.com/dotnet/docs-aspire/issues/3669
---

# Hybrid compute mode between azd and Aspire apps dropped

Aspire 9.4 removes hybrid compute mode. In this mode, Azure Developer CLI (azd) creates the Azure Container Apps Environment, and Aspire generates the individual Container App Bicep modules. Projects that rely on azd-owned environments must now model an `AzureContainerAppEnvironmentResource` inside the Aspire application.

## Version introduced

Aspire 9.4

## Previous behavior

You use `PublishAsAzureContainerApp()` to automatically connect to infrastructure that azd generates when you deploy:

```csharp
builder.AddContainer("api", "image")
       .PublishAsAzureContainerApp((infra, app) => { })     // ← automatically added infra
       .WithReference(db);
```

The `PublishAsAzureContainerApp()` method would automatically handle the connection to the Azure Container Apps Environment created by azd.

## New behavior

You must now explicitly add an Azure Container Apps Environment resource to your Aspire application:

```csharp
builder.AddAzureContainerAppEnvironment("env");   // Required to target ACA
builder.AddContainer("api", "image")
       .WithReference(db);
```

The environment resource must be explicitly defined before you can deploy containers to Azure Container Apps.

## Type of breaking change

This is a [behavioral change](../categories.md#behavioral-change).

## Reason for change

The hybrid path created hidden coupling and ambiguity:

- Known-parameter substitution (Key Vault names, volume storage accounts, etc.) couldn't be resolved reliably when multiple compute environments existed.
- The change aligns Aspire with the "compute-environment-as-resource" design and simplifies infrastructure generation logic.

## Recommended action

Add an environment resource to your Aspire application using the `AddAzureContainerAppEnvironment` method:

```csharp
builder.AddAzureContainerAppEnvironment("env");
```

This explicit declaration ensures that your application has a clear, unambiguous reference to the Azure Container Apps Environment.

## Affected APIs

- `PublishAsAzureContainerApp`
