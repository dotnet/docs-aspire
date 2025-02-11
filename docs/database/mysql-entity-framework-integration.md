---
title: .NET Aspire Pomelo MySQL Entity Framework Core integration
description: Learn how to use the .NET Aspire MySQL Entity Framework integration, which includes both hosting and client integrations.
ms.date: 02/07/2025
---

# .NET Aspire Pomelo MySQL Entity Framework Core integration

[!INCLUDE [includes-hosting-and-client](../includes/includes-hosting-and-client.md)]

[MySQL](https://www.mysql.com/) is an open-source Relational Database Management System (RDBMS) that uses Structured Query Language (SQL) to manage and manipulate data. It's employed in a many different environments, from small projects to large-scale enterprise systems and it's a popular choice to host data that underpins microservices in a cloud-native application. The .NET Aspire Pomelo MySQL Entity Framework Core integration enables you to connect to existing MySQL databases or create new instances from .NET with the [`mysql` container image](https://hub.docker.com/_/mysql).

## Hosting integration

[!INCLUDE [mysql-app-host](includes/mysql-app-host.md)]

## Client integration

To get started with the .NET Aspire Pomelo MySQL Entity Framework integration, install the [📦 Aspire.Pomelo.EntityFrameworkCore.MySql](https://www.nuget.org/packages/Aspire.Pomelo.EntityFrameworkCore.MySql) NuGet package in the client-consuming project, that is, the project for the application that uses the MySQL Entity Framework Core client.

### [.NET CLI](#tab/dotnet-cli)

```dotnetcli
dotnet add package Aspire.Pomelo.EntityFrameworkCore.MySql
```

### [PackageReference](#tab/package-reference)

```xml
<PackageReference Include="Aspire.Pomelo.EntityFrameworkCore.MySql"
                  Version="*" />
```

---

For more information, see [dotnet add package](/dotnet/core/tools/dotnet-add-package) or [Manage package dependencies in .NET applications](/dotnet/core/tools/dependencies).

### Add a MySQL database context

In the :::no-loc text="Program.cs"::: file of your client-consuming project, call the <xref:Microsoft.Extensions.Hosting.AspireEFMySqlExtensions.AddMySqlDbContext%2A> extension method on any <xref:Microsoft.Extensions.Hosting.IHostApplicationBuilder> to register a <xref:Microsoft.EntityFrameworkCore.DbContext> for use through the dependency injection container. The method takes a connection name parameter.

```csharp
builder.AddMySqlDbContext<ExampleDbContext>(connectionName: "mysqldb");
```

> [!TIP]
> The `connectionName` parameter must match the name used when adding the SQL Server database resource in the app host project. In other words, when you call `AddDatabase` and provide a name of `mysqldb` that same name should be used when calling `AddMySqlDbContext`. For more information, see [Add MySQL server resource and database resource](#add-mysql-server-resource-and-database-resource).

To retrieve `ExampleDbContext` object from a service:

```csharp
public class ExampleService(ExampleDbContext context)
{
    // Use context...
}
```

For more information on dependency injection, see [.NET dependency injection](/dotnet/core/extensions/dependency-injection).

### Enrich a MySQL database context

You may prefer to use the standard Entity Framework method to obtain a database context and add it to the dependency injection container:

```csharp
builder.Services.AddDbContext<ExampleDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("mysqldb")
        ?? throw new InvalidOperationException("Connection string 'mysqldb' not found.")));
```

> [!NOTE]
> The connection string name that you pass to the <xref:Microsoft.Extensions.Configuration.ConfigurationExtensions.GetConnectionString*> method must match the name used when adding the MySQL resource in the app host project. For more information, see [Add MySQL server resource and database resource](#add-mysql-server-resource-and-database-resource).

You have more flexibility when you create the database context in this way, for example:

- You can reuse existing configuration code for the database context without rewriting it for .NET Aspire.
- You can use Entity Framework Core interceptors to modify database operations.
- You can choose not to use Entity Framework Core context pooling, which may perform better in some circumstances.

If you use this method, you can enhance the database context with .NET Aspire-style retries, health checks, logging, and telemetry features by calling the <xref:Microsoft.Extensions.Hosting.AspireEFMySqlExtensions.EnrichMySqlDbContext*> method:

```csharp
builder.EnrichMySqlDbContext<ExampleDbContext>(
    configureSettings: settings =>
    {
        settings.DisableRetry = false;
        settings.CommandTimeout = 30 // seconds
    });
```

The `settings` parameter is an instance of the <xref:Aspire.Pomelo.EntityFrameworkCore.MySql.PomeloEntityFrameworkCoreMySqlSettings> class.

### Configuration

The .NET Aspire Pomelo MySQL Entity Framework Core integration provides multiple options to configure the database connection based on the requirements and conventions of your project.

#### Use a connection string

When using a connection string from the `ConnectionStrings` configuration section, you can provide the name of the connection string when calling `builder.AddMySqlDatabaseDbContext<TContext>()`:

```csharp
builder.AddMySqlDatabaseDbContext<MyDbContext>("mysql");
```

And then the connection string will be retrieved from the `ConnectionStrings` configuration section:

```json
{
  "ConnectionStrings": {
    "mysql": "Server=mysql;Database=mysqldb"
  }
}
```

The `EnrichMySqlDbContext` won't make use of the `ConnectionStrings` configuration section since it expects a `DbContext` to be registered at the point it's called.

For more information, see the [MySqlConnector: ConnectionString documentation](https://mysqlconnector.net/connection-options/).

#### Use configuration providers

The .NET Aspire Pomelo MySQL Entity Framework Core integration supports <xref:Microsoft.Extensions.Configuration?displayProperty=fullName>. It loads the <xref:Aspire.Pomelo.EntityFrameworkCore.MySql.PomeloEntityFrameworkCoreMySqlSettings> from configuration files such as _:::no-loc text="appsettings.json":::_ by using the `Aspire:Pomelo:EntityFrameworkCore:MySql` key.

The following example shows an _:::no-loc text="appsettings.json":::_ that configures some of the available options:

```json
{
  "Aspire": {
    "Pomelo": {
      "EntityFrameworkCore": {
        "MySql": {
          "ConnectionString": "YOUR_CONNECTIONSTRING",
          "DisableHealthChecks": true,
          "DisableTracing": true
        }
      }
    }
  }
}
```

For the complete MySQL integration JSON schema, see [Aspire.Pomelo.EntityFrameworkCore.MySql/ConfigurationSchema.json](https://github.com/dotnet/aspire/blob/main/src/Components/Aspire.Pomelo.EntityFrameworkCore.MySql/ConfigurationSchema.json).

#### Use inline delegates

You can also pass the `Action<PomeloEntityFrameworkCoreMySqlSettings>` delegate to set up some or all the options inline, for example to disable health checks from code:

```csharp
builder.AddMySqlDbContext<MyDbContext>(
    "mysqldb",
    static settings => settings.DisableHealthChecks = true);
```

or

```csharp
builder.EnrichMySqlDbContext<MyDbContext>(
    static settings => settings.DisableHealthChecks = true);
```

[!INCLUDE [client-integration-health-checks](../includes/client-integration-health-checks.md)]

The .NET Aspire Pomelo MySQL Entity Framework Core integration:

- Adds the health check when <xref:Aspire.Pomelo.EntityFrameworkCore.MySql.PomeloEntityFrameworkCoreMySqlSettings.DisableHealthChecks?displayProperty=nameWithType> is `false`, which calls EF Core's <xref:Microsoft.EntityFrameworkCore.Storage.IDatabaseCreator.CanConnectAsync%2A> method.
- Integrates with the `/health` HTTP endpoint, which specifies all registered health checks must pass for app to be considered ready to accept traffic.

[!INCLUDE [integration-observability-and-telemetry](../includes/integration-observability-and-telemetry.md)]

#### Logging

The .NET Aspire Pomelo MySQL Entity Framework Core integration uses the following log categories:

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

### Tracing

The .NET Aspire Pomelo MySQL Entity Framework Core integration will emit the following tracing activities using OpenTelemetry:

- `MySqlConnector`

### Metrics

The .NET Aspire Pomelo MySQL Entity Framework Core integration currently supports the following metrics:

- MySqlConnector:
  - `db.client.connections.create_time`
  - `db.client.connections.use_time`
  - `db.client.connections.wait_time`
  - `db.client.connections.idle.max`
  - `db.client.connections.idle.min`
  - `db.client.connections.max`
  - `db.client.connections.pending_requests`
  - `db.client.connections.timeouts`
  - `db.client.connections.usage`

## See also

- [MySQL database](https://mysqlconnector.net/)
- [Entity Framework Core docs](/ef/core)
- [.NET Aspire integrations](../fundamentals/integrations-overview.md)
- [.NET Aspire GitHub repo](https://github.com/dotnet/aspire)
