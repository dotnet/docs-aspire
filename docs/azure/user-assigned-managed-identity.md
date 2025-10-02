---
title: User-assigned managed identities
description: Learn how to use user-assigned managed identities in your Aspire applications to securely access Azure resources.
ms.date: 05/08/2025
---

# User-assigned managed identities in Aspire

In this article, you learn how to add or reference user-assigned managed identities (UMIs). You can add UMIs in your Aspire applications to securely access Azure resources. A UMI is a standalone Azure resource that you can assign to one or more service resources. UMIs give you more control over identity management and resource access.

## Add a user-assigned managed identity

To create a new user-assigned managed identity, use the `AddAzureUserAssignedIdentity` API in your distributed application builder:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var sharedMi = builder.AddAzureUserAssignedIdentity("custom-umi");

// After adding all resources, run the app...

builder.Build().Run();
```

The preceding code creates a new managed identity named "custom-umi" that you can use with other resources in your application.

## Reference an existing managed identity

If you already have a managed identity, you can reference it using the <xref:Aspire.Hosting.ExistingAzureResourceExtensions.PublishAsExisting*> method. This is useful when you want to use an identity that was created outside of your Aspire project.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var miName = builder.AddParameter("miName");
var miResourceGroup = builder.AddParameter("miResourceGroup");

var sharedMi = builder.AddAzureUserAssignedIdentity("custom-umi")
    .PublishAsExisting(miName, miResourceGroup);

// After adding all resources, run the app...

builder.Build().Run();
```

In the preceding example, you use parameters to provide the name and resource group of the existing identity. This allows you to reference the managed identity without creating a new one.

## Assign roles to managed identities

You can grant Azure roles to your managed identity using the WithRoleAssignments API. This lets your identity access other Azure resources, such as Azure Key Vault.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var sharedMi = builder.AddAzureUserAssignedIdentity("custom-umi");

builder.AddAzureKeyVault("secrets")
       .WithRoleAssignments(sharedMi, BuiltInRole.Reader);

// After adding all resources, run the app...

builder.Build().Run();
```

In this example, you give the Reader role to the managed identity for the Key Vault resource. For more information about role assignments,  see [Manage Azure role assignments](role-assignments.md).

## See also

- [Azure managed identities overview](/azure/active-directory/managed-identities-azure-resources/overview)
- [Azure Key Vault](/azure/key-vault/general/basic-concepts)
- [Manage Azure role assignments](role-assignments.md)
- [Aspire Azure integrations overview](integrations-overview.md)
