---
title: .NET Aspire Azure SQL integration
description: Learn how to use the .NET Aspire Azure SQL integration, which includes both hosting and client integrations.
ms.date: 04/24/2025
uid: database/azure-sql-integration
---

# .NET Aspire Azure SQL integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[Azure SQL](https://azure.microsoft.com/products/azure-sql) is a family of relational database management systems that run in the Azure cloud. The database systems are Platform-as-a-Service (PaaS) products that enable database administrators to implement highly scalable and available databases without implementing complex infrastructures themselves.

> AJMTODO: describe Azure SQL Database, Azure SQL Managed Instance, SQL Server on Azure VM. Does the last one need this integration or the SQL Server one?

## Hosting integration

The Azure SQL hosting integration models the Azure SQL server as the <xref:Aspire.Hosting.Azure.AzureSqlServerResource> type and the database as the <xref:Aspire.Hosting.Azure.AzureSqlDatabaseResource> type. To access these types and APIs, add the [📦 Aspire.Hosting.Azure.Sql](https://www.nuget.org/packages/Aspire.Hosting.Azure.Sql) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Azure.Sql
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Azure.Sql"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

The Azure SQL hosting integration takes a dependency on the [📦 Aspire.Hosting.SqlServer](https://www.nuget.org/packages/Aspire.Hosting.SqlServer/) NuGet package, extending it to support Azure. Everything that you can do with the [.NET Aspire SQL Server integration](sql-server-integration.md) and [.NET Aspire SQL Server Entity Framework Core integration](sql-server-entity-framework-integration.md) you can also do with this integration.

### Add Azure SQL server resource and database resource

In your app host project, call <xref:Aspire.Hosting.AzureSqlExtensions.AddAzureSqlServer*> to add and return an Azure SQL server resource builder. Chain a call to the returned resource builder to <xref:Aspire.Hosting.AzureSqlExtensions.AddDatabase*>, to add an Azure SQL database resource.



