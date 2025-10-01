---
title: "Breaking change - KeyVault default role assignment changing from KeyVaultAdministrator to KeyVaultSecretsUser"
description: "Learn about the breaking change in Aspire 9.2 where the default role for Azure KeyVault applications changes to KeyVaultSecretsUser."
ms.date: 03/27/2025
ai-usage: ai-assisted
ms.custom: https://github.com/dotnet/docs-aspire/issues/2899
---

# KeyVault default role assignment changing from KeyVaultAdministrator to KeyVaultSecretsUser

In Aspire 9.2, the default role assigned to applications referencing Azure KeyVault has changed from <xref:Azure.Provisioning.KeyVault.KeyVaultBuiltInRole.KeyVaultAdministrator> to <xref:Azure.Provisioning.KeyVault.KeyVaultBuiltInRole.KeyVaultSecretsUser>. This change enhances security by limiting default privileges to only reading secrets. Applications requiring higher privileges must explicitly configure them.

## Version introduced

Aspire 9.2

## Previous behavior

Previously, applications referencing Azure KeyVault were automatically granted the `KeyVaultAdministrator` role, which allowed full management of KeyVault settings.

## New behavior

Applications referencing Azure KeyVault are now granted the `KeyVaultSecretsUser` role by default, which restricts access to reading secrets. If higher privileges are required, they can be configured using the `WithRoleAssignments` API.

Example:

```csharp
using Azure.Provisioning.KeyVault;

var kv = builder.AddAzureKeyVault("kv");

builder.AddProject<Projects.ApiService>("api")
       .WithRoleAssignments(kv, KeyVaultBuiltInRole.KeyVaultContributor);
```

## Type of breaking change

This is a [behavioral change](../categories.md#behavioral-change).

## Reason for change

The `KeyVaultAdministrator` role provides excessive privileges for most applications, as they typically only need to read secrets. Assigning the `KeyVaultSecretsUser` role by default improves security by adhering to the principle of least privilege.

## Recommended action

If your application requires higher privileges than the `KeyVaultSecretsUser` role, explicitly configure the necessary roles using the `WithRoleAssignments` API. For example:

```csharp
using Azure.Provisioning.KeyVault;

var kv = builder.AddAzureKeyVault("kv");

builder.AddProject<Projects.ApiService>("api")
       .WithRoleAssignments(kv, KeyVaultBuiltInRole.KeyVaultContributor);
```

## Affected APIs

- <xref:Aspire.Hosting.AzureKeyVaultResourceExtensions.AddAzureKeyVault*>
