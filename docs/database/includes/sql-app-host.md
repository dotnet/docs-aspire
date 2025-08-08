---
ms.topic: include
ms.custom: sfi-ropc-nochange
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

In your AppHost project, call <xref:Aspire.Hosting.SqlServerBuilderExtensions.AddSqlServer*> to add and return a SQL Server resource builder. Chain a call to the returned resource builder to <xref:Aspire.Hosting.SqlServerBuilderExtensions.AddDatabase*>, to add SQL Server database resource.

:::code language="csharp" source="../snippets/sql-server-integration/AspireApp.AppHost/AppHost.cs":::

> [!NOTE]
> The SQL Server container is slow to start, so it's best to use a _persistent_ lifetime to avoid unnecessary restarts. For more information, see [Container resource lifetime](../../fundamentals/orchestrate-resources.md#container-resource-lifetime).

When .NET Aspire adds a container image to the AppHost, as shown in the preceding example with the `mcr.microsoft.com/mssql/server` image, it creates a new SQL Server instance on your local machine. A reference to your SQL Server resource builder (the `sql` variable) is used to add a database. The database is named `database` and then added to the `ExampleProject`.

When adding a database resource to the app model, the database is created if it doesn't already exist. The creation of the database relies on the [app host eventing APIs](../../app-host/eventing.md), specifically <xref:Aspire.Hosting.ApplicationModel.ResourceReadyEvent>. In other words, when the `sql` resource is _ready_, the event is raised and the database resource is created.

The SQL Server resource includes default credentials with a `username` of `sa` and a random `password` generated using the <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.CreateDefaultPasswordParameter*> method.

When the AppHost runs, the password is stored in the app host's secret store. It's added to the `Parameters` section, for example:

```json
{
  "Parameters:sql-password": "<THE_GENERATED_PASSWORD>"
}
```

The name of the parameter is `sql-password`, but really it's just formatting the resource name with a `-password` suffix. For more information, see [Safe storage of app secrets in development in ASP.NET Core](/aspnet/core/security/app-secrets) and [Add SQL Server resource with parameters](#add-sql-server-resource-with-parameters).

The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method configures a connection in the `ExampleProject` named `database`.

> [!TIP]
> If you'd rather connect to an existing SQL Server, call <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> instead. For more information, see [Reference existing resources](../../fundamentals/app-host-overview.md#reference-existing-resources).

### Add SQL Server resource with database scripts

By default, when you add a <xref:Aspire.Hosting.ApplicationModel.SqlServerDatabaseResource>, it relies on the following SQL script to create the database:

```sql
IF
(
    NOT EXISTS
    (
        SELECT 1
        FROM sys.databases
        WHERE name = @DatabaseName
    )
)
CREATE DATABASE [<QUOTED_DATABASE_NAME>];
```

To alter the default script, chain a call to the <xref:Aspire.Hosting.SqlServerBuilderExtensions.WithCreationScript*> method on the database resource builder:

:::code language="csharp" source="../snippets/sql-server-creation-script/AspireApp.AppHost/AppHost.cs":::

The preceding example creates a database named `app_db` with a single `todos` table. The SQL script is executed when the database resource is created. The script is passed as a string to the `WithCreationScript` method, which is then executed in the context of the SQL Server resource.

### Add SQL Server resource with data volume

To add a data volume to the SQL Server resource, call the <xref:Aspire.Hosting.SqlServerBuilderExtensions.WithDataVolume*> method on the SQL Server resource:

:::code language="csharp" source="../snippets/sql-server-data-volume/AspireApp.AppHost/AppHost.cs":::

