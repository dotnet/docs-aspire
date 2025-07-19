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

## Using with non-.NET applications

The SQL Server hosting integration can be used with any application technology, not just .NET applications. When you use <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> to reference a SQL Server resource, connection information is automatically injected as environment variables into the referencing application.

For applications that don't use the [client integration](#client-integration), you can access the connection information through environment variables. Here's an example of how to configure environment variables for a non-.NET application:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
    .WithLifetime(ContainerLifetime.Persistent);

var database = sql.AddDatabase("myDatabase");

// Example: Configure a non-.NET application with SQL Server access
var app = builder.AddExecutable("my-app", "python", "app.py", ".")
    .WithReference(database) // Provides ConnectionStrings__myDatabase
    .WithEnvironment(context =>
    {
        // Additional individual connection details as environment variables
        context.EnvironmentVariables["SQL_SERVER"] = sql.Resource.PrimaryEndpoint.Property(EndpointProperty.Host);
        context.EnvironmentVariables["SQL_PORT"] = sql.Resource.PrimaryEndpoint.Property(EndpointProperty.Port);
        context.EnvironmentVariables["SQL_USERNAME"] = "sa";
        context.EnvironmentVariables["SQL_PASSWORD"] = sql.Resource.PasswordParameter;
        context.EnvironmentVariables["SQL_DATABASE"] = database.Resource.DatabaseName;
    });

builder.Build().Run();
```

This configuration provides the non-.NET application with several environment variables:

- `ConnectionStrings__myDatabase`: The complete SQL Server connection string
- `SQL_SERVER`: The hostname/IP address of the SQL Server
- `SQL_PORT`: The port number the SQL Server is listening on
- `SQL_USERNAME`: The username (typically `sa` for SQL Server)
- `SQL_PASSWORD`: The dynamically generated password
- `SQL_DATABASE`: The name of the database

Your non-.NET application can then read these environment variables to connect to the SQL Server database using the appropriate database driver for that technology (for example, `pyodbc` for Python, `node-mssql` for Node.js, or `database/sql` with a SQL Server driver for Go).

## Client integration

[!INCLUDE [sql-server-client](includes/sql-server-client.md)]

## See also

- [Azure SQL Database](/azure/azure-sql/database)
- [SQL Server](/sql/sql-server)
- [.NET Aspire database containers sample](/samples/dotnet/aspire-samples/aspire-database-containers/)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
- [Azure SQL & SQL Server Aspire Samples](https://github.com/Azure-Samples/azure-sql-db-aspire)
