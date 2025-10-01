---
title: "Breaking change - Deprecating various known parameters in AzureBicepResource"
description: "Learn about the breaking change in Aspire where known parameter injection in AzureBicepResource has been deprecated."
ms.date: 7/4/2025
ai-usage: ai-assisted
ms.custom: https://github.com/dotnet/docs-aspire/issues/3675
---

# Deprecating various known parameters in AzureBicepResource

Known parameter injection in `AzureBicepResource` has been deprecated. The parameters `AzureBicepResource.KnownParameters.KeyVaultName`, `AzureBicepResource.KnownParameters.LogAnalyticsWorkspaceId`, `containerAppEnvironmentId`, and `containerAppEnvironmentName` are now marked as `[Obsolete]` and ignored during infrastructure generation. Developers must explicitly model these resources or use new helper APIs to ensure proper configuration.

## Version introduced

This change applies to Aspire starting from version 7.4.2025.

## Previous behavior

Previously, Aspire automatically injected the following known parameters into Bicep templates at build time without requiring explicit assignment:

| Bicep parameter name | Known parameter constant | Typical use |
|--|--|--|
| `keyVaultName` | `KeyVaultName` | Reference a Key Vault for secrets mapped to `AzureBicepResource.GetSecretOutput`. |
| `logAnalyticsWorkspaceId` | `LogAnalyticsWorkspaceId` | Reference the Log Analytics Workspace ID associated with the environment. |
| `containerAppEnvironmentId` | N/A | Reference the container app environment ID. |
| `containerAppEnvironmentName` | N/A | Reference the container app environment name. |

For example:

```bicep
// keyvault.bicep
param keyVaultName string
resource kv 'Microsoft.KeyVault/vaults@2023-07-01' = {
  name: keyVaultName
  ...
}
```

```csharp
builder.AddBicepTemplateFile("kv", "keyvault.bicep"); // No parameter assignment needed
builder.AddContainer("api", "image");
```

Aspire resolved these parameters automatically, allowing templates to deploy without explicit wiring.

## New behavior

Aspire no longer pre-populates the deprecated parameters. If a Bicep template declares any of these parameters without explicit assignment, deployment fails with an "undefined parameter" error. Developers must now explicitly model resources and pass their values to templates.

For example:

```csharp
var env = builder.AddAzureContainerAppEnvironment("env");
var kv  = builder.AddAzureKeyVault("kv");
var la  = builder.AddAzureLogAnalyticsWorkspace("la");

builder.AddBicepTemplateFile("kvTemplate", "keyvault.bicep")
       .WithParameter("keyVaultName", kv.NameOutputReference);

builder.AddBicepTemplateFile("apiTemplate", "api.bicep")
       .WithParameter("containerAppEnvironmentName", env.NameOutputReference);
```

Inside the Bicep template:

```bicep
param containerAppEnvironmentName string

resource env 'Microsoft.App/managedEnvironments@2024-03-01' existing = {
  name: containerAppEnvironmentName
}

var environmentId = env.id
```

## Type of breaking change

This is both a [source incompatible](../categories.md#source-compatibility) and [behavioral](../categories.md#behavioral-change) change.

## Reason for change

Aspire now supports modeling multiple compute environments in a single application graph. Automatically injecting global parameters created ambiguity, hid dependencies, and complicated debugging. This change enforces explicit wiring, ensuring predictable behavior and enabling future scenarios where resources target specific environments.

## Recommended action

1. **Stop using obsolete constants**

   Remove any code that relies on `AzureBicepResource.KnownParameters.KeyVaultName`, `AzureBicepResource.KnownParameters.LogAnalyticsWorkspaceId`, `containerAppEnvironmentId`, or `containerAppEnvironmentName`.

1. **Model resources explicitly**

   Define resources like Key Vaults, Log Analytics Workspaces, and Container App Environments explicitly in your code.

   ```csharp
   var env = builder.AddAzureContainerAppEnvironment("env");
   var kv = builder.AddAzureKeyVault("kv");
   var la = builder.AddAzureLogAnalyticsWorkspace("la");
   ```

1. **Pass parameters explicitly**

   Use strongly-typed properties like `NameOutputReference` to pass resource values to templates.

   ```csharp
   builder.AddBicepTemplateFile("template", "file.bicep")
          .WithParameter("keyVaultName", kv.NameOutputReference);
   ```

1. **Address warnings**

   Update code to resolve `[Obsolete]` warnings by replacing deprecated constants with explicit resource definitions.

## Affected APIs

- `AzureBicepResource.KnownParameters.KeyVaultName`: Obsolete.
- `AzureBicepResource.KnownParameters.LogAnalyticsWorkspaceId`: Obsolete.
