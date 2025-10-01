---
title: "Breaking change - AddAzureSqlServer assigns a dedicated user-assigned managed identity as the administrator"
description: "Learn about the breaking change in Aspire 9.3 where Azure SQL Server instances are assigned dedicated user managed identities as administrators."
ms.date: 5/12/2025
ai-usage: ai-assisted
ms.custom: https://github.com/dotnet/docs-aspire/issues/3386
---

# AddAzureSqlServer assigns a dedicated user-assigned managed identity as the administrator

In Aspire 9.3, Azure SQL Server instances are now assigned dedicated user managed identities as administrators. This change resolves issues with overlapping managed identities when using multiple app containers. Additionally, app containers and local users are granted the `db_owner` role for database access.

## Version introduced

Aspire 9.3

## Previous behavior

In Aspire 9.2, each container app was assigned its own managed identity as the administrator. However, when multiple app containers were used, the second container would overwrite the administrator role of the first, causing access issues.

## New behavior

In Aspire 9.3, each Azure SQL Server instance is assigned a dedicated user managed identity as its administrator. App containers using these SQL Server instances are granted the `db_owner` role during deployment. If an application isn't deployed as an app container, the current Entra ID user account is also added as a `db_owner` in the database, enabling data management.

To prevent automatic configuration, the <xref:Aspire.Hosting.ExistingAzureResourceExtensions.AsExisting*> method can be used on the Azure SQL Server resource. Existing instances aren't reconfigured.

## Type of breaking change

This is a [behavioral change](../categories.md#behavioral-change).

## Reason for change

This change resolves a limitation where multiple app containers caused conflicts in administrator assignments for Azure SQL Server instances. It ensures each instance has a dedicated administrator and improves role assignment consistency.

## Recommended action

No recommended action is required if the new behavior aligns with your requirements. However, if you prefer the previous behavior, you can use the `AsExisting` method to prevent automatic configuration of the Azure SQL Server instance. For example:

```csharp
var builder = DistributedApplication.CreateBuilder();

var existingSqlServerName = builder.AddParameter("existingSqlServerName");
var existingSqlServerResourceGroup = builder.AddParameter("existingSqlServerResourceGroup");

var sql = builder.AddAzureSqlServer("sql")
                 .AsExisting(existingSqlServerName, existingSqlServerResourceGroup);

// Use the existing SQL Server using WithReference...

builder.Build().Run();
```

## Affected APIs

- <xref:Aspire.Hosting.AzureContainerAppContainerExtensions.PublishAsAzureContainerApp``1(Aspire.Hosting.ApplicationModel.IResourceBuilder{``0},System.Action{Aspire.Hosting.Azure.AzureResourceInfrastructure,Azure.Provisioning.AppContainers.ContainerApp})>
- <xref:Aspire.Hosting.AzureContainerAppExecutableExtensions.PublishAsAzureContainerApp``1(Aspire.Hosting.ApplicationModel.IResourceBuilder{``0},System.Action{Aspire.Hosting.Azure.AzureResourceInfrastructure,Azure.Provisioning.AppContainers.ContainerApp})>
- <xref:Aspire.Hosting.AzureContainerAppExtensions.AddAzureContainerAppsInfrastructure(Aspire.Hosting.IDistributedApplicationBuilder)>
- <xref:Aspire.Hosting.AzureContainerAppProjectExtensions.PublishAsAzureContainerApp``1(Aspire.Hosting.ApplicationModel.IResourceBuilder{``0},System.Action{Aspire.Hosting.Azure.AzureResourceInfrastructure,Azure.Provisioning.AppContainers.ContainerApp})>
- <xref:Aspire.Hosting.SqlServerBuilderExtensions.AddSqlServer(Aspire.Hosting.IDistributedApplicationBuilder,System.String,Aspire.Hosting.ApplicationModel.IResourceBuilder{Aspire.Hosting.ApplicationModel.ParameterResource},System.Nullable{System.Int32})>
