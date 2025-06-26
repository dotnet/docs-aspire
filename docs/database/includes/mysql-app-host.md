---
ms.topic: include
---

The MySQL hosting integration models the server as the <xref:Aspire.Hosting.ApplicationModel.MySqlServerResource> type and the database as the <xref:Aspire.Hosting.ApplicationModel.MySqlDatabaseResource> type. To access these types and APIs, add the [📦 Aspire.Hosting.MySql](https://www.nuget.org/packages/Aspire.Hosting.MySql) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.MySql
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.MySql"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add MySQL server resource and database resource

In your app host project, call <xref:Aspire.Hosting.MySqlBuilderExtensions.AddMySql*> to add and return a MySQL resource builder. Chain a call to the returned resource builder to <xref:Aspire.Hosting.MySqlBuilderExtensions.AddDatabase*>, to add a MySQL database resource.

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var mysql = builder.AddMySql("mysql")
                   .WithLifetime(ContainerLifetime.Persistent);

var mysqldb = mysql.AddDatabase("mysqldb");

var myService = builder.AddProject<Projects.ExampleProject>()
                       .WithReference(mysqldb)
                       .WaitFor(mysqldb);

// After adding all resources, run the app...
```

> [!NOTE]
> The SQL Server container is slow to start, so it's best to use a _persistent_ lifetime to avoid unnecessary restarts. For more information, see [Container resource lifetime](../../fundamentals/orchestrate-resources.md#container-resource-lifetime).

When .NET Aspire adds a container image to the app host, as shown in the preceding example with the `mysql` image, it creates a new MySQL instance on your local machine. A reference to your MySQL resource builder (the `mysql` variable) is used to add a database. The database is named `mysqldb` and then added to the `ExampleProject`. The MySQL resource includes default credentials with a `username` of `root` and a random `password` generated using the <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.CreateDefaultPasswordParameter*> method.

When the app host runs, the password is stored in the app host's secret store. It's added to the `Parameters` section, for example:

```json
{
  "Parameters:mysql-password": "<THE_GENERATED_PASSWORD>"
}
```

The name of the parameter is `mysql-password`, but really it's just formatting the resource name with a `-password` suffix. For more information, see [Safe storage of app secrets in development in ASP.NET Core](/aspnet/core/security/app-secrets) and [Add MySQL resource with parameters](#add-mysql-resource-with-parameters).

The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method configures a connection in the `ExampleProject` named `mysqldb`.

> [!TIP]
> If you'd rather connect to an existing MySQL server, call <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> instead. For more information, see [Reference existing resources](../../fundamentals/app-host-overview.md#reference-existing-resources).

### Add a MySQL resource with a data volume

To add a data volume to the SQL Server resource, call the <xref:Aspire.Hosting.MySqlBuilderExtensions.WithDataVolume*> method on the SQL Server resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var mysql = builder.AddMySql("mysql")
                   .WithDataVolume();

var mysqldb = mysql.AddDatabase("mysqldb");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(mysqldb)
       .WaitFor(mysqldb);

// After adding all resources, run the app...
```

The data volume is used to persist the MySQL server data outside the lifecycle of its container. The data volume is mounted at the `/var/lib/mysql` path in the SQL Server container and when a `name` parameter isn't provided, the name is generated at random. For more information on data volumes and details on why they're preferred over [bind mounts](#add-a-mysql-resource-with-a-data-bind-mount), see [Docker docs: Volumes](https://docs.docker.com/engine/storage/volumes).

> [!WARNING]
> The password is stored in the data volume. When using a data volume and if the password changes, it will not work until you delete the volume.

> [!IMPORTANT]
> Some database integrations, including the .NET Aspire MySQL integration, can't successfully use data volumes after deployment to Azure Container Apps (ACA). This is because ACA uses Server Message Block (SMB) to connect containers to data volumes, and some systems can't use this connection. In the Aspire Dashboard, a database affected by this issue has a status of **Activating** or **Activation Failed** but is never listed as **Running**.
>
> You can resolve the problem by deploying to a Kubernetes cluster, such as Azure Kubernetes Services (AKS). For more information, see [.NET Aspire deployments](../../deployment/overview.md).

### Add a MySQL resource with a data bind mount

To add a data bind mount to the MySQL resource, call the <xref:Aspire.Hosting.MySqlBuilderExtensions.WithDataBindMount*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var mysql = builder.AddMySql("mysql")
                   .WithDataBindMount(source: @"C:\MySql\Data");

var db = sql.AddDatabase("mysqldb");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(mysqldb)
       .WaitFor(mysqldb);

// After adding all resources, run the app...
```

[!INCLUDE [data-bind-mount-vs-volumes](../../includes/data-bind-mount-vs-volumes.md)]

Data bind mounts rely on the host machine's filesystem to persist the MySQL data across container restarts. The data bind mount is mounted at the `C:\MySql\Data` on Windows (or `/MySql/Data` on Unix) path on the host machine in the MySQL container. For more information on data bind mounts, see [Docker docs: Bind mounts](https://docs.docker.com/engine/storage/bind-mounts).

### Add MySQL resource with parameters

When you want to provide a root MySQL password explicitly, you can pass it as a parameter. Consider the following alternative example:

```csharp
var password = builder.AddParameter("password", secret: true);

var mysql = builder.AddMySql("mysql", password)
                   .WithLifetime(ContainerLifetime.Persistent);

var mysqldb = mysql.AddDatabase("mysqldb");

var myService = builder.AddProject<Projects.ExampleProject>()
                       .WithReference(mysqldb)
                       .WaitFor(mysqldb);
```

For more information, see [External parameters](../../fundamentals/external-parameters.md).

### Add a PhpMyAdmin resource

[**phpMyAdmin**](https://www.phpmyadmin.net/) is a popular web-based administration tool for MySQL. You can use it to browse and modify MySQL objects such as databases, tables, views, and indexes. To use phpMyAdmin within your .NET Aspire solution, call the <xref:Aspire.Hosting.MySqlBuilderExtensions.WithPhpMyAdmin*> method. This method adds a new container resource to the solution that hosts phpMyAdmin and connects it to the MySQL container:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var mysql = builder.AddMySql("mysql")
                   .WithPhpMyAdmin();

var db = sql.AddDatabase("mysqldb");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(mysqldb)
       .WaitFor(mysqldb);

// After adding all resources, run the app...

```

When you run the solution, the .NET Aspire dashboard displays the phpMyAdmin resources with an endpoint. Select the link to the endpoint to view phpMyAdmin in a new browser tab.

### Hosting integration health checks

The MySQL hosting integration automatically adds a health check for the MySQL resource. The health check verifies that the MySQL server is running and that a connection can be established to it.

The hosting integration relies on the [📦 AspNetCore.HealthChecks.MySql](https://www.nuget.org/packages/AspNetCore.HealthChecks.MySql) NuGet package.
