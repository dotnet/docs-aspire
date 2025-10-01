---
title: SQL Database Projects hosting
author: erikej
description: A Aspire hosting integration for publishing SQL Database Projects from your AppHost.
ms.date: 11/12/2024
---

# Aspire SQL Database Projects hosting integration

[!INCLUDE [includes-hosting](../includes/includes-hosting.md)]

[!INCLUDE [banner](includes/banner.md)]

In this article, you learn how to use the Aspire SQL Database Projects hosting integration to publish your database schema to your SQL Server database.

## Prerequisites

This integration requires a SQL Database Project based on either [MSBuild.Sdk.SqlProj](https://github.com/rr-wfm/MSBuild.Sdk.SqlProj) or [Microsoft.Build.Sql](https://github.com/microsoft/DacFx).

## Hosting integration

To get started with the Aspire SQL Database Projects hosting integration, install the [📦 CommunityToolkit.Aspire.Hosting.SqlDatabaseProjects](https://nuget.org/packages/CommunityToolkit.Aspire.Hosting.SqlDatabaseProjects) NuGet package in the AppHost project.

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

Add a reference to the [📦 MSBuild.Sdk.SqlProj](https://www.nuget.org/packages/MSBuild.Sdk.SqlProj) or [📦 Microsoft.Build.Sql](https://www.nuget.org/packages/Microsoft.Build.Sql) project you want to publish in your Aspire AppHost project:

```dotnetcli
dotnet add reference ../MySqlProj/MySqlProj.csproj
```

> [!NOTE]
> Adding this reference will currently result in warning `ASPIRE004` on the project due to how references are parsed. The Aspire team is aware of this and we're working on a cleaner solution.

Add the project as a resource to your Aspire AppHost:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
                 .AddDatabase("test");

builder.AddSqlProject<Projects.MySqlProj>("mysqlproj")
       .WithReference(sql);
```

Now when you run your Aspire AppHost project you see the SQL Database Project being published to the specified SQL Server.

## NuGet Package support

Starting with version 9.2.0, you can deploy databases from referenced NuGet packages, such as those produced by [📦 MSBuild.Sdk.SqlProj](https://www.nuget.org/packages/MSBuild.Sdk.SqlProj) or [📦 Microsoft.Build.Sql](https://www.nuget.org/packages/Microsoft.Build.Sql). To deploy, add the NuGet package to your Aspire AppHost project, for example:

```dotnetcli
dotnet add package ErikEJ.Dacpac.Chinook
```

Next, edit your project file to set the `IsAspirePackageResource` flag to `True` for the corresponding `PackageReference`, as shown in the following example:

```xml
<PackageReference Include="ErikEJ.Dacpac.Chinook" Version="1.0.0"
                  IsAspirePackageResource="True" />
```

Finally, add the package as a resource to your app model:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
                 .AddDatabase("test");

builder.AddSqlPackage<Packages.ErikEJ_Dacpac_Chinook>("chinook")
       .WithReference(sql);
```

> [!NOTE]
> By default, the _.dacpac_ is expected to be located under `tools/<package-id>.dacpac`. In the preceding example, the _tools/ErikEJ.Dacpac.Chinook.dacpac_ path is expected. If for whatever reason the _.dacpac_ is under a different path within the package you can use `WithDacpac("relative/path/to/some.dacpac")` API to specify a path relative to the root of AppHost project directory.

### Local .dacpac file support

If you are sourcing your _.dacpac_ file from somewhere other than a project reference, you can also specify the path to the _.dacpac_ file directly:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
                 .AddDatabase("test");

builder.AddSqlProject("mysqlproj")
       .WithDacpac("path/to/mysqlproj.dacpac")
       .WithReference(sql);
```

## Support for existing SQL Server instances

Starting with version 9.2.0, you can publish the SQL Database project to an existing SQL Server instance by using a connection string:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Get an existing SQL Server connection string from the configuration
var connection = builder.AddConnectionString("Aspire");

builder.AddSqlProject<Projects.SdkProject>("mysqlproj")
       .WithReference(connection);

builder.Build().Run();
```

### Deployment options support

To define options that affect the behavior of package deployment, call the `WithConfigureDacDeployOptions` API:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
                 .AddDatabase("test");

builder.AddSqlProject("mysqlproj")
       .WithConfigureDacDeployOptions(options => options.IncludeCompositeObjects = true)
       .WithReference(sql);

builder.Build().Run();
```

The preceding code:

- Adds a SQL server resource named `sql` and adds a `test` database resource to it.
- Adds a SQL project resource named `mysqlproj` and then configures the <xref:Microsoft.SqlServer.Dac.DacDeployOptions>.
- The SQL project resource depends on the database resource.

### Redeploy support

If you make changes to your SQL Database project while the AppHost is running, you can use the `Redeploy` custom action on the Aspire dashboard to redeploy your updates without having to restart the AppHost.

## See also

- [MSBuild.Sdk.SqlProj GitHub repository](https://github.com/rr-wfm/MSBuild.Sdk.SqlProj)
- [Microsoft.Build.Sql GitHub repository](https://github.com/microsoft/DacFx)
- [Get started with SQL database projects](/sql/tools/sql-database-projects/get-started)
- [Aspire Community Toolkit GitHub repo](https://github.com/CommunityToolkit/Aspire)
