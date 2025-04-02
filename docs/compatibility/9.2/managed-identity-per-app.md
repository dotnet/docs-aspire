---
title: "Breaking change - AzureContainerApps infrastructure creates managed identity per container app"
description: "Learn about the breaking change in .NET Aspire 9.2 where each ContainerApp now has its own managed identity."
ms.date: 4/2/2025
ai-usage: ai-assisted
ms.custom: https://github.com/dotnet/docs-aspire/issues/2914
---

# Azure Container Apps managed identity changes

Starting with .NET Aspire 9.2, each Azure Container App created using [📦 Aspire.Hosting.Azure.AppContainers](https://www.nuget.org/packages/Aspire.Hosting.Azure.AppContainers) NuGet package now has its own Azure Managed Identity. This change enables more granular role assignments for Azure resources but might require updates to applications that rely on shared managed identities.

## Version introduced

.NET Aspire 9.2

## Previous behavior

All ContainerApps shared a single Azure Managed Identity. This allowed applications to interact with Azure resources using a common identity.

## New behavior

Each ContainerApp now has its own unique Azure Managed Identity. This enables applications to have distinct role assignments for different Azure resources.

## Type of breaking change

This is a [behavioral change](../categories.md#behavioral-change).

## Reason for change

This change was introduced to support scenarios where applications require different role assignments for different Azure resources. By assigning a unique managed identity to each ContainerApp, applications can now operate with more granular access control.

## Recommended action

### Azure SQL Server

Grant access to all Azure Managed Identities that need to interact with the database. Follow the guidance in [Configure and manage Azure AD authentication with Azure SQL](/azure/azure-sql/database/authentication-aad-configure).

### Azure PostgreSQL

Grant necessary privileges to all Azure Managed Identities that need to interact with the database. Use the PostgreSQL documentation on [granting privileges](https://www.postgresql.org/docs/current/ddl-priv.html) as a reference. For example:

```sql
GRANT INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA public TO <managed_identity_user>;
```

## Affected APIs

- `Aspire.Hosting.AzureContainerAppExtensions.AddAzureContainerAppsInfrastructure`
- `Aspire.Hosting.AzureContainerAppProjectExtensions.PublishAsAzureContainerApp`
- `Aspire.Hosting.AzureContainerAppExecutableExtensions.PublishAsAzureContainerApp`
- `Aspire.Hosting.AzureContainerAppContainerExtensions.PublishAsAzureContainerApp`