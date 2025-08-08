---
title: .NET Aspire Azure SQL integration
description: Learn how to use the .NET Aspire Azure SQL integration, which includes both hosting and client integrations.
ms.date: 04/30/2025
uid: database/azure-sql-integration
---

# .NET Aspire Azure SQL integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[Azure SQL](https://azure.microsoft.com/products/azure-sql) is a family of relational database management systems that run in the Azure cloud. The database systems are Platform-as-a-Service (PaaS) products that enable database administrators to implement highly scalable and available databases without maintaining complex infrastructures themselves. The .NET Aspire Azure SQL Server Hosting integration provides methods to create a new Azure Database server and databases from code in your .NET Aspire AppHost project. In a consuming project, you can use the .NET Aspire SQL Server client integration as you would for any other SQL Server instance.

## Hosting integration

[!INCLUDE [azure-sql-hosting](includes/azure-sql-hosting.md)]

## Client integration

[!INCLUDE [sql-server-client](includes/sql-server-client.md)]

## See also

- [Azure SQL Database](/azure/azure-sql/database)
- [.NET Aspire database containers sample](/samples/dotnet/aspire-samples/aspire-database-containers/)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
- [Azure SQL & SQL Server Aspire Samples](https://github.com/Azure-Samples/azure-sql-db-aspire)
