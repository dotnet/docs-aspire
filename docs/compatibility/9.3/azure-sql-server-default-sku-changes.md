---
title: "Breaking change - Change the default SKU used for creating a new Azure SQL database"
description: "Learn about the breaking change in .NET Aspire 9.3 where the default SKU for Azure SQL database deployment now uses the free offer."
ms.date: 5/7/2025
ai-usage: ai-assisted
ms.custom: https://github.com/dotnet/docs-aspire/issues/3144
---

# Change the default SKU used for creating a new Azure SQL database

The default SKU for deploying a new Azure SQL database has been updated to take advantage of the Azure SQL Database free offer. This change helps avoid unexpected monthly costs by pausing the database when free offer limits are reached. A new method, `WithSku`, has also been introduced to allow customization of the SKU during deployment.

## Version introduced

.NET Aspire 9.3

## Previous behavior

The Azure SQL database was deployed with a default SKU that did not utilize the Azure SQL Database free offer. This could result in unexpected monthly costs if the database usage exceeded the free tier limits.

## New behavior

The Azure SQL database now deploys with the default SKU `GP_S_Gen5_2` (General Purpose Serverless) and the Azure SQL Database free offer enabled. The database pauses automatically when free offer limits are reached, preventing unexpected costs. Additionally, the `WithSku` method allows you to specify a different SKU if needed.

Example usage of the new `WithSku` method:

```csharp
var database = resourceBuilder
    .AddDatabase("MyDatabase")
    .WithSku("GP_S_Gen5_4");
```

## Type of breaking change

This is a [behavioral change](../../categories.md#behavioral-change).

## Reason for change

This change ensures that deployments take advantage of the Azure SQL Database free offer, reducing the risk of incurring unexpected monthly costs. For more details, see the [Azure SQL Database free offer documentation](https://learn.microsoft.com/azure/azure-sql/database/free-offer?view=azuresql).

## Recommended action

If you want to use a different SKU for your Azure SQL database deployment, use the new `WithSku` method to specify the desired SKU. For example:

```csharp
var database = resourceBuilder
    .AddDatabase("MyDatabase")
    .WithSku("GP_S_Gen5_4");
```

Review your existing deployments to ensure they align with the new default behavior.

## Affected APIs

- <xref:Aspire.Hosting.Azure.AzureSqlDatabaseResource>: Updated to use the free offer as the default deployment option.
- <xref:Aspire.Hosting.AzureSqlExtensions.AddDatabase*>: Updated to use the free offer as the default deployment option.
- `public static IResourceBuilder<AzureSqlDatabaseResource> WithSku`: Added to allow specifying the desired SKU.
