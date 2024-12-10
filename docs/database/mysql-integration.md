---
title: .NET Aspire MySQL database integration
description: Learn how to use the .NET Aspire MySQL database integration, which includes both hosting and client integrations.
ms.date: 12/09/2024
uid: storage/mysql-integration
---

# .NET Aspire MySQL integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[MySQL](https://www.mysql.com/) is an open-source Relational Database Management System (RDBMS) that uses Structured Query Language (SQL) to manage and manipulate data. It's employed in a many different environments, from small projects to large-scale enterprise systems and it's a popular choice to host data that underpins microservices in a cloud-native application. The .NET Aspire MySQL database integration enables you to connect to existing MySQL databases or create new instances from .NET with the [`mysql` container image](https://hub.docker.com/_/mysql).

## Hosting integration

[!INCLUDE [mysql-app-host](includes/mysql-app-host.md)]

> AJMTODO: Integrate the following into the previous include?

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
> The SQL Server container is slow to start, so it's best to use a _persistent_ lifetime to avoid unnecessary restarts. For more information, see [Container resource lifetime](../../fundamentals/app-host-overview.md#container-resource-lifetime).

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

The data volume is used to persist the MySQL server data outside the lifecycle of its container. The data volume is mounted at the `/var/lib/mysql` path in the SQL Server container and when a `name` parameter isn't provided, the name is generated at random. For more information on data volumes and details on why they're preferred over [bind mounts](#add-sql-server-resource-with-data-bind-mount), see [Docker docs: Volumes](https://docs.docker.com/engine/storage/volumes).

> [!WARNING]
> The password is stored in the data volume. When using a data volume and if the password changes, it will not work until you delete the volume.

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

> AJMTODO: If you move this text into the above include, add "../" to this path.

[!INCLUDE [data-bind-mount-vs-volumes](../includes/data-bind-mount-vs-volumes.md)]

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

For more information, see [External parameters](../fundamentals/external-parameters.md).

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

## Client integration

To get started with the .NET Aspire MySQL database integration, install the [📦 Aspire.MySqlConnector](https://www.nuget.org/packages/Aspire.MySqlConnector) NuGet package in the client-consuming project, that is, the project for the application that uses the MySQL client. The MySQL client integration registers a <xref:MySqlConnector.MySqlDataSource> instance that you can use to interact with the MySQL server.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.MySqlConnector
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.MySqlConnector"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).


> AJMTODO: THis is where I got to.

## Example usage

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the `AddMySqlDataSource` extension to register a `MySqlDataSource` for use via the dependency injection container.

```csharp
builder.AddMySqlDataSource("mysqldb");
```

To retrieve your `MySqlDataSource` object, consider the following example service:

```csharp
public class ExampleService(MySqlDataSource dataSource)
{
    // Use dataSource...
}
```

After adding a `MySqlDataSource`, you can require the `MySqlDataSource` instance using DI.

## Configuration

The .NET Aspire MySQL database integration provides multiple configuration approaches and options to meet the requirements and conventions of your project.

### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddMySqlDataSource()`:

```csharp
builder.AddMySqlDataSource("mysql");
```

Then the connection string will be retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "mysql": "Server=mysql;Database=mysqldb"
  }
}
```

For more information on how to format this connection string, see [MySqlConnector: ConnectionString documentation](https://mysqlconnector.net/connection-options/).

### Use configuration providers

The .NET Aspire MySQL database supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the `MySqlConnectorSettings` from configuration files such as _:::no-loc text="appsettings.json":::_ by using the `Aspire:MySqlConnector` key. If you have set up your configurations in the `Aspire:MySqlConnector` section, you can just call the method without passing any parameter.

The following example shows an _:::no-loc text="appsettings.json":::_ file that configures some of the available options:

```json
{
  "Aspire": {
    "MySqlConnector": {
      "DisableHealthChecks": true,
      "DisableTracing": true
    }
  }
}
```

### Use inline configurations

You can also pass the `Action<MySqlConnectorSettings>` delegate to set up some or all the options inline, for example to disable health checks from code:

```csharp
builder.AddMySqlDataSource("mysql",
    static settings => settings.DisableHealthChecks  = true);
```

### Configuration options

Here are the configurable options with corresponding default values:

| Name                  | Description                                                                           |
|-----------------------|---------------------------------------------------------------------------------------|
| `ConnectionString`    | The connection string of the MySQL database database to connect to.                   |
| `DisableHealthChecks` | A boolean value that indicates whether the database health check is disabled or not.  |
| `DisableTracing`      | A boolean value that indicates whether the OpenTelemetry tracing is disabled or not.  |
| `DisableMetrics`      | A boolean value that indicates whether the OpenTelemetry metrics are disabled or not. |

[!INCLUDE [integration-health-checks](../includes/integration-health-checks.md)]

By default, the .NET Aspire MySQL database integration handles the following:

- Adds a `MySqlHealthCheck`, which verifies that a connection can be made commands can be run against the MySql database.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

## See also

- [MySQL database](https://mysqlconnector.net/)
- [.NET Aspire database containers sample](/samples/dotnet/aspire-samples/aspire-database-containers/)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
