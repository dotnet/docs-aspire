---
title: "Breaking change - Role Assignments separated from Azure resource bicep"
description: "Learn about the breaking change in .NET Aspire 9.2 where role assignments are moved to separate bicep modules."
ms.date: 4/2/2025
ai-usage: ai-assisted
ms.custom: https://github.com/dotnet/docs-aspire/issues/2911
---

# Role Assignments separated from Azure resource bicep

In .NET Aspire 9.2, role assignments for Azure resources are no longer included in the same bicep file as the resource. Instead, they're moved to separate bicep modules. This change affects how role assignments are customized during infrastructure configuration.

## Version introduced

.NET Aspire 9.2

## Previous behavior

Previously, when an Azure resource's bicep file was generated, default role assignments were included in the same bicep module as the resource. This allowed customization of role assignments in the `ConfigureInfrastructure` callback. For example:

```csharp
var storage = builder.AddAzureStorage("storage")
    .ConfigureInfrastructure(infra =>
    {
        var roles = infra.GetProvisionableResources().OfType<RoleAssignment>().ToList();

        foreach (var role in roles)
        {
            infra.Remove(role);
        }

        var storageAccount = infra.GetProvisionableResources().OfType<StorageAccount>().Single();
        infra.Add(storageAccount.CreateRoleAssignment(StorageBuiltInRole.StorageBlobDataContributor, ...));
    });
```

## New behavior

Role assignments are now moved to their own bicep modules. The `ConfigureInfrastructure` callback no longer contains any `RoleAssignment` instances. Instead, role assignments are configured using the `WithRoleAssignments` API. For example:

```csharp
var storage = builder.AddAzureStorage("storage");

builder.AddProject<Projects.AzureContainerApps_ApiService>("api")
       .WithRoleAssignments(storage, StorageBuiltInRole.StorageBlobDataContributor);
```

## Type of breaking change

This is a [behavioral change](../categories.md#behavioral-change).

## Reason for change

This change was necessary to implement the `WithRoleAssignments` APIs, which provide a more structured and flexible way to configure role assignments per application.

## Recommended action

To customize role assignments in .NET Aspire 9.2, use the `WithRoleAssignments` API instead of relying on the `ConfigureInfrastructure` callback. Update your code as shown in the [preceding example](#new-behavior).

## Affected APIs

- <xref:Aspire.Hosting.AzureProvisioningResourceExtensions.ConfigureInfrastructure``1(Aspire.Hosting.ApplicationModel.IResourceBuilder{``0},System.Action{Aspire.Hosting.Azure.AzureResourceInfrastructure})>
