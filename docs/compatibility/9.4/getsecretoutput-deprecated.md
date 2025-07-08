---
title: "Breaking change - BicepSecretOutputReference and GetSecretOutput are now obsolete"
description: "Learn about the breaking change in .NET Aspire 9.4 where BicepSecretOutputReference, GetSecretOutput, and related automatic Key Vault logic are deprecated."
ms.date: 07/08/2025
ai-usage: ai-assisted
ms.custom: https://github.com/dotnet/docs-aspire/issues/3670
---

# BicepSecretOutputReference and GetSecretOutput are now obsolete

In .NET Aspire 9.4, the `BicepSecretOutputReference` type, the `GetSecretOutput(...)` helper method, and the overload of `WithEnvironment` that accepted a `BicepSecretOutputReference` are now obsolete. Automatic Key Vault generation and secret wiring logic were removed. Projects that relied on these APIs for automatic secret management must migrate to explicit Key Vault resource modeling and secret references.

## Version introduced

.NET Aspire 9.4

## Previous behavior

Previously, you could use `GetSecretOutput(...)` to obtain a `BicepSecretOutputReference` from a resource, and pass it to `WithEnvironment`. Aspire would automatically generate a Key Vault and wire up the secret URI for you.

Example:

```csharp
var db = builder.AddAzureCosmosDB("mydb").WithAccessKeyAuthentication();

builder.AddContainer("api", "image")
       .WithEnvironment("ConnStr", db.GetSecretOutput("connectionString"));
```

## New behavior

Now, Aspire no longer creates Key Vaults or secrets automatically. You must explicitly create or reference a Key Vault and use an explicit secret reference.

Example:

```csharp
var kv = builder.AddAzureKeyVault("kv");
builder.AddContainer("api", "image")
       .WithEnvironment("ConnStr", kv.GetSecret("connectionString"));
```

`GetSecretOutput(...)` is now obsolete and will be removed in a future release. The overload of `WithEnvironment` that accepted a `BicepSecretOutputReference` is also obsolete.

## Type of breaking change

This change is a [binary incompatible](../categories.md#binary-compatibility) and [source incompatible](../categories.md#source-compatibility) change.

## Reason for change

Implicit Key Vault creation made deployments opaque and fragile. Removing the secret-output shortcut aligns Aspire with its explicit-resource philosophy, giving you full control over secret management and simplifying infrastructure generation. For more information, see the [GitHub issue](https://github.com/dotnet/docs-aspire/issues/3670).

## Recommended action

1. Create or reference a Key Vault in your Aspire graph:

   ```csharp
   var kv = builder.AddAzureKeyVault("kv");
   ```

1. Replace `GetSecretOutput` usage with an explicit secret reference:

   ```csharp
   builder.AddContainer("api", "image")
          .WithEnvironment("ConnStr", kv.GetSecret("connectionString"));
   ```

1. Remove obsolete `WithEnvironment(string, BicepSecretOutputReference)` overloads and switch to `WithEnvironment(string, IAzureKeyVaultSecretReference)` (or another appropriate overload).

Aspire's resources with support for keys were updated to handle this new change.

## Affected APIs

- <xref:Aspire.Hosting.Azure.BicepSecretOutputReference?displayProperty=fullName>
- <xref:Aspire.Hosting.AzureBicepResourceExtensions.GetSecretOutput(Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.Azure.AzureBicepResource},System.String)?displayProperty=fullName>
- <xref:Aspire.Hosting.AzureBicepResourceExtensions.WithEnvironment``1(Aspire.Hosting.ApplicationModel.IResourceBuilder{``0},System.String,Aspire.Hosting.Azure.BicepSecretOutputReference)?displayProperty=fullName>
- Automatic Key Vault generation and secret wiring logic (removed)
