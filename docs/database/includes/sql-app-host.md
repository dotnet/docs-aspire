---
ms.topic: include
---

The SQL Server hosting integration models the server as the <xref:Aspire.Hosting.ApplicationModel.SqlServerServerResource> type and the database as the <xref:Aspire.Hosting.ApplicationModel.SqlServerDatabaseResource> type. To access these types and APIs, add the [📦 Aspire.Hosting.SqlServer](https://www.nuget.org/packages/Aspire.Hosting.SqlServer) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

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

var sql = builder.AddSqlServer("sql")
                 .WithLifetime(ContainerLifetime.Persistent);

var db = sql.AddDatabase("database");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(db);

// After adding all resources, run the app...
```

> [!NOTE]
> The SQL Server container is slow to start, so it's best to use a _persistent_ lifetime to avoid unnecessary restarts. For more information, see [Container resource lifetime](../../fundamentals/app-host-overview.md#container-resource-lifetime).

When .NET Aspire adds a container image to the app host, as shown in the preceding example with the `mcr.microsoft.com/mssql/server` image, it creates a new SQL Server instance on your local machine. A reference to your SQL Server (the `sql` variable) is added to the `ExampleProject`. The SQL Server resource includes default credentials with a `username` of `sa` and a random `password` generated using the <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.CreateDefaultPasswordParameter*> method.

The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method configures a connection in the `ExampleProject` named `database`.

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

> [!CAUTION]
> The password is stored in the data volume. When using a data volume and if the password changes, it will not work until you delete the volume.

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

### Connect to database resources

When the .NET Aspire app host runs, the server's database resources can be accessed from external tools, such as [SQL Server Management Studio (SSMS)](/sql/ssms/download-sql-server-management-studio-ssms) or [Azure Data Studio](/azure-data-studio/download-azure-data-studio). The connection string for the database resource is available in the dependent resources environment variables and is accessed using the [.NET Aspire dashboard: Resource details](../../fundamentals/dashboard/explore.md#resource-details) pane. The environment variable is named `ConnectionStrings__{name}` where `{name}` is the name of the database resource, in this example it's `database`. Use the connection string to connect to the database resource from external tools. Imagine that you have a database named `todos` with a single `dbo.Todos` table.

#### [SQL Server Management Studio](#tab/ssms)

To connect to the database resource from SQL Server Management Studio, follow these steps:

1. Open SSMS.
1. In the **Connect to Server** dialog, select the **Additional Connection Parameters** tab.
1. Paste the connection string into the **Additional Connection Parameters** field and select **Connect**.

    :::image type="content" source="media/ssms-new-connection.png" lightbox="media/ssms-new-connection.png" alt-text="SQL Server Management Studio: Connect to Server dialog.":::

1. If you're connected, you can see the database resource in the **Object Explorer**:

    :::image type="content" source="media/ssms-connected.png" lightbox="media/ssms-connected.png" alt-text="SQL Server Management Studio: Connected to database.":::

For more information, see [SQL Server Management Studio: Connect to a server](/sql/ssms/quickstarts/ssms-connect-query-sql-server).

#### [Azure Data Studio](#tab/azure-data-studio)

To connect to the database resource from Azure Data Studio, follow these steps:

1. Open Azure Data Studio.
1. Select the **New** dropdown and choose **New connection**.

    :::image type="content" source="media/ads-new-connection.png" lightbox="media/ads-new-connection.png" alt-text="Azure Data Studio: New / New connection screen capture.":::

1. Change the **Input type** to **Connection string** and paste the connection string into the **Connection string** field.
1. Select **Connect**.

    :::image type="content" source="media/ads-connect-details.png" lightbox="media/ads-connect-details.png" alt-text="Azure Data Studio: Connection string input details.":::

1. If you're connected, you can see the database resource in the active tab:

    :::image type="content" source="media/ads-connected.png" lightbox="media/ads-connected.png" alt-text="Azure Data Studio: Connected to database.":::

For more information, see [Azure Data Studio: Connect to SQL Server](/azure-data-studio/quickstart-sql-server).

---