The data volume is used to persist the SQL Server data outside the lifecycle of its container. The data volume is mounted at the `/var/opt/mssql` path in the SQL Server container and when a `name` parameter isn't provided, the name is generated at random. For more information on data volumes and details on why they're preferred over [bind mounts](#add-sql-server-resource-with-data-bind-mount), see [Docker docs: Volumes](https://docs.docker.com/engine/storage/volumes).

> [!WARNING]
> The password is stored in the data volume. When using a data volume and if the password changes, it will not work until you delete the volume.

### Add SQL Server resource with data bind mount

To add a data bind mount to the SQL Server resource, call the <xref:Aspire.Hosting.SqlServerBuilderExtensions.WithDataBindMount*> method:

:::code language="csharp" source="../snippets/sql-server-bind-mount/AspireApp.AppHost/AppHost.cs":::

[!INCLUDE [data-bind-mount-vs-volumes](../../includes/data-bind-mount-vs-volumes.md)]

Data bind mounts rely on the host machine's filesystem to persist the SQL Server data across container restarts. The data bind mount is mounted at the `C:\SqlServer\Data` on Windows (or `/SqlServer/Data` on Unix) path on the host machine in the SQL Server container. For more information on data bind mounts, see [Docker docs: Bind mounts](https://docs.docker.com/engine/storage/bind-mounts).

### Add SQL Server resource with parameters

When you want to explicitly provide the password used by the container image, you can provide these credentials as parameters. Consider the following alternative example:

:::code language="csharp" source="../snippets/sql-server-parameters/AspireApp.AppHost/AppHost.cs":::

For more information on providing parameters, see [External parameters](../../fundamentals/external-parameters.md).

### Connect to database resources

When the .NET Aspire AppHost runs, the server's database resources can be accessed from external tools, such as [SQL Server Management Studio (SSMS)](/sql/ssms/download-sql-server-management-studio-ssms) or [MSSQL for Visual Studio Code](/sql/tools/visual-studio-code-extensions/mssql/mssql-extension-visual-studio-code). The connection string for the database resource is available in the dependent resources environment variables and is accessed using the [.NET Aspire dashboard: Resource details](../../fundamentals/dashboard/explore.md#resource-details) pane. The environment variable is named `ConnectionStrings__{name}` where `{name}` is the name of the database resource, in this example it's `database`. Use the connection string to connect to the database resource from external tools. Imagine that you have a database named `todos` with a single `dbo.Todos` table.

#### [SQL Server Management Studio](#tab/ssms)

To connect to the database resource from SQL Server Management Studio, follow these steps:

1. Open SSMS.
1. In the **Connect to Server** dialog, select the **Additional Connection Parameters** tab.
1. Paste the connection string into the **Additional Connection Parameters** field and select **Connect**.

    :::image type="content" source="media/ssms-new-connection.png" lightbox="media/ssms-new-connection.png" alt-text="SQL Server Management Studio: Connect to Server dialog.":::

1. If you're connected, you can see the database resource in the **Object Explorer**:

    :::image type="content" source="media/ssms-connected.png" lightbox="media/ssms-connected.png" alt-text="SQL Server Management Studio: Connected to database.":::

For more information, see [SQL Server Management Studio: Connect to a server](/sql/ssms/quickstarts/ssms-connect-query-sql-server).

#### [MSSQL for Visual Studio Code](#tab/mssql-vscode)

To connect to the database resource from MSSQL for Visual Studio Code, follow these steps:

1. Open the **SQL SERVER** extension.
1. Select the **Add Connection** option under **CONNECTIONS**.

    :::image type="content" source="media/mssql-vscode-add-connection.png" lightbox="media/mssql-vscode-add-connection.png" alt-text="MSSQL for Visual Studio Code: Connections / add connection screen capture.":::

1. Change the **Input type** to **Connection string** and paste the connection string into the **Connection string** field.
1. Select **Connect**.

    :::image type="content" source="media/mssql-vscode-connection-details.png" lightbox="media/mssql-vscode-connection-details.png" alt-text="MSSQL for Visual Studio Code: Connection string input details.":::

1. Once you're connected, you can see the database resource in the active tab and run queries against it:

    :::image type="content" source="media/mssql-vscode-connected.png" lightbox="media/mssql-vscode-connected.png" alt-text="MSSQL for Visual Studio Code: Connected to database.":::

For more information, see [MSSQL for Visual Studio Code](/sql/tools/visual-studio-code-extensions/mssql/mssql-extension-visual-studio-code).

---
