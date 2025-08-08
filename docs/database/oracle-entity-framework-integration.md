---
title: .NET Aspire Oracle Entity Framework Core integration
description: Learn how to use the .NET Aspire Oracle Entity Framework Core integration, which includes both hosting and client integrations.
ms.date: 01/21/2025
uid: database/oracle-entity-framework-integration
ms.custom: sfi-ropc-nochange
---

# .NET Aspire Oracle Entity Framework Core integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[Oracle Database](https://www.oracle.com/database/technologies/) is a widely-used relational database management system owned and developed by Oracle. The .NET Aspire Oracle Entity Framework Core integration enables you to connect to existing Oracle servers or create new servers from .NET with the [container-registry.oracle.com/database/free](https://container-registry.oracle.com/ords/f?p=113:4:5999388133692:::RP,4:P4_REPOSITORY,AI_REPOSITORY,P4_REPOSITORY_NAME,AI_REPOSITORY_NAME:1863,1863,Oracle%20Database%20Free,Oracle%20Database%20Free&cs=3L7x5hgm9Co0WJN-3xZTrFJkDyCZKiS8wlK1jg7nU2yE65gVGYh4WbMLzmX59tAHoLwbwWeAz-kjraRQzB1V5TA) container image.

## Hosting integration

The .NET Aspire Oracle hosting integration models the server as the <xref:Aspire.Hosting.ApplicationModel.OracleDatabaseServerResource> type and the database as the <xref:Aspire.Hosting.ApplicationModel.OracleDatabaseResource> type. To access these types and APIs, add the [📦 Aspire.Hosting.Oracle](https://www.nuget.org/packages/Aspire.Hosting.Oracle) NuGet package in the [app host](xref:dotnet/aspire/app-host) project.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Hosting.Oracle
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Hosting.Oracle"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add Oracle server and database resources

In your AppHost project, call <xref:Aspire.Hosting.OracleDatabaseBuilderExtensions.AddOracle*> to add and return an Oracle server resource builder. Chain a call to the returned resource builder to <xref:Aspire.Hosting.OracleDatabaseBuilderExtensions.AddDatabase*>, to add an Oracle database to the server resource:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var oracle = builder.AddOracle("oracle")
                    .WithLifetime(ContainerLifetime.Persistent);

var oracledb = oracle.AddDatabase("oracledb");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(oracledb);
       .WaitFor(oracledb);

// After adding all resources, run the app...
```

> [!NOTE]
> The Oracle database container can be slow to start, so it's best to use a _persistent_ lifetime to avoid unnecessary restarts. For more information, see [Container resource lifetime](../fundamentals/orchestrate-resources.md#container-resource-lifetime).

When .NET Aspire adds a container image to the AppHost, as shown in the preceding example with the `container-registry.oracle.com/database/free` image, it creates a new Oracle server on your local machine. A reference to your Oracle resource builder (the `oracle` variable) is used to add a database. The database is named `oracledb` and then added to the `ExampleProject`. The Oracle resource includes a random `password` generated using the <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.CreateDefaultPasswordParameter*> method.

The <xref:Aspire.Hosting.ResourceBuilderExtensions.WithReference%2A> method configures a connection in the `ExampleProject` named `"oracledb"`. For more information, see [Container resource lifecycle](../fundamentals/orchestrate-resources.md#container-resource-lifecycle).

> [!TIP]
> If you'd rather connect to an existing Oracle server, call <xref:Aspire.Hosting.ParameterResourceBuilderExtensions.AddConnectionString*> instead. For more information, see [Reference existing resources](../fundamentals/app-host-overview.md#reference-existing-resources).

### Add Oracle resource with password parameter

The Oracle resource includes default credentials with a random password. Oracle supports configuration-based default passwords by using the environment variable `ORACLE_PWD`. When you want to provide a password explicitly, you can provide it as a parameter:

```csharp
var password = builder.AddParameter("password", secret: true);

var oracle = builder.AddOracle("oracle", password)
                    .WithLifetime(ContainerLifetime.Persistent);

var oracledb = oracle.AddDatabase("oracledb");

var myService = builder.AddProject<Projects.ExampleProject>()
                       .WithReference(oracledb)
                       .WaitFor(oracledb);
```

The preceding code gets a parameter to pass to the `AddOracle` API, and internally assigns the parameter to the `ORACLE_PWD` environment variable of the Oracle container. The `password` parameter is usually specified as a _user secret_:

```json
{
  "Parameters": {
    "password": "Non-default-P@ssw0rd"
  }
}
```

For more information, see [External parameters](../fundamentals/external-parameters.md).

### Add Oracle resource with data volume

To add a data volume to the Oracle resource, call the <xref:Aspire.Hosting.OracleDatabaseBuilderExtensions.WithDataVolume*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var oracle = builder.AddOracle("oracle")
                    .WithDataVolume()
                    .WithLifetime(ContainerLifetime.Persistent);

var oracledb = oracle.AddDatabase("oracle");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(oracledb)
       .WaitFor(oracledb);

// After adding all resources, run the app...
```

The data volume is used to persist the Oracle data outside the lifecycle of its container. The data volume is mounted at the `/opt/oracle/oradata` path in the Oracle container and when a `name` parameter isn't provided, the name is generated at random. For more information on data volumes and details on why they're preferred over [bind mounts](#add-oracle-resource-with-data-bind-mount), see [Docker docs: Volumes](https://docs.docker.com/engine/storage/volumes).

> [!WARNING]
> The password is stored in the data volume. When using a data volume and if the password changes, it will not work until you delete the volume.

### Add Oracle resource with data bind mount

To add a data bind mount to the Oracle resource, call the <xref:Aspire.Hosting.OracleDatabaseBuilderExtensions.WithDataBindMount*> method:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var oracle = builder.AddOracle("oracle")
                    .WithDataBindMount(source: @"C:\Oracle\Data");

var oracledb = oracle.AddDatabase("oracledb");

builder.AddProject<Projects.ExampleProject>()
       .WithReference(oracledb)
       .WaitFor(oracledb);

// After adding all resources, run the app...
```

[!INCLUDE [data-bind-mount-vs-volumes](../includes/data-bind-mount-vs-volumes.md)]

Data bind mounts rely on the host machine's filesystem to persist the Oracle data across container restarts. The data bind mount is mounted at the `C:\Oracle\Data` on Windows (or `/Oracle/Data` on Unix) path on the host machine in the Oracle container. For more information on data bind mounts, see [Docker docs: Bind mounts](https://docs.docker.com/engine/storage/bind-mounts).

### Hosting integration health checks

The Oracle hosting integration automatically adds a health check for the Oracle resource. The health check verifies that the Oracle server is running and that a connection can be established to it.

The hosting integration relies on the [📦 AspNetCore.HealthChecks.Oracle](https://www.nuget.org/packages/AspNetCore.HealthChecks.Oracle) NuGet package.

## Client integration

You need an Oracle database and connection string for accessing the database. To get started with the .NET Aspire Oracle client integration, install the [📦 Aspire.Oracle.EntityFrameworkCore](https://www.nuget.org/packages/Aspire.Microsoft.Data.SqlClient) NuGet package in the client-consuming project, that is, the project for the application that uses the Oracle client. The Oracle client integration registers a <xref:System.Data.Entity.DbContext> instance that you can use to interact with Oracle.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Oracle.EntityFrameworkCore
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Oracle.EntityFrameworkCore"
                  Version="*" />
```

---

### Add Oracle client

In the _:::no-loc text="Program.cs":::_ file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireOracleEFCoreExtensions.AddOracleDatabaseDbContext*> extension method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register a `DbContext` for use via the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddOracleDatabaseDbContext<ExampleDbContext>(connectionName: "oracledb");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the Oracle database resource in the AppHost project. In other words, when you call `AddDatabase` and provide a name of `oracledb` that same name should be used when calling `AddOracleDatabaseDbContext`. For more information, see [Add Oracle server and database resources](#add-oracle-server-and-database-resources).

You can then retrieve the <xref:Microsoft.EntityFrameworkCore.DbContext> instance using dependency injection. For example, to retrieve the connection from an example service:

```csharp
public class ExampleService(ExampleDbContext context)
{
    // Use database context...
}
```

For more information on dependency injection, see [.NET dependency injection](/dotnet/core/extensions/dependency-injection).

### Enrich Oracle database context

You may prefer to use the standard Entity Framework method to obtain a database context and add it to the dependency injection container:

```csharp
builder.Services.AddDbContext<ExampleDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("oracledb")
        ?? throw new InvalidOperationException("Connection string 'oracledb' not found.")));
```

> [!NOTE]
> The connection string name that you pass to the <xref:Microsoft.Extensions.Configuration.ConfigurationExtensions.GetConnectionString*> method must match the name used when adding the Oracle resource in the AppHost project. For more information, see [Add Oracle server and database resources](#add-oracle-server-and-database-resources).

You have more flexibility when you create the database context in this way, for example:

- You can reuse existing configuration code for the database context without rewriting it for .NET Aspire.
- You can use Entity Framework Core interceptors to modify database operations.
- You can choose not to use Entity Framework Core context pooling, which may perform better in some circumstances.

If you use this method, you can enhance the database context with .NET Aspire-style retries, health checks, logging, and telemetry features by calling the <xref:Microsoft.Extensions.Hosting.AspireOracleEFCoreExtensions.EnrichOracleDatabaseDbContext*> method:

```csharp
builder.EnrichOracleDatabaseDbContext<ExampleDbContext>(
    configureSettings: settings =>
    {
        settings.DisableRetry = false;
        settings.CommandTimeout = 30 // seconds
    });
```

The `settings` parameter is an instance of the <xref:Aspire.Oracle.EntityFrameworkCore.OracleEntityFrameworkCoreSettings> class.

### Configuration

The .NET Aspire Oracle Entity Framework Core integration provides multiple configuration approaches and options to meet the requirements and conventions of your project.

#### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you provide the name of the connection string when calling `builder.AddOracleDatabaseDbContext<TContext>()`:

```csharp
builder.AddOracleDatabaseDbContext<ExampleDbContext>("oracleConnection");
```

The connection string is retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "oracleConnection": "Data Source=TORCL;User Id=OracleUser;Password=Non-default-P@ssw0rd;"
  }
}
```

The `EnrichOracleDatabaseDbContext` won't make use of the `ConnectionStrings` configuration section since it expects a `DbContext` to be registered at the point it is called.

For more information, see the [ODP.NET documentation](https://www.oracle.com/database/technologies/appdev/dotnet/odp.html).

#### Use configuration providers

The .NET Aspire Oracle Entity Framework Core integration supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName> from configuration files such as _:::no-loc text="appsettings.json":::_ by using the `Aspire:Oracle:EntityFrameworkCore` key. If you have set up your configurations in the `Aspire:Oracle:EntityFrameworkCore` section you can just call the method without passing any parameter.

The following is an example of an _:::no-loc text="appsettings.json":::_ that configures some of the available options:

```json
{
  "Aspire": {
    "Oracle": {
      "EntityFrameworkCore": {
        "DisableHealthChecks": true,
        "DisableTracing": true,
        "DisableRetry": false,
        "CommandTimeout": 30
      }
    }
  }
}
```

> [!TIP]
> The `CommandTimeout` property is in seconds. When set as shown in the preceding example, the timeout is 30 seconds.

#### Use inline delegates

You can also pass the `Action<OracleEntityFrameworkCoreSettings>` delegate to set up some or all the options inline, for example to disable health checks from code:

```csharp
builder.AddOracleDatabaseDbContext<ExampleDbContext>(
    "oracle",
    static settings => settings.DisableHealthChecks  = true);
```

or

```csharp
builder.EnrichOracleDatabaseDbContext<ExampleDbContext>(
    static settings => settings.DisableHealthChecks  = true);
```

#### Configuration options

Here are the configurable options with corresponding default values:

| Name                  | Description                                                                          |
|-----------------------|--------------------------------------------------------------------------------------|
| `ConnectionString`    | The connection string of the Oracle database to connect to.                          |
| `DisableHealthChecks` | A boolean value that indicates whether the database health check is disabled or not. |
| `DisableTracing`      | A boolean value that indicates whether the OpenTelemetry tracing is disabled or not. |
| `DisableRetry`        | A boolean value that indicates whether command retries should be disabled or not.    |
| `CommandTimeout`      | The time in seconds to wait for the command to execute.                              |

[!INCLUDE [client-integration-health-checks](../includes/client-integration-health-checks.md)]

By default, the .NET Aspire Oracle Entity Framework Core integration handles the following:

- Checks if the <xref:Aspire.Oracle.EntityFrameworkCore.OracleEntityFrameworkCoreSettings.DisableHealthChecks?displayProperty=nameWithType> is `true`.
- If so, adds the [`DbContextHealthCheck`](https://github.com/Xabaril/AspNetCore.Diagnostics.HealthChecks/blob/master/src/HealthChecks.NpgSql/NpgSqlHealthCheck.cs), which calls EF Core's <xref:Microsoft.EntityFrameworkCore.Storage.IDatabaseCreator.CanConnectAsync%2A> method. The name of the health check is the name of the `TContext` type.

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

#### Logging

The .NET Aspire Oracle Entity Framework Core integration uses the following log categories:

- `Microsoft.EntityFrameworkCore.ChangeTracking`
- `Microsoft.EntityFrameworkCore.Database.Command`
- `Microsoft.EntityFrameworkCore.Database.Connection`
- `Microsoft.EntityFrameworkCore.Database.Transaction`
- `Microsoft.EntityFrameworkCore.Infrastructure`
- `Microsoft.EntityFrameworkCore.Migrations`
- `Microsoft.EntityFrameworkCore.Model`
- `Microsoft.EntityFrameworkCore.Model.Validation`
- `Microsoft.EntityFrameworkCore.Query`
- `Microsoft.EntityFrameworkCore.Update`

#### Tracing

The .NET Aspire Oracle Entity Framework Core integration will emit the following tracing activities using OpenTelemetry:

- OpenTelemetry.Instrumentation.EntityFrameworkCore

#### Metrics

The .NET Aspire Oracle Entity Framework Core integration currently supports the following metrics:

- Microsoft.EntityFrameworkCore

## See also

- [Oracle Database](https://www.oracle.com/database/)
- [Oracle Database Documentation](https://docs.oracle.com/en/database/oracle/oracle-database/index.html)
- [Entity Framework Core docs](/ef/core)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
