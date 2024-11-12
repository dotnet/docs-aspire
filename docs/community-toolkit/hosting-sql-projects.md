---
title: SQL Database Projects hosting
author: erikej
description: A .NET Aspire hosting integration for publishing SQL Database Projects from your AppHost.
ms.date: 11/12/2024
---

# .NET Aspire SQL Database Projects hosting integration

[!INCLUDE [includes-hosting](../includes/includes-hosting.md)]

[!INCLUDE [banner](includes/banner.md)]

In this article, you learn how to use the .NET Aspire SQL Database Projects hosting integration to publish your database schema to your SQL Server database.

## Prerequisites

This integration requires a SQL Database Project based on either [MSBuild.Sdk.SqlProj](https://github.com/rr-wfm/MSBuild.Sdk.SqlProj) or [Microsoft.Build.Sql](https://github.com/microsoft/DacFx).

## Hosting integration

To get started with the .NET Aspire SQL Database Projects hosting integration, install the [📦 CommunityToolkit.Aspire.Hosting.SqlDatabaseProjects](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.SqlDatabaseProjects) NuGet package in the [app host](xref:aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package CommunityToolkit.Aspire.Hosting.SqlDatabaseProjects
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="CommunityToolkit.Aspire.Hosting.SqlDatabaseProjects"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

## Example usage

Add a reference to the MSBuild.Sdk.SqlProj or Microsoft.Build.Sql project you want to publish in your .NET Aspire AppHost project:

```dotnetcli
dotnet add reference ../MySqlProj/MySqlProj.csproj
```

> Note: Adding this reference will currently result in warning ASPIRE004. This is a known issue and will be resolved in a future release.

Add the project as a resource to your .NET Aspire AppHost:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
                 .AddDatabase("test");

builder.AddSqlProject<Projects.MySqlProj>("mysqlproj")
       .WithReference(sql);
```

Now when you run your .NET Aspire AppHost project you will see the SQL Database Project being published to the specified SQL Server.

### Local .dacpac file support

If you are sourcing your .dacpac file from somewhere other than a project reference, you can also specify the path to the .dacpac file directly:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
                 .AddDatabase("test");

builder.AddSqlProject("mysqlproj")
       .WithDacpac("path/to/mysqlproj.dacpac")
       .WithReference(sql);
```

### Redeploy support

If you make changes to your SQL Database project while the AppHost is running, you can use the `Redeploy` custom action on the Aspire Dashboard to redeploy your updates without having to restart the AppHost.

## See also

- [Get started with SQL database projects](/sql/tools/sql-database-projects/get-started)
- [.NET Aspire Community Toolkit GitHub repo](https://github.com/CommunityToolkit/Aspire)
