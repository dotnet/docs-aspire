---
title: .NET Aspire SQL Server integration
description: Learn how to use the .NET Aspire SQL Server integration, which includes both hosting and client integrations.
ms.date: 02/07/2025
uid: database/sql-server-integration
---

# .NET Aspire SQL Server integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[SQL Server](https://www.microsoft.com/sql-server) is a relational database management system developed by Microsoft. The .NET Aspire SQL Server integration enables you to connect to existing SQL Server instances or create new instances from .NET with the [`mcr.microsoft.com/mssql/server` container image](https://hub.docker.com/_/microsoft-mssql-server).

## Hosting integration

[!INCLUDE [sql-app-host](includes/sql-app-host.md)]

### Hosting integration health checks

The SQL Server hosting integration automatically adds a health check for the SQL Server resource. The health check verifies that the SQL Server is running and that a connection can be established to it.

The hosting integration relies on the [📦 AspNetCore.HealthChecks.SqlServer](https://www.nuget.org/packages/AspNetCore.HealthChecks.SqlServer) NuGet package.

## Client integration

[!INCLUDE [sql-server-client](includes/sql-server-client.md)]

## See also

- [Azure SQL Database](/azure/azure-sql/database)
- [SQL Server](/sql/sql-server)
- [.NET Aspire database containers sample](/samples/dotnet/aspire-samples/aspire-database-containers/)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
- [Azure SQL & SQL Server Aspire Samples](https://github.com/Azure-Samples/azure-sql-db-aspire)
