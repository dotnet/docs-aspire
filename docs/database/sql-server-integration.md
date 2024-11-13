---
title: .NET Aspire SQL Server integration
description: Learn how to use the .NET Aspire SQL Server integration, which includes both hosting and client integrations.
ms.date: 11/13/2024
uid: database/sql-server-integration
---

# .NET Aspire SQL Server integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[SQL Server](https://www.microsoft.com/en-us/sql-server) is a relational database management system developed by Microsoft. The .NET Aspire SQL Server integration enables you to connect to existing SQL Server instances or create new instances from .NET with the [`mcr.microsoft.com/mssql/server` container image](https://hub.docker.com/_/microsoft-mssql-server).

## Hosting integration

The SQL Server hosting integration models a SQL Server as the <xref:Aspire.Hosting.ApplicationModel.SqlServerResource> type. To access this type and APIs that allow you to add it to your [📦 Aspire.Hosting.SqlServer](https://www.nuget.org/packages/Aspire.Hosting.SqlServer) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

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

The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method configures a connection in the `ExampleProject` named `"database"`. For more information, see [Container resource lifecycle](../fundamentals/app-host-overview.md#container-resource-lifecycle).

> [!TIP]
> If you'd rather connect to an existing SQL Server, call <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> instead. For more information, see [Reference existing resources](../fundamentals/app-host-overview.md#reference-existing-resources).

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

var sql = builder.AddSqlServer("database")
                 .WithDataBindMount(
                     source: @"C:\SqlServer\Data",
                     isReadOnly: false);

var db = sql.AddDatabase("database");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(db);

// After adding all resources, run the app...
```

[!INCLUDE [data-bind-mount-vs-volumes](../includes/data-bind-mount-vs-volumes.md)]

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

For more information on providing parameters, see [External parameters](../fundamentals/external-parameters.md).

### Hosting integration health checks

The SQL Server hosting integration automatically adds a health check for the SQL Server resource. The health check verifies that the SQL Server is running and that a connection can be established to it.

The hosting integration relies on the [📦 AspNetCore.HealthChecks.SqlServer](https://www.nuget.org/packages/AspNetCore.HealthChecks.SqlServer) NuGet package.

## Client integration

To get started with the .NET Aspire SQL Server client integration, install the [📦 Aspire.Microsoft.Data.SqlClient](https://www.nuget.org/packages/Aspire.Microsoft.Data.SqlClient) NuGet package in the client-consuming project, that is, the project for the application that uses the SQL Server client. The SQL Server client integration registers a <xref:System.Data.SqlClient.SqlConnection> instance that you can use to interact with SQL Server.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Microsoft.Data.SqlClient
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Microsoft.Data.SqlClient"
                  Version="*" />
```

---

### Add SQL Server client

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireSqlServerExtensions.AddSqlServerClient%2A> extension method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register a `SqlConnection` for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddSqlServerClient(connectionName: "sql");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the SQL Server resource in the app host project. For more information, see [Add SQL Server resource and database resource](#add-sql-server-resource-and-database-resource).

You can then retrieve the `SqlConnection` instance using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(SqlConnection connection)
{
    // Use connection...
}
```

For more information on dependency injection, see [.NET dependency injection](/dotnet/core/extensions/dependency-injection).

### Add keyed SQL Server client

There might be situations where you want to register multiple `SqlConnection` instances with different connection names. To register keyed SQL Server clients, call the <xref:Microsoft.Extensions.Hosting.AspireSqlServerExtensions.AddKeyedSqlServerClient*> method:

```csharp
builder.AddKeyedSqlServerClient(name: "mainDb");
builder.AddKeyedSqlServerClient(name: "loggingDb");
```

Then you can retrieve the `SqlConnection` instances using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(
    [FromKeyedServices("mainDb")] SqlConnection mainDbConnection,
    [FromKeyedServices("loggingDb")] SqlConnection loggingDbConnection)
{
    // Use connections...
}
```

For more information on keyed services, see [.NET dependency injection: Keyed services](/dotnet/core/extensions/dependency-injection#keyed-services).

### Configuration

The .NET Aspire SQL Server integration provides multiple options to configure the connection based on the requirements and conventions of your project.

#### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling the <xref:Microsoft.Extensions.Hosting.AspireSqlServerExtensions.AddSqlServerClient*> method:

```csharp
builder.AddSqlServerClient(connectionName: "sql");
```

Then the connection string is retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "database": "Data Source=myserver;Initial Catalog=master"
  }
}
```

For more information on how to format this connection string, see the [ConnectionString](/dotnet/api/system.data.sqlclient.sqlconnection.connectionstring#remarks).

#### Use configuration providers

The .NET Aspire SQL Server integration supports <xref:Microsoft.Extensions.Configuration>. It loads the <xref:Aspire.Microsoft.Data.SqlClient.SqlClientSettings> from configuration by using the `Aspire:Microsoft:Data:SqlClient` key. The following snippet is an example of a _:::no-loc text="appsettings.json":::_ file that configures some of the options:

```json
{
  "Aspire": {
    "Microsoft": {
      "Data": {
        "SqlClient": {
          "ConnectionString": "YOUR_CONNECTIONSTRING",
          "DisableHealthChecks": false,
          "DisableMetrics": true
        }
      }
    }
  }
}
```

For the complete SQL Server client integration JSON schema, see [Aspire.Microsoft.Data.SqlClient/ConfigurationSchema.json](https://github.com/dotnet/aspire/blob/v8.2.2/src/Components/Aspire.Microsoft.Data.SqlClient/ConfigurationSchema.json).

#### Use inline delegates

Also you can pass the `Action<SqlClientSettings> configureSettings` delegate to set up some or all the options inline, for example to disable health checks from code:

```csharp
builder.AddSqlServerClient(
    "database",
    static settings => settings.DisableHealthChecks = true);
```

### Client integration health checks

By default, .NET Aspire integrations enable [health checks](../fundamentals/health-checks.md) for all services. For more information, see [.NET Aspire integrations overview](../fundamentals/integrations-overview.md).

The .NET Aspire SQL Server integration:

- Adds the health check when <xref:Aspire.Microsoft.Data.SqlClient.SqlClientSettings.DisableHealthChecks?displayProperty=nameWithType> is `false`, which attempts to connect to the SQL Server.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic.

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

#### Logging

The .NET Aspire SQL Server integration currently doesn't enable logging by default due to limitations of the `SqlClient`.

#### Tracing

The .NET Aspire SQL Server integration emits the following tracing activities using OpenTelemetry:

- `OpenTelemetry.Instrumentation.SqlClient`

#### Metrics

The .NET Aspire SQL Server integration will emit the following metrics using OpenTelemetry:

- Microsoft.Data.SqlClient.EventSource
  - `active-hard-connections`
  - `hard-connects`
  - `hard-disconnects`
  - `active-soft-connects`
  - `soft-connects`
  - `soft-disconnects`
  - `number-of-non-pooled-connections`
  - `number-of-pooled-connections`
  - `number-of-active-connection-pool-groups`
  - `number-of-inactive-connection-pool-groups`
  - `number-of-active-connection-pools`
  - `number-of-inactive-connection-pools`
  - `number-of-active-connections`
  - `number-of-free-connections`
  - `number-of-stasis-connections`
  - `number-of-reclaimed-connections`

## See also

- [Azure SQL Database](/azure/azure-sql/database)
- [SQL Server](/sql/sql-server)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
