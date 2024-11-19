---
ms.topic: include
---

The SQL Server hosting integration models a SQL Server as the <xref:Aspire.Hosting.ApplicationModel.SqlServerServerResource> type and the database as the <xref:Aspire.Hosting.ApplicationModel.SqlServerDatabaseResource> type. To access these types and APIs that allow you to add it to your [📦 Aspire.Hosting.SqlServer](https://www.nuget.org/packages/Aspire.Hosting.SqlServer) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.SqlServer
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.SqlServer"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add SQL Server resource and database resource

In your app host project, call <xref:Aspire.Hosting.SqlServerBuilderExtensions.AddSqlServer*> and <xref:Aspire.Hosting.SqlServerBuilderExtensions.AddDatabase*> on the `builder` instance to add a SQL Server resource and SQL Server database resource, respectively.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql");
var db = sql.AddDatabase("database");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(db);

// After adding all resources, run the app...
```

When .NET Aspire adds a container image to the app host, as shown in the preceding example with the `mcr.microsoft.com/mssql/server` image, it creates a new SQL Server instance on your local machine. A reference to your SQL Server (the `sql` variable) is added to the `ExampleProject`. The SQL Server resource includes default credentials with a `username` of `"sa"` and a randomly generated `password` using the <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.CreateDefaultPasswordParameter*> method.

The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method configures a connection in the `ExampleProject` named `"database"`. For more information, see [Container resource lifecycle](../../fundamentals/app-host-overview.md#container-resource-lifecycle).

> [!TIP]
> If you'd rather connect to an existing SQL Server, call <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> instead. For more information, see [Reference existing resources](../../fundamentals/app-host-overview.md#reference-existing-resources).

### Add SQL Server resource with data volume

To add a data volume to the SQL Server resource, call the <xref:Aspire.Hosting.SqlServerBuilderExtensions.WithDataVolume*> method on the SQL Server resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
                 .WithDataVolume(isReadOnly: false);

var db = sql.AddDatabase("database");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(db);

// After adding all resources, run the app...
```

The data volume is used to persist the SQL Server data outside the lifecycle of its container. The data volume is mounted at the `/var/opt/mssql` path in the SQL Server container and when a `name` parameter isn't provided, the name is generated at random. For more information on data volumes and details on why they're preferred over [bind mounts](#add-sql-server-resource-with-data-bind-mount), see [Docker docs: Volumes](https://docs.docker.com/engine/storage/volumes).

### Add SQL Server resource with data bind mount

To add a data bind mount to the SQL Server resource, call the <xref:Aspire.Hosting.SqlServerBuilderExtensions.WithDataBindMount*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var sql = builder.AddSqlServer("sql")
                 .WithDataBindMount(
                     source: @"C:\SqlServer\Data",
                     isReadOnly: false);

var db = sql.AddDatabase("database");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(db);

// After adding all resources, run the app...
```

[!INCLUDE [data-bind-mount-vs-volumes](../../includes/data-bind-mount-vs-volumes.md)]

Data bind mounts rely on the host machine's filesystem to persist the SQL Server data across container restarts. The data bind mount is mounted at the `C:\SqlServer\Data` on Windows (or `/SqlServer/Data` on Unix) path on the host machine in the SQL Server container. For more information on data bind mounts, see [Docker docs: Bind mounts](https://docs.docker.com/engine/storage/bind-mounts).

### Add SQL Server resource with parameters

When you want to explicitly provide the password used by the container image, you can provide these credentials as parameters. Consider the following alternative example:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var password = builder.AddParameter("password", secret: true);

var sql = builder.AddSqlServer("sql", password);
var db = sql.AddDatabase("database");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(db);

// After adding all resources, run the app...
```

For more information on providing parameters, see [External parameters](../../fundamentals/external-parameters.md).
